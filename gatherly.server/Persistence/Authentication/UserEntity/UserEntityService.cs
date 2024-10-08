﻿using System.Security.Cryptography;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Persistence.Authentication.UserEntity;

public class UserEntityService : IUserEntityService
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 10000;
    private const char Delimiter = ';';
    private readonly IUserEntityRepository _userEntityRepository;
    public UserEntityService(IUserEntityRepository userEntityRepository)
    {
        _userEntityRepository = userEntityRepository;
    }
    
    /// <summary>
    ///     Hashes a password using a secure hashing algorithm.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
    private static string HashingPassword(string password)
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
    private static bool VerifyPassword(string password, string hashedPassword)
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
    
    /// <summary>
    ///     Checks if a user with the specified email is an administrator.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns><c>true</c> if the user is an administrator; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsUserAdmin(string email)
    {
        var user = await _userEntityRepository.GetUserByEmail(email);
        if (user == null)
        {
            return false;
        }
        return user.UserRole == UserRole.Admin;
    }

    /// <summary>
    ///     Checks if a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns><c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsUserExists(string email)
    {
        var user = await _userEntityRepository.GetUserByEmail(email);
        if (user == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    ///     Gets user information by email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The user entity if found; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> GetUserInfo(string email)
    {
        return await _userEntityRepository.GetUserByEmail(email);
    }

    /// <summary>
    ///     Gets user information by user ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user entity if found; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> GetUserInfo(Guid id)
    {
        return await _userEntityRepository.GetUserByUserId(id);
    }

    /// <summary>
    ///     Updates user information.
    /// </summary>
    /// <param name="newData">The new user data.</param>
    /// <param name="email">The email of the user to be updated.</param>
    /// <returns>The updated user entity if successful; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> PatchUserInfo(UserEntityDTOUpdate newData, string email)
    {
        var existingUser = await _userEntityRepository.GetUserByEmail(email);
        if (existingUser == null)
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
                if (value != null) userProperty.SetValue(existingUser, value);
            }
        }
        await _userEntityRepository.UpdateUser(existingUser);
        return existingUser;
    }

    /// <summary>
    ///     Deletes user information by email.
    /// </summary>
    /// <param name="email">The email of the user to be deleted.</param>
    /// <returns><c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteUserInfo(string email)
    {
        var existingUser = await _userEntityRepository.GetUserByEmail(email);
        if (existingUser == null)
        {
            return false;
        }
        _userEntityRepository.DeleteUser(existingUser.Id);
        return true;
    }

    /// <summary>
    ///     Creates a new user.
    /// </summary>
    /// <param name="newData">The new user data.</param>
    /// <returns>The created user entity if successful; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> CreateNewUser(UserEntityDTOCreate newData)
    {
        var user = new Models.Authentication.UserEntity.UserEntity
        {
            Email = newData.Email,
            PasswordHash = HashingPassword(newData.Password),
            AvatarName = "avatar" + new Random().Next(1, 15),
            Name = newData.Name,
            UserRole = UserRole.User,
            LastTimeLogged = DateTime.UtcNow
        };
        await _userEntityRepository.CreateUser(user);
        return user;
        
    }

    /// <summary>
    ///     Verifies user by user's login credentials.
    /// </summary>
    /// <param name="data">The login credentials.</param>
    /// <returns>The user entity if verification is successful; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> VerifyUser(UserEntityDTOLoginPassword data)
    {
        var user = await _userEntityRepository.GetUserByEmail(data.Email);
        if (user == null)
        {
            return null;
        }
        var verify = VerifyPassword(data.Password, user.PasswordHash);
        if (!verify)
        {
            return null;
        }
        user.LastTimeLogged = DateTime.UtcNow;
        await _userEntityRepository.UpdateUser(user);
        return user;
    }

    /// <summary>
    /// Changes user password.
    /// </summary>
    /// <param name="data">Email and a new password.</param>
    /// <returns>The updated user entity if successful; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity?> ChangeUserPassword(UserEntityDTOResetPassword data)
    {
        var user = await _userEntityRepository.GetUserByEmail(data.Email);
        if (user == null)
        {
            return null;
        }
        user.PasswordHash = HashingPassword(data.NewPassword);
        await _userEntityRepository.UpdateUser(user);
        return user;
    }

    /// <summary>
    /// Changes user's status.
    /// </summary>
    /// <param name="id">ID of existing user.</param>
    /// <returns>The updated user entity if successful; otherwise, <c>null</c>.</returns>
    public async Task<Models.Authentication.UserEntity.UserEntity> ChangeUserStatus(Guid id)
    {
        var user = await _userEntityRepository.GetUserByUserId(id);
        if (user == null)
        {
            return null;
        }
        user.UserRole = (user.UserRole == UserRole.Admin) ? UserRole.Admin : UserRole.User;
        await _userEntityRepository.UpdateUser(user);
        return user;
    }
}