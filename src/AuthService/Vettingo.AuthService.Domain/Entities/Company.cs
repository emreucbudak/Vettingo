namespace Vettingo.AuthService.Domain.Entities
{
    public class Company
    {
        public Company()
        {
        }

        public Guid Id { get; set; }
        public string CompanyName { get; private set; }
        public string CompanyDescription { get; private set; }
        public string CompanyPhone { get; private set; }
        public string CompanyEmail { get; private set; }
        public string CompanyAddress { get; private set; }
        public void setCompanyName(string companyName)
        {
            CompanyName = companyName;
        }
        public void setCompanyDescription(string companyDescription)
        {
            CompanyDescription = companyDescription;
        }
        public void setCompanyPhone(string companyPhone)
        {
            CompanyPhone = companyPhone;
        }
        public void setCompanyEmail(string companyEmail)
        {
            CompanyEmail = companyEmail;
        }
        public void setCompanyAddress(string companyAddress)
        {
            CompanyAddress = companyAddress;
        }
        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }
        public void UpdateCompany(string companyName, string companyDescription, string companyPhone, string companyEmail, string companyAddress)
        {
            setCompanyName(companyName);
            setCompanyDescription(companyDescription);
            setCompanyPhone(companyPhone);
            setCompanyEmail(companyEmail);
            setCompanyAddress(companyAddress);
            SetId();

        }

    }
}
