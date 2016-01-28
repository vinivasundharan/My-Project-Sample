using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class Config_OptionVal
    {
        public int Config_OptionValID { get; set; }
        public int OptionValueID { get; set; }
        public int ConfigurationCollectionID { get; set; }
        public int OptionID { get; set; }
        //public bool OptionValChecked { get; set; }

        
        public virtual OptionValue OptionValue { get; set; }
        public virtual ConfigurationCollection ConfigurationCollection { get; set; }
    }
}