using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class Option_OptionValues
    {
        public Option_OptionValues()
        {
           // this.Option = new Option();
            this.OptionValues = new List<OptionValue>();
            //this.OptionValues.First().SetValue = new List<SetValue>();
        }
        public Option Option { get; set; }
        public IEnumerable<OptionValue> OptionValues { get; set; }
    }
}