using Reservas.Infrastructure.Auth;

namespace Reservas.Api.Tests.Auth;

public class Pbkdf2PasswordHasherTests
{
    [Fact]
    public void Hash_ThenVerify_WithSamePassword_ReturnsTrue()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("super-secret-password");

        Assert.True(hasher.Verify("super-secret-password", hash));
    }

    [Fact]
    public void Verify_WithDifferentPassword_ReturnsFalse()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("super-secret-password");

        Assert.False(hasher.Verify("wrong-password", hash));
    }
}
