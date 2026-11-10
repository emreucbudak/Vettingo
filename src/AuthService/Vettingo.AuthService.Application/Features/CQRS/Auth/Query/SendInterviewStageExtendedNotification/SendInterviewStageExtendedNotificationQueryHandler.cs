using FlashMediator;
using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendInterviewStageExtendedNotification;

public sealed class SendInterviewStageExtendedNotificationQueryHandler(
    UserManager<User> userManager,
    ICompanyRepository companyRepository,
    IInterviewStageExtendedPublisher publisher) : IRequestHandler<SendInterviewStageExtendedNotificationQueryRequest>
{
    public async Task Handle(SendInterviewStageExtendedNotificationQueryRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByIdAsync(request.CandidateId.ToString("D"))
            ?? throw new NotFoundException("Bildirim gönderilecek aday bulunamadı.");
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new BusinessException("Bildirim gönderilecek adayın e-posta adresi bulunamadı.");
        }

        var company = await companyRepository.GetCompanyByIdAsync(request.CompanyId)
            ?? throw new NotFoundException("Bildirimdeki şirket bulunamadı.");

        await publisher.SendExtendedNotification(new InterviewStageExtendedMessage
        {
            Email = email,
            CompanyName = company.CompanyName,
            Title = request.Title,
            AdditionalDays = request.AdditionalDays,
            EndDate = request.EndDate
        }, cancellationToken);
    }
}
