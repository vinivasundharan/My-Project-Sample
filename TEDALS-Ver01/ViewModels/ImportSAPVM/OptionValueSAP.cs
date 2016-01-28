using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.ViewModels.ImportSAP
{
    public class SAP_DB_Mismatch2
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public string Optionval { get; set; }
        public string Description { get; set; }
        public int SystemID { get; set; }
        public int? OptionID { get; set; }

    }

    public class DB_SAP_Mismatch2
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public string Optionval { get; set; }
        public string Description { get; set; }
        public int SystemID { get; set; }
        public int OptionID { get; set; }
        public int OptionValID { get; set; }
    }

    public class DB_SAP_Match2
    {
        public string MaterialNo { get; set; }
        public string SystemName { get; set; }
        public string OptionName { get; set; }
        public string Optionval { get; set; }
        public int SystemID { get; set; }
        public int OptionID { get; set; }
        public int OptionValID { get; set; }
        public string DescriptionDB { get; set; }
        public string DescriptionSAP { get; set; }
    }

    public class OptionValueSAP
    {
        public virtual List<SAP_DB_Mismatch2> s_d_mismatch { get; set; }
        public virtual List<DB_SAP_Mismatch2> d_s_mismatch { get; set; }
        public virtual List<DB_SAP_Match2> d_s_match { get; set; }
    }
}