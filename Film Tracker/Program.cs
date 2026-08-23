using System.Security;
using System.Text.Json.Serialization;
using System.Text.Json;

// Create browser
using HttpClient client = new HttpClient();

// Tell the browser where to go
string url = "https://api.themoviedb.org/3/search/movie?query=Jack+Reacher&api_key=c59abf43762dd8a06f11594d0d89b6cf";

// Wait for the browser to get the response
HttpResponseMessage response = await client.GetAsync(url);

// Read the text from the response
string rawData = await response.Content.ReadAsStringAsync();

// Convert the JSON text into our C# Classes
TmdbResponse movieData = JsonSerializer.Deserialize<TmdbResponse>(rawData);

// Loop through the list of movies and print just the titles and dates!
Console.WriteLine("--- SEARCH RESULTS ---");
foreach (Movie movie in movieData.Results)
{
    Console.WriteLine($"{movie.Title} ({movie.ReleaseDate})");
}

public class TmdbResponse
{
    // TMDB sends a list of movies inside a property called "results"
    [JsonPropertyName("results")]
    public List<Movie> Results { get; set; }
}

public class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }
    
}