using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{

    //Not Used
    public class Config_Create
    {
        public int ConfigurationCollectionID { get; set; }
        public virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigurationCollection { get; set; }
        public LsystemFamily LsystemFamily { get; set; }
        public Lsystem Lsystem { get; set; }
        public IEnumerable<Option> Option { get; set; }
    }
}