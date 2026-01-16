-- 1) List indexes on hospital-related tables
SELECT
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'public'
  AND tablename IN ('Doctors','Patients','Appointments','Invoices')
ORDER BY tablename, indexname;

-- 2) Index sizes for hospital-related tables looking for large or redundant indexes
SELECT
    t.relname  AS table_name,
    i.relname  AS index_name,
    pg_size_pretty(pg_relation_size(i.oid)) AS index_size
FROM pg_class t
         JOIN pg_index ix ON t.oid = ix.indrelid
         JOIN pg_class i  ON i.oid = ix.indexrelid
         JOIN pg_namespace n ON n.oid = t.relnamespace
WHERE n.nspname = 'public'
  AND t.relname IN ('Doctors','Patients','Appointments','Invoices')
ORDER BY pg_relation_size(i.oid) DESC;
-- 3) Indexes being used?
SELECT
    relname AS table_name,
    indexrelname AS index_name,
    idx_scan, idx_tup_read, idx_tup_fetch
FROM pg_stat_user_indexes
WHERE schemaname = 'public'
  AND relname IN ('Doctors','Patients','Appointments','Invoices')
ORDER BY relname, idx_scan DESC, indexrelname;

-- 5) Table scan vs index scan tendency
SELECT
    relname AS table_name,
    seq_scan,
    idx_scan,
    n_live_tup
FROM pg_stat_user_tables
WHERE schemaname = 'public'
  AND relname IN ('Doctors','Patients','Appointments','Invoices')
ORDER BY relname;

-- 6) EXPLAIN ANALYZE sample query to see index usage
--     IMPORTANT TO SEE THE DIFFERENCE WITH AND WITHOUT INDEXES
--     Make sure to turn off the indexscans to see the difference
SET enable_indexscan = on; -- on to turn it back on
SET enable_bitmapscan = on;
SET enable_seqscan    = on;
-- 6a) Sample query on Patients table
EXPLAIN (ANALYZE ,BUFFERS )
SELECT *
FROM public."Patients"
WHERE "FullName_LastName" = 'Smith'; 
-- With indexes Planning Time: 0.080 ms Execution Time: 0.781 ms
-- Without indexes Planning Time: 0.125 ms Execution Time: 1.053 ms
EXPLAIN (ANALYZE , BUFFERS )
SELECT *
FROM public."Patients"
WHERE "DateOfBirth" = '1986-01-16';
-- With Indexes Planning Time: 0.067 ms Execution Time: 0.156 ms
-- Without Indexes Planning Time: 0.066 ms Execution Time: 0.679 ms

-- 6b) Sample query on Doctor table
EXPLAIN (ANALYZE ,BUFFERS )
SELECT *
FROM public."Doctors"
WHERE "Specialisation" = 2; 
-- 2 == Cardiology
-- With Indexes Planning Time: 0.073 ms Execution Time: 0.034 ms
-- Without Indexes Planning Time: 0.072 ms Execution Time: 0.114 ms
