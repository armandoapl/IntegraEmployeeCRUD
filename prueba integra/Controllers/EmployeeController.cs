using Microsoft.AspNetCore.Mvc;
using prueba_integra.Data;
using prueba_integra.Models;




using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;

namespace prueba_integra.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DataContext _db;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(DataContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult List(string? searchText)
        {
            List<Employee> employeesList;

            if (!String.IsNullOrEmpty(searchText))
            {
                employeesList = _db.Employees.Where(emp => emp.Name.Contains(searchText)).ToList();
            }
            else
            {
                employeesList = _db.Employees.ToList();
                
            }

            return View(employeesList);
        }

        // esta sera la accion que retorne la vista para crear un  nuevo registro de empleado
        //GET: espesificamente esta accion es un metodo GET ya que renderizara el formulario vacio 
        //pero con un modelo de tipo Employee para bindearlo (enlazarlo) al formulario
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employeeModel)
        {
            if (ModelState.IsValid)
            {
                string FileName = "";
                if (employeeModel.PhotoIformFile != null)
                {
                    FileName = UploadFile(employeeModel.PhotoIformFile);
                }

                var employee = new Employee
                {
                    Lastname = employeeModel.Lastname,
                    Name = employeeModel.Name,
                    PhoneNumber = employeeModel.PhoneNumber,
                    Mail = employeeModel.Mail,
                    PhotoFileName = FileName == "" ? null : FileName,
                    ContractDate = employeeModel.ContractDate
                };

                _db.Employees.Add(employee);
                _db.SaveChanges();
                TempData["success"] = "Empleado Creado exitosamente";
                return RedirectToAction("List");
            }

            return View(employeeModel);

        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            var employeeFromDb = _db.Employees.Find(id);

            if(employeeFromDb == null)
            {
                return NotFound();
            }


            return View(employeeFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                string FileName = UploadFile(employee.PhotoIformFile);
                employee.PhotoFileName = FileName;


                _db.Employees.Update(employee);
                _db.SaveChanges();
                TempData["success"] = "Empleado Editado exitosamente";
                return RedirectToAction("List");
            }

            return View(employee);

        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var employeeFromDb = _db.Employees.Find(id);

            if (employeeFromDb == null)
            {
                return NotFound();
            }


            return View(employeeFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {   
            var employeeToDelete = _db.Employees.Find(id);
            if(employeeToDelete == null)
            {
                return NotFound();
            }
            _db.Employees.Remove(employeeToDelete);
            _db.SaveChanges();
            TempData["success"] = "Empleado Eliminado exitosamente";
            return RedirectToAction("List");

        }


        // Para crear el PDF utilice la libreria de syncfusion para creacion de PDF y me guie de su documentacion
        public FileStreamResult CreateDocument(
            string employeeName, 
            string employeeLastname,
            string phoneNumber,
            string mail,
            string contractDate
            )
        {
            PdfDocument document = new PdfDocument();

            //Add a page to the document
            PdfPage page = document.Pages.Add();

            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;

            //Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

            string text = $@"
                            Ficha de empleado:
            -----------------------------------------------------

            Nombre: {employeeName}              Apellido: {employeeLastname}

            Tel: {phoneNumber}                  Correo Electronico: {mail}

            Fecha de contratacion: {contractDate}";
            

            graphics.DrawString(text, font, PdfBrushes.Purple, new PointF(0, 0));
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Set the position as '0'.
            stream.Position = 0;

            //Download the PDF document in the browser
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

            fileStreamResult.FileDownloadName = "Sample.pdf";

            return fileStreamResult;
        }



        private string UploadFile(IFormFile photoFormFile)
        {
            string fileName = null;
            if(photoFormFile != null)
            {
                var uploadAddress = Path.Combine(_webHostEnvironment.WebRootPath, "Photos");
                fileName = Guid.NewGuid().ToString() + "-" + photoFormFile.FileName;
                var filePath = Path.Combine(uploadAddress, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photoFormFile.CopyTo(stream);
                }
            }
            return fileName;
        }

        private IFormFile GetFile(string fileName)
        {
            var uploadAddress = Path.Combine(_webHostEnvironment.WebRootPath, "Photos");
            var filePath = Path.Combine(uploadAddress, fileName);
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                return file;
            }

        }

    }
}
