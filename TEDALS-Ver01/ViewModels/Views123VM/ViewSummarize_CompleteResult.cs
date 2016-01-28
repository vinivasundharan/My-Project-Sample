using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.Views123
{
    //used in ViewSummarize
    public class Result_EachRow
    {
        public string setname { get; set; }
        public string value {get;set;}
        public int OptionValID { get; set; }
    }
    public class ViewSummarize_CompleteResult
    {
        public List<Result_EachRow> EachRow {get;set;}

    }
}