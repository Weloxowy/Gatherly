using gatherly.server.Entities.Authentication;

namespace gatherly.server.Models.Authentication.UserEntity;

public interface IUserEntityRepository
{
    public UserEntity? GetUserInfo(string email); //pobieranie danych o uzytkowniku poprzez maila
    public UserEntity? GetUserInfo(Guid id); //pobieranie danych o uzytkowniku poprzez id
    public bool IsUserExists(string email); //sprawdzenie czy uzytkownik istnieje
    public bool IsUserAdmin(string email); //sprawdzenie czy uzytkownik jest adminem
    public UserEntity? PatchUserInfo(UserEntityDTOUpdate newData, string email);
    public bool DeleteUserInfo(string email);
    public UserEntity? CreateNewUser(UserEntityDTOCreate newData);
    public UserEntity? VerifyUser(UserEntityDTOLoginPassword data);
    public UserEntity? ChangeUserPassword(UserEntityDTOResetPassword data);
}