-- 1) Confirm you?re in the expected database + schema
SELECT current_database() AS db, current_schema() AS schema;

-- 2) Check if EF migrations history table exists
SELECT EXISTS (
  SELECT 1
  FROM information_schema.tables
  WHERE table_schema = 'public'
    AND table_name = '__EFMigrationsHistory'
) AS has_ef_history;

-- 3) If it exists, view applied migrations
SELECT "MigrationId", "ProductVersion"
FROM public."__EFMigrationsHistory"
ORDER BY "MigrationId";

-- 4) List hospital-related tables (adjust schema if not public)
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
  AND table_type = 'BASE TABLE'
  AND table_name IN ('Doctors','Patients','Appointments','Invoices')
ORDER BY table_name;

-- 5) Check whether those tables have rows (helps confirm app is writing)
SELECT 'Doctors' AS table, COUNT(*) FROM public."Doctors"
UNION ALL
SELECT 'Patients', COUNT(*) FROM public."Patients"
UNION ALL
SELECT 'Appointments', COUNT(*) FROM public."Appointments"
UNION ALL
SELECT 'Invoices', COUNT(*) FROM public."Invoices";
SELECT 'Patients', COUNT(*) FROM public."Patients";

-- 6) Show foreign keys for Appointments (confirms relations exist)
SELECT
  tc.table_name,
  kcu.column_name,
  ccu.table_name AS referenced_table,
  ccu.column_name AS referenced_column
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu
  ON tc.constraint_name = kcu.constraint_name
 AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage ccu
  ON ccu.constraint_name = tc.constraint_name
 AND ccu.table_schema = tc.table_schema
WHERE tc.table_schema = 'public'
  AND tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name = 'Appointments';

-- 7) (Optional) Inspect columns for a table
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name = 'Appointments'
ORDER BY ordinal_position;