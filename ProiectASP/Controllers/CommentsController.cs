using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectASP.Data;
using ProiectASP.Models;

namespace ProiectASP.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // Stergerea unui comentariu asociat unei postari din baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a sterge comentariul !";
                return RedirectToAction("Index", "Posts");
            }
        }

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a edita comentariul !";
                return RedirectToAction("Index", "Posts");
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            /// todo : query explicit cu where (lambda expression) 
             
            Comment comm = db.Comments.Find(id);

            if(ModelState.IsValid)
            {
                if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {

                    comm.Text = requestComment.Text;
                    db.SaveChanges();
                    return Redirect("/Posts/Show/" + comm.PostId);

                }
                else
                {

                    TempData["message"] = "Nu aveti dreptul de a edita comentariul !";
                    return RedirectToAction("Index", "Posts");

                }
            }
            else
            {
                return View(requestComment);
            }

        }
    }
}
