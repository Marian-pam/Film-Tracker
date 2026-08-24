using System.Text.Json.Serialization;

namespace Film_Tracker_Web.Models
{
    public class TmdbModeIs
    {
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
}
