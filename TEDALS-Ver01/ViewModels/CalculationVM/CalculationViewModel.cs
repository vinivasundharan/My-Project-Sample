using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    public class CalculationViewModel
    {
        //public List<FamilyName>   { get; set; }
        public string FamilyName { get; set; }
        public bool isSelected { get; set; }

        public virtual ICollection<LsystemFamily> LsystemFamily { get; set; }
        //public virtual Calculation Calculation { get; set; }

    }
}