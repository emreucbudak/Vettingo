using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Certificate : BaseEntity
{
    public string CertificateName { get; private set; } = string.Empty;
    public int CertificateInstitutionId { get; private set; }
    public CertificateInstitution CertificateInstitution { get; private set; } = null!;
}
