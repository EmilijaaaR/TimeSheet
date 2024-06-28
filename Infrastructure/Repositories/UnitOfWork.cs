using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TimeSheetDbContext _dbContext;

        private ICategoryRepository _categoryRepository;
        private IClientRepository _clientRepository;
        private ICountryRepository _countryRepository;
        private IProjectRepository _projectRepository;
        private ITimesheetRepository _timesheetRepository;
        private IUserRepository _userRepository;
        private IReportRepository _reportRepository;

        public UnitOfWork(TimeSheetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_dbContext);
            }
        }

        public IClientRepository ClientRepository
        {
            get
            {
                return _clientRepository ??= new ClientRepository(_dbContext);
            }
        }

        public ICountryRepository CountryRepository
        {
            get
            {
                return _countryRepository ??= new CountryRepository(_dbContext);
            }
        }

        public IProjectRepository ProjectRepository
        {
            get
            {
                return _projectRepository ??= new ProjectRepository(_dbContext);
            }
        }

        public ITimesheetRepository TimesheetRepository
        {
            get
            {
                return _timesheetRepository ??= new TimesheetRepository(_dbContext);
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ??= new UserRepository(_dbContext);
            }
        }

        public IReportRepository ReportRepository
        {
            get
            {
                return _reportRepository ??= new ReportRepository(_dbContext);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
