using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //In fucntion List configuration
    public class ListConfiguration
    {
        public virtual Option Option { get; set; }
        public virtual List<OptionValue> OptionValue { get; set; }
        
    }
}