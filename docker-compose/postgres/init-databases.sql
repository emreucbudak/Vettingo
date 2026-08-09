\set ON_ERROR_STOP on

SELECT format('CREATE DATABASE %I', database_name)
FROM (
    VALUES
        ('AnalyticsServiceDb'),
        ('ApplicationServiceDb'),
        ('AuthServiceDb'),
        ('EvaluationServiceDb'),
        ('ExamServiceDb'),
        ('InterviewServiceDb'),
        ('JobServiceDb'),
        ('NotificationServiceDb')
) AS databases(database_name)
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = database_name)
\gexec
