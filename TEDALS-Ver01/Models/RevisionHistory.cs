using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class RevisionHistory
    {
        public int RevisionHistoryID { get; set; }
        
        [Display(Name="System name")]
        public string SystemName { get; set; }
        [Display(Name="Option")]
        public string Option { get; set; }
        [Display(Name = "Option value")]
        public string Optionvalue { get; set; }
        [Display(Name="TC Property")]
        public string TCSetName { get; set; }
        [Display(Name= "Initial Value")]
        public string InitialValue { get; set; }
        [Display(Name = "Modified Value")]
        public string ModifiedValue { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        
        public int SetValueID { get; set; }
        public string Action { get; set; }
    }
}