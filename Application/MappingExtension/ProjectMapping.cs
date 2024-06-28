using Application.ViewEntities;
using Domain.Entities;

namespace Application.MappingExtension
{
    public static class ProjectMapping
    {
        public static ProjectView ToView(this Project project)
        {
            var projectUser = project.ProjectUsers?.FirstOrDefault();
            int userId = projectUser != null ? projectUser.UserId : 0;

            return new ProjectView
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                ClientId = project.ClientId,
                UserId = userId,
                Status = project.Status.ToString() 
            };
        }
    }
}
