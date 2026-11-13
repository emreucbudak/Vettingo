using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed class JobStageNotificationRecipients(
    UserManager<User> userManager,
    ICompanyRepository companyRepository,
    ILogger<JobStageNotificationRecipients> logger)
{
    public sealed record Recipient(Guid Id, string Email);

    public async Task<(string CompanyName, IReadOnlyList<Recipient> Candidates)> ResolveAsync(
        Guid companyId, IEnumerable<Guid> candidateIds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ids = candidateIds.ToArray();
        if (ids.Length == 0)
        {
            return (string.Empty, []);
        }

        var company = await companyRepository.GetCompanyByIdAsync(companyId)
            ?? throw new NotFoundException("Bildirimdeki şirket bulunamadı.");

        var candidates = await userManager.Users
            .Where(user => ids.Contains(user.Id) && !string.IsNullOrWhiteSpace(user.Email))
            .Select(user => new Recipient(user.Id, user.Email!))
            .ToListAsync(cancellationToken);

        if (candidates.Count < ids.Length)
        {
            logger.LogWarning(
                "Aşama bildirimi için {MissingRecipientCount} adayın e-posta adresi bulunamadı. CompanyId: {CompanyId}",
                ids.Length - candidates.Count, companyId);
        }

        return (company.CompanyName, candidates);
    }
}
