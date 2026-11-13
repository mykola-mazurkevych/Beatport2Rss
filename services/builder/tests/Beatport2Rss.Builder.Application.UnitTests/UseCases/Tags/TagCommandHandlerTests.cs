using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Tags.Commands;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Tags;

public sealed class TagCommandHandlerTests
{
    [Fact]
    public async Task CreateTagCommandHandler_ShouldAddAndSaveTag()
    {
        var tagId = Guid.NewGuid();
        var repository = new Mock<ITagCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.ExistsAsync(It.IsAny<TagId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreateTagCommandHandler(
            repository.Object,
            unitOfWork.Object);

        await handler.HandleAsync(
            new CreateTagCommand(tagId, DateTimeOffset.UtcNow, "Techno"),
            CancellationToken.None);

        repository.Verify(
            value => value.AddAsync(
                It.Is<Tag>(tag => tag.Id == TagId.Create(tagId) && tag.Name == TagName.Create("Techno")),
                It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTagCommandHandler_WhenTagAlreadyExists_ShouldNotAddOrSave()
    {
        var repository = new Mock<ITagCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.ExistsAsync(It.IsAny<TagId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var handler = new CreateTagCommandHandler(
            repository.Object,
            unitOfWork.Object);

        await handler.HandleAsync(
            new CreateTagCommand(Guid.NewGuid(), DateTimeOffset.UtcNow, "Techno"),
            CancellationToken.None);

        repository.Verify(value => value.AddAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTagCommandHandler_ShouldUpdateAndSaveTag()
    {
        var tag = Tag.Create(TagId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, TagName.Create("Techno"));
        var repository = new Mock<ITagCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.FindAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);
        
        var handler = new UpdateTagCommandHandler(
            repository.Object,
            unitOfWork.Object);

        await handler.HandleAsync(
            new UpdateTagCommand(tag.Id.Value, "House"),
            CancellationToken.None);

        Assert.Equal(TagName.Create("House"), tag.Name);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTagCommandHandler_ShouldDeleteAndSaveTag()
    {
        var tag = Tag.Create(TagId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, TagName.Create("Techno"));
        var repository = new Mock<ITagCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.FindAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);
        
        var handler = new DeleteTagCommandHandler(
            repository.Object,
            unitOfWork.Object);

        await handler.HandleAsync(
            new DeleteTagCommand(tag.Id.Value),
            CancellationToken.None);

        repository.Verify(value => value.Delete(tag), Times.Once);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}