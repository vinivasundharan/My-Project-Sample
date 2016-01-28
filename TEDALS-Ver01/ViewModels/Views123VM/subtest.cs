using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Used in TestView (Function not used)
    public class subtest
    {
        public int TcSetID { get; set; }
        public virtual TcSet TcSet { get; set; }

        public virtual List<string> Values { get; set; }
    }
}