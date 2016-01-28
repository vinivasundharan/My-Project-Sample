using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class OptionValue_SetVal
    {
        public OptionValue OptionValue { get; set; }
        public IEnumerable<SetValue> SetValues { get; set; }
    }
}