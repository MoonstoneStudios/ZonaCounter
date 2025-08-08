using Newtonsoft.Json;

namespace ZonaCounter;

class Program 
{
    private const string APP_FOLDER_NAME = "ZonaCounter";

    private static int Main(string[] args) 
    {
        // Get JSON data.
        string storageFilePath = GetStorageFileFullPath();

        // Open the file.
        using FileStream stream = File.Open(storageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using StreamReader reader = new StreamReader(stream);

        // Read JSON data.
        string storageContents = reader.ReadToEnd();
        ZonaData? data;

        try 
        {
            data = JsonConvert.DeserializeObject<ZonaData>(storageContents); 
            data ??= new ZonaData([]);
        }
        catch 
        {
            Console.WriteLine("Unknown json. Please fix json save file located at: " + GetStorageFileFullPath());
            return -1;
        }

        // If we have a command.
        bool handled = HandleCommands(data, args, stream);
        if (handled) return 0;

        HandleArguments(data, args);
        Save(data, stream);
        DisplayStats(data);

        return 0;
    }
    
    
    /// <summary>Save data to the file system.</summary>
    private static void Save(ZonaData data, FileStream stream) 
    {
        string json = JsonConvert.SerializeObject(data);
        // https://stackoverflow.com/a/5346088
        stream.SetLength(0);
        stream.Flush();
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write(json);
        writer.Flush();
    }
    
    
    /// <summary>Handle file commands.</summary>
    private static bool HandleCommands(ZonaData data, string[] args, FileStream stream) 
    {
        if (args.Length > 0) 
        {
            string firstArg = args[0].ToLower();
            // check if we have a command. If we cant convert it is a command.
            if (!int.TryParse(firstArg, out int _)) 
            {
                switch (firstArg)
                {
                    case "clear":
                        Console.WriteLine("Clearing save data...");
                        stream.Close();
                        File.Delete(GetStorageFileFullPath());
                        if (NotLinux()) Directory.Delete(GetStorageDirectory());
                        Console.WriteLine("Data cleared.");
                        return true;
                    case "stats":
                        DisplayStats(data);
                        break;
                    case "default_product":
                        string product = args[1];
                        data.DefaultProduct = product;
                        break;
                    case "change_unit_price":
                        string productName = args[1];
                        float newPrice = float.Parse(args[2]);
                        data.ChangePrice(productName, newPrice);
                        break;
                    case "help":
                        Console.WriteLine("Usage:");
                        Console.WriteLine("> ./ZonaCounter [count] [name] [unit price]");
                        Console.WriteLine("\tAll above arguments are optional. If you use an argument, all previous arguments must be also present.");
                        Console.WriteLine("\tWhen no arguments are used, the default product will be incremented by 1.");
                        Console.WriteLine("\tDefault unit price = $0.99");
                        Console.WriteLine("\nOR\n");
                        Console.WriteLine("> ./ZonaCounter clear/help/stats");
                        Console.WriteLine("\t'clear' will delete files, 'help' shows this message, 'stats' shows all data.");
                        Console.WriteLine("\nOR\n");
                        Console.WriteLine("> ./ZonaCounter default_product (name)");
                        Console.WriteLine("\t'name' here is required.");
                        Console.WriteLine("\nOR\n");
                        Console.WriteLine("> ./ZonaCounter change_unit_price (name) (unit price)");
                        Console.WriteLine("\t'name' and 'unit price' here are required.");
                        return true;
                    default:
                        throw new Exception("Error occured. Unknown command.");
                }
                
                Save(data, stream);
                return true;
            }
        }

        return false;
    }
    
    // Usage
    // zonacounter 
    // zonacounter 3
    // zonacounter 3 "Sweet Tea"
    // zonacounter 3 "Sweet Tea" 0.99
    /// <summary>When given arguments that are not commands, alter the product info.</summary>
    private static void HandleArguments(ZonaData data, string[] args) 
    {
        switch (args.Length)
        {
            case 0:
                data.IncrementProduct(data.DefaultProduct, 1);
                break;
            case 1:
                {
                    int count = Convert.ToInt32(args[0]);
                    data.IncrementProduct(data.DefaultProduct, count);
                    break;
                }
            case 2:
                {
                    int count = Convert.ToInt32(args[0]);
                    string name = args[1];
                    data.IncrementProduct(name, count);
                    break;
                }
            case 3: 
                {
                    int count = Convert.ToInt32(args[0]);
                    string name = args[1];
                    float unitPrice = Convert.ToSingle(args[2]);
                    data.IncrementProduct(name, count, unitPrice);
                    break;
                }
            default:
                throw new Exception("Error occured. Unknown arguments.");
        }
    }
    
    
    /// <summary>Show the stats to the user.</summary>
    public static void DisplayStats(ZonaData data) 
    {
        Console.WriteLine("Stats:");
        Console.WriteLine($"\tTotal Items Purchased: {data.SumCount()}");
        Console.WriteLine($"\tTotal Cost: ${data.SumCost()}");
        Console.WriteLine();
        foreach (ZonaProduct product in data.Products)
        {
            if (product.Name == data.DefaultProduct)
                Console.WriteLine("\t\tDEFAULT:");

            Console.WriteLine($"\t\t{product.Name}:");
            Console.WriteLine($"\t\tCount Purchased: {product.TotalCount}");
            Console.WriteLine($"\t\tTotal Cost: ${product.TotalCost}");
            Console.WriteLine($"\t\tCurrent Unit Price: ${product.UnitPrice}");
            Console.WriteLine();
        }
    }
    
    
    /// <summary>Get the folder for the data storage.</summary>
    private static string GetStorageDirectory() 
    {
        if (OperatingSystem.IsLinux())
            // https://askubuntu.com/a/4421
            // https://stackoverflow.com/a/7404448
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        else if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_FOLDER_NAME);

        throw new Exception("This operating system is not supported.");
    }

    private static bool NotLinux() => OperatingSystem.IsWindows() || OperatingSystem.IsMacOS();
    private static string GetStorageFileName() => OperatingSystem.IsLinux() ? $".{APP_FOLDER_NAME}" : $"{APP_FOLDER_NAME}.json";
    private static string GetStorageFileFullPath() => Path.Combine(GetStorageDirectory(), GetStorageFileName());
}