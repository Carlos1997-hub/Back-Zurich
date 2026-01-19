using FluentAssertions;
using Moq;
using Xunit;
using ZurichApp.Api.Dtos.Auth;
using ZurichApp.Api.Models.Auth;
using ZurichApp.Api.Repositories.Interfaces;
using ZurichApp.Api.Services;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _repo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenService> _jwt = new();

    private AuthService BuildSut()
        => new AuthService(_repo.Object, _hasher.Object, _jwt.Object);

    [Fact]
    public async Task LoginAsync_WhenUserIsNull_ShouldThrowUnauthorized()
    {
        var sut = BuildSut();
        var req = new LoginRequest { UsernameOrEmail = "nope", Password = "123" };

        _repo.Setup(x => x.GetByUsernameOrEmailAsync(req.UsernameOrEmail))
             .ReturnsAsync(((User?)null, (string?)null));

        Func<Task> act = async () => _ = await sut.LoginAsync(req);

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_WhenRoleNameMissing_ShouldThrowUnauthorized()
    {
        var sut = BuildSut();
        var req = new LoginRequest { UsernameOrEmail = "client", Password = "123" };

        var user = new User
        {
            UserId = 10,
            Username = "client",
            Email = "c@c.com",
            DisplayName = "Cliente",
            ClientId = 1,
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 9, 9, 9 },
            IsActive = true
        };

        _repo.Setup(x => x.GetByUsernameOrEmailAsync(req.UsernameOrEmail))
             .ReturnsAsync((user, ""));

        Func<Task> act = async () => _ = await sut.LoginAsync(req);

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_WhenUserInactive_ShouldThrowUnauthorized()
    {
        var sut = BuildSut();
        var req = new LoginRequest { UsernameOrEmail = "client", Password = "123" };

        var user = new User
        {
            UserId = 10,
            Username = "client",
            Email = "c@c.com",
            DisplayName = "Cliente",
            ClientId = 1,
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 9, 9, 9 },
            IsActive = false
        };

        _repo.Setup(x => x.GetByUsernameOrEmailAsync(req.UsernameOrEmail))
             .ReturnsAsync((user, "Cliente"));

        Func<Task> act = async () => _ = await sut.LoginAsync(req);

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ShouldThrowUnauthorized_AndNotCreateToken()
    {
        var sut = BuildSut();
        var req = new LoginRequest { UsernameOrEmail = "client", Password = "bad" };

        var user = new User
        {
            UserId = 10,
            Username = "client",
            Email = "c@c.com",
            DisplayName = "Cliente",
            ClientId = 1,
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 9, 9, 9 },
            IsActive = true
        };

        _repo.Setup(x => x.GetByUsernameOrEmailAsync(req.UsernameOrEmail))
             .ReturnsAsync((user, "Cliente"));

        _hasher.Setup(x => x.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
               .Returns(false);

        Func<Task> act = async () => _ = await sut.LoginAsync(req);

        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");

        _jwt.Verify(x => x.CreateToken(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ShouldReturnLoginResponse()
    {
        var sut = BuildSut();
        var req = new LoginRequest { UsernameOrEmail = "client", Password = "ok" };

        var user = new User
        {
            UserId = 10,
            Username = "client",
            Email = "c@c.com",
            DisplayName = "Cliente Uno",
            ClientId = 1,
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 9, 9, 9 },
            IsActive = true
        };

        _repo.Setup(x => x.GetByUsernameOrEmailAsync(req.UsernameOrEmail))
             .ReturnsAsync((user, "Cliente"));

        _hasher.Setup(x => x.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
               .Returns(true);

        _jwt.Setup(x => x.CreateToken(user, "Cliente"))
            .Returns(("token123", 3600));

        var res = await sut.LoginAsync(req);

        res.AccessToken.Should().Be("token123");
        res.ExpiresIn.Should().Be(3600);
        res.UserId.Should().Be(10);
        res.Role.Should().Be("Cliente");
        res.ClientId.Should().Be(1);
        res.DisplayName.Should().Be("Cliente Uno");
        res.Email.Should().Be("c@c.com");
        res.Username.Should().Be("client");

        _repo.Verify(x => x.GetByUsernameOrEmailAsync("client"), Times.Once);
        _hasher.Verify(x => x.Verify("ok", user.PasswordHash, user.PasswordSalt), Times.Once);
        _jwt.Verify(x => x.CreateToken(user, "Cliente"), Times.Once);
    }
}
