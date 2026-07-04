namespace Vettingo.AuthService.Domain.Entities
{
    public class Company
    {
        public Company()
        {
        }

        public Guid Id { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public string CompanyDescription { get; private set; } = string.Empty;
        public string CompanyPhone { get; private set; } = string.Empty;
        public string CompanyEmail { get; private set; } = string.Empty;
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

        public void setCompanyPhone(string companyPhone)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyPhone, nameof(companyPhone));
            CompanyPhone = companyPhone;
        }

        public void setCompanyEmail(string companyEmail)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyEmail, nameof(companyEmail));
            CompanyEmail = companyEmail;
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

        public void UpdateCompany(string companyName, string companyDescription, string companyPhone, string companyEmail, string companyAddress)
        {
            CheckCompanyContent(companyName, companyDescription, companyPhone, companyEmail, companyAddress);
            setCompanyName(companyName);
            setCompanyDescription(companyDescription);
            setCompanyPhone(companyPhone);
            setCompanyEmail(companyEmail);
            setCompanyAddress(companyAddress);
            SetId();
        }

        public void CheckCompanyContent(string companyName, string companyDescription, string companyPhone, string companyEmail, string companyAddress)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyName, nameof(companyName));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyDescription, nameof(companyDescription));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyPhone, nameof(companyPhone));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyEmail, nameof(companyEmail));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(companyAddress, nameof(companyAddress));
        }
    }
}
