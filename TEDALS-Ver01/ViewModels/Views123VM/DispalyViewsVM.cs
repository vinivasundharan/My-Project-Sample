using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in DisplayViews
    public class DispalyViewsVM
    {
        public virtual Views Views { get; set; }
        public virtual List<TcSet> tcSet { get; set; }
        public virtual List<TEDALS_Ver01.Models.Calculation> Calculation { get; set; }
    }
}