using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    public class Option_OptionVal
    {
        public virtual Option Option {get;set;}
        public List<OptionValue> OptionValue {get;set;}
    }
    public class ViewsResultWithout
    {
        public virtual Lsystem Lsystem { get; set; }
        public virtual List<Option_OptionVal> Option_OptionVal { get; set; }
        public virtual ViewsResult ViewsResult { get; set; }
    }
}