using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TEDALS_Ver01.ViewModels.ExportClass
{
    [Serializable]
    //[XmlElement(ElementName="SystemFamily")]
    public class XML
    {
        public string Family = "AllFamily";
        [XmlElement(ElementName="SystemFamilyGroup")]
        public List<XMLSystemFamily> XMLFamilyGroup { get; set; }
    }

    public class XMLSystemFamily
    {
        [XmlElement(ElementName = "FamilyName")]
        public string FamilyName { get; set; }
       
        public int SystemCount { get; set; }


        [XmlElement(IsNullable = true)]
        public string DescriptionDE { get; set; }
        [XmlElement(IsNullable = true)]
        public string DescriptionEN { get; set; }
       
        public DateTime ModifiedOn { get; set; }
        [XmlElement(ElementName = "SystemGroup")]
        public List<XMLSystems> Systems { get; set; }
    }
    public class XMLSystems
    {
        
        public string SystemName { get; set; }
        
        public string MaterialNumber { get; set; }


        [XmlElement(IsNullable = true)]
        public string DescriptionDE { get; set; }
        [XmlElement(IsNullable = true)]
        public string DescriptionEN { get; set; }
        
        public DateTime ModifiedOn { get; set; }
        [XmlElement(ElementName = "OptionGroup")]
        public List<XMLOptions> Options { get; set; }
    }
    public class XMLOptions
    {
        
        public string Option { get; set; }
       
        public string TechnicalCharacteristic { get; set; }


        [XmlElement(IsNullable = false)]
        public string DescriptionDE { get; set; }
        [XmlElement(IsNullable = true)]
        public string DescriptionEN { get; set; }
       
        public DateTime ModifiedOn { get; set; }
        [XmlElement(ElementName = "OptionValueGroup")]
        public List<XMLOptionValues> OptionValues { get; set; }
    }
    public class XMLOptionValues
    {
        
        public string OptionValue { get; set; }


        [XmlElement(IsNullable = true)]
        public string DescriptionDE { get; set; }
        [XmlElement(IsNullable = true)]
        public string DescriptionEN { get; set; }
        
        public DateTime ModifiedOn { get; set; }
        [XmlElement(ElementName = "PropertiesGroup")]
        public List<XMLSetValues> SetValues { get; set; }

    }
    public class XMLSetValues
    {
        
        public string Property { get; set; }
       
        public string Value { get; set; }
      
        public string Unit { get; set; }
      
        public string Status { get; set; }
       
        public DateTime ModifiedOn { get; set; }

    }
}