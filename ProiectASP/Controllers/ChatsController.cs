using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectASP.Data;
using ProiectASP.Models;
using System.Data;

namespace ProiectASP.Controllers
{
    public class ChatsController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ChatsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        /// stergere chat
        /// 

        // Stergerea unui chat asociat unui grup din baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Chat chat = db.Chats.Find(id);

            if (chat.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Chats.Remove(chat);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + chat.GroupId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a sterge chat-ul !";
                return RedirectToAction("Index", "Groups");
            }
        }

    }
}
