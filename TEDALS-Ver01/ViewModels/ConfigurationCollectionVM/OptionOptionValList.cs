using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.ConfigurationCollection
{
    public class OptionOptionValList
    {
        public string OptionName { get; set; }
        public List<string> Optionvalue { get; set; }
    }

    public class ConfigAllList
    {
        public string ConfigurationName { get; set; }
        public int ConfigID { get; set; }
        public virtual List<OptionOptionValList> OpOvList { get; set; }
    }
}