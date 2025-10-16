using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Projekat_B_isTovar.Resources;

namespace Projekat_B_isTovar.UserThemes
{
    public static class ApplyManager
    {
        public static void ApplyTheme(string themeFile)
        {
            var appResources = Application.Current.Resources;

            var existing = appResources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("/UserThemes/"));

            if (existing != null)
                appResources.MergedDictionaries.Remove(existing);

            appResources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"/UserThemes/{themeFile}", UriKind.Relative)
            });
        }

        public static void ApplyLanguage(string langCode, INotifyPropertyChanged loc)
        {
            Strings.Culture = new CultureInfo(langCode);
            if (loc != null)
            {
                var refreshMethod = loc.GetType().GetMethod("Refresh");
                refreshMethod?.Invoke(loc, null);
            }
        }
    }
}