# Personalized job posting publication

Evaluation Application exposes `IPersonalizedJobPostingPublisher.PublishAsync` with
`PersonalizedJobPostingDto`. The message contains only `UserId` and `JobPostingId`
(both nonempty GUIDs). The referenced job posting must already exist in JobService.

```csharp
await publisher.PublishAsync(new PersonalizedJobPostingDto
{
    UserId = userId,
    JobPostingId = jobPostingId
}, cancellationToken);
```

CAP publishes the DTO to `vettingo.job.personalized-job-posting.requested.v1`, using
PostgreSQL message storage and RabbitMQ. Both API projects register CAP on startup
and read `ConnectionStrings:DefaultConnection` and the existing `RabbitMq`
configuration keys. Docker Compose supplies these settings for both APIs and waits
for RabbitMQ to be healthy before starting them.

Job Infrastructure consumes that topic in group `vettingo.job-service`, validates
both IDs, and sends `CreatePersonalizedJobPostingCommandRequest` through
FlashMediator. The handler stores the user-to-job-posting association rather than
copying job details. The entity declares `UserId`, `JobPostingId`, and the
`JobPosting` navigation; identity and timestamps remain inherited from `BaseEntity`.

`GET /api/personalized-job-postings/user` requires a Candidate token and uses its
nonempty user GUID to list only that user's personalized postings, ordered by
association `CreatedAt` descending, then `Id` descending. The response is a flat
array of mobile card data: `jobPostingId`, `companyId`, `companyName`, `title`, `cityName`,
`employmentType`, `workingModel`, `minimumExperienceYear`, `maximumExperienceYear`,
`minSalary`, `maxSalary`, and `requirementSkills`. Employment type and working model
use their numeric enum values. Association IDs, user IDs, timestamps and a nested
job object are not included. The job and city are loaded with the listing query.

Job creation reads `companyId` and `companyName` from the validated JWT and stores
them on the job posting. Both GetAll listings return the stored company name;
mobile maps it to the existing card's company label without an additional request.
Startup adds the `CompanyName` column to existing databases when needed. Existing
postings keep an empty name until their company names are populated separately.

CAP retries failed processing;
delivery is at least once, so a replay after a committed insert can produce another
association.
