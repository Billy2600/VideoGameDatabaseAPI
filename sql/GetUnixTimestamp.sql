-- Used to get Unix timestamp
-- So we can ensure files will be sequential, by filename
SELECT DATEDIFF(SECOND,'1970-01-01', GETUTCDATE())