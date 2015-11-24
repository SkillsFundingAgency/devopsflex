namespace ProjectDigger.ViewModels
{
    using System.Windows.Media;
    using Caliburn.Micro;
    using DevOpsFlex.InspectSolution;

    public class ProjectViewModel : Screen
    {
        private readonly ProjectFile _projectFile;

        public string ProjectName
        {
            get { return _projectFile.ProjectFileName; }
        }

        public Brush StyleCopFill
        {
            get
            {
                switch (_projectFile.HasStyleCop)
                {
                    case false:
                        return Brushes.Orange;
                    case true:
                        return Brushes.Green;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public Brush StyleCopFileFill
        {
            get
            {
                switch (_projectFile.HasStyleCopSettingsFile)
                {
                    case false:
                        return Brushes.Orange;
                    case true:
                        return Brushes.Transparent;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public Brush StyleCopFileStroke
        {
            get
            {
                switch (_projectFile.HasStyleCopSettingsFile)
                {
                    case false:
                        return Brushes.Black;
                    case true:
                        return Brushes.Transparent;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public Brush ResharperFill
        {
            get
            {
                switch (_projectFile.HasResharper)
                {
                    case true:
                        return Brushes.Green;
                    default:
                        return Brushes.Transparent;
                }
            }
        }

        public ProjectViewModel(ProjectFile projectFile)
        {
            _projectFile = projectFile;
        }
    }
}
