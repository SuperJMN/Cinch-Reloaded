using System;

namespace Cinch.Reloaded.Workspaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewnameToViewLookupKeyMetadataAttribute : Attribute
    {
        public string ViewName { get; private set; }
        public Type ViewLookupKey { get; private set; }


        public ViewnameToViewLookupKeyMetadataAttribute(string viewName, Type viewLookupKey)
        {
            ViewName = viewName;
            ViewLookupKey = viewLookupKey;
        }

    }
}
