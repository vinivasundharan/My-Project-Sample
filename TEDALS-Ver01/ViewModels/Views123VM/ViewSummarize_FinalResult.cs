using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.Views123
{
    //Used in ViewSummarize (Corresponds to one row in the display table)
    public class Option_Optionvalue_Summarize 
    {
        public string OptionName {get;set;}
        public List<string> OptionValues{get;set;}
    }
    public class ViewSummarize_FinalResult
    {
        public List<Option_Optionvalue_Summarize> Op_ov_summarize { get; set; }
        public string SystemName { get; set; }
        public List<string> Values { get; set; }
        public List<string> Property { get; set; }
    }
}