
using gatherly.server.Models.Authentication.UserEntity;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace gatherly.server.Persistence.Authentication.UserEntity;

/// <summary>
///     Repository for managing user-related operations.
/// </summary>
public class UserEntityRepository : IUserEntityRepository
{
    
    private readonly ISession _session;
    private readonly IUnitOfWork _unitOfWork;

    public UserEntityRepository(ISession session, IUnitOfWork unitOfWork)
    {
        _session = session;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Models.Authentication.UserEntity.UserEntity?> GetUserByUserId(Guid id)
    {
        return await _session.GetAsync<Models.Authentication.UserEntity.UserEntity>(id);
    }

    public async Task<Models.Authentication.UserEntity.UserEntity?> GetUserByEmail(string email)
    {
        return await _session.Query<Models.Authentication.UserEntity.UserEntity>()
            .SingleOrDefaultAsync(x => x.Email.Equals(email));
    }

    public async Task UpdateUser(Models.Authentication.UserEntity.UserEntity userEntity)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _session.UpdateAsync(userEntity);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task CreateUser(Models.Authentication.UserEntity.UserEntity userEntity)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _session.SaveAsync(userEntity);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task DeleteUser(Guid id)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var recoverySession = await _session.GetAsync<Models.Authentication.RecoverySession.RecoverySession>(id);
            if (recoverySession != null)
            {
                await _session.DeleteAsync(recoverySession);
                _unitOfWork.Commit();
            }
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}