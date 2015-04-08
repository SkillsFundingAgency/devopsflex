namespace ProjectDigger.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using Caliburn.Micro;
    using DevOpsFlex.InspectSolution;

    public class SolutionViewModel : Screen
    {
        private readonly ISolutionManager _solutionManager;

        public SolutionFile Solution
        {
            get { return _solutionManager.CurrentSolution; }
        }

        public string SolutionName
        {
            get { return _solutionManager.CurrentSolution.SolutionName; }
        }

        public Brush ResharperFill
        {
            get
            {
                switch (_solutionManager.CurrentSolution.HasResharper)
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

        public IEnumerable<ProjectViewModel> Projects
        {
            get
            {
                return _solutionManager.CurrentSolution
                                       .ProjectFiles
                                       .Where(p => !p.IsTestProject)
                                       .Select(p => new ProjectViewModel(p));
            }
        }

        public SolutionViewModel(ISolutionManager solutionManager)
        {
            _solutionManager = solutionManager;
        }
    }
}
