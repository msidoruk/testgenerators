using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Eps.DbFeatures.Attributes
{
    public abstract class DbTableFeatureAttribute : Attribute
    {
        private const string Delimiter = ":=>";

        public string Serialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(GetType());

            StringBuilder dbTableFeatureContent = new StringBuilder();
            dbTableFeatureContent.Append($"{GetType().AssemblyQualifiedName}{Delimiter}");
            xmlSerializer.Serialize(new StringWriter(dbTableFeatureContent), this);
            //
            return dbTableFeatureContent.ToString();
        }

        public static DbTableFeatureAttribute Deserialize(string feature)
        {
            int delimiterIndex = feature.IndexOf(Delimiter);
            if (delimiterIndex < 0)
                throw new Exception($"Bad dbTableFeature instance: {feature}");

            int delimiterLength = Delimiter.Length;

            string serializedTypeName = feature.Substring(0, delimiterIndex);
            string serializedData = feature.Substring(delimiterIndex + delimiterLength);
            Type serializedType = Type.GetType(serializedTypeName);
            XmlSerializer serializedTypeSerializer = new XmlSerializer(serializedType);
            DbTableFeatureAttribute dbTableFeature =
                (DbTableFeatureAttribute)
                    serializedTypeSerializer.Deserialize(
                        new XmlTextReader(new StringReader(serializedData)));

            return dbTableFeature;
        }
    }
}
