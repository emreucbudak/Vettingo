-- Existing databases use EnsureCreated, which does not update existing columns.
-- Run this once before starting the updated Job API against an existing database.
-- Prefer the old minimum salary, then the maximum, then 0; round to an integer.
BEGIN;

ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "Salary" integer;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = current_schema()
          AND table_name = 'JobPostings' AND column_name = 'MinSalary'
    ) THEN
        UPDATE "JobPostings"
        SET "Salary" = ROUND(COALESCE("MinSalary", "MaxSalary", 0))::integer
        WHERE "Salary" IS NULL;
    END IF;
END $$;

UPDATE "JobPostings" SET "Salary" = 0 WHERE "Salary" IS NULL;
ALTER TABLE "JobPostings" ALTER COLUMN "Salary" SET NOT NULL;
ALTER TABLE "JobPostings" ALTER COLUMN "Salary" SET DEFAULT 0;
ALTER TABLE "JobPostings" DROP COLUMN IF EXISTS "MinSalary";
ALTER TABLE "JobPostings" DROP COLUMN IF EXISTS "MaxSalary";

ALTER TABLE "PersonalizedJobPostings"
    ADD COLUMN IF NOT EXISTS "Salary" integer NOT NULL DEFAULT 0;

COMMIT;
