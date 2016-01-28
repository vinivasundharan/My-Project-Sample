using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.ImportSAP
{
    public class SAP_DB_Mismatch1
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public int LsystemID {get;set;}
    }

    public class DB_SAP_Mismatch1
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public int LsystemID { get; set; }
        public int OptionID { get; set; }
    }

    public class DB_SAP_Match1
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public int LsystemID { get; set; }
        public int OptionID { get; set; }
    }

    public class OptionSAP
    {
        public virtual List<SAP_DB_Mismatch1> s_d_mismatch { get; set; }
        public virtual List<DB_SAP_Mismatch1> d_s_mismatch { get; set; }
        public virtual List<DB_SAP_Match1> d_s_match { get; set; }
    }
}