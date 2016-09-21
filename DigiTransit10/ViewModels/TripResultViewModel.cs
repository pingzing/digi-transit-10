﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;
using Template10.Common;
using DigiTransit10.Models;
using DigiTransit10.Localization.Strings;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Windows.Devices.Geolocation;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Styles;
using System;
using System.IO;
using System.IO.Compression;
using Windows.Storage;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Services.SettingsServices;
using Newtonsoft.Json;
using MetroLog;

namespace DigiTransit10.ViewModels
{
    public class TripResultViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;
        private readonly IFavoritesService _favoritesService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;

        public RelayCommand<TripItinerary> ShowTripDetailsCommand => new RelayCommand<TripItinerary>(ShowTripDetails);
        public RelayCommand GoBackToTripListCommand => new RelayCommand(GoBackToTripList);
        public RelayCommand<TripItinerary> SaveRouteCommand => new RelayCommand<TripItinerary>(SaveRoute);

        private ObservableCollection<TripItinerary> _tripResults = new ObservableCollection<TripItinerary>();
        public ObservableCollection<TripItinerary> TripResults
        {
            get { return _tripResults; }
            set { Set(ref _tripResults, value); }
        }

        private ObservableCollection<ColoredMapLine> _coloredMapLinePoints = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> ColoredMapLinePoints
        {
            get { return _coloredMapLinePoints; }
            set { Set(ref _coloredMapLinePoints, value); }
        }

        private IEnumerable<IMapPoi> _mapStops = new List<IMapPoi>();
        public IEnumerable<IMapPoi> MapStops
        {
            get { return _mapStops; }
            set { Set(ref _mapStops, value); }
        }

        private string _fromName;
        public string FromName
        {
            get { return _fromName?.ToUpperInvariant(); }
            set { Set(ref _fromName, value); }
        }

        private string _toName;
        public string ToName
        {
            get { return _toName?.ToUpperInvariant(); }
            set { Set(ref _toName, value); }
        }

        private bool _isinDetailedState = false;
        public bool IsInDetailedState
        {
            get { return _isinDetailedState; }
            set { Set(ref _isinDetailedState, value); }
        }

        private List<TripLeg> _selectedDetailLegs = null;
        public List<TripLeg> SelectedDetailLegs
        {
            get { return _selectedDetailLegs; }
            set { Set(ref _selectedDetailLegs, value); }
        }

        public TripResultViewModel(INetworkService networkService, IMessenger messengerService, SettingsService settings,
            IFavoritesService favorites, IFileService fileService, ILogger logger)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _settingsService = settings;
            _favoritesService = favorites;
            _fileService = fileService;
            _logger = logger;

