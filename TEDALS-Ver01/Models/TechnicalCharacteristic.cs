using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class TechnicalCharacteristic
    {
        public int TechnicalCharacteristicID { get; set; }
        [Display (Name = "Technical Characteristic Name")]
        [Required(ErrorMessage="Technical Characteristic name cannot be empty")]
        public string TCName { get; set; }
        [Display (Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display (Name = "Description in German")]
        public string DescriptionDE { get; set; }
         [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
         [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
         [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
         [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }


        public virtual ICollection<TcSet> TcSets { get; set; }
        //public virtual ICollection<Option> Options { get; set; }
    }
}