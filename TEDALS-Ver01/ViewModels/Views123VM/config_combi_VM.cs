using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;
using TEDALS_Ver01.ViewModels.ConfigurationCollection;

namespace TEDALS_Ver01.ViewModels
{
    //used in ViewsResultWithout (View without summarize
    //Get all permutation combination of configuration collection
    public class config_combi_VM
    {
        public virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigurationCollection { get; set; }
        public List<string> configcombi { get; set; }
    }
}