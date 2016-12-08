using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Autofac;

namespace DropshipListingCheckBusiness
{
    public class AppContext
    {
        private static AppContext _instance;
        private ContainerManager _containerManager;
        private readonly IDictionary<Type, object> _allSingletons = new Dictionary<Type, object>();

        private AppContext()
        {

        }

        public static AppContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppContext();
                }
                return _instance;
            }
        }

        public void Initialize()
        { 
            
            //dependency injection
            _containerManager = new ContainerManager(new ContainerBuilder().Build());
            _containerManager.RegisterDependency();

            //Initialize Mapper
            
        }

        public Config Config
        {
            get
            {
                return Config.Instance;
            }
        }

        public T SingleInstance<T>()
        {
            if (!_allSingletons.ContainsKey(typeof(T)))
            {
                _allSingletons[typeof(T)] = Activator.CreateInstance<T>();
            }
            return (T)_allSingletons[typeof(T)];
        }

        public T Resolve<T>(string key = "") where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return _containerManager.Container.Resolve<T>();
                }
                return _containerManager.Container.ResolveKeyed<T>(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T[] ResolveAll<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return _containerManager.Container.Resolve<IEnumerable<T>>().ToArray();
            }
            return _containerManager.Container.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }
    }
}
