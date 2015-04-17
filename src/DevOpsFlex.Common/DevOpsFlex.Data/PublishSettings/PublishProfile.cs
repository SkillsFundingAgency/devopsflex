namespace DevOpsFlex.Data.PublishSettings
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough]
    [XmlType(AnonymousType = true)]
    public class PublishProfile
    {
        [XmlElement("Subscription")]
        public Subscription[] Subscriptions { get; set; }

        [XmlAttribute]
        public decimal SchemaVersion { get; set; }

        [XmlAttribute]
        public string PublishMethod { get; set; }
    }
}
