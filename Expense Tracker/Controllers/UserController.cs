using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class UserController : Controller
    {
        const string SessionName = "_Email";
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"]!=null)
            {
                List<User> users = _db.Users.ToList();
                return View(users);
            }
            TempData["Error"] = "You didn't Log In";
            return View();
            
        }

        public IActionResult GetProfile()
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                User user = _db.Users.FirstOrDefault(u=>u.Email== TempData["email"]);
                return View(user);
            }
            TempData["Error"] = "You didn't Log In";
            return View();

        }



        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if(ModelState.IsValid)
            {
                int salt = 10;
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password,salt);
                _db.Users.Add(user);
                _db.SaveChanges();
                TempData["success"] = "User created successfully";
                return RedirectToAction("Login","User");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login user)
        {
            if (ModelState.IsValid)
            {
               var user1= _db.Users.Where(u => u.Email == user.Email).First();
                if (user1 != null)
                {

                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(user.Password, user1.Password);
                    if (isValidPassword)
                    {
                        HttpContext.Session.SetString(SessionName, user.Email);
                        TempData["success"] = "User Logged In successfully";
                        return RedirectToAction("GetProfile", "User");
                    }
                    TempData["Error"] = "Youre Password is incorrect";
                    return View();

                }               
            }
            return View();
        }
        public IActionResult UserUpdate(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            User user = _db.Users.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UserUpdate(User user)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                if (ModelState.IsValid)
                {

                    _db.Users.Update(user);
                    _db.SaveChanges();
                    TempData["success"] = "User updated successfully";
                    return RedirectToAction("GetProfile", "User");
                }
                return View();
            }
            TempData["Error"] = "You didn't Log In";
            return View();

        }


        public IActionResult DeleteUser(int? id)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                User user = _db.Users.FirstOrDefault(c => c.Id == id);

                _db.Users.Remove(user);
                _db.SaveChanges();
                return RedirectToAction("GetProfile", "User");
            }
            TempData["Error"] = "You didn't Log In";
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                if (ModelState.IsValid)
                {
                    User user = _db.Users.Where(l => l.Email == TempData["email"]).FirstOrDefault();
                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(oldPassword, user.Password);
                    if (isValidPassword)
                    {
                        int salt = 10;
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                        _db.Users.Update(user);
                        _db.SaveChanges();
                        TempData["success"] = "Password Changed successfully";
                        return RedirectToAction("GetProfile", "User");
                    }
                    TempData["Error"] = "You old password is incorrect";
                    return View();
                    
                }
                return View();
            }
            TempData["Error"] = "You didn't Log In";
            return View();

        }

        public IActionResult Logout()
        {
            HttpContext.Session.SetString(SessionName,"");
            
            return RedirectToAction("Login","User");
        }


    }
}
