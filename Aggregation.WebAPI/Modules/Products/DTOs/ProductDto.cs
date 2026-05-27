using System.Text.Json.Serialization;

namespace Aggregation.WebAPI.Modules.Products.DTOs
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public Uri? Image { get; set; }

        [JsonPropertyName("rating")]
        public ProductRatingDto? Rating { get; set; }
    }

    public class ProductRatingDto
    {
        [JsonPropertyName("rate")]
        public double Rate { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
