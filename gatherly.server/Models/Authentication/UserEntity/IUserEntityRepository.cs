using gatherly.server.Entities.Authentication;

namespace gatherly.server.Models.Authentication.UserEntity;

public interface IUserEntityRepository
{
    Task<UserEntity?> GetUserByUserId(Guid id);
    Task<UserEntity?> GetUserByEmail(string email);
    Task UpdateUser(UserEntity userEntity);
    Task CreateUser(UserEntity userEntity);
    Task DeleteUser(Guid id);

}