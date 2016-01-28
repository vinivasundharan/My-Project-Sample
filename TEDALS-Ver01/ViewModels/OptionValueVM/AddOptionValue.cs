using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class AddOptionValue
    {
        public virtual OptionValue OptionValue { get; set; }
        public virtual SetValue SetValue { get; set; }
        public virtual TechnicalCharacteristic TechnicalCharacteristic { get; set; }
        public virtual Option Option { get; set; }
        public virtual IEnumerable<TcSet> TcSets { get; set; }
    }
}