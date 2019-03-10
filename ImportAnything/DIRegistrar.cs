using ImportAnything.Services.Interfaces;
using StructureMap;
using System.Linq;

namespace ImportAnything
{
    class DIRegistrar
    {
        private static Container container;

        private static Container GetContainer()
        {
            return container ?? (container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AddAllTypesOf<IConsolidator>();
                    s.ConnectImplementationsToTypesClosing(typeof(IConsolidator<>));
                });        
                
                
            }));
        }

        public static T[] GetInstances<T>()
        {
            return GetContainer().GetAllInstances<T>().ToArray();
        }
    }
}
