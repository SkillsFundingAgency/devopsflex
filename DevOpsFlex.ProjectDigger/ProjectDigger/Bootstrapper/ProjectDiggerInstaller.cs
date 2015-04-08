namespace ProjectDigger.Bootstrapper
{
    using System.Diagnostics.CodeAnalysis;
    using Caliburn.Micro;
    using Castle.Core;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using DevOpsFlex.InspectSolution.Bootstrapper;
    using ViewModels;

    [ExcludeFromCodeCoverage]
    public class ProjectDiggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // installer chain
            container.Install(new InspectSolutionInstaller());
            
            // configuration
            container.Register(
                Component.For<IShell>()
                         .ImplementedBy<ShellViewModel>()
                         .LifeStyle.Is(LifestyleType.Transient));

            container.Register(
                Component.For<ISolutionManager>()
                         .ImplementedBy<SolutionManager>()
                         .LifeStyle.Is(LifestyleType.Singleton),
                Component.For<IWindowManager>()
                         .ImplementedBy<WindowManager>()
                         .LifeStyle.Is(LifestyleType.Singleton),
                Component.For<IEventAggregator>()
                         .ImplementedBy<EventAggregator>()
                         .LifeStyle.Is(LifestyleType.Singleton));

            // convention
            container.Register(
                Component.For(typeof(IScreenFactory<>))
                         .AsFactory());

            container.Register(
                Classes.FromAssembly(GetType().Assembly)
                       .Where(x => x.Name.EndsWith("ViewModel"))
                       .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
