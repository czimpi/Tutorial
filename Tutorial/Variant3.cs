using System.Text.Json;

namespace Tutorial;

public class Variant3
{
    /* Manual deserialization
     */
    
    public struct InputItem
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public static void DoWork(string inputFile, string outputFile)
    {
        using Stream fileStream = File.OpenRead(inputFile);
        using Stream outputStream = File.OpenWrite(outputFile);
        using Utf8JsonStreamReader reader = new Utf8JsonStreamReader(fileStream, 32 * 1024);
        using Utf8JsonWriter writer = new Utf8JsonWriter(outputStream);

        InputItem item = new InputItem();
        
        writer.WriteStartObject();
        writer.WritePropertyName("Items");
        writer.WriteStartArray();

        int prop = 0;

        int itemsRead = 0;

        int green = 0, brown = 0, blue = 0;
        
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    item = new InputItem();
                    break;
                
                case JsonTokenType.EndObject:
                    itemsRead++;
                    writer.WriteStringValue($"{item.FirstName} {item.LastName}");
                    if (itemsRead % 150 == 0)
                    {
                        writer.Flush();
                    }
                    break;
                
                case JsonTokenType.PropertyName when reader.GetString() == "firstname":
                    prop = 0;
                    break;
                
                case JsonTokenType.PropertyName when reader.GetString() == "surname":
                    prop = 1;
                    break;
                
                case JsonTokenType.PropertyName when reader.GetString() == "eyeColor":
                    prop = 2;
                    break;
                
                case JsonTokenType.String when prop == 0:
                    item.FirstName = reader.GetString()!;
                    break;
                
                case JsonTokenType.String when prop == 1:
                    item.LastName = reader.GetString()!;
                    break;
                
                case JsonTokenType.String when prop == 2:
                    string eyeColor = reader.GetString();
                    
                    if (string.Equals(eyeColor, "brown", StringComparison.OrdinalIgnoreCase))
                    {
                        brown++;
                    }
                    else if (string.Equals(eyeColor, "green", StringComparison.OrdinalIgnoreCase))
                    {
                        green++;
                    }
                    else if (string.Equals(eyeColor, "blue", StringComparison.OrdinalIgnoreCase))
                    {
                        blue++;
                    }
                    break;
            }
        }
        
        writer.WriteEndArray();
        writer.WriteNumber("BrownCount", brown);
        writer.WriteNumber("GreenCount", green);
        writer.WriteNumber("BlueCount", blue);
        writer.WriteEndObject();
    }
}