using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ExpManagerToOwnMoney.Models
{
    [Serializable]
    [XmlRoot("expmanager")]
    public class ExpManagerXmlModel
    {
        public ExpManagerXmlModel()
        {
            Operations = new List<Operation>();
            Dictionaries = new EMDictionaries();
        }
        
        [XmlArray("operations")]
        [XmlArrayItem("operation")]
        public List<Operation> Operations { get; set; }

        [XmlElement("dictionaries")]
        public EMDictionaries Dictionaries { get; set; }
        
        [Serializable]
        public class Operation
        {
            [XmlElement("id")]
            public Int32 Id { get; set; }

            [XmlElement("type_id")]
            public OperationTypeEnum Type { get; set; }

            [XmlElement("date")]
            public DateTime Date { get; set; }

            [XmlElement("source_id")]
            public Int32 SourceId { get; set; }

            [XmlElement("destination_id")]
            public Int32 DestinationId { get; set; }

            [XmlElement("quantity")]
            public Double Quantity { get; set; }

            [XmlElement("sumperunit")]
            public Double SumPerUnit { get; set; }

            [XmlElement("currensy_sum")]
            public Double CurrencySum { get; set; }

            [XmlElement("comment")]
            public String Comment { get; set; }
        }

        [Serializable]
        public class EMDictionaries
        {
            public EMDictionaries()
            {
                Accounts = new List<Account>();
                CategoryGroups = new List<CategoryGroup>();
                Categories = new List<Category>();
            }

            [XmlArray("accounts")]
            [XmlArrayItem("account")]
            public List<Account> Accounts { get; set; }


            [XmlArray("categorygroups")]
            [XmlArrayItem("categorygroup")]
            public List<CategoryGroup> CategoryGroups { get; set; }


            [XmlArray("categories")]
            [XmlArrayItem("category")]
            public List<Category> Categories { get; set; }
        }

        [Serializable]
        public class Account
        {
            [XmlElement("id")]
            public Int32 Id { get; set; }

            [XmlElement("name")]
            public String Name { get; set; }

            [XmlElement("balance")]
            public Double Balance { get; set; }

            [XmlElement("currensy_id")]
            public Int32 CurrencyId { get; set; }

            [XmlElement("isdefault")]
            public Boolean IsDefault { get; set; }

            [XmlElement("istotalinclude")]
            public Boolean IsToTotalInclude { get; set; }
        }

        [Serializable]
        public class CategoryGroup
        {
            [XmlElement("id")]
            public Int32 Id { get; set; }

            [XmlElement("name")]
            public String Name { get; set; }
        }

        [Serializable]
        public class Category
        {
            [XmlElement("id")]
            public Int32 Id { get; set; }

            [XmlElement("name")]
            public String Name { get; set; }

            [XmlElement("group_id")]
            public Int32 GroupId { get; set; }

            [XmlElement("category_type_id")]
            public CategoryTypeEnum CategoryTypeId { get; set; }

            [XmlElement("isdefault")]
            public Boolean IsDefault { get; set; }
        }

        public enum OperationTypeEnum
        {
            [XmlEnum("0")]
            Расход = 0,
            [XmlEnum("1")]
            Доход = 1,
            [XmlEnum("2")]
            Перевод = 2
        }

        public enum CategoryTypeEnum
        {
            [XmlEnum("0")]
            Расходная = 0,
            [XmlEnum("1")]
            Доходная = 1,
        }
    }
}
