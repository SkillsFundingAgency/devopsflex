namespace ProjectDigger.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;

    public class ShellViewModel : Conductor<Screen>, IShell
    {
        private readonly ISolutionManager _solutionManager;
        private readonly IScreenFactory<DragViewModel> _dragScreenFactory;
        private readonly IScreenFactory<ErrorViewModel> _errorScreenFactory;
        private readonly IScreenFactory<SolutionViewModel> _solutionScreenFactory;

        public ShellViewModel(
            ISolutionManager solutionManager,
            IScreenFactory<DragViewModel> dragScreenFactory,
            IScreenFactory<ErrorViewModel> errorScreenFactory,
            IScreenFactory<SolutionViewModel> solutionScreenFactory)
        {
            _solutionManager = solutionManager;
            _dragScreenFactory = dragScreenFactory;
            _errorScreenFactory = errorScreenFactory;
            _solutionScreenFactory = solutionScreenFactory;

            _solutionManager.PropertyChanged +=
                (_, args) =>
                {
                    // TODO: WRITE SOMETHING THAT BINDS THE PROPERTYNAME TO AN EXPRESSION<T> OF SENDER, this kind of code with the switch(**MAGIC STRING**) is a piece of crap.
                    switch (args.PropertyName)
                    {
                        case "HasError":
                            if (_solutionManager.HasError)
                            {
                                ActivateItem(_errorScreenFactory.Create());
                            }
                            break;
                        case "CurrentSolution":
                            ActivateItem(_solutionScreenFactory.Create());
                            break;
                    }
                };

            SwitchToDragView();
        }

        public void SwitchToDragView()
        {
            ActivateItem(_dragScreenFactory.Create());
        }

        public void SolutionFileOnDrop(DragEventArgs e)
        {
            _solutionManager.LoadSolution(e);
        }
    }
}