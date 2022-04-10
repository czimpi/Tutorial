using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tutorial;

public class Variant2
{
    /* Struct usage
     * Better string comparison (eyeColor)
     * Interpolated string
     */
    
    public struct InputItem
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("surname")]
        public string LastName { get; set; }

        [JsonPropertyName("eyeColor")]
        public string EyeColor { get; set; }
    }

    public class OutputStructure
    {
        public int BrownCount { get; set; }
    
        public int BlueCount { get; set; }
    
        public int GreenCount { get; set; }

        public List<string> Items { get; set; } = new List<string>();
    }
    
    public static async Task DoWorkAsync(string inputFile, string outputFile, CancellationToken cancellationToken = default)
    {
        await using Stream fileStream = File.OpenRead(inputFile);
        List<InputItem> items = JsonSerializer.Deserialize<List<InputItem>>(fileStream)!;

        OutputStructure output = new OutputStructure();

        foreach (InputItem item in items)
        {
            if (string.Equals(item.EyeColor, "brown", StringComparison.OrdinalIgnoreCase))
            {
                output.BrownCount++;
            }
            else if (string.Equals(item.EyeColor, "green", StringComparison.OrdinalIgnoreCase))
            {
                output.GreenCount++;
            }
            else if (string.Equals(item.EyeColor, "blue", StringComparison.OrdinalIgnoreCase))
            {
                output.BlueCount++;
            }
            
            output.Items.Add($"{item.FirstName} {item.LastName}");
        }

        await using Stream outputStream = File.OpenWrite(outputFile);
        await JsonSerializer.SerializeAsync(outputStream, output, cancellationToken: cancellationToken);
    }
}