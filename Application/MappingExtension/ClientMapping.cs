using API.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class ClientMapping
    {
        public static ClientView ToView(this Client client)
        {
            return new ClientView
            {
                Id = client.Id,
                Name = client.Name,
                Address = client.Address,
                City = client.City,
                PostalCode = client.PostalCode,
                CountryId = client.CountryId
            };
        }
    }
}
