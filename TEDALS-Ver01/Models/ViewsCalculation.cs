using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class ViewsCalculation
    {
        public int ViewsCalculationID { get; set; }
        public int ViewsID { get; set; }
        public int CalculationID { get; set; }
        public virtual Views Views { get; set; }
        public virtual Calculation Calculation { get; set; }
    }
}