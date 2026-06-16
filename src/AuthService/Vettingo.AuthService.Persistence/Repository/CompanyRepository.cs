using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.Repository
{
    public class CompanyRepository(IdentityDbContext identity) : ICompanyRepository
    {
        private DbSet<Company> CompanySet => identity.Set<Company>();

        public async Task AddCompanyAsync(Company company)
        {
            await CompanySet.AddAsync(company);
        }

        public void DeleteCompany(Company company)
        {
            CompanySet.Remove(company);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await CompanySet.ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
        {
            return await CompanySet.FirstOrDefaultAsync(c => c.Id == companyId);
        }

        public Task<int> SaveChangesAsync()
        {
            return identity.SaveChangesAsync();
        }

        public void UpdateCompany(Company company)
        {
            CompanySet.Update(company);
        }
    }
}
