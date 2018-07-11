using System;
using System.Windows;
using System.Windows.Controls;

namespace Cinch.Reloaded.UIMisc
{
    public class SeparatorStyleSelector : StyleSelector
    {
        public Style RegularItemStyle { get; set; }
        public Style SeparatorItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is CinchMenuItem)
            {
                var mi = item as CinchMenuItem;
                return mi.Text.Equals("--", StringComparison.OrdinalIgnoreCase) ? SeparatorItemStyle : RegularItemStyle;
            }
            return null;
        }
    }
}