using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in Edit View
    public class EditViewsVM
    {
        //public int ViewsID { get; set; }
        //[Display(Name = "View Name")]
        //[Required(ErrorMessage = "View name cannot be empty")]
        //public string ViewsName { get; set; }
        //[Display(Name = "Description in English")]
        //public string DescriptionEN { get; set; }
        //[Display(Name = "Description in German")]
        //public string DescriptionDE { get; set; }
        public virtual Views Views12 { get; set; }
        public List<TcSet> TcSet { get; set; }
        public List<TEDALS_Ver01.Models.Calculation> Calculation { get; set; }

    }
}