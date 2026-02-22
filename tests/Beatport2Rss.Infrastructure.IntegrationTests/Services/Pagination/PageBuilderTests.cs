// ReSharper disable NotAccessedPositionalProperty.Local

using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Infrastructure.Services.Pagination;
using Beatport2Rss.SharedKernel.Common;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Beatport2Rss.Infrastructure.IntegrationTests.Services.Pagination;

public sealed class PageBuilderTests : IAsyncLifetime
{
    private const int Size = 5;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { Converters = { new PersonIdJsonConverter() } };

    private readonly List<Person> _persons = [];
    private readonly TestDbContext _dbContext;
    private readonly CursorEncoder _cursorEncoder;
    private readonly PageBuilder _pageBuilder;

    public PageBuilderTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new TestDbContext(options);

        _cursorEncoder = new CursorEncoder(Microsoft.Extensions.Options.Options.Create(JsonSerializerOptions));
        _pageBuilder = new PageBuilder(_cursorEncoder);
    }

    public async ValueTask InitializeAsync()
    {
        _persons.AddRange(await ReadPersonsAsync());
        await _dbContext.Persons.AddRangeAsync(_persons);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task BuildAsync_WithoutCursors_Page1Returned()
    {
        var page = await _pageBuilder.BuildAsync<Person, PersonId, PersonPageDto>(
            _dbContext.Persons,
            Size,
            next: null,
            previous: null,
            PersonPageDto.FromPerson,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(Size, page.Size);
        Assert.Contains(page.Items, p => p.Id.Value == 1);
        Assert.Contains(page.Items, p => p.Id.Value == 2);
        Assert.Contains(page.Items, p => p.Id.Value == 3);
        Assert.Contains(page.Items, p => p.Id.Value == 4);
        Assert.Contains(page.Items, p => p.Id.Value == 5);
        Assert.Equal(Size, page.Count);
        Assert.Equal(_dbContext.Persons.Count(), page.TotalCount);
        Assert.NotNull(page.Next);
        var pageNext = _cursorEncoder.Decode<PersonId>(page.Next);
        Assert.NotNull(pageNext);
        Assert.Equal(5, pageNext.Id.Value);
        Assert.Null(page.Previous);
    }

    [Fact]
    public async Task BuildAsync_WithNextCursor_Page2Returned()
    {
        var page1LastPerson = _persons[Size - 1];
        var next = _cursorEncoder.Encode(new Cursor<PersonId>(page1LastPerson.CreatedAt, page1LastPerson.Id));
        var page = await _pageBuilder.BuildAsync<Person, PersonId, PersonPageDto>(
            _dbContext.Persons,
            Size,
            next,
            previous: null,
            PersonPageDto.FromPerson,
            TestContext.Current.CancellationToken);

        Assert.Equal(Size, page.Size);
        Assert.Contains(page.Items, p => p.Id.Value == 6);
        Assert.Contains(page.Items, p => p.Id.Value == 7);
        Assert.Contains(page.Items, p => p.Id.Value == 8);
        Assert.Contains(page.Items, p => p.Id.Value == 9);
        Assert.Contains(page.Items, p => p.Id.Value == 10);
        Assert.Equal(Size, page.Count);
        Assert.Equal(_dbContext.Persons.Count(), page.TotalCount);
        Assert.NotNull(page.Next);
        var pageNext = _cursorEncoder.Decode<PersonId>(page.Next);
        Assert.NotNull(pageNext);
        Assert.Equal(10, pageNext.Id.Value);
        Assert.NotNull(page.Previous);
        var pagePrevious = _cursorEncoder.Decode<PersonId>(page.Previous);
        Assert.NotNull(pagePrevious);
        Assert.Equal(6, pagePrevious.Id.Value);
    }

    [Fact]
    public async Task BuildAsync_WithPreviousCursor_Page1Returned()
    {
        var page2FirstPerson = _persons[Size];
        var previous = _cursorEncoder.Encode(new Cursor<PersonId>(page2FirstPerson.CreatedAt, page2FirstPerson.Id));
        var page = await _pageBuilder.BuildAsync<Person, PersonId, PersonPageDto>(
            _dbContext.Persons,
            Size,
            next: null,
            previous,
            PersonPageDto.FromPerson,
            TestContext.Current.CancellationToken);

        Assert.Equal(Size, page.Size);
        Assert.Contains(page.Items, p => p.Id.Value == 1);
        Assert.Contains(page.Items, p => p.Id.Value == 2);
        Assert.Contains(page.Items, p => p.Id.Value == 3);
        Assert.Contains(page.Items, p => p.Id.Value == 4);
        Assert.Contains(page.Items, p => p.Id.Value == 5);
        Assert.Equal(Size, page.Count);
        Assert.Equal(_dbContext.Persons.Count(), page.TotalCount);
        Assert.NotNull(page.Next);
        var pageNext = _cursorEncoder.Decode<PersonId>(page.Next);
        Assert.NotNull(pageNext);
        Assert.Equal(5, pageNext.Id.Value);
        Assert.Null(page.Previous);
    }

    [Fact(Skip = "Sorting does not work for now.")]
    public async Task BuildAsync_WhenSortedByNameAscending_AndWithoutCursors_Page1Returned()
    {
        var page = await _pageBuilder.BuildAsync<Person, PersonId, PersonPageDto>(
            _dbContext.Persons,
            Size,
            next: null,
            previous: null,
            PersonPageDto.FromPerson,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(Size, page.Size);
        Assert.Contains(page.Items, p => p.Id.Value == 14); // Amelia
        Assert.Contains(page.Items, p => p.Id.Value == 1); // Andrew
        Assert.Contains(page.Items, p => p.Id.Value == 8); // Ava
        Assert.Contains(page.Items, p => p.Id.Value == 17); // Benjamin
        Assert.Contains(page.Items, p => p.Id.Value == 16); // Charlotte
        Assert.Equal(Size, page.Count);
        Assert.Equal(_dbContext.Persons.Count(), page.TotalCount);
        Assert.NotNull(page.Next);
        var pageNext = _cursorEncoder.Decode<PersonId>(page.Next);
        Assert.NotNull(pageNext);
        Assert.Equal(16, pageNext.Id.Value);
        Assert.Null(page.Previous);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    private static async Task<IEnumerable<Person>> ReadPersonsAsync()
    {
        const int indexId = 0;
        const int indexName = 1;
        const int indexAge = 2;
        const int indexCreatedAt = 3;

        var lines = await File.ReadAllLinesAsync(@"Services\Pagination\PageBuilderTestsData.csv");

        var persons = lines
            .Select(line =>
            {
                var properties = line.Split(',');
                var person = new Person(
                    PersonId.Create(int.Parse(properties[indexId], provider: CultureInfo.InvariantCulture)),
                    properties[indexName],
                    int.Parse(properties[indexAge], provider: CultureInfo.InvariantCulture),
                    new DateTimeOffset(DateTime.ParseExact(properties[indexCreatedAt], "yyyy-MM-dd", CultureInfo.InvariantCulture)));

                return person;
            });

        return persons;
    }

    private readonly record struct PersonId :
        IId<PersonId>
    {
        private PersonId(int value) =>
            Value = value;

        public int Value { get; }

        public static bool operator <(PersonId left, PersonId right) => left.Value < right.Value;
        public static bool operator >(PersonId left, PersonId right) => left.Value > right.Value;
        public static bool operator <=(PersonId left, PersonId right) => left.Value <= right.Value;
        public static bool operator >=(PersonId left, PersonId right) => left.Value >= right.Value;

        public static PersonId Create(int value) =>
            new(value);

        public int CompareTo(PersonId other) =>
            Value.CompareTo(other.Value);
    }

    private sealed class PersonIdJsonConverter :
        JsonConverter<PersonId>
    {
        public override PersonId Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            PersonId.Create(reader.GetInt32());

        public override void Write(
            Utf8JsonWriter writer,
            PersonId value,
            JsonSerializerOptions options) =>
            writer.WriteNumberValue(value.Value);
    }

    private sealed record Person(
        PersonId Id,
        string Name,
        int Age,
        DateTimeOffset CreatedAt) :
        IEntity<PersonId>;

    private sealed record PersonPageDto(
        PersonId Id,
        string Name,
        int Age,
        DateTimeOffset CreatedAt) :
        IPageDto<PersonId>
    {
        public static Expression<Func<Person, PersonPageDto>> FromPerson =>
            person => new(person.Id,
                person.Name,
                person.Age,
                person.CreatedAt);
    }

    private sealed class TestDbContext(
        DbContextOptions<TestDbContext> options) :
        DbContext(options)
    {
        public DbSet<Person> Persons =>
            Set<Person>();
    }
}