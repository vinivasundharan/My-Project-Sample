using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace TEDALS_Ver01.Models
{
    public class LsystemFamily
    {
        //[HiddenInput(DisplayValue = false)]
        public int LsystemFamilyID { get; set; }
        [Required(ErrorMessage="System Family Name is required")]
        [Display (Name = "Family Name")]
        //[Remote("DuplicateFamilyName","LsystemFamilies",HttpMethod = "POST",ErrorMessage= "System Family Name already Exists",AdditionalFields= "LsystemFamilyID")]
        //[Unique(ErrorMessage = "Family Name Already Exists")]
        public string FamilyName { get; set; }
        [Display (Name = "System Count")]
        public int LsystemCount { get; set; }
        [Display ( Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display (Name = "Description in German")]
        public string DescriptionDE { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<Lsystem> Lsystems { get; set; }
    }
}