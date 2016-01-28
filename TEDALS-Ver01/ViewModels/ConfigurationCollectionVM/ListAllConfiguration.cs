using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //In function List all Configuration
    public class ListAllConfiguration
    {
        public virtual List<ListConfiguration> ListConfiguration { get; set; }
        public  virtual TEDALS_Ver01.Models.ConfigurationCollection ConfigurationCollection {get;set;}
    }
}