using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Common;
using Common.Models;
using DropshipListingCheckData;
using DropshipListingCheckBusiness.Handlers;
using DropshipListingCheckBusiness.Services;

namespace DropshipListingCheckBusiness
{
    public class ContainerManager
    {
        private readonly IContainer _container;

        public ContainerManager(IContainer container)
        {
            _container = container;
        }

        public IContainer Container
        {
            get { return _container; }
        }



        public void RegisterDependency()
        {
            var builder = new ContainerBuilder();

            //sigleton

            //PerLifetimeScope
            builder.RegisterType<AppDBContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DropshipListingCheckService>().As<IDropshipListingCheckService>().InstancePerLifetimeScope();
            builder.RegisterType<DropshipListingCheckHandler>().As<IListingCheck>().InstancePerLifetimeScope();
            builder.RegisterType<CSVHelper>().As<ICSVHelper>().InstancePerLifetimeScope();
            
            builder.Update(_container);
        }

    }
}
