namespace DevOpsFlex.Activities.PublishSettings
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough]
    [XmlType(AnonymousType = true), XmlRoot(Namespace = "", IsNullable = false)]
    public class PublishData
    {
        public static PublishData FromFile(string filePath)
        {
            var reader = File.OpenRead(filePath);
            var serializer = new XmlSerializer(typeof(PublishData));

            var data = (PublishData)serializer.Deserialize(reader);
            reader.Close();

            return data;
        }

        [XmlElement("PublishProfile")]
        public PublishProfile[] PublishProfiles { get; set; }
    }
}
