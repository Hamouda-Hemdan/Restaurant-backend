using System.Text.Json.Serialization;

namespace resturant1.Models.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DishCategory
    {
        Wok,
        Pizza,
        Soup,
        Dessert,
        Drink
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortingOptions
    {
        NameAsc,
        NameDesc,
        PriceAsc,
        PriceDesc,
        RatingAsc,
        RatingDesc
    }
}
