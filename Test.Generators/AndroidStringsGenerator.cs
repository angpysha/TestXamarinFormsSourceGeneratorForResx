using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Test.Generators;

internal class AndroidStringsGenerator
{
    public static void GenerateAndroidStrings(string resxFilePath, string androidStringsPath)
    {
        var xDoc = XDocument.Load(resxFilePath);

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(
            """
                <?xml version="1.0" encoding="utf-8" ?>
                <resources>
                """);

        if (xDoc.Root != null)
        {
            foreach (var element in xDoc.Root.Descendants("data"))
            {
                if (element?.Attribute("name") is not null)
                {
                    var name = GetName(element);
                    var value = GetValue(element);

                    stringBuilder.AppendLine($"<string name=\"{name}\">{value}</string> ");
                }
            }
        }

        stringBuilder.Append("</resources>");

        using StreamWriter writer = new (androidStringsPath, false, new UTF8Encoding(false), 4096);
        // Write the text to the file
        writer.Write(stringBuilder.ToString());
    }

    private static string GetName(XElement element)
    {
        string name = element.Attribute("name")!.Value.ToLower();
        return Regex.Replace(name, "[^a-zA-Z0-9_]{1,1}", "_");
    }

    private static string GetValue(XElement element)
    {
        XElement vitem = element.Descendants().FirstOrDefault();
        string name = vitem.Value;
        name = name.Replace("'", "\\'")
            .Replace("&", "&amp;");
        return name;
    }
}
