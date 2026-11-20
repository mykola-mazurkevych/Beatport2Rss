using Beatport2Rss.Web.Components;
using Beatport2Rss.Web.EmailConfirmations;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Suppress only routine request diagnostics, which include confirmation URL paths.
builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);

builder.Services.AddHealthChecks();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = false);
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en")
        .AddSupportedCultures("en", "uk")
        .AddSupportedUICultures("en", "uk");
    options.RequestCultureProviders.Remove(
        options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().Single());
});

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5162";
if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiAddress)
    || apiAddress.Scheme is not ("http" or "https")
    || apiAddress.UserInfo.Length != 0
    || apiAddress.Query.Length != 0
    || apiAddress.Fragment.Length != 0)
{
    throw new InvalidOperationException("ApiBaseUrl must be an absolute HTTP or HTTPS base URL without credentials, a query, or a fragment.");
}

builder.Services.AddHttpClient<EmailConfirmationClient>(client =>
    {
        client.BaseAddress = new Uri($"{apiAddress.AbsoluteUri.TrimEnd('/')}/");
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        AllowAutoRedirect = false,
        UseCookies = false
    })
    .RemoveAllLoggers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var sensitive = context.Request.Path.StartsWithSegments("/confirm-email", StringComparison.OrdinalIgnoreCase)
        || context.Request.Path.StartsWithSegments("/_blazor", StringComparison.OrdinalIgnoreCase);

    context.Response.OnStarting(() =>
    {
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        if (sensitive)
        {
            context.Response.Headers.CacheControl = "no-store";
        }

        return Task.CompletedTask;
    });

    try
    {
        await next(context);
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        context.Abort();
    }
#pragma warning disable CA1031 // Avoid logging confirmation URL paths or exception data.
    catch (Exception exception) when (sensitive)
#pragma warning restore CA1031
    {
        app.Logger.LogError(
            "Confirmation web request failed ({ExceptionType}); trace {TraceId}.",
            exception.GetType().Name,
            context.TraceIdentifier);
        if (context.Response.HasStarted)
        {
            context.Abort();
            return;
        }

        await Results.Problem(statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(context);
    }
});
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRequestLocalization();
app.Use(async (context, next) =>
{
    var culture = context.Features.Get<IRequestCultureFeature>();
    if (culture?.Provider is QueryStringRequestCultureProvider)
    {
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(culture.RequestCulture),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(365)
            });
    }

    await next(context);
});

app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
