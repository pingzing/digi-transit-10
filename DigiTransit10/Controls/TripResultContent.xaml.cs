﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultContent : UserControl, INotifyPropertyChanged
    {
        private const int TripListStateIndex = 0;
        private const int DetailedStateIndex = 1;
        private readonly VisualState _tripListState;
        private readonly VisualState _detailedTripState;

        public TripResultViewModel ViewModel => DataContext as TripResultViewModel;

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TripResultContent), new PropertyMetadata(null));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public TripResultContent()
        {
            this.InitializeComponent();
            _tripListState = TripStateGroup.States[TripListStateIndex];
            _detailedTripState = TripStateGroup.States[DetailedStateIndex];
            VisualStateManager.GoToState(this, _tripListState.Name, false);

            this.DataContextChanged += (s, e) => RaisePropertyChanged(nameof(ViewModel));            
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);            
        }        

        private void SwitchToDetailedState(MessageTypes.ViewPlanDetails obj)
        {
            if (TripStateGroup.CurrentState == _tripListState)
            {                               
                VisualStateManager.GoToState(this, _detailedTripState.Name, true);                
                DetailedTripList.ItemsSource = obj.BackingModel.BackingItinerary.Legs.Select(x => 
                {
                    var listLeg = DetailedTripListLeg.FromApiLeg(x);
                    if(obj.BackingModel.BackingItinerary.Legs.Last() == x)
                    {
                        listLeg.IsEnd = true;
                        listLeg.ToName = obj.BackingModel.EndingPlaceName;
                    }
                    return listLeg;
                });
            }
            else if(TripStateGroup.CurrentState == _detailedTripState)
            {                
                VisualStateManager.GoToState(this, _tripListState.Name, true);
            }
        }

        private async void DirectionsFloatingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
            this.SizeChanged += TripResultContent_SizeChanged;

            //await adjust map view bounds
        }

        private void TripResultContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DirectionsFloatingPanel.ExpandedHeight = this.ActualHeight * .66;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void DetailedTripList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ListView list = sender as ListView;
            if (list == null)
            {
                return;
            }
            var element = list.ContainerFromItem(e.ClickedItem);
            var intermediatesControl = element.FindChild<TripDetailListIntermediates>("IntermediateStops");
            intermediatesControl.ToggleViewState();
        }
    }
}
