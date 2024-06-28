using API.RequestEntities;
using API.ViewEntities;
using Application.MappingExtension;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class ClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientView> CreateAsync(ClientRequest request)
        {
            var countryExists = await _unitOfWork.CountryRepository.ExistsAsync(request.CountryId);
            if (!countryExists)
            {
                throw new ArgumentException("Country does not exist.");
            }

            var client = new Client
            {
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                CountryId = request.CountryId
            };

            await _unitOfWork.ClientRepository.InsertAsync(client);
            await _unitOfWork.SaveChangesAsync();
            return client.ToView();
        }


        public async Task<ClientView> GetByIdAsync(int id)
        {
            var client = await _unitOfWork.ClientRepository.GetByIdAsync(id);
            if (client == null)
            {
                throw new ArgumentException("Client doesn't exist.");
            }
            return client.ToView();
        }

        public async Task<ClientView> UpdateAsync(int id, ClientRequest request)
        {
            var client = await _unitOfWork.ClientRepository.GetByIdAsync(id);
            if (client == null) 
            {
                throw new ArgumentException("Client not found");
            }

            var countryExists = await _unitOfWork.CountryRepository.ExistsAsync(request.CountryId);
            if (!countryExists)
            {
                throw new ArgumentException("Country does not exist.");
            }

            client.Name = request.Name;
            client.Address = request.Address;
            client.City = request.City;
            client.PostalCode = request.PostalCode;
            client.CountryId = request.CountryId;

            await _unitOfWork.SaveChangesAsync();
            return client.ToView();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _unitOfWork.ClientRepository.GetByIdAsync(id);
            if (client == null)
            {
                throw new ArgumentException("Client not found");
            }
            await _unitOfWork.ClientRepository.DeleteAsync(client);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClientView>> GetAllAsync()
        {
            var clients = await _unitOfWork.ClientRepository.GetAllAsync();
            return clients.Select(client => client.ToView()).ToList();
        }

        public async Task<IEnumerable<ClientView>> GetClientsByFiltersAsync(int pageNumber, int pageSize, char? letter, string searchTerm)
        {
            var clients = await _unitOfWork.ClientRepository.GetClientsByFiltersAsync(pageNumber, pageSize, letter, searchTerm);
            return clients.Select(client => client.ToView()).ToList();
            
        }

        public async Task<IEnumerable<ClientView>> GetClientsByUserIdAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.");
            }
            var clients =  await _unitOfWork.ClientRepository.GetClientsByUserIdAsync(userId);
            return clients.Select(client => client.ToView()).ToList();
        }
    }

}
