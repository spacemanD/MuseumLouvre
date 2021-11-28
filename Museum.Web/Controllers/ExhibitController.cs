using BLL.Interfaces;
using DAL.EF.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class ExhibitController : Controller
    {
        private readonly IExhibitService _service;

        private readonly List<Exhibit> products = new List<Exhibit>();

        public ExhibitController(IExhibitService service)
        { 
            _service = service;
            products = _service.GetAllListAsync().Result.ToList();
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var re = await _service.GetAllListAsync();
            return View(re.ToList());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var exhibit = await Task.Run(() => _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.ExhibitId == id));

            _service.UpdateStatistics(exhibit.ExhibitId);

            return View(exhibit);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExhibitId,Name,AuthorId,CollectionId,CreationYear,Desciption,Type,Cost,Direction,Materials,Country")] Exhibit product)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _service.AddAsync(product));
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult SortByPrice()
        {
            var appDbContext =  _service.GetAllListAsync().Result.ToList().OrderBy(x => x.CreationYear);
            return View("Index", appDbContext);
        }

        public  IActionResult SortByPriceDesc()
        {
            var appDbContext =  _service.GetAllListAsync().Result.ToList().OrderByDescending(x => x.CreationYear);
            return View("Index", appDbContext);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.ExhibitId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitId,Name,AuthorId,CollectionId,CreationYear,Desciption,Type,Cost,Direction,Materials,Country")] Exhibit product)
        {
            if (id != product.ExhibitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _service.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ExhibitId))
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
            return View(product);
        }

        public IActionResult DeleteItem()
        {
            products.RemoveAll(x => x != null);
            return View("Buy", products);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.ExhibitId == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.ExhibitId == id);
            _service.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _service.GetAllListAsync().Result.ToList().Any(e => e.ExhibitId == id);
        }
    }
}
