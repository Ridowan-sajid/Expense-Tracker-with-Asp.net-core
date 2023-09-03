using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class ItemController : Controller
    {
        const string SessionName = "_Email";

        private readonly ApplicationDbContext _db;
        public ItemController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            double itemPrice = 0;
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            User user = _db.Users.FirstOrDefault(x => x.Email == TempData["email"]);
            if (TempData["email"] != null)
            {
                List<Item> items = _db.Items.Where(l=>l.UserId==user.Id).ToList();
                foreach (var i in items)
                {
                    itemPrice += i.Expense;
                }
                TempData["totalPrices"] = itemPrice;

                return View(items);
            }
            TempData["Error"] = "You did not Log In";
            return View();
        }

        [HttpGet]
        public IActionResult CreateItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateItem(Item item)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                Item item1 = _db.Items.FirstOrDefault(x => x.Name == item.Name);
                if (item1 == null)
                {
                    if (ModelState.IsValid)
                    {
                        User user = _db.Users.FirstOrDefault(x => x.Email == TempData["email"]);
                        item.Expense = item.Expense * item.Number;
                        item.UserId = user.Id;
                        _db.Items.Add(item);
                        _db.SaveChanges();
                        TempData["success"] = "Items created successfully";
                        return RedirectToAction("Index", "Item");
                    }
                    
                    return View();
                }
                TempData["Error"] = "A item you already created with this name";
                return View();
            }
            TempData["Error"] = "You didn't Log In";
            return View();
        }



        public IActionResult ItemUpdate(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Item item = _db.Items.FirstOrDefault(c => c.Id == id);
            item.Expense=item.Expense / item.Number;
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        public IActionResult ItemUpdate(Item item)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                Item item1 = _db.Items.FirstOrDefault(x => x.Id != item.Id && x.Name == item.Name);
                if (item1 == null)
                {
                    if (ModelState.IsValid)
                    {
                        item.Expense = item.Expense * item.Number;
                        _db.Items.Update(item);
                        _db.SaveChanges();
                        TempData["success"] = "Items updated successfully";
                        return RedirectToAction("Index", "Item");
                    }
                    return View();
                }
                TempData["Error"] = "A item you already created with this name";
                return View();
            }
            TempData["Error"] = "You didn't Log In";
            return View();
           

        }

        public IActionResult DeleteItem(int? id)
        {
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                Item item = _db.Items.FirstOrDefault(c => c.Id == id);
                _db.Items.Remove(item);
                _db.SaveChanges();
                return RedirectToAction("Index", "Item");
            }
            TempData["Error"] = "You didn't Log In";
            return View();
        }
        public IActionResult SearchItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SearchItem(DateTime date)
        {
            double itemPrice = 0;
            TempData["email"] = HttpContext.Session.GetString(SessionName);
            if (TempData["email"] != null)
            {
                User user = _db.Users.FirstOrDefault(c => c.Email== TempData["email"]);
                List<Item> items = _db.Items.Where(c => c.UserId == user.Id && c.date.Date == date.Date).ToList();
                
                foreach (var i in items)
                {
                    itemPrice += i.Expense;
                }
                TempData["totalPrices"] = itemPrice;
                return View(items);
            }
            TempData["Error"] = "You didn't Log In";
            return View();
        }

    }
}
