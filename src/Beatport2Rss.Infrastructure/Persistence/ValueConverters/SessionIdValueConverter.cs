using Beatport2Rss.Domain.Sessions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SessionIdValueConverter() :
    ValueConverter<SessionId, Guid>(
        sessionId => sessionId.Value,
        value => SessionId.Create(value));