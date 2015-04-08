namespace DevOpsFlex.InspectSolution
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class SolutionFile
    {
        private readonly string _solutionContent;

        private IEnumerable<string> _projectPaths;

        private IEnumerable<ProjectFile> _projectFiles;

        private readonly IProjectFileFactory _projectFileFactory;

        public string SolutionPath { get; private set; }

        public string SolutionName
        {
            get { return Path.GetFileName(SolutionPath); }
        }

        public string InspectionConfiguration
        {
            get { return "DebugCI"; } // TODO: PUT A PROPER SELECTION VIEW IN HERE !
        }

        public ProjectFile ResharperProject
        {
            get { return ProjectFiles.FirstOrDefault(p => p.InspectCode != null); }
        }

        public bool? HasResharper
        {
            get
            {
                var count = ProjectFiles.Count(p => p.InspectCode != null);
                switch (count)
                {
                    case 0:
                        return null;
                    case 1:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public IEnumerable<ProjectFile> ProjectFiles
        {
            get
            {
                return _projectFiles ??
                       (_projectFiles = ProjectPaths.Select(s => _projectFileFactory.Create(s, this)));
            }
        }

        public IEnumerable<string> ProjectPaths
        {
            get
            {
                return _projectPaths ??
                       (_projectPaths = new Regex("Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\"", RegexOptions.Compiled)
                           .Matches(_solutionContent)
                           .Cast<Match>()
                           .Select(c => c.Groups[2].Value)
                           .Select(s =>
                               !Path.IsPathRooted(s) ?
                                   Path.Combine(Path.GetDirectoryName(SolutionPath), s) :
                                   Path.GetFullPath(s)));
            }
        }

        public SolutionFile(string solutionPath, IProjectFileFactory projectFileFactory)
        {
            SolutionPath = solutionPath;
            _solutionContent = File.ReadAllText(solutionPath);
            _projectFileFactory = projectFileFactory;
        }
    }

    public interface ISolutionFileFactory
    {
        SolutionFile Create(string solutionPath);

        void Release(SolutionFile solutionFile);
    }
}
