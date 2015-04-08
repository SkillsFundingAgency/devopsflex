namespace DevOpsFlex.InspectSolution
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Core;

    public class ProjectFile
    {
        private readonly IStyleCopSettingsFileFactory _settingsFileFactory;

        private readonly IInspectCodeDefinitionFactory _inspectCodeFactory;

        private StyleCopSettingsFile _styleCopSettingsFile;

        private bool _didStyleCopSettingsFile;

        private InspectCodeDefinition _inspectCode;

        private bool _didInspectCode;

        public SolutionFile Solution { get; private set; }

        public string ProjectPath { get; private set; }

        public XElement ProjectRoot { get; private set; }

        public string ProjectFileName
        {
            get { return Path.GetFileName(ProjectPath); }
        }

        public string ProjectName
        {
            get { return ProjectFileName.Substring(0, ProjectFileName.IndexOf(".", StringComparison.Ordinal)); }
        }

        public bool HasResharper
        {
            get
            {
                if(Solution.ResharperProject == null) return false;

                return Solution.ResharperProject.InspectCode.Projects
                               .Select(p => p.ToLowerInvariant())
                               .Contains(ProjectName.ToLowerInvariant());
            }
        }

        /// <summary>
        /// Gets if this project file is related to a test project or not.
        /// Some types of projects (i.e. class libraries) don't have project type guids, so a first check for
        /// the existance of these needs to be done.
        /// </summary>
        /// <remarks>
        /// Test projects should always have project type guids, but there are cases where they don't. For example if
        /// I create a C# library project and start adding unit tests to it. In these cases they will be picked up as not
        /// being test projects.
        /// </remarks>
        public bool IsTestProject
        {
            get
            {
                var projectGuids = ProjectRoot.Descendants()
                                  .SingleOrDefault(e => e.Name.LocalName == "ProjectTypeGuids");
                                  
                if(projectGuids == null) return false;

                return projectGuids.Value.ToUpper()
                                   .Contains("3AC096D0-A1C2-E12C-1390-A8335801FDAB");
            }
        }

        public bool? HasStyleCop
        {
            get
            {
                var pathElement = ProjectRoot.FindElementThatContains("Import", "Project", "StyleCop.MSBuild.Targets");
                if (pathElement == null) return null;

                return pathElement.IsConfigurationConditional(Solution.InspectionConfiguration);
            }
        }

        public InspectCodeDefinition InspectCode
        {
            get
            {
                if (_didInspectCode) return _inspectCode;

                try
                {
                    var usingTask = ProjectRoot.FindElementThatContains("UsingTask", "AssemblyFile", "JetBrains.CommandLine.InspectCode.MsBuild.dll");
                    if (usingTask == null) return null;

                    var inspectElement =
                        ProjectRoot.FindElementThatContains(usingTask.Attributes()
                                                                     .Single(a => a.Name.LocalName == "TaskName")
                                                                     .Value);

                    _inspectCode = _inspectCodeFactory.Create(usingTask, inspectElement);
                }
                catch (Exception)
                {
                    _inspectCode = null;
                }

                _didInspectCode = true;
                return _inspectCode;
            }
        }

        public StyleCopSettingsFile StyleCopSettings
        {
            get
            {
                if (_didStyleCopSettingsFile) return _styleCopSettingsFile;

                try
                {
                    var pathElement = ProjectRoot.FindElementThatContains("None", "Include", "Settings.StyleCop");
                    if (pathElement == null) return null;

                    _styleCopSettingsFile = _settingsFileFactory.Create(Path.Combine(ProjectPath, pathElement.Value));
                }
                catch (Exception)
                {
                    _styleCopSettingsFile = null;
                }

                _didStyleCopSettingsFile = true;
                return _styleCopSettingsFile;
            }
        }

        public ProjectFile(
            string projectPath,
            SolutionFile solution,
            IStyleCopSettingsFileFactory settingsFileFactory,
            IInspectCodeDefinitionFactory inspectCodeFactory)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(projectPath));
            Contract.Requires(solution != null);

            ProjectPath = projectPath;
            Solution = solution;
            _settingsFileFactory = settingsFileFactory;
            _inspectCodeFactory = inspectCodeFactory;

            ProjectRoot = XElement.Load(ProjectPath);
        }
    }

    /// <summary>
    /// Factory proxy interface for <see cref="ProjectFile"/>.
    /// </summary>
    public interface IProjectFileFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProjectFile"/> initialized by the Castle.Windsor container.
        /// </summary>
        /// <param name="projectPath">The absolute path of the project file.</param>
        /// <param name="solution">The <see cref="SolutionFile"/> that is the logical parent of this <see cref="ProjectFile"/>.</param>
        /// <returns>A new instance of <see cref="ProjectFile"/> initialized by the Castle.Windsor container.</returns>
        ProjectFile Create(string projectPath, SolutionFile solution);

        /// <summary>
        /// Release an instance of <see cref="ProjectFile"/> from the Castle.Windsor container.
        /// </summary>
        /// <param name="projectFile">The instance that we want released.</param>
        void Release(ProjectFile projectFile);
    }
}
