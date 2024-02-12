using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace SharedLib.Models
{
    [Collection("products")]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string id { get; set; }

        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public List<Tag> tags { get; set; }
        public float[]? vector { get; set; }
        public bool featuredProduct { get; set; }
        public string imageUrl { get; set; }
        public string image { get; set; } = string.Empty;
    }

    public record ProductCatalogResult(int PageIndex, int PageSize, long Count, List<Product> Data);

    [Collection("products")]
    public class DisplayProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string id { get; set; }

        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public List<Tag> tags { get; set; }
        public bool featuredProduct { get; set; }
        public string imageUrl { get; set; }
        public string image { get; set; }
    }

    public class ProductCategory
    {
        public ProductCategory(string id, string type, string name)
        {
            this.id = id;
            this.type = type;
            this.name = name;
        }

        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Tag
    {
        public Tag(string id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string id { get; set; }
        public string name { get; set; }
    }
}