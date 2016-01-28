using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{

    public class Calculation
    {
        public int CalculationID { get; set; }
        [Required(ErrorMessage="Calculation Name cannot be empty")]
        [Display (Name="Calculation Name")]
        public string CalculationName { get; set; }
        [Required(ErrorMessage = "Formula cannot be empty")]
        [Display (Name = "Formula")]
        [DataType(DataType.MultilineText)]
        public string CalculationFormula { get; set; }
        [Display(Name = "Physical Unit")]
        [Required(ErrorMessage="Physical Unit cannot be empty")]
        public string PhysicalUnit { get; set; }
        [Display(Name = "Data Status")]
        public DataStatus DataStatus { get; set; }
        [Display(Name = "Desription in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name="Data Format")]
        public int DataFormatID { get; set; }

        public virtual DataFormat DataFormat {get;set;}
    }
}