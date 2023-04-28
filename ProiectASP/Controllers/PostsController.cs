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
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
         
        public PostsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // Se afiseaza lista tuturor postarilor impreuna cu comentariile corespunzatoare
        // HttpGet implicit
        // pentru fiecare postare se afiseaza si utilizatorul care a publicat postarea

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var posts = db.Posts.Include("User").OrderBy(a => a.Data);

            /// motor de cautare
            /// 

            var search = "";


            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in postare (Title si Content)

                List<int> postIds = db.Posts.Where
                                        (at => at.PostName.Contains(search) || at.Content.Contains(search)
                                        ).Select(a => a.Id).ToList();

                // Cautare in comentarii (Text)
                List<int> postIdsOfCommentsWithSearchString = db.Comments
                                        .Where
                                        (
                                         c => c.Text.Contains(search)
                                        ).Select(c => (int)c.PostId).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = postIds.Union(postIdsOfCommentsWithSearchString).ToList();


                // Lista postarilor care contin cuvantul cautat
                // fie in postare -> PostName si Content
                // fie in comentarii -> Text
                posts = (IOrderedQueryable<Post>)db.Posts.Where(post => mergedIds.Contains(post.Id))
                                      .Include("User");
                                      

            }

            ViewBag.SearchString = search;


            /// afisare pginata
            /// 

            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            // Fiind un numar variabil de postari, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = posts.Count();


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
            var paginatedPosts = posts.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem postarile cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Posts = paginatedPosts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index/?page";
            }


            return View();
        }

        /// Show 
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            Post postare = db.Posts.Include("Comments")
                                    .Include("User")
                                    .Include("Comments.User")
                                    .Where(p => p.Id == id)
                                    .First();

            SetAccessRights();

            return View(postare);


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

        [HttpPost]
        public IActionResult Show([FromForm] Comment comm)
        {
            comm.Data = DateTime.Now;
            comm.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }

            else
            {
                Post postare = db.Posts.Include("Comments")
                                    .Include("User")
                                    .Include("Comments.User")
                                    .Where(p => p.Id == comm.PostId)
                                    .First();

                SetAccessRights();

                return View(postare);
            }

        }

        /// Se afiseaza formularul in care se vor completa datele unei postari 
        /// HttpGet implicit
        /// doar utilizatorii User si Admin pot adauga postari
        
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {

            Post post = new Post();

            return View(post);
        }

        /// Adaugarea postarii in baza de date
   
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Post post)
        {
            post.UserId = _userManager.GetUserId(User);

            if(ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugata cu succes !";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
           
        }

        // Se editeaza o postare existenta in baza de date
        // HttpGet implicit
        // Se afiseaza formularul

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Post post = db.Posts.Where(p => p.Id == id)
                                .First();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine !";
                return RedirectToAction("Index");
            }
            
            
        }


        // Se adauga postarea modificata in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);

            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    post.PostName = requestPost.PostName;
                    post.Content = requestPost.Content;
                    TempData["message"] = "Postarea a fost editata cu succes !";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine !";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestPost);
            }
        }

        // Se sterge o postare din baza de date 
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Include("Comments")
                                .Where(p => p.Id == id)
                                .First();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {                
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost stearsa";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine !";
                return RedirectToAction("Index");
            }
        }

    }
}
