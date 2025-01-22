using resturant1.Models.Dto;
using resturant1.Models.DTOs;

public class PagedDishResponse
{
    public List<DishDto> Dishes { get; set; }
    public PaginationMetadata Pagination { get; set; }
    public int TotalCount { get; set; }  // Add this property
}
