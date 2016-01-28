using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class LsystemCheckbox
    {
        public virtual Lsystem Lsystem { get; set; }
        public bool check{get; set;}
        public int LsystemID { get; set; }
        public string LsystemName { get; set; }
    }
}