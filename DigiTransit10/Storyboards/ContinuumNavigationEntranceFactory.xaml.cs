﻿using DigiTransit10.ExtensionMethods;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DigiTransit10.Storyboards
{
    public sealed partial class ContinuumNavigationEntranceFactory : ResourceDictionary
    {
        private ContinuumNavigationEntranceFactory(FrameworkElement mover)
        {
            this.InitializeComponent();           
                        
            mover.RenderTransform = new CompositeTransform();
                        
            Storyboard.SetTarget(this.ScaleXComponent, mover);
            Storyboard.SetTarget(this.ScaleYComponent, mover);
            Storyboard.SetTarget(this.TranslateXComponent, mover);
            Storyboard.SetTarget(this.TranslateYComponent, mover);                       
        }

        public static Storyboard GetAnimation(FrameworkElement mover)
        {
            return new ContinuumNavigationEntranceFactory(mover).ContinuumNavigationEntranceStoryboard;
        }
    }
}