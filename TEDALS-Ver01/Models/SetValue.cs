using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class SetValue
    {
        public int SetValueID { get; set; }
        [Display(Name = "Value")]
        [Required(ErrorMessage="Property value cannot be empty")]
        public string Value { get; set; }
        [Display(Name="Internal")]
        public bool Status { get; set; }
        [Display(Name="Created On")]
        public DateTime CreatedOn {get;set;}
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        //public int LsystemID { get; set; }
        //public int OptionID { get; set; }
        public int TcSetID { get; set; }
        public int OptionValueID { get; set; }

        //public virtual Lsystem Lsystem { get; set; }
        //public virtual Option Option { get; set; }
        public virtual OptionValue OptionValue { get; set; }
        public virtual TcSet TcSet { get; set; }

    }
}