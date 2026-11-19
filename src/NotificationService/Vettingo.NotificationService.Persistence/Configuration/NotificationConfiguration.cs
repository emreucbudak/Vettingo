using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.NotificationService.Domain.Entities;

namespace Vettingo.NotificationService.Persistence.Configuration;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(notification => notification.Id);
    }
}
