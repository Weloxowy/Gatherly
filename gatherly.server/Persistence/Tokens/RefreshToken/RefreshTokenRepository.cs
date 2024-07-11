using gatherly.server.Models.Tokens.RefreshToken;
using NHibernate;
using System;
using System.Threading.Tasks;

namespace gatherly.server.Persistence.Tokens.RefreshToken
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public RefreshTokenRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        public Models.Tokens.RefreshToken.RefreshToken GenerateRefreshToken(Guid userId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var refreshToken = new Models.Tokens.RefreshToken.RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Expiration = DateTime.Now.AddMinutes(60),
                    IsRevoked = false
                };

                session.Save(refreshToken);
                transaction.Commit();

                return refreshToken;
            }
        }

        public async Task<Models.Tokens.RefreshToken.RefreshToken> GetRefreshToken(string token)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                return await session.QueryOver<Models.Tokens.RefreshToken.RefreshToken>()
                    .Where(rt => rt.Token == token && !rt.IsRevoked)
                    .SingleOrDefaultAsync();
            }
        }
        
        public async Task RevokeRefreshToken(string token)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var refreshToken = await session.QueryOver<Models.Tokens.RefreshToken.RefreshToken>()
                    .Where(rt => rt.Token == token)
                    .SingleOrDefaultAsync();

                if (refreshToken != null)
                {
                    refreshToken.IsRevoked = true;
                    await session.UpdateAsync(refreshToken);
                    await transaction.CommitAsync();
                }
            }
        }
    }
}
