namespace gatherly.server.Models.Tokens.RefreshToken;

public interface IRefreshTokenRepository
{
    public RefreshToken GenerateRefreshToken(Guid userId); //tworzenie refresh tokena
    public RefreshToken? GetRefreshToken(string token); //sprawdzanie szczegółów tokena
    public Task RevokeRefreshToken(string token); //dewalidacja tokenu
}