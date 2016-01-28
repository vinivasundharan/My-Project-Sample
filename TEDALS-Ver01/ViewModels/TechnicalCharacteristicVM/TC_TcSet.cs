using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in ViewTcSet
    public class TC_TcSet
    {
        public TechnicalCharacteristic TechnicalCharacteristic { get; set; }
        public IEnumerable<TcSet> TcSet { get; set; }
    }
}