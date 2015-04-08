namespace DevOpsFlex.InspectSolution
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class InspectCodeDefinition
    {
        private readonly XElement _usingTask;

        private readonly XElement _inspectElement;

        private IEnumerable<string> _projects;

        private bool _didProjects;

        public IEnumerable<string> Projects
        {
            get
            {
                if(_didProjects) return _projects;

                var att = _inspectElement.Attributes().FirstOrDefault(a => a.Name.LocalName == "IncludedProjects");
                if (att == null)
                {
                    _projects = Enumerable.Empty<string>();
                }
                else
                {
                    _projects = att.Value.Split(';');
                }

                _didProjects = true;
                return _projects;
            }
        }

        public InspectCodeDefinition(XElement usingTask, XElement inspectElement)
        {
            _usingTask = usingTask;
            _inspectElement = inspectElement;
        }
    }

    public interface IInspectCodeDefinitionFactory
    {
        InspectCodeDefinition Create(XElement usingTask, XElement inspectElement);

        void Release(InspectCodeDefinition inspectCode);
    }
}
