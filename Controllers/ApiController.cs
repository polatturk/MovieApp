using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using Api.Models;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApiController : Controller
    {
		string connectionString = "";
		public IActionResult Index(MovieModel model)
		{
			using var connection = new SqlConnection(connectionString);
			var movies = connection.Query<MovieModel>("SELECT * FROM Movies").ToList();

			return Json(movies);
		}

		[HttpPost]
		public IActionResult Add(MovieModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new
				{
					msg = "Eksik veya hatalı bilgi girişi yaptın"
				});
			}

			using var connection = new SqlConnection(connectionString);
			var newRecordId = connection.ExecuteScalar<int>("Insert Into Movies (Name, Summary, CategoryId, Imdb, Actors, Director, CommentId, Year, Type, Image) values (@Name, @Summary, @CategoryId, @Imdb, @Actors, @Director, @CommentId, @Year, @Type, @Image) SELECT SCOPE_IDENTITY()", model);

			model.Id = newRecordId;
			return Ok(new { msg = "Film Eklendi." });

		}
		[HttpDelete]
		public IActionResult Delete(int id)
		{
			using var connection = new SqlConnection(connectionString);
			var sql = "DELETE FROM Movies WHERE Id = @Id";
			var rowsAffected = connection.Execute(sql, new { Id = id });

			return Ok(new { msg = "Ürün silindi." });
		}

		[HttpPost]
		public IActionResult Edit(MovieModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new
				{
					msg = "Eksik veya hatalı bilgi girişi yaptın"
				});

			}
			using var connection = new SqlConnection(connectionString);

			var sql = "Update Movies Set Name = @Name, Summary = @Summary, CategoryId = @CategoryId, Imdb = @Imdb, Actors = @Actor, Director = @Director, CommentId = @CommentId, Year = @Year, Type = @Type, Image = @Image Where Id = @Id";

			var movies = new
			{
				Id = model.Id,
				Name = model.Name,
				Summary = model.Summary,
				CategoryId = model.CategoryId,
				Imdb = model.Imdb,
				Actors = model.Actors,
				Director = model.Director,
				CommentId = model.CommentId,
				Year = model.Year,
				Type = model.Type,
				Image = model.Image

			};

			var affectedRow = connection.Execute(sql, movies);

			return Ok(new { msg = "Film bilgileri Güncellendi." });
		}
	}
}
