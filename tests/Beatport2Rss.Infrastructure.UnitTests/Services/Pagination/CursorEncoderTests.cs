using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Infrastructure.Services.Pagination;
using Beatport2Rss.SharedKernel.Common;

using Bogus;

using Xunit;

namespace Beatport2Rss.Infrastructure.UnitTests.Services.Pagination;

public sealed class CursorEncoderTests
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { Converters = { new IntIdJsonConverter(), new GuidIdJsonConverter(), } };

    private readonly Faker _faker = new();
    private readonly CursorEncoder _cursorEncoder = new(Microsoft.Extensions.Options.Options.Create(JsonSerializerOptions));

    [Fact]
    public void EncodeAndDecode_WhenIdIsInt_ShouldSucceed()
    {
        var createdAt = _faker.Date.RecentOffset();
        var intId = IntId.Create(_faker.Random.Int());
        var cursor = new Cursor<IntId>(createdAt, intId);

        var encodedCursor = _cursorEncoder.Encode(cursor);
        var decodedCursor = _cursorEncoder.Decode<IntId>(encodedCursor);

        Assert.NotNull(decodedCursor);
        Assert.Equal(createdAt, decodedCursor.CreatedAt);
        Assert.Equal(intId, decodedCursor.Id);
    }

    [Fact]
    public void EncodeAndDecode_WhenIdIsGuid_ShouldSucceed()
    {
        var createdAt = _faker.Date.RecentOffset();
        var guidId = GuidId.Create(_faker.Random.Guid());
        var cursor = new Cursor<GuidId>(createdAt, guidId);

        var encodedCursor = _cursorEncoder.Encode(cursor);
        var decodedCursor = _cursorEncoder.Decode<GuidId>(encodedCursor);

        Assert.NotNull(decodedCursor);
        Assert.Equal(createdAt, decodedCursor.CreatedAt);
        Assert.Equal(guidId, decodedCursor.Id);
    }

    private readonly record struct IntId :
        IValueObject
    {
        private IntId(int value) =>
            Value = value;

        public int Value { get; }

        public static IntId Create(int value) =>
            new(value);
    }

    private sealed class IntIdJsonConverter :
        JsonConverter<IntId>
    {
        public override IntId Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            IntId.Create(reader.GetInt32());

        public override void Write(
            Utf8JsonWriter writer,
            IntId value,
            JsonSerializerOptions options) =>
            writer.WriteNumberValue(value.Value);
    }

    private readonly record struct GuidId :
        IValueObject
    {
        private GuidId(Guid value) =>
            Value = value;

        public Guid Value { get; }

        public static GuidId Create(Guid value) =>
            new(value);
    }

    private sealed class GuidIdJsonConverter :
        JsonConverter<GuidId>
    {
        public override GuidId Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            GuidId.Create(reader.GetGuid());

        public override void Write(
            Utf8JsonWriter writer,
            GuidId value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Value);
    }
}