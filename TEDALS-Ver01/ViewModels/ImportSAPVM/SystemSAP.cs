using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.ImportSAP
{
    public class SAP_DB_mismatch
    {
        public string MaterialNo { get; set; }
    }
    public class DB_SAP_Mismatch
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public int LsystemID { get; set; }
    }
    public class DB_SAP_Match
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public int LsystemID { get; set; }
    }

    public class SystemSAP
    {
        public virtual List<SAP_DB_mismatch> s_d_mismatch {get;set;}
        public virtual List<DB_SAP_Mismatch> d_s_mismatch { get; set; }
        public virtual List<DB_SAP_Match> d_s_match { get; set; }
    }
}