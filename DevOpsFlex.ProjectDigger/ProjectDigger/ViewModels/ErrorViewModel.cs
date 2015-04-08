namespace ProjectDigger.ViewModels
{
    using Caliburn.Micro;

    public class ErrorViewModel : Screen
    {
        private readonly ISolutionManager _solutionManager;

        public string ErrorMessage
        {
            get { return _solutionManager.ErrorMessage; }
        }

        public ErrorViewModel(ISolutionManager solutionManager)
        {
            _solutionManager = solutionManager;
        }
    }
}
