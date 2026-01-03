namespace Beatport2Rss.Application.Results;

public readonly record struct Success;

public readonly record struct Success<TValue>(in TValue Value);