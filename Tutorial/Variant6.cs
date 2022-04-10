using System.Text;
using System.Text.Json;

namespace Tutorial;

public class Variant6
{
    /* Get rid of allocations related to name
     */
    
    public ref struct InputItem
    {
        public ReadOnlySpan<byte> FirstName { get; set; }

        public string FirstNameFallback { get; set; }

        public ReadOnlySpan<byte> LastName { get; set; }

        public string LastNameFallback { get; set; }
    }

    public static void DoWork(string inputFile, string outputFile)
    {
        Span<byte> name = stackalloc byte[512];
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

        ReadOnlySpan<byte> firstNamePropName = Encoding.UTF8.GetBytes("firstname");
        ReadOnlySpan<byte> surNamePropName = Encoding.UTF8.GetBytes("surname");
        ReadOnlySpan<byte> eyeColorPropName = Encoding.UTF8.GetBytes("eyeColor");
        ReadOnlySpan<byte> whiteSpace = Encoding.UTF8.GetBytes(" ");

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    item = new InputItem();
                    break;
                
                case JsonTokenType.EndObject:
                    itemsRead++;
                    if (item.LastNameFallback == null && item.FirstNameFallback == null && item.FirstName.Length + item.LastName.Length + whiteSpace.Length <= 512)
                    {
                        item.FirstName.CopyTo(name);
                        whiteSpace.CopyTo(name[item.FirstName.Length..]);
                        item.LastName.CopyTo(name[(item.FirstName.Length+whiteSpace.Length)..]);
                        if (Encoding.UTF8.GetString(
                                name[..(item.FirstName.Length + whiteSpace.Length + item.LastName.Length)]) ==
                            "Lizzie Buckley")
                        {
                            ;
                        }
                        writer.WriteStringValue(name[..(item.FirstName.Length + whiteSpace.Length + item.LastName.Length)]);
                    }
                    else
                    {
                        string firstName = item.FirstNameFallback ?? Encoding.UTF8.GetString(item.FirstName);
                        string lastName = item.LastNameFallback ?? Encoding.UTF8.GetString(item.LastName);
                        writer.WriteStringValue($"{firstName} {lastName}");    
                    }
                    
                    if (itemsRead % 150 == 0)
                    {
                        writer.Flush();
                    }
                    break;
                
                case JsonTokenType.PropertyName:
                    if (reader.HasValueSequence)
                    {
                        string propName = reader.GetString()!;
                        prop = propName switch
                        {
                            "firstname" => 0,
                            "surname" => 1,
                            "eyeColor" => 2
                        };    
                    }
                    else
                    {
                        if (reader.ValueSpan.IndexOf(firstNamePropName) == 0)
                        {
                            prop = 0;
                        }
                        else if (reader.ValueSpan.IndexOf(surNamePropName) == 0)
                        {
                            prop = 1;
                        }
                        else if (reader.ValueSpan.IndexOf(eyeColorPropName) == 0)
                        {
                            prop = 2;
                        }
                    }
                    
                    break;

                case JsonTokenType.String when prop == 0:
                    if (reader.HasValueSequence)
                    {
                        item.FirstNameFallback = reader.GetString()!;
                    }
                    else
                    {
                        // BUG! The underlying memory can be freed / changed if the buffer is full
                        // --> The interior pointer is then pointing to garbage!
                        item.FirstName = reader.ValueSpan;
                    }
                    break;
                
                case JsonTokenType.String when prop == 1:
                    if (reader.HasValueSequence)
                    {
                        item.LastNameFallback = reader.GetString()!;
                    }
                    else
                    {
                        // BUG! The underlying memory can be freed / changed if the buffer is full
                        // --> The interior pointer is then pointing to garbage!
                        item.LastName = reader.ValueSpan;
                    }
                    break;
                
                case JsonTokenType.String when prop == 2:
                    string eyeColor = reader.GetString()!;
                    
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