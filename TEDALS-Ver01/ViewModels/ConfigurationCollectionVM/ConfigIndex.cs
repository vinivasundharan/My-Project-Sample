using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class ConfigIndex
    {
        public virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigurationCollection {get;set;}
        public List<LsystemFamily> LsystemFamily { get; set; }
        public SelectList Lsystem { get; set; }
        public virtual IEnumerable<Option> Option { get; set; }

    }
}