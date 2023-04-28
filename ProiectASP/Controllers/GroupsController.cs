using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectASP.Data;
using ProiectASP.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;

namespace ProiectASP.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // Se afiseaza lista tuturor gruprurilor

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var groups = db.Groups.Include("User");
            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        /// Creare grup
        /// 

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {

            Group group = new Group();

            return View(group);
        }

        /// New cu post
        /// 

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Group group)
        {
            group.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost adaugat cu succes !";
                return RedirectToAction("Index");
            }
            else
            {
                return View(group);
            }

        }


        /// Show 
        /// 

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
             Group group = db.Groups.Include("User")
                                    .Include("ProfileGroups.Profile.User")
                                    .Include("Chats")
                                    .Include("Chats.User")
                                    .Where(p => p.Id == id)
                                    .First();

            

            SetAccessRights();

            return View(group);


        }


        // Edit 

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Group group = db.Groups.Where(p => p.Id == id)
                                .First();

            if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(group);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine !";
                return RedirectToAction("Index");
            }

        }


        // Edit cu post

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Group requestGroup)
        {
            Group group = db.Groups.Find(id);

            if (ModelState.IsValid)
            {
                if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    group.GroupName = requestGroup.GroupName;
                    group.Subject = requestGroup.Subject;
                    TempData["message"] = "Grupul a fost editat cu succes !";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine !";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestGroup);
            }
        }


        // Delete 

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Include("Chats")
                                .Where(p => p.Id == id)
                                .First();

            if (group.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un grup care nu va apartine !";
                return RedirectToAction("Index");
            }
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }







        /// show from form pt chat
        /// 
        [HttpPost]
        public IActionResult Show([FromForm] Chat chat)
        {

            chat.UserId = _userManager.GetUserId(User);

            
            if (ModelState.IsValid)
            {
                db.Chats.Add(chat);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + chat.GroupId);
            }

            else
            {
                Group group = db.Groups.Include("Chats")
                                    .Include("User")
                                    .Include("Chats.User")
                                    .Include("ProfileGroups.Profile.User")
                                    .Where(p => p.Id == chat.GroupId)
                                    .First();

                SetAccessRights();

                return View(group);
            }
            
        }

    }
}