            _messengerService.Register<MessageTypes.PlanFoundMessage>(this, PlanFound);
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if(IsInDetailedState)
            {
                e.Handled = true;
                GoBackToTripList();
            }
        }

        private async void PlanFound(MessageTypes.PlanFoundMessage _)
        {
            _logger.Debug($"Entering {nameof(PlanFound)} in TripResultViewModel.");
            if (!BootStrapper.Current.SessionState.ContainsKey(NavParamKeys.PlanResults))
            {
                return;
            }

            _logger.Debug($"Pulling PlanFound out of session state dict.");
            var foundPlan = BootStrapper.Current.SessionState[NavParamKeys.PlanResults] as TripPlan;

            _logger.Debug($"Removing PlanFound from sesstionState dict");
            BootStrapper.Current.SessionState.Remove(NavParamKeys.PlanResults);
            if (foundPlan?.PlanItineraries == null)
            {
                _logger.Debug($"No itineraries in PlanFound. Returning...");
                return;
            }

            _logger.Debug($"Clearing TripResults...");
            TripResults.Clear();

            _logger.Debug($"SettingFromName and ToName in the foundPlan.");
            FromName = foundPlan.StartingPlaceName ?? AppResources.TripPlanStrip_StartingPlaceDefault;
            ToName = foundPlan.EndingPlaceName ?? AppResources.TripPlanStrip_EndPlaceDefault;

            _logger.Debug($"Setting up delay for animation...");
            //----todo: leaking abstraction here, see if we can move this to the view
            // Give the control enough time to animate back from the DetailedState, 
            // so that when the TripPlanStrip does it's second render pass, it gets accurate values.            
            if (IsInDetailedState)
            {
                _logger.Debug($"In detailed state, but got a new plan. Navigating TripResultForm back to trip list...");
                GoBackToTripList();
                await Task.Delay(450);
            }
            //----end todo

            _logger.Debug($"Adding trip results to the results list...");
            foreach (TripItinerary itinerary in foundPlan.PlanItineraries)
            {
                _logger.Debug($"Adding {itinerary.StartingPlaceName} -> {itinerary.EndingPlaceName} with {itinerary.ItineraryLegs.Count} legs to the list.");
                TripResults.Add(itinerary);
            }
        }

        private void ShowTripDetails(TripItinerary model)
        {
            SelectedDetailLegs = model.ItineraryLegs;

            var mapLine = new ColoredMapLine(model.ItineraryLegs
                .SelectMany(y =>
                    GooglePolineDecoder.Decode(y.LegGeometryString)
                    .Select(x =>
                    {
                        if (y.Mode == ApiEnums.ApiMode.Walk)
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(y.Mode), true);
                        }
                        else
                        {
                            return new ColoredMapLinePoint(x, HslColors.GetModeColor(y.Mode));
                        }
                    }))
                );

            ColoredMapLinePoints.Clear();
            ColoredMapLinePoints.Add(mapLine);

            var stops = new List<IMapPoi>();
            stops.AddRange(model.ItineraryLegs.Select(x => x.StartPlaceToPoi()));
            stops.Add(model.ItineraryLegs.Last().EndPlaceToPoi());

            MapStops = stops;

            _messengerService.Send(new MessageTypes.ViewPlanDetails(model));
            IsInDetailedState = true;
        }

        private void SaveRoute(TripItinerary routeToSave)
        {
            var route = new FavoriteRoute
            {
                FontIconGlyph = FontIconGlyphs.FilledStar,
                FavoriteId = Guid.NewGuid(),
                IconFontFace = Constants.SegoeMdl2FontName,
                IconFontSize = Constants.SymbolFontSize,
                UserChosenName = $"{routeToSave.StartingPlaceName} → {routeToSave.EndingPlaceName}",
            };

            TripLeg startPlace = routeToSave.ItineraryLegs.First();
            TripLeg endPlace = routeToSave.ItineraryLegs.Last();
            var places = new List<FavoriteRoutePlace>();
            places.Add(new FavoriteRoutePlace
            {
                Lat = startPlace.StartCoords.Latitude,
                Lon = startPlace.StartCoords.Longitude,
                Name = routeToSave.StartingPlaceName
            });
            places.Add(new FavoriteRoutePlace
            {
                Lat = endPlace.EndCoords.Latitude,
                Lon = endPlace.EndCoords.Longitude,
                Name = routeToSave.EndingPlaceName
            });

            route.RoutePlaces = places;
            route.RouteGeometryStrings = routeToSave.RouteGeometryStrings.ToList();
            _favoritesService.AddFavorite(route);
        }

        private void GoBackToTripList()
        {
            _messengerService.Send(new MessageTypes.ViewPlanStrips());
            IsInDetailedState = false;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            BootStrapper.BackRequested += BootStrapper_BackRequested;
            if (suspensionState.ContainsKey(SuspensionKeys.TripResults_HasSavedState)
                && (bool)suspensionState[SuspensionKeys.TripResults_HasSavedState])
            {
                IStorageFile tripResultCacheFile = await _fileService.GetTempFileAsync(
                    SuspensionKeys.TripResults_HasSavedState, CreationCollisionOption.OpenIfExists);

                using(Stream fileInStream = await tripResultCacheFile.OpenStreamForReadAsync())
                using (var gzip = new GZipStream(fileInStream, CompressionMode.Decompress))
                {
                    TripItinerary[] tripsArray = gzip.DeseriaizeJsonFromStream<TripItinerary[]>();
                    TripResults = new ObservableCollection<TripItinerary>(tripsArray);
                }

                FromName = (string)suspensionState[SuspensionKeys.TripResults_FromName];
                ToName = (string) suspensionState[SuspensionKeys.TripResults_ToName];

                suspensionState.Remove(SuspensionKeys.TripResults_HasSavedState);
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                IStorageFile tripResultCacheFile = await _fileService.GetTempFileAsync(
                    SuspensionKeys.TripResults_HasSavedState, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await tripResultCacheFile.OpenStreamForWriteAsync())
                using (GZipStream jsonStream = new GZipStream(fileStream, CompressionLevel.Fastest))
                {
                    jsonStream.SerializeJsonToStream(TripResults.ToArray());
                }

                //and make a note of it in the suspension dict:
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_HasSavedState, true);
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_FromName, FromName);
                suspensionState.AddOrUpdate(SuspensionKeys.TripResults_ToName, ToName);
            }
            else //Don't want to unhook the "back-returns from Detailed View" behavior if the user is just switching apps.
            {
                BootStrapper.BackRequested -= BootStrapper_BackRequested;
            }
            await Task.CompletedTask;
        }
    }
}
