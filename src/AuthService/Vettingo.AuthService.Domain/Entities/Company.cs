namespace Vettingo.AuthService.Domain.Entities
{
    public class Company
    {
        public Company()
        {
        }

        public Guid Id { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public string CompanySector { get; private set; } = string.Empty;
        public string CompanyWebsite { get; private set; } = string.Empty;
        public string CompanySize { get; private set; } = string.Empty;
        public string CompanyDescription { get; private set; } = string.Empty;
        public string CompanyAddress { get; private set; } = string.Empty;

        public void setCompanyName(string companyName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyName, nameof(companyName));
            CompanyName = companyName;
        }

        public void setCompanyDescription(string companyDescription)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyDescription, nameof(companyDescription));
            CompanyDescription = companyDescription;
        }

        public void setCompanySector(string companySector)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companySector, nameof(companySector));
            CompanySector = companySector;
        }

        public void setCompanyWebsite(string companyWebsite)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyWebsite, nameof(companyWebsite));
            CompanyWebsite = companyWebsite;
        }

        public void setCompanySize(string companySize)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companySize, nameof(companySize));
            CompanySize = companySize;
        }

        public void setCompanyAddress(string companyAddress)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyAddress, nameof(companyAddress));
            CompanyAddress = companyAddress;
        }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void SetId(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }

            Id = companyId;
        }

        public void RegisterCompany(string companyName)
        {
            setCompanyName(companyName);
            SetId();
        }

        public void RegisterCompany(
            Guid companyId,
            string companyName)
        {
            setCompanyName(companyName);
            SetId(companyId);
        }

        public void UpdateCompany(string companyName, string companySector, string companyWebsite, string companySize, string companyDescription, string companyAddress)
        {
            CheckCompanyContent(companyName, companySector, companyWebsite, companySize, companyDescription, companyAddress);
            setCompanyName(companyName);
            setCompanySector(companySector);
            setCompanyWebsite(companyWebsite);
            setCompanySize(companySize);
            setCompanyDescription(companyDescription);
            setCompanyAddress(companyAddress);
        }

        public void CheckCompanyContent(string companyName, string companySector, string companyWebsite, string companySize, string companyDescription, string companyAddress)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyName, nameof(companyName));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companySector, nameof(companySector));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyWebsite, nameof(companyWebsite));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companySize, nameof(companySize));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyDescription, nameof(companyDescription));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyAddress, nameof(companyAddress));
        }
    }
}
