using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels
{
    //Used in class config_combi_VM
    public class Configcombi
    {
        public string ConfigurationName { get; set; }
        public List<Tuple<string,string>> OptionValues { get; set; }
    }
}