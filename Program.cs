using System.Diagnostics.Tracing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class SatelitePass
{
    public EventData rise { get; set; }
    public EventData set { get; set; }
    public EventData culmination { get; set; }

    public SatelitePass(EventData rise, EventData set, EventData culmination)
    {
        this.rise = rise;
        this.set = set;
        this.culmination = culmination;
    }
}

class EventData
{
    public string alt { get; set; }
    public string az { get; set; }
    public string[] azOctant;
    public string timetamp { get; set; }

    public EventData(string alt, string az, string azOctant, string timetamp)
    {
        this.alt = alt;
        this.az = az;
        this.azOctant = azOctant;
        this.timetamp = timetamp;
    }
}

class Program
{
    public static async Task Main(string[] args)
    {
        // Get the noradID and coordinates from the user and validate it
        int intTryCatchInput()
        {
            while (true)
            {
                try
                {
                    int userInput = int.Parse(Console.ReadLine());
                    return userInput;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid number.");
                    Console.ReadKey();
                }
            }
        }
        double doubleTryCatchInput()
        {
            while (true)
            {
                try
                {
                    double userInput = double.Parse(Console.ReadLine());
                    return userInput;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid number.");
                    Console.ReadKey();
                }
            }
        }
        Console.WriteLine("Enter the noradID of the satellite you want to track: ");
        int noradID = intTryCatchInput();
        Console.WriteLine("Enter the latitude of the location you want to track the satellite from: ");
        double latitude = doubleTryCatchInput();
        Console.WriteLine("Enter the longitude of the location you want to track the satellite from: ");
        double longitude = doubleTryCatchInput();
        var passes = await GetSatelitePassesAsync(noradID, latitude, longitude);
        Console.WriteLine(JsonConvert.SerializeObject(passes, Formatting.Indented));
    }

    // Get the data from the API

    public static async Task<List<SatelitePass>> GetSatelitePassesAsync(int noradID, double latitude, double longitude)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://sat.terrestre.ar/passes/{noradID}?lat={latitude}&lon={longitude}&limit=1";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var passes = JsonConvert.DeserializeObject<List<SatelitePass>>(jsonResponse);
                return passes;
            }
            else
            {
                throw new Exception("Failed to get the data from the API.");
            }
        }
    }
}