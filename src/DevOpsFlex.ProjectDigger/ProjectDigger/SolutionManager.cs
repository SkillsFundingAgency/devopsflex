namespace ProjectDigger
{
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using DevOpsFlex.InspectSolution;

    public class SolutionManager : PropertyChangedBase, ISolutionManager
    {
        private readonly ISolutionFileFactory _solutionFileFactory;
        private SolutionFile _currentSolution;
        private string _errorMessage;

        public SolutionFile CurrentSolution
        {
            get { return _currentSolution; }
            private set
            {
                _currentSolution = value;
                NotifyOfPropertyChange(() => CurrentSolution);
            }
        }

        public bool HasError
        {
            get { return !string.IsNullOrWhiteSpace(_errorMessage); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => HasError);
            }
        }

        public SolutionManager(ISolutionFileFactory solutionFileFactory)
        {
            _solutionFileFactory = solutionFileFactory;
        }

        public void LoadSolution(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Count() > 1)
                {
                    ErrorMessage = "This application currently only supports one single file";
                    return;
                }

                var file = files.Single();
                if (!file.EndsWith(".sln"))
                {
                    ErrorMessage = "This application currently only supports a single .sln file.\n" +
                                   "I have plans to add folder (through discovery) support later on.";

                    return;
                }

                CurrentSolution = _solutionFileFactory.Create(file);
                ErrorMessage = null;
            }
            else
            {
                ErrorMessage = "You droped something I have no clue what to do with.\n" +
                               "Try again with a .sln solution file";
            }
        }
    }
}
