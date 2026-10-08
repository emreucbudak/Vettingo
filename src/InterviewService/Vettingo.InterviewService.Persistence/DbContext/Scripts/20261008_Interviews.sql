-- Run against InterviewServiceDb before using the new interviews endpoint.
BEGIN;

CREATE TABLE IF NOT EXISTS "Interviews" (
    "Id" uuid NOT NULL PRIMARY KEY,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "StartedTime" time without time zone NOT NULL,
    "InterviewDate" date NOT NULL,
    "UserId" uuid NOT NULL,
    "CompanyId" uuid NOT NULL,
    "Name" text NOT NULL,
    "Surname" text NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Interviews_CompanyId_InterviewDate_StartedTime"
    ON "Interviews" ("CompanyId", "InterviewDate", "StartedTime");

COMMIT;
