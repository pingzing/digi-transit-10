using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using System.Collections.Generic;
using Template10.Services.SettingsService;
using Newtonsoft.Json;
using Windows.Globalization;
using Windows.System.UserProfile;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Immutable;

namespace DigiTransit10.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = Application.Current.RequestedTheme;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        //todo: make the list only actually hold PinnedFavoritePlacesNumber of elements
        private List<Guid> _pinnedFavoriteIds;
        public ImmutableList<Guid> PinnedFavoriteIds
        {
            get
            {
                if (_pinnedFavoriteIds == null)
                {
                    string serialized = _helper.Read(nameof(PinnedFavoriteIds), "", SettingsStrategies.Roam);
                    if (String.IsNullOrEmpty(serialized) || serialized == "null")
                    {
                        _pinnedFavoriteIds = new List<Guid>();
                        return _pinnedFavoriteIds.ToImmutableList();
                    }
                    else
                    {
                        _pinnedFavoriteIds = JsonConvert.DeserializeObject<List<Guid>>(serialized);
                        return _pinnedFavoriteIds.ToImmutableList();
                    }
                }
                else
                {
                    return _pinnedFavoriteIds.ToImmutableList();
                }
            }
            set
            {
                _pinnedFavoriteIds = value.ToList();
                _helper.Write(nameof(PinnedFavoriteIds), JsonConvert.SerializeObject(_pinnedFavoriteIds), SettingsStrategies.Roam);
            }
        }

        /// <summary>
        /// Attempts to add the specified Favorite ID to the list of Pinned FavoriteIds. If this causes
        /// the size of the Pinned FavoriteIds list to exceed the maximum number as defined by PinnedFavoritePlacesDisplayNumber,
        /// then older Pinned FavoriteIds will be pushed out of the list to make room.
        /// </summary>
        /// <param name="newId"></param>
        public void PushFavoriteId(Guid newId)
        {
            int maxAllowed = PinnedFavoritePlacesDisplayNumber;

            //We're using the Property accessor here rather than the backing value to ensure that the backing value is initialized and up to date.
            int newCount = PinnedFavoriteIds.Count + 1; 
            int numToRemove = newCount - maxAllowed;
            for(int i = 0; i < numToRemove; i++)
            {
                _pinnedFavoriteIds.Remove(_pinnedFavoriteIds.First());
            }

            _pinnedFavoriteIds.Add(newId);
            FlushPinnedFavoriteIdsToStorage();
        }

        /// <summary>
        /// Removed the FavoriteId from the list of Pinned FavoriteIds.
        /// </summary>
        /// <param name="removedId"></param>
        public void RemovedFavoriteId(Guid removedId)
        {
            _pinnedFavoriteIds.Remove(removedId);
            FlushPinnedFavoriteIdsToStorage();
        }

        public void FlushPinnedFavoriteIdsToStorage()
        {
            _helper.Write(nameof(PinnedFavoriteIds), JsonConvert.SerializeObject(_pinnedFavoriteIds), SettingsStrategies.Roam);
        }

        /// <summary>
        /// The number of favorites to show in the Pinned Favorites on MainPage. Defaults to 3.
        /// </summary>
        public int PinnedFavoritePlacesDisplayNumber
        {
            //local, because this is going to differ greatly on display size, and roaming it doesn't make a lot of sense.
            get { return _helper.Read(nameof(PinnedFavoritePlacesDisplayNumber), 3, SettingsStrategies.Local); }
            set { _helper.Write(nameof(PinnedFavoritePlacesDisplayNumber), value, SettingsStrategies.Local); }
        }

        //todo: make this persist somehow
        public string CurrentLanguage
        {
            get
            {
                string langOverride = ApplicationLanguages.PrimaryLanguageOverride;
                if(String.IsNullOrWhiteSpace(langOverride))
                {
                    return ApplicationLanguages.Languages.FirstOrDefault() ?? "en-US";
                }
                else
                {
                    return ApplicationLanguages.PrimaryLanguageOverride;
                }
            }
            set
            {
                ApplicationLanguages.PrimaryLanguageOverride = value;
                //do something to reload values?
            }
        }
    }
}

