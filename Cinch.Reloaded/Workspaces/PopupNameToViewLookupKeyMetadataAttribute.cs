using System;

namespace Cinch.Reloaded.Workspaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PopupNameToViewLookupKeyMetadataAttribute : Attribute
    {
        public string PopupName { get; private set; }
        public Type ViewLookupKey { get; private set; }


        public PopupNameToViewLookupKeyMetadataAttribute(string popupName, Type viewLookupKey)
        {
            PopupName = popupName;
            ViewLookupKey = viewLookupKey;
        }

    }
}
