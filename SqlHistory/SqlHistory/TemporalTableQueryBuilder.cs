using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    public class TemporalTableQueryBuilder
    {
        private const string CreateFormat =
            @"
ALTER TABLE {0}
    ADD ValidFrom datetime2(0)
		GENERATED ALWAYS AS ROW START NOT NULL
        DEFAULT SYSUTCDATETIME(),
    ValidTo datetime2(0)
		GENERATED ALWAYS AS ROW END NOT NULL
        DEFAULT CAST('9999-12-31 23:59:59.9999999' AS datetime2),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
GO

ALTER TABLE {0}
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {0}History))
GO";

        private const string DropFormat =
            @"ALTER TABLE {0} SET ( SYSTEM_VERSIONING = OFF )
GO

ALTER TABLE {0} DROP PERIOD FOR SYSTEM_TIME
GO

DECLARE @sql NVARCHAR(MAX)
WHILE 1=1
BEGIN
    SELECT TOP 1 @sql = N'ALTER TABLE {0} DROP CONSTRAINT ['+dc.NAME+N']'
    from sys.default_constraints dc
    JOIN sys.columns c
        ON c.default_object_id = dc.object_id
    WHERE 
        dc.parent_object_id = OBJECT_ID('{0}')
    AND (c.name = N'ValidFrom' OR c.name = N'ValidTo')
    IF @@ROWCOUNT = 0 BREAK
    EXEC (@sql)
END

ALTER TABLE {0} DROP COLUMN ValidFrom
GO

ALTER TABLE {0} DROP COLUMN ValidTo
GO

DROP TABLE {0}History
GO";

        public static string GetCreateSql(string tableName)
        {
            return string.Format(CreateFormat, tableName);
        }

        public static string GetDropSql(string tableName)
        {
            return string.Format(DropFormat, tableName);
        }
    }
}
