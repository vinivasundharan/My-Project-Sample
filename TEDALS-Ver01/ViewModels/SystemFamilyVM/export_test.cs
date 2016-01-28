using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class export_test
    {
        public virtual LsystemFamily LsystemFamily { get; set; }
        public virtual List<Lsystem> Lsystem {get;set;}
        public virtual List<Option> Option { get; set; }
        public virtual List<OptionValue> OptionValue { get; set; }
        public virtual List<SetValue> SetValue { get; set; }
    }
}