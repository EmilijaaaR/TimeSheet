using Application.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class CategoryMapping
    {
        public static CategoryView ToView(this Category category)
        {
            return new CategoryView
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
