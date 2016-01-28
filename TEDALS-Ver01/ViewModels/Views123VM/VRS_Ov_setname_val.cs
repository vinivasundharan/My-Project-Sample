using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.Views123
{
    //Used in ViewResultSummarize (Function not used)
    public class Set_Val
    {
        public string setname { get; set; }
        public string value { get; set; }
    }
    public class VRS_Ov_setname_val
    {
        public int OptionValueID { get; set; }
        public List<Set_Val> Set_val_list { get; set; }
    }
}