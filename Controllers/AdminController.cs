using Api.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Helpers;
using Business;

namespace Api.Controllers
{
	public class AdminController : Controller
    {
		public readonly IHelper _helper;
		public readonly IAdminService _adminService;
		public AdminController(IHelper helper,IAdminService adminService)
		{
			_helper = helper;
            _adminService = adminService;
		}
		public async Task<IActionResult> Index()
		{
			var movieList = await _adminService.GetAllMovieForAdminDashboard();

			return View(movieList);
		}

        public IActionResult AddMovie()
        {
            using var connection = _helper.ConnectToSql();

            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(MovieModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Ürün eklenirken bir hata oluştu";
                ViewBag.MessageCssClass = "alert-danger";
                return View("Message");
            }

            var ImageName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImgFile.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", ImageName);

            using var stream = new FileStream(path, FileMode.Create);
            model.ImgFile.CopyTo(stream);

            model.Image = ImageName;
            //admin movie ekliyor
            using var connection = _helper.ConnectToSql();
            var sql = "INSERT INTO Movies (Name, Summary, CategoryId, Imdb, Actors, Director, CommentId, Year, Type, Image) VALUES (@Name, @Summary, @CategoryId, @Imdb, @Actors, @Director, @CommentId, @Year, @Type, @Image)";


            var data = new
            {
                Name = model.Name,
                Summary = model.Summary,
                CategoryId = model.CategoryId,
                Imdb = model.Imdb,
                Actors = model.Actors,
                Director = model.Director,
                CommentId = model.CommentId,
                Year = model.Year,
                Type = model.Type,
                Image = model.Image,
            };

            var rowsAffected = connection.Execute(sql, data);
            ViewBag.Message = "Ürün eklendi.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _adminService.DeleteMovie(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _adminService.GetMovie(id);
            return View(movie);
        }

        [HttpPut]
        public IActionResult Edit(MovieModel model)
        {
            using var connection = _helper.ConnectToSql();

            if (model.ImgFile != null && model.ImgFile.Length > 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImgFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", imageName);

                using var stream = new FileStream(path, FileMode.Create);
                model.ImgFile.CopyTo(stream);

                model.Image = imageName;
            }
            //image ekleme
            var sql = "UPDATE Movies SET Name = @Name, Summary = @Summary, CategoryId = @CategoryId, Imdb = @Imdb, Actors = @Actors, Director = @Director, Year = @Year, Type = @Type";

            if (model.ImgFile != null && model.ImgFile.Length > 0)
            {
                sql += ", Image = @Image";
            }

            sql += " WHERE Id = @Id";

            var param = new
            {
                Id = model.Id,
                Name = model.Name,
                Summary = model.Summary,
                CategoryId = model.CategoryId,
                Imdb = model.Imdb,
                Actors = model.Actors,
                Director = model.Director,
                Year = model.Year,
                Type = model.Type,
                Image = model.Image
            };

            var affectedRows = connection.Execute(sql, param);

            TempData["Message"] = "Film başarıyla güncellendi.";

            return RedirectToAction("Message", "Home");
        }

        public IActionResult Message()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

    }
}
