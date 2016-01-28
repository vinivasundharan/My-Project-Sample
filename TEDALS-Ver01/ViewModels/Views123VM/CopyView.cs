using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class CopyView
    {
        public int CopyViewID { get; set; }
        [Display(Name="Views")]
        public int ViewsID { get; set; }
        public virtual Views Views {get;set;}
         [Display(Name = "Views Name")]
        [Required(ErrorMessage="View Name cannot be empty")]
        public string ViewNewName { get; set; }
         [Display(Name = "Description in English")]
        public string DescriptionEN { get; set; }
         [Display(Name = "Description in German")]
        public string DescriptionDE { get; set; }
    }
}