-- Run against SubscriptionServiceDb when upgrading an existing database.
-- Fresh development/test databases are created from SubscriptionDbContext.

BEGIN;

DO $$
BEGIN
    IF to_regclass('"CompanySubscriptions"') IS NULL
       AND to_regclass('"Subscriptions"') IS NOT NULL THEN
        ALTER TABLE "Subscriptions" RENAME TO "CompanySubscriptions";
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS "CandidateSubscriptions"
(
    "Id" uuid NOT NULL,
    "CandidateId" uuid NOT NULL,
    "PlanId" integer NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NULL,
    CONSTRAINT "PK_CandidateSubscriptions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CandidateSubscriptions_Plans_PlanId"
        FOREIGN KEY ("PlanId") REFERENCES "Plans" ("Id")
        ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "IX_CandidateSubscriptions_CandidateId"
    ON "CandidateSubscriptions" ("CandidateId");

CREATE INDEX IF NOT EXISTS "IX_CandidateSubscriptions_PlanId"
    ON "CandidateSubscriptions" ("PlanId");

COMMIT;
