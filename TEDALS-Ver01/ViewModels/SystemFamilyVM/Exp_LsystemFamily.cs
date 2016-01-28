using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TEDALS_Ver01.ViewModels
{
    //Not used
    [Serializable]
    public class Exp_LsystemFamily
    {
        public Exp_LsystemFamily() { }
        [XmlElement(ElementName="familyName")]
        public string FamilyName { get { return this.FamilyName; } set { this.FamilyName="abcd";} }
        [XmlArrayItem(Type=typeof(Exp_Lsystem)),
            XmlArrayItem(Type=typeof(Exp_Option))]
        [XmlElement(ElementName="Systems")]
        public Exp_Lsystem[] Lsystem { get; set; }
    }
    [Serializable]
    public class Exp_Lsystem
    {
        public Exp_Lsystem() { }
        [XmlElement(ElementName="SystemName")]
        public string LsystemName { get; set; }
        
        [XmlElement(ElementName="Options")]
        public Exp_Option[] Option { get; set; }

    }
    [Serializable]
    public class Exp_Option
    {
        public Exp_Option() { }
        [XmlElement(ElementName = "OptionName")]
        public string OptionName { get; set; }
    }
}