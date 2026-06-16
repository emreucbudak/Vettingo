namespace Vettingo.AuthService.Application.Repository
{
    public interface ICompanyRepository
    {
        Task AddCompanyAsync(AuthService.Domain.Entities.Company company);
        void UpdateCompany(AuthService.Domain.Entities.Company company);
        void DeleteCompany(AuthService.Domain.Entities.Company company);
        Task<AuthService.Domain.Entities.Company> GetCompanyByIdAsync(Guid companyId);
        Task<IEnumerable<AuthService.Domain.Entities.Company>> GetAllCompaniesAsync();
        Task<int> SaveChangesAsync();
    }
}
