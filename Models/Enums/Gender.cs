using System.Text.Json.Serialization;

namespace resturant1.Models.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
