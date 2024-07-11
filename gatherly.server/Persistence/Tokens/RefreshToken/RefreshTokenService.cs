﻿using gatherly.server.Models.Tokens.RefreshToken;

namespace gatherly.server.Persistence.Tokens.RefreshToken;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly RefreshTokenRepository _refreshTokenRepository = new RefreshTokenRepository(NHibernateHelper.SessionFactory);
    
    public Models.Tokens.RefreshToken.RefreshToken GenerateRefreshToken(Guid userId)
    {
        return _refreshTokenRepository.GenerateRefreshToken(userId);
    }

    public Task<Models.Tokens.RefreshToken.RefreshToken> GetRefreshToken(string token)
    {
        return _refreshTokenRepository.GetRefreshToken(token);
    }

    public Task RevokeRefreshToken(string token)
    {
        return _refreshTokenRepository.RevokeRefreshToken(token);
    }
}