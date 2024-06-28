using Application.MappingExtension;
using Application.ViewEntities;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Services
{
    public class CountryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CountryView> CreateAsync(string name)
        {
            var country = new Country
            {
                Name = name
            };

            await _unitOfWork.CountryRepository.InsertAsync(country);
            await _unitOfWork.SaveChangesAsync();
            return country.ToView();
        }

        public async Task<CountryView> GetByIdAsync(int id)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(id);
            if (country == null) 
            {
                throw new ArgumentException("Country does not exist.");
            }
            return country.ToView();
        }

        public async Task<CountryView> UpdateAsync(int id, string name)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(id);
            if (country == null) 
            {
                throw new ArgumentException("Country not found");
            }
            country.Name = name;
            await _unitOfWork.SaveChangesAsync();
            return country.ToView();
        }

        public async Task DeleteAsync(int id)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(id);
            if (country == null)
            {
                throw new ArgumentException("Country not found");
            }
            await _unitOfWork.CountryRepository.DeleteAsync(country);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CountryView>> GetAllAsync()
        {
            var countries = await _unitOfWork.CountryRepository.GetAllAsync();
            return countries.Select(country => country.ToView());
        }
    }

}
