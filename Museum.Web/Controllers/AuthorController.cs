using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class AuthorController : Controller
    {
        // GET: AuthorController

        private readonly IAuthorService _service;

        private readonly IExhibitService _serviceExhib;

        private static List<PopAuthor> popAuthors;


        public AuthorController(IAuthorService authorService, IExhibitService exhibit)
        {
            _service = authorService;
            _serviceExhib = exhibit;
        }
        public IActionResult PopularAuthors(string sortOrder, string category)
        {
            ViewBag.CurrentFilter = category;
            ViewBag.RateSortParm = String.IsNullOrEmpty(sortOrder) ? "rate_desc" : "";


            var categoryQuery = Enum.GetValues(typeof(CountryList)).Cast<CountryList>();

            var authors = new List<PopAuthor>();
            ViewBag.Category = new SelectList(categoryQuery);

            switch (sortOrder)
            {
                case "rate_desc":
                    authors = _service.GetLast10PopularAuthors().ToList();
                    break;
                default:
                    authors = _service.GetTop10PopularAuthors().ToList();
                    break;
            }

            if (category != null)
            {
                authors = authors.Where(i => i.Author.Country.ToString() == category).ToList();
            }

            popAuthors = authors;
            return View(authors);
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

        public IActionResult CreatePDF()
        {
            //Create a new PDF document.
            PdfDocument doc = new PdfDocument();
            //Add a page.
            PdfPage page = doc.Pages.Add();
            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;

            //Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            PdfFont font1 = new PdfStandardFont(PdfFontFamily.TimesRoman, 20);

            //Draw the text
            string currentDate = "DATE " + DateTime.Now.ToString("yyyy'-'MM'-'dd 'Time:' HH':'mm':'ss");
            string assign = "Assigned By Administators";
            graphics.DrawString(currentDate, font, PdfBrushes.Black, new PointF(50, 500));
            graphics.DrawString(assign, font, PdfBrushes.Black, new PointF(370, 500));
            graphics.DrawString("Report", font1, PdfBrushes.Black, new PointF(250, 0));
            //Create a PdfGrid.
            PdfGrid pdfGrid = new PdfGrid();
            //Add values to list
            //Add list to IEnumerable
            List<object> data = new List<object>();
            foreach (var item in popAuthors)
            {
                data.Add(new
                {
                    Rate = item.Rate,
                    AuthorId = item.AuthorId,
                    Name = item.Author.Name,
                    Surname = item.Author.Surname,
                    MiddleName = item.Author.MiddleName,
                    BirthDate = item.Author.BirthDate,
                    DeathDate = item.Author.DeathDate,
                    Country = item.Author.Country
                });
            }
            IEnumerable<object> dataTable = data;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            graphics.DrawString(currentDate, font, PdfBrushes.AliceBlue, new PointF(100, 10));

            //Draw grid to the page of PDF document.
            //Draw the text
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 30));
            //Save the PDF document to stream
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.
            string fileName = "Report.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,Name,Surname,MiddleName,BirthDate,DeathDate,Country")] Author author
            , string[] selectedCourses)
        {

            if (ModelState.IsValid)
            {
                await Task.Run(() => _service.AddAsync(author));
                var author1 = _service.GetAllListAsync().FirstOrDefault(x => x.Name == author.Name && x.Surname == author.Surname && x.MiddleName == author.MiddleName);

                if (selectedCourses != null)
                {
                    author1 = CreateAuthorExhibits(selectedCourses, author1);

                }
                return RedirectToAction(nameof(Index));
            }
                    
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
            var authExhibits = _serviceExhib.GetAllListAsync().Where(x => x?.AuthorId == id || x?.Author?.AuthorId == id);
            await _serviceExhib.DeleteRangeAsync(authExhibits);
            await _service.DeleteAsync(product);
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

        private Author CreateAuthorExhibits(string[] selectedCourses, Author authorToupdate)
        {
            if (selectedCourses == null)
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
                    exhibit.Author = authorToupdate;
                    exhibit.AuthorId = authorToupdate.AuthorId;
                    _serviceExhib.UpdateAsync(exhibit);
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
