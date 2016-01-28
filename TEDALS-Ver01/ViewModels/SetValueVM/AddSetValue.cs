using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class AddSetValue
    {
        public virtual SetValue SetValue { get; set; }
        public virtual ICollection<TcSet> TcSet { get; set; }
    }
}