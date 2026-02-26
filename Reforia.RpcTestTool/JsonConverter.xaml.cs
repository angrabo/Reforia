using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace Reforia.RpcTestTool;

public partial class JsonConverter : Window
{
    public JsonConverter()
    {
        InitializeComponent();
    }
    
    private void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        string input = InputCSharp.Text;
        if (string.IsNullOrWhiteSpace(input)) return;

        try
        {
            var propertyRegex = new Regex(@"public\s+(?:required\s+|virtual\s+|override\s+)?([\w\<\>\[\]\?]+)\s+(\w+)\s*\{");
        
            var matches = propertyRegex.Matches(input);
            var resultModel = new Dictionary<string, object>();

            foreach (Match match in matches)
            {
                string type = match.Groups[1].Value;
                string name = match.Groups[2].Value;

                if (type == "class")
                    continue;

                resultModel.Add(name, GetDefaultValue(type.ToLower()));
            }

            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
            };
        
            OutputJson.Text = JsonSerializer.Serialize(resultModel, options);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Błąd: " + ex.Message);
        }
    }

    private object GetDefaultValue(string type)
    {
        if (type.Contains("int") || type.Contains("long")) return 0;
        if (type.Contains("bool")) return false;
        if (type.Contains("decimal") || type.Contains("double") || type.Contains("float")) return 0.0;
        if (type.Contains("datetime")) return DateTime.Now;
        if (type.Contains("list") || type.Contains("[]")) return new List<object>();

        return "string";
    }
}