using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cinch.Reloaded.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;

namespace Cinch.Reloaded.Workspaces
{
    public static class PopupResolver
    {
        static PopupResolver()
        {
            var compositionContainer = ViewModelRepository.Instance.Resolver.Container;
            var lazyVisualizerServices = compositionContainer.GetExports<IUIVisualizerService>();
            VisualizerServices = lazyVisualizerServices.Select(lazyVisualizerService => lazyVisualizerService.Value).ToList();
        }

        public static IList<IUIVisualizerService> VisualizerServices { get; private set; }

        public static void ResolvePopupLookups(IEnumerable<Assembly> assembliesToExamine)
        {
            try
            {
                foreach (var ass in assembliesToExamine)
                {
                    foreach (var type in ass.GetTypes())
                    {
                        foreach (PopupNameToViewLookupKeyMetadataAttribute attrib in type.GetCustomAttributes(typeof(PopupNameToViewLookupKeyMetadataAttribute), true))
                        {
                            foreach (var uiVisualizerService in VisualizerServices)
                            {
                                uiVisualizerService.Register(attrib.PopupName, attrib.ViewLookupKey);
                            }                                                       
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The PopupResolver is unable to ResolvePopupLookups based on current parameters", ex);
            }
        }
       
    }
}
