using System.Security.Cryptography;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.UserEntity;
using NHibernate;

namespace gatherly.server.Persistence.Authentication.UserEntity;

/// <summary>
///     Repository for managing user-related operations.
/// </summary>
public class UserEntityRepository : IUserEntityRepository
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 10000;
    private const char Delimiter = ';';
    private readonly ISessionFactory _sessionFactory;

    public UserEntityRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    /// <summary>
    ///     Checks if a user with the specified email is an administrator.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns><c>true</c> if the user is an administrator; otherwise, <c>false</c>.</returns>
    public bool IsUserAdmin(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);
            return user != null && user.UserRole == UserRole.Admin;
        }
    }

    /// <summary>
    ///     Checks if a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns><c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
    public bool IsUserExists(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);
            if (user != null) return true;

            return false;
        }
    }

    /// <summary>
    ///     Gets user information by email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The user entity if found; otherwise, <c>null</c>.</returns>
    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                .SingleOrDefault(x => x.Email == email);

            return user;
        }
    }

    /// <summary>
    ///     Gets user information by user ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user entity if found; otherwise, <c>null</c>.</returns>
    public Models.Authentication.UserEntity.UserEntity? GetUserInfo(Guid id)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Get<Models.Authentication.UserEntity.UserEntity>(id);
            return user;
        }
    }

    /// <summary>
    ///     Updates user information.
    /// </summary>
    /// <param name="newData">The new user data.</param>
    /// <param name="email">The email of the user to be updated.</param>
    /// <returns>The updated user entity if successful; otherwise, <c>null</c>.</returns>
    public Models.Authentication.UserEntity.UserEntity? PatchUserInfo(UserEntityDTOUpdate newData, string email)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == email);

                if (user == null) return null;

                var userType = typeof(Models.Authentication.UserEntity.UserEntity);
                var newDataType = newData.GetType();
                var userProperties = userType.GetProperties();

                foreach (var userProperty in userProperties)
                {
                    var newDataProperty = newDataType.GetProperty(userProperty.Name);
                    if (newDataProperty != null && newDataProperty.PropertyType == userProperty.PropertyType)
                    {
                        var value = newDataProperty.GetValue(newData);
                        if (value != null) userProperty.SetValue(user, value);
                    }
                }

                session.Update(user);
                transaction.Commit();
                return user;
            }
        }
    }

    /// <summary>
    ///     Deletes user information by email.
    /// </summary>
    /// <param name="email">The email of the user to be deleted.</param>
    /// <returns><c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.</returns>
    public bool DeleteUserInfo(string email)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                        .FirstOrDefault(x => x.Email == email);
                    if (user == null) return false;

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

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="newData">The new user data.</param>
    /// <returns>The created user entity if successful; otherwise, <c>null</c>.</returns>
    public Models.Authentication.UserEntity.UserEntity? CreateNewUser(UserEntityDTOCreate newData)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == newData.Email);

                if (user != null) return null;

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

    /// <summary>
    ///     Verifies user by user's login credentials.
    /// </summary>
    /// <param name="data">The login credentials.</param>
    /// <returns>The user entity if verification is successful; otherwise, <c>null</c>.</returns>
    public Models.Authentication.UserEntity.UserEntity? VerifyUser(UserEntityDTOLoginPassword data)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == data.Email);
                if (user == null) return null;
                var verify = VerifyPassword(data.Password, user.PasswordHash);
                if (verify) return user;
                return null;
            }
        }
    }

    /// <summary>
    ///     Hashes a password using a secure hashing algorithm.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
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

    /// <summary>
    ///     Verifies a plain text password against a hashed password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="hashedPassword">The hashed password.</param>
    /// <returns><c>true</c> if the passwords match; otherwise, <c>false</c>.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(Delimiter);
        if (parts.Length != 2) return false;

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

    /// <summary>
    ///     Compares two byte arrays for equality.
    /// </summary>
    /// <param name="key1">The first byte array.</param>
    /// <param name="key2">The second byte array.</param>
    /// <returns><c>true</c> if the byte arrays are equal; otherwise, <c>false</c>.</returns>
    private static bool AreKeysEqual(byte[] key1, byte[] key2)
    {
        if (key1.Length != key2.Length) return false;

        for (var i = 0; i < key1.Length; i++)
            if (key1[i] != key2[i])
                return false;

        return true;
    }

    public Models.Authentication.UserEntity.UserEntity? ChangeUserPassword(UserEntityDTOResetPassword data)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Query<Models.Authentication.UserEntity.UserEntity>()
                    .SingleOrDefault(x => x.Email == data.Email);

                if (user == null) return null;
                
                user.PasswordHash = HashingPassword(data.NewPassword);
                session.Update(user);
                transaction.Commit();
                return user;

            }
        }
    }

}