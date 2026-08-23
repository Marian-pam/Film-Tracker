using System.Security;
using System.Text.Json.Serialization;
using System.Text.Json;

// Create browser
using HttpClient client = new HttpClient();

// Lets the user search for whatever they want
Console.WriteLine("What film or TV show would you like to search for?");
string userInput = Console.ReadLine();
// If the user adds a space while searching for films, TMDB will crash, this is a safety new to ensure it doesn't
string safeSearchQuery = Uri.EscapeDataString(userInput);


// API
// Tell the browser where to go
string url = $"https://api.themoviedb.org/3/search/movie?query={safeSearchQuery}&api_key=c59abf43762dd8a06f11594d0d89b6cf";

// Wait for the browser to get the response
HttpResponseMessage response = await client.GetAsync(url);

// Read the text from the response
string rawData = await response.Content.ReadAsStringAsync();

// Convert the JSON text into our C# Classes
TmdbResponse movieData = JsonSerializer.Deserialize<TmdbResponse>(rawData);


// For loop that prints out the results and numbers them for the user
Console.WriteLine("--- SEARCH RESULTS ---");

for (int i = 0; i < movieData.Results.Count; i++)
{
    Movie movie = movieData.Results[i]; // i + i makes the list start at 1, 2, 3 instead of 0, 1, 2 for the user
    Console.WriteLine($"{i + 1}. {movie.Title} ({movie.ReleaseDate})");
}

Console.WriteLine("\nEnter the number of the film to check exactly where it is streaming: ");
string choice = Console.ReadLine();

int selectedNumber = int.Parse(choice); // Convert the text into an integer
int selectedIndex = selectedNumber - 1; // Subtract 1 because C# starts counting at 0, but the user starts counting at 1



if (movieData.Results.Count > 0) // Make sure we find a movie before continuing
{
    // Grab the first movie in the results
    Movie topMovie = movieData.Results[selectedIndex];
    Console.WriteLine($"\nLooking up streaming services for: {topMovie.Title}...");

    string providerUrl = $"https://api.themoviedb.org/3/movie/{topMovie.Id}/watch/providers?api_key=c59abf43762dd8a06f11594d0d89b6cf";
    HttpResponseMessage providerResponse = await client.GetAsync(providerUrl); // Wait for the browser to get the response and save it to a new variable
    string providerRawData = await providerResponse.Content.ReadAsStringAsync(); // Read the internet data as a string
    Console.WriteLine(providerRawData); // Print the internet data to the screen

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



// Reads the data from TMDB
public class TmdbResponse
{
    // TMDB sends a list of movies inside a property called "results"
    [JsonPropertyName("results")]
    public List<Movie> Results { get; set; }
}

// Convers the data from TMDB into separate C# classes
public class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }
    
}

public class ProviderResponse
{
    [JsonPropertyName("results")]
    public ProviderCountries Results { get; set; }
}

public class ProviderCountries
{
    // You can change "US" to "GB", "CA", "AU", depending on where you live!
    [JsonPropertyName("GB")]
    public CountryData GB { get; set; }
}

public class CountryData
{
    // "flatrate" is TMDB's word for standard streaming subscriptions (Netflix, Hulu, etc)
    [JsonPropertyName("flatrate")]
    public List<ProviderInfo> Flatrate { get; set; }
}

public class ProviderInfo
{
    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }
}