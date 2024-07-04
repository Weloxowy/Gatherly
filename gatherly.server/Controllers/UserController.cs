using gatherly.server.Entities;
using gatherly.server.Models.Users;
using gatherly.server.Persistence.Users;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace gatherly.server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private UserService _userService = new UserService();
    
    [SwaggerOperation(Summary = "Pobierz wszystkich użytkowników")]
    [HttpGet]
    public ActionResult<IEnumerable<Users>> GetAll()
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var users = session.Query<Users>().ToList();
            return Ok(users);
        }
    }
    
    [SwaggerOperation(Summary = "Pobierz dane użytkownika o wybranym id")]
    [HttpGet("id/{id}")]
    public ActionResult<Users> GetById(string id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var user = session.Get<Users>(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
    
    [SwaggerOperation(Summary = "Dodaj użytkownika")]
    [HttpPost]
    public ActionResult<Users> CreateNewUser([FromBody] Entities.NewUserDTO data)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var user = new Users
                    {
                        Name = data.Name,
                        Email = data.Email,
                        AvatarName = data.AvatarName,       
                        LastTimeLogged = DateTime.Now 
                    };
                    session.Save(user);
                    transaction.Commit();
                    return CreatedAtAction(nameof(GetById), new { id = user.Id }, data);   //zwracamy przekazane dane a nie całą encję             
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
                }
            }
        }
    }
    
    [SwaggerOperation(Summary = "Dodaj użytkownika")]
    [HttpDelete]
    public ActionResult<Users> DeleteExistingUser(string id)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var user = session.Get<Users>(Guid.Parse(id));
                    if (user == null)
                    {
                        return NotFound();
                    }
                    session.Delete(user);
                    transaction.Commit();
                    return Ok("User was deleted correctly.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
                }
            }
        }
    }
    /*
     * metoda DELETE, PATCH dla Usera
     * metoda Login
     * metoda Register
     * metoda Generate SSO
     * metody do wysyłania maili
     * metoda uwierzytelniania kodu SSO
     * metoda sprawdzenia tokenu JWT
     * metoda odświeżenia tokenu JWT
     * metoda dewaluacji (?) tokenu JWT
     */
}