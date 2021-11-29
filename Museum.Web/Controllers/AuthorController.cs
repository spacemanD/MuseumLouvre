using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class AuthorController : Controller
    {
        // GET: AuthorController

        private readonly IAuthorService _service;

        private readonly IExhibitService _serviceExhib;


        public AuthorController(IAuthorService authorService, IExhibitService exhibit)
        {
            _service = authorService;
            _serviceExhib = exhibit;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string searchString, string category)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";

            if (sortOrder == null)
            {
                sortOrder = ViewBag.CategorySortParm;
            }

            ViewBag.CurrentFilter = category;
            var exhibits = _service.GetAllListAsync();

            var categoryQuery = Enum.GetValues(typeof(CountryList)).Cast<CountryList>();


            ViewBag.Category = new SelectList(categoryQuery);
            if (!String.IsNullOrEmpty(searchString))
            {
                exhibits = exhibits.Where(s => s.Name.Contains(searchString)
                                       || s.MiddleName.Contains(searchString)
                                       || s.Surname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    exhibits = exhibits.OrderByDescending(s => s.Name);
                    break;
                case "name_asc":
                    exhibits = exhibits.OrderBy(s => s.Name);
                    break;
                case "Date":
                    exhibits = exhibits.OrderBy(s => s.BirthDate);
                    break;
                case "date_desc":
                    exhibits = exhibits.OrderByDescending(s => s.BirthDate);
                    break;
                case "category":
                    if (category != null)
                    {
                        exhibits = exhibits.Where(i => i.Country.ToString() == category);
                    }
                    else
                    {
                        exhibits.OrderBy(i => i.Name);
                    }
                    break;
                case "category_desc":
                    exhibits = exhibits.Where(i => i.Country.ToString() == category).OrderByDescending(i => i.Name);
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
            var exhibit = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.AuthorId == id);

            _service.UpdateStatistics(exhibit.AuthorId);

            return View(exhibit);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var author = new Author();
            author.Exhibits = new List<Exhibit>();
            PopulateAssignedCourseData(author);
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,Name,Surname,MiddleName,BirthDate,DeathDate,Country")] Author author
            , string[] selectedExhibits)
        {
            if (selectedExhibits != null)
            {
                author.Exhibits = new List<Exhibit>();
                foreach (var exhibit in selectedExhibits)
                {
                    var courseToAdd = new Exhibit { AuthorId = author.AuthorId, Name = exhibit};
                    author.Exhibits.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                await Task.Run(() => _service.AddAsync(author));
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedCourseData(author);
            return View(author);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.AuthorId == id);

            if (author == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(author);
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("AuthorId,Name,Surname,MiddleName,BirthDate,DeathDate,Country")] Author author,
            string[] selectedCourses)
        {
            if (id == 0)
            {
                return NotFound();
            }

            author.AuthorId = id;

            if (ModelState.IsValid)
            {
                try
                {
                    author = UpdateInstructorCourses(selectedCourses, author);
                    _service.UpdateAsync(author);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitExists(author.AuthorId))
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
            return View(author);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.AuthorId == id);

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
            var product = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.AuthorId == id);
            var authExhibits = _serviceExhib.GetAllListAsync().Where(x => x.AuthorId == id || x.Author.AuthorId == id);
            await _service.DeleteAsync(product);
            await _serviceExhib.DeleteRangeAsync(authExhibits);
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitExists(int id)
        {
            return _service.GetAllListAsync().ToList().Any(e => e.AuthorId == id);
        }

        private Author UpdateInstructorCourses(string[] selectedCourses, Author authorToupdate)
        {
            if (selectedCourses == null )
            {
                authorToupdate.Exhibits = new List<Exhibit>();
                return authorToupdate;
            }
            if (authorToupdate.Exhibits == null)
            {
                authorToupdate.Exhibits = new List<Exhibit>();
            }
            var exhibitList = _serviceExhib.GetAllListAsync();
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var author = _service.GetAllListAsync().FirstOrDefault(x => x.AuthorId == authorToupdate.AuthorId);
            var authExhi = author.Exhibits.Select(x => x.ExhibitId);
            foreach (var exhibit in exhibitList)
            {
                if (selectedCoursesHS.Contains(exhibit.ExhibitId.ToString()))
                {
                    if (exhibit.AuthorId == null || exhibit.AuthorId != authorToupdate.AuthorId)
                    {
                        exhibit.AuthorId = authorToupdate.AuthorId;
                        authorToupdate.Exhibits.Add(exhibit);
                    }
                }
                else
                {
                    if (authExhi.Contains(exhibit.ExhibitId))
                    {
                        Exhibit courseToRemove = _serviceExhib.GetAllListAsync().FirstOrDefault(i => i.ExhibitId == exhibit.ExhibitId);
                        if (exhibit.AuthorId == authorToupdate.AuthorId)
                        {
                            authorToupdate.Exhibits.Remove(exhibit);
                            _serviceExhib.DeleteAsync(exhibit);
                        }
                    }
                }
            }
            return authorToupdate;
        }
        private void PopulateAssignedCourseData(Author instructor)
        {
            var allCourses = _serviceExhib.GetAllListAsync();
            var instructorCourses = new HashSet<int>(instructor.Exhibits.Select(c => c.ExhibitId));
            var viewModel = new List<Exhibit>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new Exhibit
                {
                    ExhibitId = course.ExhibitId,
                    Name = course.Name,
                    AuthorId = instructor.AuthorId
                });
            }
            ViewData["Exhibits"] = viewModel;
        }
    }
}
