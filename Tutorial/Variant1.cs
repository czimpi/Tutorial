using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tutorial;

public class Variant1
{
    public class InputItem
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
            switch (item.EyeColor.ToLower())
            {
                case "brown": output.BrownCount++;
                    break;
                case "green": output.GreenCount++;
                    break;
                case "blue": output.BlueCount++;
                    break;
            }
            output.Items.Add(item.FirstName + " " + item.LastName);
        }

        await using Stream outputStream = File.OpenWrite(outputFile);
        await JsonSerializer.SerializeAsync(outputStream, output, cancellationToken: cancellationToken);
    }
}