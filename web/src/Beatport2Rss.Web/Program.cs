using Beatport2Rss.Web.Components;
using Beatport2Rss.Web.EmailConfirmations;

var builder = WebApplication.CreateBuilder(args);

// Hosting and circuit diagnostics can include the original confirmation URL.
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.None);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = false);

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

    await next(context);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
