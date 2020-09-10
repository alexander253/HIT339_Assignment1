using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesBoard.Data;
using SalesBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SalesBoard.Controllers
{
    public class ItemsController : Controller
    {
        private readonly SalesBoardContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _session;

        public ItemsController(SalesBoardContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor session)
        {
            _context = context;
            _userManager = userManager;
            _session = session;
        }

        // GET: Items
        public async Task<IActionResult> Index(string searchString)
        {
            var movies = from m in _context.Items
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Name.Contains(searchString));
            }

            return View(await movies.ToListAsync());
        }

        // GET: Items/myItems
        public ActionResult MyItems()
        {
          
            var seller = _userManager.GetUserName(User);
            var items = _context.Items
                .Where(m => m.Seller == seller);
            return View("Index", items);
        }


        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            var user = _userManager.GetUserName(User);

            if (user == null)
            {
                ViewBag.errorMessage = "You are not logged in, log in or register to create an item for sale";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Quantity")] Items items)
        {
            var user = _userManager.GetUserName(User);

            if (user == null)
            {
                ViewBag.errorMessage = "You are not logged in, log in or register to create an item for sale";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (ModelState.IsValid)
            {
                var seller = _userManager.GetUserName(User);
                items.Seller = seller;
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var items = await _context.Items.FindAsync(id);

            var user = _userManager.GetUserName(User);
            var seller = items.Seller;

            if (user == null)
            {
                ViewBag.errorMessage = "You are not logged in, log in or register to start shopping";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (user!=seller)
            {
                ViewBag.errorMessage = "You are not the seller of this item, therefore you cannot edit this item";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }


            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Quantity,Price,Seller")] Items items)
        {

            var user = _userManager.GetUserName(User);
            var seller = items.Seller;

            if (id != items.Id)
            {
                return NotFound();
            }

            if (user != seller)
            {
                ViewBag.errorMessage = "You are not the seller of this item, therefore you cannot edit this item";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemsExists(items.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var items = await _context.Items.FindAsync(id);

            var user = _userManager.GetUserName(User);
            var seller = items.Seller;

            if (user != seller)
            {
                ViewBag.errorMessage = "You are not the seller of this item, therefore you cannot delete this item";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (id == null)
            {
                return NotFound();
            }

            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.Items.FindAsync(id);
            var user = _userManager.GetUserName(User);
            var seller = items.Seller;

            if (user != seller)
            {
                ViewBag.errorMessage = "You are not the seller of this item, therefore you cannot delete this item";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            
            _context.Items.Remove(items);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        // GET: Items/Purchase/5
        public async Task<IActionResult> Purchase(int? id)
        {
            var items = await _context.Items.FindAsync(id);
            var user = _userManager.GetUserName(User);
            var seller = items.Seller;

            if (user == seller)
            {
                ViewBag.errorMessage = "You are the seller of this item, therefore you cannot buy this item";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (user == null)
            {
                ViewBag.errorMessage = "You are not logged in, log in or register to start shopping";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (id == null)
            {
                return NotFound();
            }


            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Purchase/5
        [HttpPost, ActionName("Purchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseConfirmed([Bind("Item,Name,Seller,Quantity,Price")] Cart cart, Sales sale)
        {

            // get or create a cart id
            string cartId = _session.HttpContext.Session.GetString("cartId");

            if (string.IsNullOrEmpty(cartId) == true) cartId = Guid.NewGuid().ToString();

            // use the cart id
            cart.CartId = cartId.ToString();

            var items = await _context.Items
             .FirstOrDefaultAsync(m => m.Id == sale.Item);

            if (items == null)
            {
                return NotFound();
            }

            if (items.Quantity < sale.Quantity)
            {
                ViewBag.errorMessage = "We do not have enough stock for your order.";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items.Quantity == 0)
            {
                ViewBag.errorMessage = "Sorry, we are currently running out of stock.";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            //get the name of item sale
            var name = items.Name;
            cart.Name = name;

            //get name of the seller 
            var seller = items.Seller;
            cart.Seller = seller;

            var cost = items.Price * cart.Quantity;
            cart.Price = cost;


            // make the sale/save to cart DB
            _context.Add(cart);
   


            // update the quantity?? only update after purchase all
            // items.Quantity -= cart.Quantity;
            //_context.Update(items);


            // Save the changes
            await _context.SaveChangesAsync();

            // add to cart
            var checkCount = _session.HttpContext.Session.GetInt32("cartCount");
            int cartCount = checkCount == null ? 0 : (int)checkCount;
            _session.HttpContext.Session.SetString("cartId", cartId.ToString());
            _session.HttpContext.Session.SetInt32("cartCount", ++cartCount);

            return RedirectToAction(nameof(Index));

            

            
         


        }


        
    


    }


}
