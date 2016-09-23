﻿using DigiTransit10.Models;
using DigiTransit10.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Maps;
using DigiTransit10.ExtensionMethods;
using System.Threading.Tasks;
using DigiTransit10.ViewModels.ControlViewModels;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigiTransit10.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        public FavoritesViewModel ViewModel => this.DataContext as FavoritesViewModel;

        public FavoritesPage()
        {
            this.InitializeComponent();

            //Doing this in code-behind, because doing it in XAML breaks the XAML designer.
            var collectionViewSourceBinding = new Binding();
            collectionViewSourceBinding.Source = ViewModel.Favorites;
            collectionViewSourceBinding.Mode = BindingMode.OneWay;

            BindingOperations.SetBinding(FavoritesViewSource,
                CollectionViewSource.SourceProperty,
                collectionViewSourceBinding);
        }

        private void FavoritesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var list = sender as ListView;
            var tappedItem = (IFavorite)e.ClickedItem;
            if (list == null)
            {
                return;
            }

            if(list.SelectionMode == ListViewSelectionMode.Multiple)
            {
                return;
            }

            if (ViewModel.SetAsDestinationCommand.CanExecute(tappedItem))
            {
                ViewModel.SetAsDestinationCommand.Execute(tappedItem);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var boundingBox = this.FavoritesMap.GetAllMapElementsBoundingBox();
            if (boundingBox != null)
            {
                await this.FavoritesMap.TrySetViewBoundsAsync(boundingBox, new Thickness(450, 50, 50, 50), MapAnimationKind.None);
            }
        }
    }
}
