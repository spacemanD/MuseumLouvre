using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
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
    public class ExhibitController : Controller
    {
        private readonly IExhibitService _service;

        private readonly IAuthorService _serviceAuth;

        private readonly ICollectionService _serviceColl;

        private static List<PopExhibit> _exhibits;

        public ExhibitController(IExhibitService service, IAuthorService authorService, ICollectionService collectionService)
        { 
            _service = service;
            _serviceAuth = authorService;
            _serviceColl = collectionService;
        }

        public IActionResult PopularExhibits(string sortOrder ,string category)
        {
            ViewBag.CurrentFilter = category;
            ViewBag.RateSortParm = String.IsNullOrEmpty(sortOrder) ? "rate_desc" : "";


            var categoryQuery = Enum.GetValues(typeof(CountryList)).Cast<CountryList>();

            var exhibits = new List<PopExhibit>();
            ViewBag.Category = new SelectList(categoryQuery);

            switch (sortOrder)
            {
                case "rate_desc":
                    exhibits = _service.GetLast10PopularExhibits().ToList();
                    break;
                default:
                    exhibits = _service.GetTop10PopularExhibits().ToList();
                    break;
            }

            if (category != null)
            {
                exhibits = exhibits.Where(i => i.Exhibit.Country.ToString() == category).ToList();
            }

            _exhibits = exhibits;
            return View(exhibits);
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

        public async Task<IActionResult> ProsseccFile()
        {
            await _service.ProccessFile();

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
            foreach (var item in _exhibits)
            {
                data.Add(new
                {
                    Rate = item.Rate,
                    ExhibitId = item.ExhibitId,
                    Name = item.Exhibit.Name,
                    Description = item.Exhibit.Description,
                    Country = item.Exhibit.Country,
                    CreationYear = item.Exhibit.CreationYear,
                    ArtDirection = item.Exhibit.Direction,
                    Cost = item.Exhibit.Cost,
                    Author = item.Exhibit.Author.Name
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
            PopulateDepartmentsDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AuthorId,CollectionId,CreationYear,Description,Type,Cost,Direction,Materials,Country")] Exhibit exhibit)
        {
            PopulateDepartmentsDropDownList(exhibit.AuthorId, exhibit.CollectionId);
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
            PopulateDepartmentsDropDownList(exhibit.AuthorId, exhibit.CollectionId);
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

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null, object selectedCollection = null)
        {
            var authorList = _serviceAuth.GetAllListAsync();
            var collectionList = _serviceColl.GetAllListAsync();
            ViewBag.AuthorId = new SelectList(authorList.ToList(), "AuthorId", "Name", selectedDepartment);
            ViewBag.CollectionId = new SelectList(collectionList.ToList(), "CollectionId", "Name", selectedCollection);
        }
    }
}
