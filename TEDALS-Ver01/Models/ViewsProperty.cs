using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class ViewsProperty
    {
        public int ViewsPropertyID { get; set; }
        public int ViewsID { get; set; }
        public int TcSetID { get; set; }

        public virtual Views Views { get; set; }
        public virtual TcSet TcSet { get; set; }
    }
}