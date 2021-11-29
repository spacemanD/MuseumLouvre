using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class ExhibitController : Controller
    {
        private readonly IExhibitService _service;

        private readonly IAuthorService _serviceAuth;

        public ExhibitController(IExhibitService service, IAuthorService authorService)
        { 
            _service = service;
            _serviceAuth = authorService;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string searchString, string category)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";

            if (sortOrder == null)
            {
                sortOrder = ViewBag.CategorySortParm;
            }

            ViewBag.CurrentFilter = category;
            var exhibits =  _service.GetAllListAsync();

            var categoryQuery = Enum.GetValues(typeof(ExhibitType)).Cast<ExhibitType>();


            ViewBag.Category = new SelectList(categoryQuery);
            if (!String.IsNullOrEmpty(searchString))
            {
                exhibits = exhibits.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    exhibits = exhibits.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    exhibits = exhibits.OrderBy(s => s.CreationYear);
                    break;
                case "date_desc":
                    exhibits = exhibits.OrderByDescending(s => s.CreationYear);
                    break;
                case "category":
                    if (category != null)
                    {
                        exhibits = exhibits.Where(i => i.Type.ToString() == category);
                    }
                    else 
                    {
                        exhibits.OrderBy(i => i.Cost);
                    }
                    break;
                case "category_desc":
                    exhibits = exhibits.Where(i => i.Type.ToString() == category).OrderByDescending(i => i.Cost);
                    break;
                default:
                    exhibits = exhibits.OrderBy(s => s.Name);
                    break;
            }
            return View(exhibits.ToList());          
        }

        // GET: Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var exhibit = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.ExhibitId == id);

            _service.UpdateStatistics(exhibit.ExhibitId);

            return View(exhibit);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AuthorId,CollectionId,CreationYear,Description,Type,Cost,Direction,Materials,Country")] Exhibit exhibit)
        {
            PopulateDepartmentsDropDownList(exhibit.AuthorId);
            if (ModelState.IsValid)
            {
                await _service.AddAsync(exhibit);
                return RedirectToAction(nameof(Index));
            }
            return View(exhibit);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.ExhibitId == id);

            if (exhibit == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(exhibit.AuthorId);
            return View(exhibit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,AuthorId,CollectionId,CreationYear,Description,Type,Cost,Direction,Materials,Country")] Exhibit exhibit)
        {
            if (id == 0)
            {
                return NotFound();
            }

            exhibit.ExhibitId = id;

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(exhibit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitExists(exhibit.ExhibitId))
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
            return View(exhibit);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.ExhibitId == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product =  _service.GetAllListAsync().ToList().FirstOrDefault(x => x.ExhibitId == id);
            await _service.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitExists(int id)
        {
            return _service.GetAllListAsync().ToList().Any(e => e.ExhibitId == id);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var authorList = _serviceAuth.GetAllListAsync();
            ViewBag.AuthorId = new SelectList(authorList.ToList(), "AuthorId", "Name", selectedDepartment);
        }
    }
}
