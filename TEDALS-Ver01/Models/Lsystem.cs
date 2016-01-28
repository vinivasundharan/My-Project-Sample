using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TEDALS_Ver01.Models
{
    public class Lsystem
    {
        public int LsystemID { get; set; }
        [Display (Name = "System Name") ]
        [Required (ErrorMessage ="System name cannot be empty")]
        //[Remote("DuplicateSystemName", "Lsystems", HttpMethod = "POST", ErrorMessage = "System Name already Exists", AdditionalFields="LsystemID")]
        public string LsystemName { get; set; }
        [Required(ErrorMessage="Material Number cannot be empty")]
        [StringLength(10,MinimumLength=10,ErrorMessage="Material number should be 10 characters long")]
        [Display (Name = "Material Number")]
        public string MaterialNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        [Display(Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }

        public int LsystemFamilyID { get; set; }

        public virtual LsystemFamily LsystemFamily { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}