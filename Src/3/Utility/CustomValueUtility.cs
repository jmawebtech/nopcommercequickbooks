using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using static Nop.Services.Payments.PaymentExtensions;

namespace Nop.Plugin.Accounting.QuickBooks.Utility
{
    public static class CustomValueUtility
    {
        /// <summary>
        /// Adds PayPal fee to order custom values
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AddCustomValues(string fieldName, string value)
        {
            Dictionary<string, object> items = new Dictionary<string, object>();
            items.Add(fieldName, value);

            var ds = new DictionarySerializer(items);
            var xs = new XmlSerializer(typeof(DictionarySerializer));

            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter))
                {
                    xs.Serialize(xmlWriter, ds);
                }
                var result = textWriter.ToString();
                return result;
            }
        }

        public static Dictionary<string, object> DeserializeCustomValues(string customValuesXml)
        {
            if (string.IsNullOrWhiteSpace(customValuesXml))
            {
                return new Dictionary<string, object>();
            }

            var serializer = new XmlSerializer(typeof(DictionarySerializer));

            using (var textReader = new StringReader(customValuesXml))
            {
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    var ds = serializer.Deserialize(xmlReader) as DictionarySerializer;
                    if (ds != null)
                        return ds.Dictionary;
                    return new Dictionary<string, object>();
                }
            }
        }

    }
}
