using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectASP.Data;
using ProiectASP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace ProiectASP.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfilesController(
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

            var profiles = db.Profiles.Include("User");

            /// motor de cautare
            /// 

            var search = "";


            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in postare (Title si Content)

                List<int> profileIds = db.Profiles.Where
                                        (at => at.First_Name.Contains(search) || at.Last_Name.Contains(search)
                                        ).Select(a => a.Id).ToList();



                // Lista postarilor care contin cuvantul cautat
                // fie in postare -> PostName si Content
                // fie in comentarii -> Text
                profiles = (IOrderedQueryable<Profile>)db.Profiles.Where(profile => profileIds.Contains(profile.Id))
                                      .Include("User");


            }

            ViewBag.SearchString = search;

            /// afisare pginata
            /// 

            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            // Fiind un numar variabil de postari, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = profiles.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Posts/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de postari care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau postarile corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedProfiles = profiles.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem postarile cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Profiles = paginatedProfiles;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Profiles/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Profiles/Index/?page";
            }


            ViewBag.UserCurent = _userManager.GetUserId(User);

            return View();

        }


        /// crearea unui profil
        /// 

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Profile profile = new Profile();

   
            return View(profile);

        }

        /// new cu post
        /// 

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Profile profile)
        {

            profile.UserId = _userManager.GetUserId(User);


            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Profilul nu a fost adaugat";
                return View(profile);
            }
        }

        /// afisarea profilului
        /// 

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            Profile profile = db.Profiles.Include("User")
                                         .Where(p => p.Id == id)
                                         .First();




            /// preluam grupurile utilizatorului
            ViewBag.UserGroups = db.Groups
                                      .Where(g => g.UserId == _userManager.GetUserId(User))
                                      .ToList();


            /// preluam toate grupurile
            /// 
            ViewBag.Groups = db.Groups.ToList();


            /// preluam prietenii userului
            /// 

            ViewBag.Friends = db.Friends
                                            .Where(g => g.UserId == _userManager.GetUserId(User))
                                            .ToList();


            SetAccessRights();

            return View(profile);

        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        /// sterge profil
        /// 

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles
                                .Include("Friends")
                                .Where(p => p.Id == id)
                                .First();

            
                db.Profiles.Remove(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost sters";
                return RedirectToAction("Index");
            
        }


        /// Editare profil
        /// 
        // Se editeaza un profil existent in baza de date
        // HttpGet implicit
        // Se afiseaza formularul

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Profile profile = db.Profiles.Where(p => p.Id == id)
                                .First();

            if (profile.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(profile);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine !";
                return RedirectToAction("Index");
            }


        }


        // Se adauga postarea modificata in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Profile requestProfile)
        {
            Profile profile = db.Profiles.Find(id);

            if (ModelState.IsValid)
            {
                if (profile.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    profile.First_Name = requestProfile.First_Name;
                    profile.Last_Name = requestProfile.Last_Name;
                    profile.Description = requestProfile.Description;
                    profile.Job = requestProfile.Job;
                    profile.Image = requestProfile.Image;
                    profile.Phone = requestProfile.Phone;
                    profile.Profile_Status = requestProfile.Profile_Status;
                    TempData["message"] = "Profilul a  fost editat cu succes !";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine !";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestProfile);
            }
        }




        /// Adaugare grup in profil
        /// 

        [HttpPost]
        public IActionResult AddGroup([FromForm] ProfileGroup profileGroup)
        {
            // Daca modelul este valid
            if (ModelState.IsValid)
            {
                // Verificam daca avem deja articolul in colectie
                if (db.ProfileGroups
                    .Where(ab => ab.ProfileId == profileGroup.ProfileId)
                    .Where(ab => ab.GroupId == profileGroup.GroupId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest profil este deja adaugat in grup";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    // Adaugam asocierea intre grup si profil 
                    db.ProfileGroups.Add(profileGroup);
                    // Salvam modificarile
                    db.SaveChanges();

                    // Adaugam un mesaj de success
                    TempData["message"] = "Profilul a fost adaugat in grupul selectat";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga profilul in grup";
                TempData["messageType"] = "alert-danger";
            }

            // Ne intoarcem la pagina grupului
            return Redirect("/Profiles/Show/" + profileGroup.ProfileId);
        }

        /// Adaugare friend in profil
        /// 

        [HttpPost]
        public IActionResult AddFriend([FromForm] Friend friend)
        {
            friend.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Friends.Add(friend);
                db.SaveChanges();
                return Redirect("/Profiles/Show/" + friend.ProfileId);
            }

            else
            {
                Profile profil = db.Profiles.Include("Friends")
                                    .Include("User")
                                    .Include("Friends.User")
                                    .Where(p => p.Id == friend.ProfileId)
                                    .First();

                SetAccessRights();

                return View(profil);
            }

        }

    }

}