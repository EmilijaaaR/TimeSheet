using Application.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class CountryMapping
    {
        public static CountryView ToView(this Country country)
        {
            return new CountryView
            {
                Id = country.Id,
                Name = country.Name
            };
        }
    }
}
