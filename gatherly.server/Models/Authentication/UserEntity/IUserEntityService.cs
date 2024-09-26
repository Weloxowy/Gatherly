using gatherly.server.Entities.Authentication;

namespace gatherly.server.Models.Authentication.UserEntity;

public interface IUserEntityService
{
    public Task<UserEntity?> GetUserInfo(string email);
    public Task<UserEntity?> GetUserInfo(Guid id);
    public Task<bool> IsUserExists(string email);
    public Task<bool> IsUserAdmin(string email);
    public Task<UserEntity?> PatchUserInfo(UserEntityDTOUpdate newData, string email);
    public Task<bool> DeleteUserInfo(string email);
    public Task<UserEntity?> CreateNewUser(UserEntityDTOCreate newData);
    public Task<UserEntity?> VerifyUser(UserEntityDTOLoginPassword data);
    public Task<UserEntity?> ChangeUserPassword(UserEntityDTOResetPassword data);
    public Task<UserEntity> ChangeUserStatus(Guid id);

}