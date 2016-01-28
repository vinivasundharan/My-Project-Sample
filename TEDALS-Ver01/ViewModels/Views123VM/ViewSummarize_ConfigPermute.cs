using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.Views123
{
    //Used in ViewSummarize (get the list of all option value permutations of each configurations)
    public class ViewSummarize_ConfigPermute
    {
        public int ConfigID { get; set; }
        public List<string> OptionValCombi {get;set;}
    }
}