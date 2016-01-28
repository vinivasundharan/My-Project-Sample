using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels
{
    //used in ViewResultsWithout
    public class Option_Opval
    {
        public string Option {get;set;}
        public string OptionVal {get;set;}
    }
    public class Calc_Ans
    {
        public string CalcName { get; set; }
        public string Ans { get; set; }
    }
    public class View_Without
    {
        public string Systemname { get; set; }
        public List<Option_Opval> Option_Opval {get;set;}
        public List<string> PropValues { get; set; }
        public List<Calc_Ans> CalcAns { get; set; }
    }
}