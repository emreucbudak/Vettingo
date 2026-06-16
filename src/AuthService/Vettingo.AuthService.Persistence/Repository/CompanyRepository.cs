using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Persistence.Repository
{
    public class CompanyRepository(IdentityDbContext identity) : ICompanyRepository
    {
        private  DbSet<Company> _companySet => identity.Set<Company>();
        public async Task AddCompanyAsync(Company company)
        {
            await _companySet.AddAsync(company);
        }

        public void DeleteCompanyAsync(Company company)
        {
             _companySet.Remove(company);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _companySet.ToListAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(Guid companyId)
        {
            return await _companySet.Where(c => c.Id == companyId).FirstOrDefaultAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return identity.SaveChangesAsync();
        }

        public void UpdateCompanyAsync(Company company)
        {
           _companySet.Update(company);
        }
        
    }
}
