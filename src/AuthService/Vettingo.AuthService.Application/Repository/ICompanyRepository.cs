using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Repository
{
    public interface ICompanyRepository
    {
        Task AddCompanyAsync(Company company);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);
        Task<Company?> GetCompanyByIdAsync(Guid companyId);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<int> SaveChangesAsync();
    }
}
