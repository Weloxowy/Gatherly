using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Persistence.Authentication.UserEntity;

public class UserEntityService : IUserEntityService
{
    private readonly UserEntityRepository _userEntityRepository = new UserEntityRepository(NHibernateHelper.SessionFactory);
    
    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(string email)
    {
        return _userEntityRepository.GetUserInfo(email);
    }

    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(Guid id)
    {
        return _userEntityRepository.GetUserInfo(id);
    }

    public bool IsUserExists(string email)
    {
        return _userEntityRepository.IsUserExists(email);
    }

    public bool IsUserAdmin(string email)
    {
        return _userEntityRepository.IsUserAdmin(email);
    }

    public Models.Authentication.UserEntity.UserEntity? PatchUserInfo(UserEntityDTOUpdate newData, string email)
    {
        return _userEntityRepository.PatchUserInfo(newData,email);
    }

    public bool DeleteUserInfo(string email)
    {
        return _userEntityRepository.DeleteUserInfo(email);
    }

    public Models.Authentication.UserEntity.UserEntity? CreateNewUser(UserEntityDTOCreate newData)
    {
        return _userEntityRepository.CreateNewUser(newData);
    }

    public Models.Authentication.UserEntity.UserEntity? VerifyUser(UserEntityDTOLoginPassword data)
    {
        return _userEntityRepository.VerifyUser(data);
    }
}