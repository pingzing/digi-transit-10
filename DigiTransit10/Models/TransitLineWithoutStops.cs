﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DigiTransit10.Models.ApiModels.ApiEnums;

namespace DigiTransit10.Models
{
    public class TransitLineWithoutStops
    {
        public ApiMode TransitMode { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
    }
}