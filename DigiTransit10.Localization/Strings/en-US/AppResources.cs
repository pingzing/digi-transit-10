//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------
// <auto-generatedInfo>
// 	This code was generated by ResW File Code Generator (http://reswcodegen.codeplex.com)
// 	ResW File Code Generator was written by Christian Resma Helle
// 	and is under GNU General Public License version 2 (GPLv2)
// 
// 	This code contains a helper class exposing property representations
// 	of the string resources defined in the specified .ResW file
// 
// 	Generated: 07/06/2016 22:55:24
// </auto-generatedInfo>
// --------------------------------------------------------------------------------------------------
namespace DigiTransit10.Localization.Strings
{
    using Windows.ApplicationModel.Resources;
    
    
    public partial class AppResources
    {
        
        private static ResourceLoader resourceLoader;
        
        static AppResources()
        {
            string executingAssemblyName;
            executingAssemblyName = Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName;
            string[] executingAssemblySplit;
            executingAssemblySplit = executingAssemblyName.Split(',');
            executingAssemblyName = executingAssemblySplit[1];
            string currentAssemblyName;
            currentAssemblyName = typeof(AppResources).AssemblyQualifiedName;
            string[] currentAssemblySplit;
            currentAssemblySplit = currentAssemblyName.Split(',');
            currentAssemblyName = currentAssemblySplit[1];
            if (executingAssemblyName.Equals(currentAssemblyName))
            {
                resourceLoader = ResourceLoader.GetForCurrentView("AppResources");
            }
            else
            {
                resourceLoader = ResourceLoader.GetForCurrentView(currentAssemblyName + "/AppResources");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "You haven't saved any favorites yet"
        /// </summary>
        public static string Favorites_EmptyList
        {
            get
            {
                return resourceLoader.GetString("Favorites_EmptyList");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Favorites"
        /// </summary>
        public static string Favorites_FavoritesHeader
        {
            get
            {
                return resourceLoader.GetString("Favorites_FavoritesHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Date"
        /// </summary>
        public static string LiteralDate
        {
            get
            {
                return resourceLoader.GetString("LiteralDate");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "From"
        /// </summary>
        public static string MainPage_FromHeader
        {
            get
            {
                return resourceLoader.GetString("MainPage_FromHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Trip Planning"
        /// </summary>
        public static string MainPage_Header
        {
            get
            {
                return resourceLoader.GetString("MainPage_Header");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "To"
        /// </summary>
        public static string MainPage_ToHeader
        {
            get
            {
                return resourceLoader.GetString("MainPage_ToHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Addresses"
        /// </summary>
        public static string SuggestBoxHeader_Addresses
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_Addresses");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Transit Stops"
        /// </summary>
        public static string SuggestBoxHeader_TransitStops
        {
            get
            {
                return resourceLoader.GetString("SuggestBoxHeader_TransitStops");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Arrival"
        /// </summary>
        public static string TimeType_Arrival
        {
            get
            {
                return resourceLoader.GetString("TimeType_Arrival");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Departure"
        /// </summary>
        public static string TimeType_Departure
        {
            get
            {
                return resourceLoader.GetString("TimeType_Departure");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Arrival or departure time?"
        /// </summary>
        public static string TimeType_PlaceholderText
        {
            get
            {
                return resourceLoader.GetString("TimeType_PlaceholderText");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Plan a trip"
        /// </summary>
        public static string TripForm_PlanATripHeader
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanATripHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Planning..."
        /// </summary>
        public static string TripForm_PlanningTrip
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanningTrip");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Plan"
        /// </summary>
        public static string TripForm_PlanTripButton
        {
            get
            {
                return resourceLoader.GetString("TripForm_PlanTripButton");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Resolving locations..."
        /// </summary>
        public static string TripFrom_GettingsAddresses
        {
            get
            {
                return resourceLoader.GetString("TripFrom_GettingsAddresses");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Trip plans"
        /// </summary>
        public static string TripResults_TripPlansHeader
        {
            get
            {
                return resourceLoader.GetString("TripResults_TripPlansHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Fewer options"
        /// </summary>
        public static string TripForm_FewerOptions
        {
            get
            {
                return resourceLoader.GetString("TripForm_FewerOptions");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "More options"
        /// </summary>
        public static string TripForm_MoreOptions
        {
            get
            {
                return resourceLoader.GetString("TripForm_MoreOptions");
            }
        }
    }
}
