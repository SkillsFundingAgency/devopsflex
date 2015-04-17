namespace DevOpsFlex.Data.PublishSettings
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;

    [Serializable, DebuggerStepThrough]
    [XmlType(AnonymousType = true)]
    public class Subscription
    {
        [XmlAttribute]
        public string ServiceManagementUrl { get; set; }

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ManagementCertificate { get; set; }
    }
}
