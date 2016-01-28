using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class test2
    {
        public virtual Option Option { get; set; }
        public virtual TcSet TcSet { get; set; }
        public virtual IEnumerable<SetValue> SetValue { get; set; }
        
    }
}