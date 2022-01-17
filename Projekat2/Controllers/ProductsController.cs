using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekat2.Models;

namespace Projekat2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {

            var northwindContext = _context.Products.Include(p => p.Category).Include(p => p.Supplier);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View(await northwindContext.ToListAsync());
        }

        public IActionResult ProductsForOrder()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName");
            ViewData["OrderId"] = new SelectList(new List<Orders>(), "OrderId", "OrderId");
            return View(new List<Products>());
        }

        public IActionResult ProductsForOrderByCustomer(string CustomerId)
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", CustomerId);
            ViewData["OrderId"] = new SelectList(_context.Orders.Where(x=> x.CustomerId == CustomerId), "OrderId", "OrderId");
            return View("ProductsForOrder", new List<Products>());
        }

        public IActionResult ProductsForOrderByOrder(int OrderId)
        {
            Orders orders = _context.Orders.FirstOrDefault(x => x.OrderId == OrderId);
            List<OrderDetails> orderDetails =  _context.OrderDetails.Include(x => x.Product).Where(x => x.OrderId == OrderId).ToList();
            string CustomerId = orders.CustomerId;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", CustomerId);
            ViewData["OrderId"] = new SelectList(_context.Orders.Where(x => x.CustomerId == CustomerId), "OrderId", "OrderId", OrderId);

            List<Products> products = new List<Products>();

            foreach(OrderDetails orderDetail in orderDetails) 
            {
                products.Add(orderDetail.Product);
            }

            return View("ProductsForOrder", products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create(int SupplierId)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(x=> x.SupplierId == SupplierId), "SupplierId", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers.Where(x => x.SupplierId == products.SupplierId), "SupplierId", "CompanyName");
            return RedirectToAction(nameof(Create), new { SupplierId = products.SupplierId });
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", products.SupplierId);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (id != products.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", products.SupplierId);
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
