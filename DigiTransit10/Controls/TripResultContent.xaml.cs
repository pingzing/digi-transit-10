﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DigiTransit10.Helpers;
using DigiTransit10.ViewModels;
using GalaSoft.MvvmLight.Messaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Controls
{
    public sealed partial class TripResultContent : UserControl, INotifyPropertyChanged
    {
        private const int TripListStateIndex = 0;
        private const int DetailedStateIndex = 1;

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
           this.DataContextChanged += (s, e) => RaisePropertyChanged(nameof(ViewModel));
            Messenger.Default.Register<MessageTypes.ViewPlanDetails>(this, SwitchToDetailedState);
            this.FloatingPanelOpenStoryboard.Completed += FloatingPanelOpenStoryboard_Completed;
        }        

        private void SwitchToDetailedState(MessageTypes.ViewPlanDetails obj)
        {
            if (TripStateGroup.CurrentState == TripStateGroup.States[TripListStateIndex]
                || TripStateGroup.CurrentState == null)
            {
                VisualStateManager.GoToState(this, this.TripStateGroup.States[DetailedStateIndex].Name, true);
                DetailedTripList.ItemsSource = obj.BackingModel.BackingItinerary.Legs;
            }
            else if(TripStateGroup.CurrentState == TripStateGroup.States[DetailedStateIndex])
            {
                VisualStateManager.GoToState(this, this.TripStateGroup.States[TripListStateIndex].Name, true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void GridGrabHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.FloatingPanelOpenStoryboard.Begin();
        }

        private void FloatingPanelOpenStoryboard_Completed(object sender, object e)
        {
            ((CompositeTransform) this.DirectionsFloatingPanel.RenderTransform).TranslateY = 0;
            this.DirectionsFloatingPanel.MaxHeight += 200;
            this.DirectionsFloatingPanel.Height = this.DirectionsFloatingPanel.ActualHeight + 200;
        }
    }
}