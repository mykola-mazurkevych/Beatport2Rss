using System.Net;

using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class TransientHttpError(
    string message,
    HttpStatusCode httpStatusCode) :
    Error(message)
{
    public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;
}