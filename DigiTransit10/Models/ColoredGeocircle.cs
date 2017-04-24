﻿using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace DigiTransit10.Models
{
    public class ColoredGeocircle
    {
        public static readonly Color DefaultColor = new Color { A = 128, R = Colors.Gray.R, G = Colors.Gray.G, B = Colors.Gray.B };

        public IEnumerable<Geopoint> CirclePoints { get; private set; }
        public Color FillColor { get; private set; }
        public Color StrokeColor { get; private set; }
        public double StrokeThickness { get; private set; }

        public ColoredGeocircle(IEnumerable<Geopoint> circlePoints)
        {
            CirclePoints = circlePoints;
            FillColor = DefaultColor;
            StrokeColor = DefaultColor;
            StrokeThickness = 2;
        }

        public ColoredGeocircle(IEnumerable<Geopoint> circlePoints, Color fillColor)
        {
            CirclePoints = circlePoints;
            FillColor = fillColor;
            StrokeColor = DefaultColor;
            StrokeThickness = 2;
        }        

        public ColoredGeocircle(IEnumerable<Geopoint> circlePoints, Color fillColor, Color strokeColor, double strokeThickness = 2)
        {
            CirclePoints = circlePoints;
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;
        }
    }
}
