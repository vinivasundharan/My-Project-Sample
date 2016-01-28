using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class View_TC
    {
        public virtual Views Views { get; set; }
        public virtual List<TechnicalCharacteristic> TechnicalCharacteristic { get; set; }
    }
}