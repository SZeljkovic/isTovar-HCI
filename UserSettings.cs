using Newtonsoft.Json;
using System.IO;
using System.Xml;

public class UserSettings
{
    public string Language { get; set; } = "sr";
    public string Theme { get; set; } = "LightTheme.xaml";

    private static readonly string filePath = "userSettings.json";

    public static UserSettings Load()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();
        }
        return new UserSettings();
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}
