using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels
{
    //Used in ViewsResult (Function not used)
    public class ViewsResult_Complete
    {
        public virtual List<List<Configcombi>> ConfigCombi { get; set; }
        public virtual ViewsResult ViewsResult { get; set; }
    }
}