using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class ConfigurationCollection
    {
        public int ConfigurationCollectionID { get; set; }
        [Display(Name = "Configuration Collection Name")]
        [Required(ErrorMessage="Collection Name cannot be empty")]
        public string CollectionName { get; set; }
        [Display(Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display(Name="Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "System Name")]
        public int LsystemID { get; set; }

        public virtual Lsystem Lsystem { get; set; }
        public virtual ICollection<Option> Options { get; set; }
        public virtual ICollection<OptionValue> OptionValues { get; set; }
    }
}