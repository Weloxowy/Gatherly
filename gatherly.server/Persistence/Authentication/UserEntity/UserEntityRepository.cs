using System.Reflection;
using System.Security.Cryptography;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.UserEntity;
using NHibernate;

namespace gatherly.server.Persistence.Authentication.UserEntity;

public class UserEntityRepository : IUserEntityRepository
{
    private readonly ISessionFactory _sessionFactory;

    public UserEntityRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 10000;
    private const char Delimiter = ';';

    public static string HashingPassword(string password)
    {
        using (var algorithm = new Rfc2898DeriveBytes(
                   password,
                   SaltSize,
                   Iterations,
                   HashAlgorithmName.SHA256))
        {
            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);
            var base64Salt = Convert.ToBase64String(salt);
            var base64Key = Convert.ToBase64String(key);

            return $"{base64Salt}{Delimiter}{base64Key}";
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 2)
        {
            return false;
        }

        var base64Salt = parts[0];
        var base64Key = parts[1];

        var salt = Convert.FromBase64String(base64Salt);
        var key = Convert.FromBase64String(base64Key);

        using (var algorithm = new Rfc2898DeriveBytes(
                   password,
                   salt,
                   Iterations,
                   HashAlgorithmName.SHA256))
        {
            var keyToCheck = algorithm.GetBytes(KeySize);
            return AreKeysEqual(key, keyToCheck);
        }
    }

    private static bool AreKeysEqual(byte[] key1, byte[] key2)
    {
        if (key1.Length != key2.Length)
        {
            return false;
        }

        for (int i = 0; i < key1.Length; i++)
        {
            if (key1[i] != key2[i])
            {
                return false;
            }
        }

        return true;
    }
    
    
    public bool IsUserAdmin(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);
            return user != null && user.UserRole == UserRole.Admin;
        }
    }

    public bool IsUserExists(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);
            if (user != null)
            {
                return true;
            }

            return false;
        }
    }



    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);

            return user;
        }
    }

    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(Guid id)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Get<Models.Authentication.UserEntity.UserEntity>(id);
            return user;
        }
    }
    
    public Models.Authentication.UserEntity.UserEntity? PatchUserInfo(UserEntityDTOUpdate newData, string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == email);

                if (user == null)
                {
                    return null;
                }

                var userType = typeof(Models.Authentication.UserEntity.UserEntity);
                var newDataType = newData.GetType();
                var userProperties = userType.GetProperties();

                foreach (var userProperty in userProperties)
                {
                    var newDataProperty = newDataType.GetProperty(userProperty.Name);
                    if (newDataProperty != null && newDataProperty.PropertyType == userProperty.PropertyType)
                    {
                        var value = newDataProperty.GetValue(newData);
                        if (value != null)
                        {
                            userProperty.SetValue(user, value);
                        }
                    }
                }
                session.Update(user);
                transaction.Commit();
                return user;
            }
        }
    }

    public bool DeleteUserInfo(string email)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var user = session.Query<Models.Authentication.UserEntity.UserEntity>().FirstOrDefault(x => x.Email == email);
                    if (user == null)
                    {
                        return false;
                    }

                    session.Delete(user);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
    
    public Models.Authentication.UserEntity.UserEntity? CreateNewUser(UserEntityDTOCreate newData)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == newData.Email);

                if (user != null)
                {
                    return null;
                }

                var newUser = new Models.Authentication.UserEntity.UserEntity
                {
                    Email = newData.Email,
                    PasswordHash = HashingPassword(newData.Password),
                    AvatarName = "default",
                    Name = newData.Name,
                    UserRole = UserRole.User,
                    LastTimeLogged = DateTime.Now
                };
                session.Save(newUser);
                transaction.Commit();
                return newUser;
            }
        }
    }

    public Models.Authentication.UserEntity.UserEntity? VerifyUser(UserEntityDTOLoginPassword data)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == data.Email);
                
                var verify = VerifyPassword(data.Password, user.PasswordHash);
                if (verify == true)
                {
                    return user;
                }
                return null;
            }
        }
    }
}