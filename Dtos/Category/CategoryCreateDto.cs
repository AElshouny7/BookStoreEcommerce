using System.ComponentModel.DataAnnotations;

namespace BookStoreEcommerce.Dtos.Category
{
    public class CategoryCreateDto
    {
        [Required, StringLength(120)]
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}