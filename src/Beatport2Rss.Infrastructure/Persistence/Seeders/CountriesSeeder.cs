using System.Reflection;

using Beatport2Rss.Domain.Countries;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Seeders;

internal static class CountriesSeeder
{
    private const string ResourceName = "Beatport2Rss.Infrastructure.Persistence.Seeders.Data.countries_csv.md.md";

    private const int CountryLinesToSkip = 3;

    private const int CountryArrayLength = 4;

    private const int IndexCountryCode = 0;
    ////private const int IndexLatitude = 1;
    ////private const int IndexLongitude = 2;
    private const int IndexCountryName = 3;

    public static void Seed(DbSet<Country> dbSet)
    {
        var existingCountryCodes = dbSet
            .Select(country => country.Id)
            .ToHashSet();
        var resourceContent = GetResourceContent();

        var countriesToSeed = GetCountriesToSeed(
            existingCountryCodes,
            resourceContent);

        dbSet.AddRange(countriesToSeed);
    }

    public static async Task SeedAsync(
        DbSet<Country> dbSet,
        CancellationToken cancellationToken = default)
    {
        var getExistingCountryCodesTask = dbSet
            .Select(country => country.Id)
            .ToHashSetAsync(cancellationToken);
        var getCountryLinesTask = GetResourceContentAsync(cancellationToken);

        var countriesToSeed = GetCountriesToSeed(
            await getExistingCountryCodesTask,
            await getCountryLinesTask);

        await dbSet.AddRangeAsync(countriesToSeed, cancellationToken);
    }

    private static string GetResourceContent()
    {
        using var stream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName) ??
            throw new InvalidOperationException($"Cannot find resource: {ResourceName}");

        using var streamReader = new StreamReader(stream);
        var resourceContent = streamReader.ReadToEnd();

        return resourceContent;
    }

    private static async Task<string> GetResourceContentAsync(CancellationToken cancellationToken)
    {
        await using var stream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName) ??
            throw new InvalidOperationException($"Cannot find resource: {ResourceName}");

        using var streamReader = new StreamReader(stream);
        var resourceContent = await streamReader.ReadToEndAsync(cancellationToken);

        return resourceContent;
    }

    private static List<Country> GetCountriesToSeed(
        HashSet<CountryCode> existingCountryCodes,
        string resourceContent)
    {
        List<Country> countriesToSeed = [];

        var countryLines = resourceContent
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Skip(CountryLinesToSkip);

        foreach (var countryLine in countryLines)
        {
            var countryArray = countryLine
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var countryCode = CountryCode.Create(countryArray[IndexCountryCode]);

            if (existingCountryCodes.Contains(countryCode) || countryArray.Length < CountryArrayLength)
            {
                continue;
            }

            var country = Country.Create(
                countryCode,
                DateTimeOffset.UtcNow,
                CountryName.Create(countryArray[IndexCountryName])
            );
            countriesToSeed.Add(country);
        }

        return countriesToSeed;
    }
}