﻿using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models.ApiModels;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models
{
    public class TransitLine : ITransitLine
    {
        public ApiMode TransitMode { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string GtfsId { get; set; }

        public IEnumerable<TransitStop> Stops { get; set; }
        public IEnumerable<BasicGeoposition> Points { get; set; }        

        public TransitLine() { }

        public TransitLine(ApiRoute route)
        {
            TransitMode = route.Mode;
            ShortName = route.ShortName;
            LongName = route.LongName;
            GtfsId = route.GtfsId;
            Stops = route.Patterns
                .FirstOrDefault()
                ?.Stops
                ?.Select(x => new TransitStop
                    {
                        Coords = BasicGeopositionExtensions.Create(0.0, x.Lon, x.Lat),
                        Name = x.Name,
                        Code = x.Code
                    });
            Points = route.Patterns
                .FirstOrDefault()
                ?.Geometry
                .Select(x => BasicGeopositionExtensions.Create(0.0, x.Lon, x.Lat));
        }
    }
}
