-- Run from the folder containing job-applications.csv, against the JobService database.
-- The old database is read-only during this migration and is never deleted.
\set ON_ERROR_STOP on
BEGIN;
CREATE TEMP TABLE imported_applications (LIKE "JobApplications" INCLUDING DEFAULTS);
\copy imported_applications ("Id", "CandidateId", "JobPostingId", "AppliedAt", "Status", "CreatedAt", "UpdatedAt") FROM 'job-applications.csv' WITH (FORMAT csv, HEADER true)
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM imported_applications WHERE "Status" NOT IN ('Submitted','UnderReview','Interview','Offer','Rejected')) THEN
        RAISE EXCEPTION 'Unsupported application status. Resolve legacy statuses explicitly before importing.';
    END IF;
    IF EXISTS (SELECT 1 FROM imported_applications a LEFT JOIN "JobPostings" j ON j."Id" = a."JobPostingId" WHERE j."Id" IS NULL) THEN
        RAISE EXCEPTION 'Some applications reference missing job postings. No records were imported.';
    END IF;
    IF EXISTS (SELECT 1 FROM imported_applications a JOIN "JobApplications" b ON b."Id" = a."Id"
        WHERE ROW(a."CandidateId", a."JobPostingId", a."AppliedAt", a."Status", a."CreatedAt", a."UpdatedAt")
        IS DISTINCT FROM ROW(b."CandidateId", b."JobPostingId", b."AppliedAt", b."Status", b."CreatedAt", b."UpdatedAt")) THEN
        RAISE EXCEPTION 'Existing application ID has different data. No records were imported.';
    END IF;
END $$;
INSERT INTO "JobApplications" ("Id", "CandidateId", "JobPostingId", "AppliedAt", "Status", "CreatedAt", "UpdatedAt")
SELECT "Id", "CandidateId", "JobPostingId", "AppliedAt", "Status", "CreatedAt", "UpdatedAt" FROM imported_applications
ON CONFLICT ("Id") DO NOTHING;
SELECT COUNT(*) AS imported_source_rows FROM imported_applications;
COMMIT;
