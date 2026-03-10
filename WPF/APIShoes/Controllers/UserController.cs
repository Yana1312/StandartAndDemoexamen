using APIShoes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace APIShoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public List<User> GetUsers()
        {
            return ShoesContext.context.Users.ToList();
        }

        [HttpGet("{id}")]
        public User GetUser(int id)
        {
            var user = ShoesContext.context.Users.Find(id);
            if (user == null) NotFound("Пользователь не найден");

            return user;
        }



        //[HttpPost]
        //public void PostUser(User user)
        //{

        //}

        [HttpPut("{id}")]
        public ActionResult PutUser(int id, User user)
        {
            if (id != user.UserId) return BadRequest();

            ShoesContext.context.Entry(user).State = EntityState.Modified;

            try
            {
                ShoesContext.context.SaveChanges();
            } catch (Exception ex)
            {
                if (ShoesContext.context.Users.Any(e => e.UserId == id)) return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public void DeleteUser(int id)
        {
            var user = ShoesContext.context.Users.Find(id);
            if (user == null) NotFound();

            ShoesContext.context.Remove(user);
            ShoesContext.context.SaveChanges();
        }
    }
}
