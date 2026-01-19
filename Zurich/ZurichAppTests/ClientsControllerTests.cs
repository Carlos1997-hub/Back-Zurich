using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZurichApp.Api.Controllers;
using ZurichApp.Api.Dtos.Clients;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Tests.Controllers;

public class ClientsControllerTests
{
    private readonly Mock<IClientService> _service = new();

    private ClientsController BuildSut()
        => new ClientsController(_service.Object);

    [Fact]
    public async Task GetAll_ShouldReturnOkWithList()
    {
        var sut = BuildSut();

        var list = new List<ClientResponse>
        {
            new() { ClientId = 1, FullName = "A", Email = "a@a.com", Phone = "111", IdentificationNumber = "1234567890" }
        };

        _service.Setup(x => x.GetAllAsync()).ReturnsAsync(list);

        var result = await sut.GetAll();

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<IEnumerable<ClientResponse>>();
        ((IEnumerable<ClientResponse>)ok.Value!).Count().Should().Be(1);
    }

    [Fact]
    public async Task GetById_WhenFound_ShouldReturnOk()
    {
        var sut = BuildSut();

        var client = new ClientResponse { ClientId = 7, FullName = "Carlos" };

        _service.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(client);

        var result = await sut.GetById(7);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ((ClientResponse)ok.Value!).ClientId.Should().Be(7);
    }

    [Fact]
    public async Task GetById_WhenNull_ShouldReturnNotFound()
    {
        var sut = BuildSut();

        _service.Setup(x => x.GetByIdAsync(7)).ReturnsAsync((ClientResponse?)null);

        var result = await sut.GetById(7);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateWithUser_ShouldReturnCreatedAtAction()
    {
        var sut = BuildSut();

        var req = new ClientCreateWithUserRequest();
        var created = new ClientResponse { ClientId = 99, FullName = "Nuevo" };

        _service.Setup(x => x.CreateClientWithUserAsync(req)).ReturnsAsync(created);

        var result = await sut.CreateWithUser(req);

        var createdAt = result.Result as CreatedAtActionResult;
        createdAt.Should().NotBeNull();
        createdAt!.StatusCode.Should().Be(201);
        createdAt.ActionName.Should().Be(nameof(ClientsController.GetById));
        createdAt.RouteValues.Should().NotBeNull();
        createdAt.RouteValues!["clientId"].Should().Be(99);
        ((ClientResponse)createdAt.Value!).ClientId.Should().Be(99);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        var sut = BuildSut();

        var req = new ClientUpdateRequest();

        _service.Setup(x => x.UpdateAsync(5, req)).Returns(Task.CompletedTask);

        var result = await sut.Update(5, req);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.UpdateAsync(5, req), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        var sut = BuildSut();

        _service.Setup(x => x.DeleteAsync(5)).Returns(Task.CompletedTask);

        var result = await sut.Delete(5);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.DeleteAsync(5), Times.Once);
    }
}
