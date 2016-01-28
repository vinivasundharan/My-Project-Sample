using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TEDALS_Ver01.ViewModels;

namespace TEDALS_Ver01.Models
{
    public class Views
    {
        public int ViewsID { get; set; }
        [Display(Name="View Name") ]
        [Required (ErrorMessage="View name cannot be empty")]
        public string ViewsName { get; set; }
        [Display(Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
       // public virtual List<TechnicalCharacteristic>  TechnicalCharacteristic { get; set; }

    }
}