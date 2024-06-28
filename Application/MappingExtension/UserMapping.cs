using Application.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class UserMapping
    {
        public static UserView ToView(this User user)
        {
            return new UserView
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            };
        }
    }
}
