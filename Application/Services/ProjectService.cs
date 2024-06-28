using API.RequestEntities;
using Application.MappingExtension;
using Application.RequestEntities;
using Application.ViewEntities;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Services
{
    public class ProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectView> CreateAsync(ProjectRequest request)
        {
            var clientExists = await _unitOfWork.ClientRepository.ExistsAsync(request.ClientId);
            if (!clientExists)
            {
                throw new ArgumentException("Client does not exist.");
            }

            var userExists = await _unitOfWork.UserRepository.ExistsAsync(request.UserId);
            if (!userExists)
            {
                throw new ArgumentException("User does not exist.");
            }

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                ClientId = request.ClientId
            };
            await _unitOfWork.ProjectRepository.InsertAsync(project);
            await _unitOfWork.SaveChangesAsync();

            var projectUser = new ProjectUser
            {
                ProjectId = project.Id,
                UserId = request.UserId
            };
            await _unitOfWork.UserRepository.AssignLeadToProjectAsync(projectUser);
            await _unitOfWork.SaveChangesAsync();

            return project.ToView();
        }

        public async Task<ProjectView> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new ArgumentException("Project does not exist.");
            }
            return project.ToView();
        }

        public async Task<ProjectView> UpdateAsync(ProjectUpdateRequest request)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.Id);
            if (project == null)
            {
                throw new ArgumentException("Project not found.");
            }

            var clientExists = await _unitOfWork.ClientRepository.ExistsAsync(request.ClientId);
            if (!clientExists)
            {
                throw new ArgumentException("Client does not exist.");
            }

            var userExists = await _unitOfWork.UserRepository.ExistsAsync(request.UserId);
            if (!userExists)
            {
                throw new ArgumentException("User does not exist.");
            }

            if (!Enum.TryParse(request.Status, true, out ProjectStatus status))
            {
                throw new ArgumentException("Invalid status value");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.ClientId = request.ClientId;
            project.Status = status;

            await _unitOfWork.SaveChangesAsync();

            var existingProjectUser = await _unitOfWork.UserRepository.GetByProjectIdAsync(project.Id);
            if (existingProjectUser == null)
            {
                var projectUser = new ProjectUser
                {
                    ProjectId = project.Id,
                    UserId = request.UserId
                };
                await _unitOfWork.UserRepository.AssignLeadToProjectAsync(projectUser);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {

                await _unitOfWork.UserRepository.DeleteLeadToProjectAsync(existingProjectUser);
                await _unitOfWork.SaveChangesAsync();
                var projectUser = new ProjectUser
                {
                    ProjectId = project.Id,
                    UserId = request.UserId
                };
                await _unitOfWork.UserRepository.AssignLeadToProjectAsync(projectUser);
                await _unitOfWork.SaveChangesAsync();
            }
            return project.ToView();
        }



        public async Task DeleteAsync(int id)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new ArgumentException("Project not found.");
            }

            var projectUser = await _unitOfWork.UserRepository.GetByProjectIdAsync(id);
            if (projectUser != null)
            {
                await _unitOfWork.UserRepository.DeleteLeadToProjectAsync(projectUser);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.ProjectRepository.DeleteAsync(project);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<IEnumerable<ProjectView>> GetAllAsync()
        {
            var projects = await _unitOfWork.ProjectRepository.GetAllAsync();
            return projects.Select(project => project.ToView());
        }

        public async Task<IEnumerable<ProjectView>> GetProjectsByClientIdAsync(int clientId)
        {
            var clientExists = await _unitOfWork.ClientRepository.ExistsAsync(clientId);
            if (!clientExists)
            {
                throw new ArgumentException("Client does not exist.");
            }
            var projects = await _unitOfWork.ProjectRepository.GetProjectsByClientIdAsync(clientId);
            var projectIds = projects.Select(p => p.Id).ToList();
            var projectUsers = await _unitOfWork.UserRepository.GetByProjectsIdsAsync(projectIds);
            var projectViews = projects
                .Join(projectUsers,
                      project => project.Id,
                      projectUser => projectUser.ProjectId,
                      (project, projectUser) => new ProjectView
                      {
                          Id = project.Id,
                          Name = project.Name,
                          Description = project.Description,
                          ClientId = project.ClientId,
                          UserId = projectUser.UserId,
                          Status = project.Status.ToString()
                      })
                .ToList();
            return projectViews;
        }

        public async Task<IEnumerable<ProjectView>> GetProjectsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var projects = await _unitOfWork.ProjectRepository.GetProjectsByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            var projectIds = projects.Select(p => p.Id).ToList();
            var projectUsers = await _unitOfWork.UserRepository.GetByProjectsIdsAsync(projectIds);

            var projectViews = projects
                .Join(projectUsers,
                      project => project.Id,
                      projectUser => projectUser.ProjectId,
                      (project, projectUser) => new ProjectView
                      {
                          Id = project.Id,
                          Name = project.Name,
                          Description = project.Description,
                          ClientId = project.ClientId,
                          UserId = projectUser.UserId,
                          Status = project.Status.ToString()
                      })
                .ToList();
            return projectViews;
        }

    }

}
