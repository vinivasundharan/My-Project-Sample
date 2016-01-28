using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;
using TEDALS_Ver01.ViewModels.ConfigurationCollection;

namespace TEDALS_Ver01.ViewModels
{
    //Editing a configuration collection
    public class EditConfigVM
    {
        public virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigCol {get;set;}
        public List<OptionValue> selected { get; set; }
        public List<Option> Options { get ; set; }
     
    }
}