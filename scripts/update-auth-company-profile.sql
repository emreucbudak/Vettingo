-- Run against the AuthService PostgreSQL database before deploying the new model.
-- Existing company contact columns are retained to preserve their data.
BEGIN;

ALTER TABLE "Companys"
    ADD COLUMN IF NOT EXISTS "CompanySector" text NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS "CompanyWebsite" text NOT NULL DEFAULT '',
    ADD COLUMN IF NOT EXISTS "CompanySize" text NOT NULL DEFAULT '';

ALTER TABLE "AspNetUsers"
    ADD COLUMN IF NOT EXISTS "CompanyId" uuid NULL;

-- Preserve existing employer/company associations before email leaves the model.
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = current_schema()
          AND table_name = 'Companys' AND column_name = 'CompanyEmail'
    ) THEN
        IF EXISTS (
            SELECT lower(trim(c."CompanyEmail"))
            FROM "Companys" c
            JOIN "AspNetUsers" u ON lower(trim(u."Email")) = lower(trim(c."CompanyEmail"))
            WHERE u."CompanyId" IS NULL
            GROUP BY lower(trim(c."CompanyEmail"))
            HAVING count(*) > 1
        ) THEN
            RAISE EXCEPTION 'Duplicate company email associations must be resolved before migration.';
        END IF;

        UPDATE "AspNetUsers" u
        SET "CompanyId" = c."Id"
        FROM "Companys" c
        WHERE u."CompanyId" IS NULL
          AND lower(trim(u."Email")) = lower(trim(c."CompanyEmail"));
    END IF;
END $$;

COMMIT;
