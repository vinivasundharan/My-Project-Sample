using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in ViewSystemValues(Function not used)
    public class ViewSystemValuesVM
    {
        
        public int TcSetID { get; set; }
        public string Value { get; set; }

        public virtual SetValue SetValue { get; set;}


    }
}