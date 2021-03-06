﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Trippit.Controls;
using Trippit.ExtensionMethods;
using Trippit.Helpers;
using Trippit.Helpers.PageNavigationContainers;
using Trippit.Localization.Strings;
using Trippit.Models;
using Trippit.Services;
using Trippit.Services.SettingsServices;
using Trippit.Styles;
using Trippit.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Trippit.Models.ApiModels.ApiEnums;

namespace Trippit.ViewModels
{
    public class FavoritesViewModel : ViewModelBase
    {
        private readonly INetworkService _networkService;
        private readonly IMessenger _messengerService;
        private readonly SettingsService _settingsService;
        private readonly IFavoritesService _favoritesService;
        private readonly ITileService _tileService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<object> _selectedItems = null;
        public ObservableCollection<object> SelectedItems
        {
            get { return _selectedItems; }
            set { Set(ref _selectedItems, value); }
        }

        public bool IsFavoritesEmpty
        {
            get
            {
                if (Favorites?.Count == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var list in Favorites)
                    {
                        if (list?.Count > 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        private ObservableCollection<IMapPoi> _mappableFavoritePlaces = new ObservableCollection<IMapPoi>();
        public ObservableCollection<IMapPoi> MappableFavoritePlaces
        {
            get { return _mappableFavoritePlaces; }
            set { Set(ref _mappableFavoritePlaces, value); }
        }

        private ObservableCollection<ColoredMapLine> _mappableFavoriteRoutes = new ObservableCollection<ColoredMapLine>();
        public ObservableCollection<ColoredMapLine> MappableFavoriteRoutes
        {
            get { return _mappableFavoriteRoutes; }
            set { Set(ref _mappableFavoriteRoutes, value); }
        }

        private GroupedFavoriteList _groupedFavoritePlaces = new GroupedFavoriteList(AppResources.Favorites_PlacesGroupHeader);
        public GroupedFavoriteList GroupedFavoritePlaces
        {
            get { return _groupedFavoritePlaces; }
            set
            {
                Set(ref _groupedFavoritePlaces, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        private GroupedFavoriteList _groupedFavoriteRoutes = new GroupedFavoriteList(AppResources.Favorites_RoutesGroupHeader);
        public GroupedFavoriteList GroupedFavoriteRoutes
        {
            get { return _groupedFavoriteRoutes; }
            set
            {
                Set(ref _groupedFavoriteRoutes, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        private ObservableCollection<GroupedFavoriteList> _favorites = new ObservableCollection<GroupedFavoriteList>();
        public ObservableCollection<GroupedFavoriteList> Favorites
        {
            get { return _favorites; }
            set
            {
                Set(ref _favorites, value);
                RaisePropertyChanged(nameof(IsFavoritesEmpty));
            }
        }

        private ListViewSelectionMode _listSelectionMode = ListViewSelectionMode.None;
        public ListViewSelectionMode ListSelectionMode
        {
            get { return _listSelectionMode; }
            set
            {
                Set(ref _listSelectionMode, value);
                RaisePropertyChanged(nameof(IsMultiSelectionActive));
            }
        }

        public bool IsMultiSelectionActive => ListSelectionMode == ListViewSelectionMode.Multiple;

        public RelayCommand AddNewFavoriteCommand => new RelayCommand(AddNewFavorite);
        public RelayCommand<IFavorite> EditFavoriteCommand => new RelayCommand<IFavorite>(EditFavorite);
        public RelayCommand<IFavorite> DeleteFavoriteCommand => new RelayCommand<IFavorite>(DeleteFavorite);
        public RelayCommand<IPlace> SetAsOriginCommand => new RelayCommand<IPlace>(SetAsOrigin);
        public RelayCommand<IPlace> SetAsDestinationCommand => new RelayCommand<IPlace>(SetAsDestination);
        public RelayCommand<IFavorite> ToggleSelectionCommand => new RelayCommand<IFavorite>(ToggleSelection);
        public RelayCommand<IFavorite> PinToStartCommand => new RelayCommand<IFavorite>(PinToStart);
        public RelayCommand<IFavorite> PinToMainPageCommand => new RelayCommand<IFavorite>(PinToMainPage);
        public RelayCommand<IList<object>> SelectionChangedCommand => new RelayCommand<IList<object>>(SelectionChanged);
        public RelayCommand<IFavorite> SetAsRouteCommand => new RelayCommand<IFavorite>(SetAsRoute);

        public FavoritesViewModel(INetworkService networkService, IMessenger messengerService,
            IFavoritesService favoritesService, ITileService tileService, IDialogService dialogService)
        {
            _networkService = networkService;
            _messengerService = messengerService;
            _favoritesService = favoritesService;
            _settingsService = SimpleIoc.Default.GetInstance<SettingsService>();
            _tileService = tileService;
            _dialogService = dialogService;

            Favorites.Add(GroupedFavoritePlaces);
            Favorites.Add(GroupedFavoriteRoutes);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            _favoritesService.FavoritesChanged += FavoritesChanged;

            //todo: change this from nuking/rebuilding every time to add/remove what's changed
            GroupedFavoritePlaces.Clear();
            MappableFavoritePlaces.Clear();
            GroupedFavoriteRoutes.Clear();
            MappableFavoriteRoutes.Clear();

            var favorites = await _favoritesService.GetFavoritesAsync();

            foreach (IPlace place in favorites.OfType<IPlace>())
            {
                AddFavoritePlace((IFavorite)place);
            }

            foreach (FavoriteRoute route in favorites.OfType<FavoriteRoute>())
            {
                AddFavoriteRoute(route);
            }


            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            _favoritesService.FavoritesChanged -= FavoritesChanged;
            await Task.CompletedTask;
        }

        private void DeleteFavorite(IFavorite favorite)
        {
            if (ListSelectionMode == ListViewSelectionMode.Multiple
                && _selectedItems != null
                && _selectedItems.Count > 0)
            {
                _favoritesService.RemoveFavorite(_selectedItems.Cast<IFavorite>());
            }
            else
            {
                _favoritesService.RemoveFavorite(favorite);
            }
        }

        private void SetAsOrigin(IPlace obj)
        {
            var args = new NavigateWithFavoritePlaceArgs(obj, NavigationType.AsOrigin);
            NavigationService.NavigateAsync(typeof(MainPage), args);
        }

        private void SetAsDestination(IPlace obj)
        {
            var args = new NavigateWithFavoritePlaceArgs(obj, NavigationType.AsDestination);
            NavigationService.NavigateAsync(typeof(MainPage), args);
        }

        private void SetAsRoute(IFavorite obj)
        {
            var args = obj as FavoriteRoute;
            if (args != null)
            {
                NavigationService.NavigateAsync(typeof(MainPage), args);
            }
        }

        private async void AddNewFavorite()
        {
            var dialog = new AddOrEditFavoriteDialog();
            await dialog.ShowAsync();
            if (dialog.ResultFavorite != null)
            {
                _favoritesService.AddFavorite(dialog.ResultFavorite);
            }
        }

        private async void EditFavorite(IFavorite obj)
        {
            //bring up the AddOrEdit dialog in Edit mode
            var dialog = new AddOrEditFavoriteDialog(obj);
            await dialog.ShowAsync();
            if (dialog.ResultFavorite != null)
            {
                _favoritesService.EditFavorite(dialog.ResultFavorite);
            }
        }

        private void ToggleSelection(IFavorite obj)
        {
            if (ListSelectionMode == ListViewSelectionMode.None)
            {
                ListSelectionMode = ListViewSelectionMode.Multiple;
                if (_selectedItems != null && obj != null)
                {
                    _selectedItems.Add(obj);
                }
            }
            else
            {
                ListSelectionMode = ListViewSelectionMode.None;
            }
        }

        private async void PinToStart(IFavorite clickedFavorite)
        {
            if (ListSelectionMode == ListViewSelectionMode.Multiple
                && _selectedItems != null
                && _selectedItems.Count > 0)
            {
                foreach (var selectedFave in _selectedItems.Cast<IFavorite>())
                {
                    await _tileService.PinFavoriteToStartAsync(selectedFave);
                }
            }
            else
            {
                await _tileService.PinFavoriteToStartAsync(clickedFavorite);
            }
        }        

        private void PinToMainPage(IFavorite clickedFavorite)
        {
            if (ListSelectionMode == ListViewSelectionMode.Multiple
                && _selectedItems != null
                && _selectedItems.Count > 0)
            {
                foreach (var fave in _selectedItems.Cast<IFavorite>())
                {
                    _settingsService.PushFavoriteId(fave.Id);
                }
            }
            else
            {
                _settingsService.PushFavoriteId(clickedFavorite.Id);
            }
        }

        private void FavoritesChanged(object sender, FavoritesChangedEventArgs args)
        {
            if (args.AddedFavorites?.Count > 0)
            {
                foreach (var favePlace in args.AddedFavorites.OfType<FavoritePlace>())
                {
                    AddFavoritePlace(favePlace);
                }
                foreach (var faveRoute in args.AddedFavorites.OfType<FavoriteRoute>())
                {
                    AddFavoriteRoute(faveRoute);
                }
            }

            if (args.RemovedFavorites?.Count > 0)
            {
                foreach (var deletedFave in args.RemovedFavorites.OfType<FavoritePlace>())
                {
                    RemoveFavoritePlace(deletedFave);
                }
                foreach (var deletedRoute in args.RemovedFavorites.OfType<FavoriteRoute>())
                {
                    RemoveFavoriteRoute(deletedRoute);
                }
            }
            if (args.EditedFavorites?.Count > 0)
            {
                foreach (var editedFave in args.EditedFavorites)
                {
                    var toEdit = GroupedFavoritePlaces.FirstOrDefault(x => x.Id == editedFave.Id);
                    if (toEdit != null)
                    {
                        RemoveFavoritePlace(toEdit);
                        AddFavoritePlace(editedFave);
                    }

                    toEdit = GroupedFavoriteRoutes.FirstOrDefault(x => x.Id == editedFave.Id);
                    if (toEdit != null)
                    {
                        RemoveFavoriteRoute(toEdit);
                        AddFavoriteRoute(editedFave);
                    }
                }
            }
        }

        //---These methods happen in reaction to the FavoritesChanged Event.
        private void AddFavoritePlace(IFavorite place)
        {
            GroupedFavoritePlaces.AddSorted(place);
            MappableFavoritePlaces.Add(place as IMapPoi);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void RemoveFavoritePlace(IFavorite deletedFave)
        {
            GroupedFavoritePlaces.Remove(deletedFave);
            MappableFavoritePlaces.Remove(deletedFave as IMapPoi);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void AddFavoriteRoute(IFavorite route)
        {
            GroupedFavoriteRoutes.AddSorted(route);

            var faveRoute = (FavoriteRoute)route;

            IEnumerable<ColoredMapLinePoint> mapPoints = faveRoute.RouteGeometryStrings
                    .SelectMany(str => GooglePolineDecoder.Decode(str))
                    .Select(coords => new ColoredMapLinePoint(coords, HslColors.GetModeColor(ApiMode.Bus)));
            var mapLine = new ColoredMapLine(mapPoints, faveRoute.Id);
            mapLine.OptionalId = faveRoute.Id;
            MappableFavoriteRoutes.Add(mapLine);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }

        private void RemoveFavoriteRoute(IFavorite route)
        {
            GroupedFavoriteRoutes.Remove(route);

            var faveRoute = (FavoriteRoute)route;
            ColoredMapLine toRemove = MappableFavoriteRoutes
                .FirstOrDefault(x => x.OptionalId == faveRoute.Id);
            MappableFavoriteRoutes.Remove(toRemove);

            RaisePropertyChanged(nameof(IsFavoritesEmpty));
        }
        //---End FavoritesChanged Event reactions

        private void SelectionChanged(IList<object> obj)
        {
            SelectedItems = new ObservableCollection<object>(obj);
        }
    }
}