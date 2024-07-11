namespace gatherly.server.Models.Tokens.RefreshToken;

public interface IRefreshTokenRepository
{
    public Models.Tokens.RefreshToken.RefreshToken GenerateRefreshToken(Guid userId); //tworzenie refresh tokena
    public Task<RefreshToken> GetRefreshToken(string token); //sprawdzanie szczegółów tokena
    public Task RevokeRefreshToken(string token); //dewalidacja tokenu
}