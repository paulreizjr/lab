using System;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("BigInteger Serialization and Deserialization Demo");
        Console.WriteLine("=================================================\n");

        // Create some BigInteger values to demonstrate serialization
        BigInteger smallNumber = 12345;
        BigInteger largeNumber = BigInteger.Parse("12345678901234567890123456789012345678901234567890");
        BigInteger negativeNumber = BigInteger.Parse("-987654321098765432109876543210");

        Console.WriteLine("Original BigInteger values:");
        Console.WriteLine($"Small number: {smallNumber}");
        Console.WriteLine($"Large number: {largeNumber}");
        Console.WriteLine($"Negative number: {negativeNumber}\n");

        // Method 1: JSON Serialization with System.Text.Json
        DemoJsonSerialization(smallNumber, largeNumber, negativeNumber);

        // Method 2: Binary Serialization (using ToByteArray/custom approach)
        DemoBinarySerialization(smallNumber, largeNumber, negativeNumber);

        // Method 3: String-based Serialization
        DemoStringSerialization(smallNumber, largeNumber, negativeNumber);

        // Method 4: XML Serialization (custom wrapper)
        DemoXmlSerialization(smallNumber, largeNumber, negativeNumber);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void DemoJsonSerialization(BigInteger small, BigInteger large, BigInteger negative)
    {
        Console.WriteLine("1. JSON Serialization (System.Text.Json)");
        Console.WriteLine("----------------------------------------");

        try
        {
            // Create a container class since BigInteger needs custom handling in JSON
            var data = new BigIntegerContainer
            {
                SmallNumber = small,
                LargeNumber = large,
                NegativeNumber = negative
            };

            // Serialize to JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new BigIntegerJsonConverter() }
            };

            string json = JsonSerializer.Serialize(data, options);
            Console.WriteLine($"Serialized JSON:\n{json}");

            // Deserialize from JSON
            var deserializedData = JsonSerializer.Deserialize<BigIntegerContainer>(json, options);
            
            if (deserializedData != null)
            {
                Console.WriteLine("Deserialized values:");
                Console.WriteLine($"Small: {deserializedData.SmallNumber}");
                Console.WriteLine($"Large: {deserializedData.LargeNumber}");
                Console.WriteLine($"Negative: {deserializedData.NegativeNumber}");
                Console.WriteLine($"Values match: {data.SmallNumber == deserializedData.SmallNumber && 
                                                    data.LargeNumber == deserializedData.LargeNumber && 
                                                    data.NegativeNumber == deserializedData.NegativeNumber}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON Serialization error: {ex.Message}\n");
        }
    }

    static void DemoBinarySerialization(BigInteger small, BigInteger large, BigInteger negative)
    {
        Console.WriteLine("2. Binary Serialization (ToByteArray/Constructor)");
        Console.WriteLine("------------------------------------------------");

        try
        {
            // Serialize using ToByteArray()
            byte[] smallBytes = small.ToByteArray();
            byte[] largeBytes = large.ToByteArray();
            byte[] negativeBytes = negative.ToByteArray();

            Console.WriteLine($"Small number byte array length: {smallBytes.Length}");
            Console.WriteLine($"Large number byte array length: {largeBytes.Length}");
            Console.WriteLine($"Negative number byte array length: {negativeBytes.Length}");

            // Deserialize using BigInteger constructor
            BigInteger deserializedSmall = new BigInteger(smallBytes);
            BigInteger deserializedLarge = new BigInteger(largeBytes);
            BigInteger deserializedNegative = new BigInteger(negativeBytes);

            Console.WriteLine("Deserialized values:");
            Console.WriteLine($"Small: {deserializedSmall}");
            Console.WriteLine($"Large: {deserializedLarge}");
            Console.WriteLine($"Negative: {deserializedNegative}");
            Console.WriteLine($"Values match: {small == deserializedSmall && 
                                                large == deserializedLarge && 
                                                negative == deserializedNegative}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Binary Serialization error: {ex.Message}\n");
        }
    }

    static void DemoStringSerialization(BigInteger small, BigInteger large, BigInteger negative)
    {
        Console.WriteLine("3. String-based Serialization");
        Console.WriteLine("-----------------------------");

        try
        {
            // Serialize to strings
            string smallStr = small.ToString();
            string largeStr = large.ToString();
            string negativeStr = negative.ToString();

            Console.WriteLine("Serialized as strings:");
            Console.WriteLine($"Small: \"{smallStr}\"");
            Console.WriteLine($"Large: \"{largeStr}\"");
            Console.WriteLine($"Negative: \"{negativeStr}\"");

            // Deserialize from strings
            BigInteger deserializedSmall = BigInteger.Parse(smallStr);
            BigInteger deserializedLarge = BigInteger.Parse(largeStr);
            BigInteger deserializedNegative = BigInteger.Parse(negativeStr);

            Console.WriteLine("Deserialized values:");
            Console.WriteLine($"Small: {deserializedSmall}");
            Console.WriteLine($"Large: {deserializedLarge}");
            Console.WriteLine($"Negative: {deserializedNegative}");
            Console.WriteLine($"Values match: {small == deserializedSmall && 
                                                large == deserializedLarge && 
                                                negative == deserializedNegative}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"String Serialization error: {ex.Message}\n");
        }
    }

    static void DemoXmlSerialization(BigInteger small, BigInteger large, BigInteger negative)
    {
        Console.WriteLine("4. XML Serialization (Custom Wrapper)");
        Console.WriteLine("-------------------------------------");

        try
        {
            var data = new XmlBigIntegerContainer
            {
                SmallNumber = small.ToString(),
                LargeNumber = large.ToString(),
                NegativeNumber = negative.ToString()
            };

            // Serialize to XML
            var serializer = new XmlSerializer(typeof(XmlBigIntegerContainer));
            string xml;
            
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, data);
                xml = stringWriter.ToString();
            }

            Console.WriteLine($"Serialized XML:\n{xml}");

            // Deserialize from XML
            XmlBigIntegerContainer? deserializedData;
            using (var stringReader = new StringReader(xml))
            {
                deserializedData = (XmlBigIntegerContainer?)serializer.Deserialize(stringReader);
            }

            if (deserializedData != null)
            {
                BigInteger deserializedSmall = BigInteger.Parse(deserializedData.SmallNumber);
                BigInteger deserializedLarge = BigInteger.Parse(deserializedData.LargeNumber);
                BigInteger deserializedNegative = BigInteger.Parse(deserializedData.NegativeNumber);

                Console.WriteLine("Deserialized values:");
                Console.WriteLine($"Small: {deserializedSmall}");
                Console.WriteLine($"Large: {deserializedLarge}");
                Console.WriteLine($"Negative: {deserializedNegative}");
                Console.WriteLine($"Values match: {small == deserializedSmall && 
                                                    large == deserializedLarge && 
                                                    negative == deserializedNegative}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"XML Serialization error: {ex.Message}\n");
        }
    }
}

// Container class for JSON serialization
public class BigIntegerContainer
{
    public BigInteger SmallNumber { get; set; }
    public BigInteger LargeNumber { get; set; }
    public BigInteger NegativeNumber { get; set; }
}

// Custom JSON converter for BigInteger
public class BigIntegerJsonConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return stringValue != null ? BigInteger.Parse(stringValue) : BigInteger.Zero;
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

// Container class for XML serialization (strings only)
[Serializable]
public class XmlBigIntegerContainer
{
    public string SmallNumber { get; set; } = string.Empty;
    public string LargeNumber { get; set; } = string.Empty;
    public string NegativeNumber { get; set; } = string.Empty;
}
