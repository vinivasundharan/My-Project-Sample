using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in testView(Function not used)
    public class testmain
    {
        public int ConfigurationCollectionID { get; set; }
        public virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigurationCollection { get; set; }

        public virtual List<subtest> subset { get; set; }
    }
}