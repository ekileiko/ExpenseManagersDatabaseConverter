using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ExpManagerToOwnMoney.Helpers
{
    public class SimpleSerializer
    {
        /// <summary>
        /// Сериализует сообщение в АСИВ в xml
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SerializeToXML(object message)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(message.GetType());
                MemoryStream ms = new MemoryStream();

                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");

                var sb = new StringBuilder();
                var w = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture);
                xmlSerializer.Serialize(w, message, xmlnsEmpty);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Сериализует сообщение в АСИВ в xml
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string SerializeToXML(object message, String encodingName)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(message.GetType());
                MemoryStream ms = new MemoryStream();

                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");

                xmlSerializer.Serialize(ms, message, xmlnsEmpty);

                XmlDocument xmlDocument = new XmlDocument();
                ms.Position = 0;
                xmlDocument.Load(ms);


                if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDocument.FirstChild;
                    xmlDeclaration.Encoding = encodingName;
                    xmlDeclaration.Standalone = "yes";
                }

                return xmlDocument.OuterXml;

            }
            catch (Exception ex)
            {
                return String.Empty;
            }

        }


        public static string SerializeToXMLWithoutStandalone(object message, String encodingName)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(message.GetType());
                MemoryStream ms = new MemoryStream();

                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");

                xmlSerializer.Serialize(ms, message, xmlnsEmpty);

                XmlDocument xmlDocument = new XmlDocument();
                ms.Position = 0;
                xmlDocument.Load(ms);


                if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDocument.FirstChild;
                    xmlDeclaration.Encoding = encodingName;

                }

                return xmlDocument.OuterXml;

            }
            catch (Exception ex)
            {
                return String.Empty;
            }

        }

        /// <summary>
        /// Преобразует объект в массив byte[].
        /// </summary>
        public static byte[] ObjectToByteArray(object _Object)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, _Object);

                return memoryStream.ToArray();
            }

            catch (Exception ex)
            {

            }

            return null;
        }

        public static T ByteArrayToObject<T>(byte[] bytes)
        {
            try
            {
                var memoryStream = new MemoryStream();
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Position = 0;
                var binaryFormatter = new BinaryFormatter();

                var result = (T)binaryFormatter.Deserialize(memoryStream);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Десериализует строку XML в объект заданного типа.
        /// </summary>
        public static T DeserializeXmlToObject<T>(string xml)
        {
            if (xml == null)
                return default(T);

            if (xml == string.Empty)
                return (T)Activator.CreateInstance(typeof(T));

            var reader = new StringReader(xml);
            var sr = new XmlSerializer(typeof(T));

            return (T)sr.Deserialize(reader);
        }

    }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {

            return null;

        }



        public void ReadXml(System.Xml.XmlReader reader)
        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            bool wasEmpty = reader.IsEmptyElement;

            reader.Read();



            if (wasEmpty)

                return;



            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {

                reader.ReadStartElement("item");



                reader.ReadStartElement("key");

                TKey key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();



                reader.ReadStartElement("value");

                TValue value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();



                this.Add(key, value);



                reader.ReadEndElement();

                reader.MoveToContent();

            }

            reader.ReadEndElement();

        }



        public void WriteXml(System.Xml.XmlWriter writer)
        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            foreach (TKey key in this.Keys)
            {

                writer.WriteStartElement("item");



                writer.WriteStartElement("key");

                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();



                writer.WriteStartElement("value");

                TValue value = this[key];

                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();



                writer.WriteEndElement();

            }

        }

        #endregion

    }
}
