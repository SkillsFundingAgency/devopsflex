namespace DevOpsFlex.InspectSolution.Bootstrapper
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    [ExcludeFromCodeCoverage]
    public class InspectSolutionInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // configuration
            container.Register(
                Component.For<SolutionFile>()
                         .LifestyleTransient(),
                Component.For<ProjectFile>()
                         .LifestyleTransient(),
                Component.For<InspectCodeDefinition>()
                         .LifestyleTransient(),
                Component.For<StyleCopSettingsFile>()
                         .LifestyleTransient());

            container.Register(
                Component.For<ISolutionFileFactory>()
                         .AsFactory()
                         .LifestyleTransient(),
                Component.For<IProjectFileFactory>()
                         .AsFactory()
                         .LifestyleTransient(),
                Component.For<IInspectCodeDefinitionFactory>()
                         .AsFactory()
                         .LifestyleTransient(),
                Component.For<IStyleCopSettingsFileFactory>()
                         .AsFactory()
                         .LifestyleTransient());
        }
    }
}
