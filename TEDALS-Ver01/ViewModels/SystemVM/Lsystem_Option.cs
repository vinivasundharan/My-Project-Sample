using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class Lsystem_Option
    {
        public virtual IEnumerable<Option> Options { get; set; }
        public virtual Lsystem Lsystem { get; set; }
    }
}