using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEDALS_Ver01.Models
{
    public class Option
    {
        public int OptionID { get; set;}
        [Display (Name = "Option Type")]
        [Required(ErrorMessage="Option cannot be empty")]
        public string OptionName { get; set; }
        [Display (Name ="Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        [Required(ErrorMessage="Technical Characteristic cannot be empty")]
        public int TechnicalCharacteristicID { get; set; }
        public int LsystemID { get; set; }

        public virtual ICollection<OptionValue> OptionValues { get; set; }
        public virtual TechnicalCharacteristic TechnicalCharacteristic { get; set; }
        public virtual Lsystem Lsystem { get; set; }
       // public virtual ICollection< SetValue> SetValue { get; set; }
    }
}