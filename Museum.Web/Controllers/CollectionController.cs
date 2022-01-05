using BLL.Interfaces;
using DAL.EF.Entities;
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
using System.Threading;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ICollectionService _service;

        private readonly IExhibitService _serviceAuth;

        private static List<PopCollection> _exhibits;

        public CollectionController(ICollectionService service, IExhibitService authorService)
        {
            _service = service;
            _serviceAuth = authorService;
        }

        public IActionResult PopularCollections(string sortOrder, string category)
        {
            ViewBag.CurrentFilter = category;
            ViewBag.RateSortParm = String.IsNullOrEmpty(sortOrder) ? "rate_desc" : "";

            var exhibits = new List<PopCollection>();

            switch (sortOrder)
            {
                case "rate_desc":
                    exhibits = _service.GetLast10PopularCollections().ToList();
                    break;
                default:
                    exhibits = _service.GetTop10PopularCollections().ToList();
                    break;
            }

            _exhibits = exhibits;
            return View(exhibits);
        }
        // GET: Products
        public IActionResult Index(string sortOrder, string searchString, string dateSort)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.StartDateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.EndDateSortParm = dateSort == "Date" ? "date_desc" : "Date";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";

            if (sortOrder == null)
            {
                sortOrder = ViewBag.CategorySortParm;
            }

            var exhibits = _service.GetAllListAsync();

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
                    exhibits = exhibits.OrderBy(s => s.StartTime);
                    break;
                case "date_desc":
                    exhibits = exhibits.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    exhibits = exhibits.OrderBy(s => s.Name);
                    break;
            }
            if (dateSort != null)
            {
                switch (dateSort)
                {
                    case "Date":
                        exhibits = exhibits.OrderBy(s => s.EndTime);
                        break;
                    case "date_desc":
                        exhibits = exhibits.OrderByDescending(s => s.EndTime);
                        break;
                    default:
                        exhibits = exhibits.OrderBy(s => s.Name);
                        break;
                }
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
            var exhibit = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.CollectionId == id);

            _service.UpdateStatistics(exhibit.CollectionId);

            return View(exhibit);
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
            foreach (var item in _exhibits)
            {
                data.Add(new
                {
                    Rate = item.Rate,
                    CollectionId = item.CollectionId,
                    Name = item.Collection.Name,
                    Description = item.Collection.Description,
                    StartTime = item.Collection.StartTime,
                    EndTime = item.Collection.EndTime,
                    Exhibits = string.Join(" ;", 
                    item.Collection.Exhibits.Select(x => String.Format("{0} - {1}",
                    x?.Name, x.Author == null ? "None Author" : x.Author.Surname))),
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
        // GET: Products/Create
        public IActionResult Create()
        {
            var author = new Collection();
            author.Exhibits = new List<Exhibit>();
            PopulateAssignedCourseData(author);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollectionId,Name,Description,StartTime,EndTime")] Collection author
           , string[] selectedCourses)
        {

            if (ModelState.IsValid)
            {
                await Task.Run(() => _service.AddAsync(author));
                var author1 = _service.GetAllListAsync().FirstOrDefault(x => x.Name == author.Name);

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

            var author = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.CollectionId == id);

            if (author == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(author);
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CollectionId,Name,Description,StartTime,EndTime")] Collection author,
            string[] selectedCourses)
        {
            if (id == 0)
            {
                return NotFound();
            }

            author.CollectionId = id;

            if (ModelState.IsValid)
            {
                try
                {
                    author = UpdateInstructorCourses(selectedCourses, author);
                    Thread.Sleep(100);
                    _service.UpdateAsync(author);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitExists(author.CollectionId))
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

            var product = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.CollectionId == id);

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
            var product = _service.GetAllListAsync().ToList().FirstOrDefault(x => x.CollectionId == id);
            await _service.DeleteAsync(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitExists(int id)
        {
            return _service.GetAllListAsync().ToList().Any(e => e.CollectionId == id);
        }

        private Collection UpdateInstructorCourses(string[] selectedCourses, Collection authorToupdate)
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
            var exhibitList = _serviceAuth.GetAllListAsyncNonAuthors();
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var author = _service.GetAllListAsync().FirstOrDefault(x => x.CollectionId == authorToupdate.CollectionId);
            var authExhi = author.Exhibits.Select(x => x.ExhibitId);
            foreach (var exhibit in exhibitList)
            {
                if (selectedCoursesHS.Contains(exhibit.ExhibitId.ToString()))
                {
                    if (exhibit.CollectionId == null || exhibit.CollectionId != authorToupdate.CollectionId)
                    {
                        exhibit.CollectionId = authorToupdate.CollectionId;
                        authorToupdate.Exhibits.Add(exhibit);
                    }
                }
                else
                {
                    if (authExhi.Contains(exhibit.ExhibitId))
                    {
                        Exhibit courseToRemove = _serviceAuth.GetAllListAsyncNonAuthors().FirstOrDefault(i => i.ExhibitId == exhibit.ExhibitId);
                        if (exhibit.CollectionId == authorToupdate.CollectionId)
                        {
                            authorToupdate.Exhibits.Remove(exhibit);
                           // _serviceAuth.DeleteAsync(exhibit);
                        }
                    }
                }
            }
            Thread.Sleep(100);
            return authorToupdate;
        }

        private Collection CreateAuthorExhibits(string[] selectedCourses, Collection authorToupdate)
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
            var exhibitList = _serviceAuth.GetAllListAsyncNonAuthors();
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var author = _service.GetById(authorToupdate.CollectionId);
            var authExhi = author.Exhibits.Select(x => x.ExhibitId);
            foreach (var exhibit in exhibitList)
            {
                if (selectedCoursesHS.Contains(exhibit.ExhibitId.ToString()))
                {
                    exhibit.Collection = authorToupdate;
                    exhibit.CollectionId = authorToupdate.CollectionId;
                    _serviceAuth.UpdateAsync(exhibit);
                }
            }
            return authorToupdate;
        }
        private void PopulateAssignedCourseData(Collection instructor)
        {
            var allCourses = _serviceAuth.GetAllListAsyncNonAuthors();
            var instructorCourses = new HashSet<int>(instructor.Exhibits.Select(c => c.ExhibitId));
            var viewModel = new List<Exhibit>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new Exhibit
                {
                    ExhibitId = course.ExhibitId,
                    Name = course.Name,
                    CollectionId = instructor.CollectionId
                });
            }
            ViewData["Exhibits"] = viewModel;
        }
    }
}
