using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEDALS_Ver01.Models
{
   
    public class DataFormat
    {
        public int DataFormatID { get; set; }
        [Display (Name = "Data Format Name")]
        [Required (ErrorMessage ="Name cannot be empty")]
        //[Remote("DuplicateFormatName", "DataFormats", HttpMethod = "POST", ErrorMessage = "Data Format Name already Exists")]
        public string FormatName { get; set; }
        [Display (Name = "Data Format Type")]
        [Required (ErrorMessage = "Format type cannot be empty")]
        public string FormatType { get; set; }
        [Display (Name = "Precision Digits")]
        public int? PrecisionDigits { get; set; }
        [Display (Name = "Scaling Digits")]
        public int? ScalingDigits { get; set; }
        [Display (Name = "Description in German")]
        public string DescriptionDE { get; set; }
        [Display (Name = "Description in English")]
        public string DescriptionEN { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set;}
        public string ModifiedBy { get; set; }

        public virtual ICollection<TcSet> TcSets { get; set; }
    }
}