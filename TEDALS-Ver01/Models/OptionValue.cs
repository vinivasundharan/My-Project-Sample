using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class OptionValue
    {
        public int OptionValueID { get; set; }
        [Required(ErrorMessage="Option Value cannot  be empty")]
        [Display(Name = "Option Value")]
        public string OptionVal { get; set; }
        [Display (Name="Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display (Name="Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name="Description in German")]
        public string DescriptionDE { get; set; }
        public int OptionID { get; set; }
        //public int ConfigurationCollectionID { get; set; }
       // public int SetValueID { get; set; }

        public virtual ICollection<ConfigurationCollection> ConfigurationCollections { get; set; }
        public virtual Option Option { get; set; }
        public virtual ICollection< SetValue> SetValue { get; set; }
    }
}