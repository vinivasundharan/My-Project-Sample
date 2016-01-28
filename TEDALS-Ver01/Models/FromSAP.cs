using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class FromSAP
    {
        public virtual int Id { get; set; }
        public string MaterialNumber { get; set; }
        public string OptionName { get; set; }
        public string OptionValue { get; set; }
        public string Description { get; set; }
    }
}