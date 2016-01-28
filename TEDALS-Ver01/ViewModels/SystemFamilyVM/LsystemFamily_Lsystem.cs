using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in ViewSystems
    public class LsystemFamily_Lsystem
    {
        public LsystemFamily_Lsystem()
        {
            this.Lsystems = new List<Lsystem>().OrderByDescending(x=>x.LsystemName);
        }
        public IEnumerable<Lsystem> Lsystems { get; set; }
        public LsystemFamily LsystemFamily { get; set; }
    }
}