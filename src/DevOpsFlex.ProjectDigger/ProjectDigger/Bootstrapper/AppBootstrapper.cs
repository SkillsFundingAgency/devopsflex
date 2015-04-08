namespace ProjectDigger.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using Caliburn.Micro;
    using Castle.Facilities.TypedFactory;
    using Castle.Windsor;

    public class AppBootstrapper : BootstrapperBase
    {
        private IWindsorContainer _container;

        public AppBootstrapper()
        {
            Initialize();

            LogManager.GetLog = type => new DebugLog(typeof(Debug));
        }

        protected override void Configure()
        {
            _container =
                new WindsorContainer().AddFacility<TypedFactoryFacility>()
                                      .Install(new ProjectDiggerInstaller());
        }

        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key) ?
                _container.Resolve(service) :
                _container.Resolve(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return new[] { _container.ResolveAll(service) };
        }

        protected override void BuildUp(object instance) { }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}