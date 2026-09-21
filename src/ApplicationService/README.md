# Job application counter

The create handler saves the application first, then calls IJobApplicationCreatedPublisher.PublishAsync in a try/catch.
The Infrastructure implementation publishes only the JobPostingId (GUID) through CAP on vettingo.job-application.created.v1.
A publishing failure is logged and does not undo or fail the completed application.

JobService's consumer loads the posting using IJobPostingRepository, calls IncrementApplicationCount on the domain entity and saves it.
Both services use the existing PostgreSQL and RabbitMQ CAP configuration.
JobService startup ensures the ApplicationCount column exists for existing databases.

This deliberately simple flow has no custom transaction or duplicate-message table.
A failed publish can leave the count behind; repeated deliveries can increment again and concurrent updates may lose increments.
Existing applications are not backfilled.
