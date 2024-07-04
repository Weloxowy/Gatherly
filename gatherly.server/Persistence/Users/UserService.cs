
using gatherly.server.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace gatherly.server.Persistence.Users;

public class UserService : IUserService
{
    private UserRepository _userRepository = new UserRepository();
    
}