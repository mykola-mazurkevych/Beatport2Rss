using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Beatport2Rss.Collector.Application.Extensions;

internal static class DbUpdateExceptionExtensions
{
    extension(DbUpdateException dbUpdateException)
    {
        public bool IsUniqueViolation =>
            dbUpdateException.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }
}