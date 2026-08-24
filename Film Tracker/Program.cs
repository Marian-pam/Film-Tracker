using System.Security;
using System.Text.Json.Serialization;
using System.Text.Json;


using HttpClient client = new HttpClient(); // Create browser
Console.WriteLine("What film or TV show would you like to search for?"); // Lets the user search for whatever they want
string userInput = Console.ReadLine(); // Reads userinput
string safeSearchQuery = Uri.EscapeDataString(userInput); // If the user adds a space while searching for films, TMDB will crash, this is a safety new to ensure it doesn't


// API
// URL for search query
string url = $"https://api.themoviedb.org/3/search/multi?query={safeSearchQuery}&api_key=c59abf43762dd8a06f11594d0d89b6cf";
HttpResponseMessage response = await client.GetAsync(url); // Wait for the browser to get the response
string rawData = await response.Content.ReadAsStringAsync(); // Read the text from the response

TmdbResponse movieData = JsonSerializer.Deserialize<TmdbResponse>(rawData); // Convert the JSON text into our C# Classes


// For loop that prints out the results and numbers them for the user
Console.WriteLine("--- SEARCH RESULTS ---");

for (int i = 0; i < movieData.Results.Count; i++)
{
    Media media = movieData.Results[i]; // i + i makes the list start at 1, 2, 3 instead of 0, 1, 2 for the user
    Console.WriteLine($"{i + 1}. [{media.MediaType.ToUpper()}] {media.DisplayName} ({media.DisplayDate})");
}


Console.WriteLine("\nEnter the number of the film to check exactly where it is streaming: ");
string choice = Console.ReadLine();
int selectedNumber = int.Parse(choice); // Convert the text into an integer
int selectedIndex = selectedNumber - 1; // Subtract 1 because C# starts counting at 0, but the user starts counting at 1




if (movieData.Results.Count > 0) // Make sure we find a movie before continuing
{
    Media topMovie = movieData.Results[selectedIndex]; // Grab the first movie in the results
    Console.WriteLine($"\nLooking up streaming services for: {topMovie.Title}...");

    string providerUrl = $"https://api.themoviedb.org/3/movie/{topMovie.Id}/watch/providers?api_key=c59abf43762dd8a06f11594d0d89b6cf";
    HttpResponseMessage providerResponse = await client.GetAsync(providerUrl); // Wait for the browser to get the response and save it to a new variable
    string providerRawData = await providerResponse.Content.ReadAsStringAsync(); // Read the internet data as a string
    // (Don't need it) Console.WriteLine(providerRawData); // Print the internet data to the screen

    ProviderResponse providerData = JsonSerializer.Deserialize<ProviderResponse>(providerRawData);

    List<ProviderInfo> gbStreaming = providerData.Results?.GB?.Flatrate;

    if (gbStreaming != null)
    {
        Console.WriteLine("\nCurrently Streaming On (UK): ");
        foreach (ProviderInfo provider in gbStreaming)
        {
            Console.WriteLine($"- {provider.ProviderName}");
        }
    }
    else
    {
        Console.WriteLine("\nSorry, this movie is not currently streaming on any subscription service in the UK."); ;
    }


}



// C# Classes to read the data from TMDB
public class TmdbResponse // Reads the data from TMDB
{
    // TMDB sends a list of movies inside a property called "results"
    [JsonPropertyName("results")]
    public List<Media> Results { get; set; }
}

// Convers the data from TMDB into separate C# classes
public class Media
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; }

    // Films use "title" and "release_date"
    [JsonPropertyName("title")]
    public string Title { get; set; }


    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }

    // TV shows use "name" and "first_air_date"
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("first_air_date")]
    public string FirstAirDate { get; set; }

    // Helper to easily ge the display name, whether it's a TV show or Film
    public string DisplayName => Title ?? Name ?? "Unknown Title";
    public string DisplayDate => ReleaseDate ?? FirstAirDate ?? "Unknown Date";
    
}

public class ProviderResponse
{
    [JsonPropertyName("results")]
    public ProviderCountries Results { get; set; }
}

public class ProviderCountries
{
    [JsonPropertyName("GB")] // You can change "US" to "GB", "CA", "AU", depending on where you live!
    public CountryData GB { get; set; }
}

public class CountryData
{
    [JsonPropertyName("flatrate")] // "flatrate" is TMDB's word for standard streaming subscriptions (Netflix, Hulu, etc)
    public List<ProviderInfo> Flatrate { get; set; }
}

public class ProviderInfo
{
    [JsonPropertyName("provider_name")] // Json version of variable name
    public string ProviderName { get; set; } // Creates a string variable called ProviderName, and reads the Json version and saves it into this variable
}