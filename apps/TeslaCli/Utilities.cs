using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Julmar.TeslaApi;

namespace TeslaCli
{
    internal static class Utilities
    {
        static string lastJsonResponse;

        static public string LastJsonResponse
        {
            get
            {
                using MemoryStream stream = new();
                using Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true });
                var e = JsonSerializer.Deserialize<JsonElement>(lastJsonResponse);
                e.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        internal static void JsonCatcher(LogLevel level, string text)
        {
            if (level == LogLevel.RawData)
                lastJsonResponse = text;
        }

        internal static string ToCamelCase(this string value)
        {
            return string.IsNullOrWhiteSpace(value) || value.Length == 1
                ? value : Char.ToUpper(value[0]).ToString() + value.Substring(1);
        }

        internal static int Prompt(string prompt, string[] values)
        {
            Console.WriteLine();
            Console.WriteLine(prompt);

            for (int i = 0; i < values.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {values[i]}");
            }

            int choice = -1;
            while (choice == -1)
            {
                Console.Write("Select: ");
                string value = Console.ReadLine();
                if (int.TryParse(value, out choice))
                {
                    if (choice <= 0 || choice > values.Length)
                        choice = -1;
                }
            }

            return choice-1;
        }

        const string TokenCacheFilename = ".teslacli";

        internal static async Task<AccessToken> ReadTokenInfo()
        {
            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                TokenCacheFilename);

            if (File.Exists(filename))
            {
                try
                {
                    return JsonSerializer.Deserialize<AccessToken>(await File.ReadAllTextAsync(filename));
                }
                catch
                {
                }
            }
            return null;
        }

        internal static async Task WriteTokenInfo(AccessToken token)
        {
            string filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                TokenCacheFilename);

            await File.WriteAllTextAsync(filename, JsonSerializer.Serialize(token));
        }
    }
}