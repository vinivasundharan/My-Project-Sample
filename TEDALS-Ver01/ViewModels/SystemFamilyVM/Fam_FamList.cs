using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class Fam_FamList
    {
        public virtual LsystemFamily LsystemFamily { get; set; }
        public virtual IList<LsystemFamily> LsystemFamilies { get; set; }
    }
}