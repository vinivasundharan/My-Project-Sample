using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.ViewModels
{
    public class SAP_Table_View
    {
        public virtual List<SAP_Existing> SAP_Existing { get; set; }
        public virtual List<FromSAP> FromSAP { get; set; }
    }
}