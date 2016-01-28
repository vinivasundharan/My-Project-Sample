using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TEDALS_Ver01.Models
{
    public enum DataStatus
    {
        [Display (Name = "Internal")]
        Internal = 0,
        [Display (Name = "External")]
        External = 1
    }
    public enum DataUsage
    {
        [Display (Name ="Preliminary")]
        Preliminary = 2,
        [Display (Name = "Valid")]
        Valid = 1,
        [Display (Name = "Invalid")]
        Invalid = 0
    }
    public class TcSet
    {
        public int TcSetID { get; set; }
        [Display (Name = "Property name")]
        [Required(ErrorMessage="Set Name cannot be empty")]
        public string SetName { get; set; }
        [Display(Name = "PhysicalUnit")]
        public string PhysicalUnit { get; set; }
        [Display (Name = "Data Usage")]
        public DataUsage DataUsage { get; set; }
        [Display (Name = "Data Status")]
        public DataStatus DataStatus { get; set; }
        [Display(Name="Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified On")]
        public DateTime ModifiedOn { get; set; }
        [Display (Name = "Description in German")]
        public string DescriptionDE { get; set; }
        [Display (Name = "Description in English")]
        public string DescriptionEN { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        public int TechnicalCharacteristicID { get; set; }
        public int DataFormatID { get; set; }

        public virtual ICollection<SetValue> SetValues { get; set; }
        public virtual DataFormat DataFormat { get; set; }
        public virtual TechnicalCharacteristic TechnicalCharacteristic { get; set; }
    }
}