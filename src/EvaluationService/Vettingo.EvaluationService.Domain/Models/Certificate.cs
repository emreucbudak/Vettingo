using Vettingo.EvaluationService.Domain.Common;
using Vettingo.EvaluationService.Domain.Enums;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Certificate : BaseEntity
{
    public string CertificateName { get; private set; } = string.Empty;
    public CertificateInstitution CertificateInstitution { get; private set; }
}
