using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public class ImportSAP_ExistingOptionValues
    {
        public int ImportSAP_ExistingOptionValuesID { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}