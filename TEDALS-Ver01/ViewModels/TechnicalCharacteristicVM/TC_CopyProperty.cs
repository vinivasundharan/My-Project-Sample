using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class TC_CopyProperty
    {
        public virtual TechnicalCharacteristic TC { get; set; }
        public virtual List<TcSet> TcSet { get; set; }
    }
}