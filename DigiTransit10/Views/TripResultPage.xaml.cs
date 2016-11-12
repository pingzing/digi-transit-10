﻿using DigiTransit10.Storyboards;
using DigiTransit10.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripResultPage : Page
    {
        public TripResultViewModel ViewModel => DataContext as TripResultViewModel;

        public TripResultPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200); //delay while the page finishes animating
            ToTextBlock.Opacity = 1;
            Storyboard storyboard = ContinuumNavigationEntranceFactory.GetAnimation(ToTextBlock);
            storyboard.Begin();
        }
    }
}
