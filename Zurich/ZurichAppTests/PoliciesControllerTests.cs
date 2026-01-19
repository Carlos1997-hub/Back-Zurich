using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZurichApp.Api.Controllers;
using ZurichApp.Api.Dtos.Policies;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Tests.Controllers;

public class PoliciesControllerTests
{
    private readonly Mock<IPolicyService> _service = new();

    private PoliciesController BuildSut()
        => new PoliciesController(_service.Object);

    [Fact]
    public async Task GetAllOrByClient_WhenClientIdProvided_ShouldReturnOk_AndCallGetByClientId()
    {
        var sut = BuildSut();

        _service.Setup(x => x.GetByClientIdAsync(3)).ReturnsAsync(new List<PolicyResponse>());

        var result = await sut.GetAllOrByClient(3);

        _service.Verify(x => x.GetByClientIdAsync(3), Times.Once);
        _service.Verify(x => x.GetAllAsync(), Times.Never);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAllOrByClient_WhenClientIdNull_ShouldReturnOk_AndCallGetAll()
    {
        var sut = BuildSut();

        _service.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<PolicyResponse>());

        var result = await sut.GetAllOrByClient(null);

        _service.Verify(x => x.GetAllAsync(), Times.Once);
        _service.Verify(x => x.GetByClientIdAsync(It.IsAny<int>()), Times.Never);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetMine_ShouldReturnOk_AndCallGetMine()
    {
        var sut = BuildSut();

        _service.Setup(x => x.GetMineAsync()).ReturnsAsync(new List<PolicyResponse>());

        var result = await sut.GetMine();

        _service.Verify(x => x.GetMineAsync(), Times.Once);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOkWithCreated()
    {
        var sut = BuildSut();

        var req = new PolicyCreateRequest();
        var created = new PolicyResponse { PolicyId = 10 };

        _service.Setup(x => x.CreateAsync(req)).ReturnsAsync(created);

        var result = await sut.Create(req);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ((PolicyResponse)ok.Value!).PolicyId.Should().Be(10);
    }

    [Fact]
    public async Task Cancel_ShouldReturnNoContent()
    {
        var sut = BuildSut();

        _service.Setup(x => x.CancelAsync(22)).Returns(Task.CompletedTask);

        var result = await sut.Cancel(22);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.CancelAsync(22), Times.Once);
    }
}
