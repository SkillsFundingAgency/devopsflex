namespace ProjectDigger
{
    using System.Windows;
    using Caliburn.Micro;
    using DevOpsFlex.InspectSolution;

    public interface ISolutionManager : INotifyPropertyChangedEx
    {
        SolutionFile CurrentSolution { get; }

        bool HasError { get; }

        string ErrorMessage { get; }

        void LoadSolution(DragEventArgs e);
    }
}