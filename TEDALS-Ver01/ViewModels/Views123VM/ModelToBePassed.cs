using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in ViewSystemValue (Function not required)
    public class ModelToBePassed
    {
        public virtual List<ViewsVM> ViewsVM { get; set;}
        public List<TcSet> TcSet { get; set; }
    }
}