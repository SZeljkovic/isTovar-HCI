using System.Configuration;
using System.Data;
using System.Windows;

using System.Globalization;
using System.Threading;

namespace Projekat_B_isTovar
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = UserSettings.Load();

            // Primijeni jezik
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);

            // Primijeni temu
            // Remove only previously applied user theme
            var existing = Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("/UserThemes/"));

            if (existing != null)
                Resources.MergedDictionaries.Remove(existing);

            // Add the new one
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"/UserThemes/{settings.Theme}", UriKind.Relative)
            });
        }
    }
}

