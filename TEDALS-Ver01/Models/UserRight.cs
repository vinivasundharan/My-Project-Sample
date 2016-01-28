using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class UserRight
    {
        public int UserRightID { get; set; }
        [Required(ErrorMessage="User Name cannot be empty")]
        [Display(Name="User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage="User Code cannot be empty")]
        [Display(Name = "User Code")]
        public string UserCode { get; set; }
        [Display(Name="Reader")]
        public bool IsReader { get; set; }
        [Display(Name="Editor")]
        public bool IsEditor { get; set; }
        [Display(Name="Exporter")]
        public bool IsExporter { get; set; }
        [Display(Name="Admin")]
        public bool IsAdmin { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

    }
}