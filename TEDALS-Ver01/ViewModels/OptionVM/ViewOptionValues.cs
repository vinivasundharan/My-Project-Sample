using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    public class ViewOptionValues
    {
        public Option Option { get; set; }
        public IEnumerable<OptionValue_SetVal> Values { get; set; }
    }
}