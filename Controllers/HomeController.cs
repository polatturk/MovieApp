using Dapper;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Helpers;

namespace Api.Controllers;

public class HomeController : Controller
{
	public readonly IHelper _helper;
    public HomeController(IHelper helper)
    {
		_helper = helper;	
    }

    public IActionResult Index()
	{
		//tüm filmleri çekme
		using (var connection = _helper.ConnectToSql())
		{
			ViewData["UserName"] = HttpContext.Session.GetString("UserName");


			var carouselMovies = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Type = 'CarouselMovies'").ToList();

			var bannerMovies = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Type = 'BannerMovies'").ToList();

			var featureMovies = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Type = 'FeatureMovies'").ToList();

			var trendingMovies = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Type = 'TrendingMovies'").ToList();

			var viewModel = new MoviesViewModel
			{
				CarouselMovies = carouselMovies,
				BannerMovies = bannerMovies,
				FeatureMovies = featureMovies,
				TrendingMovies = trendingMovies
			};

			return View(viewModel);
		}
	}

    //  public IActionResult BlogDetail(int Id)
    //  {
    //      ViewData["UserName"] = HttpContext.Session.GetString("UserName");
    //      ViewData["Title"] = "Film Detay";
    //if (Id == null)
    //{
    //	ViewBag.Message2 = "Böyle bir film bulunamadý";
    //	ViewBag.MessageCssClass = "alert-danger";
    //	return View("Message2");
    //}

    ////ViewBag.Yorum = true;
    ////if (!CheckLogin())
    ////{
    ////	ViewBag.Yorum = false;
    ////}

    //using var connection = new SqlConnection(connectionString);
    //var sql = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Id = @Id", new { Id = Id }).ToList().FirstOrDefault();        

    //      return View(sql);

    //  }




    public IActionResult BlogDetail(int Id)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        ViewData["Title"] = "Film Detay";

        if (Id == null)
        {
            ViewBag.Message2 = "Böyle bir film bulunamadý";
            ViewBag.MessageCssClass = "alert-danger";
            return View("Message2");
        }

        using var connection = _helper.ConnectToSql();

        // Fetch movie details
        var movie = connection.Query<MovieModel>("SELECT * FROM Movies WHERE Id = @Id", new { Id = Id }).FirstOrDefault();

        // If no movie found, return error
        if (movie == null)
        {
            ViewBag.Message2 = "Böyle bir film bulunamadý";
            ViewBag.MessageCssClass = "alert-danger";
            return View("Message2");
        }

        var comments = connection.Query<CommentModel>("SELECT * FROM Comments WHERE MoviesId = @Id", new { Id = Id }).ToList();

        var detailComment = new MovieModel 
        {
            Movie = movie,
            Comments = comments
        };

        return View(detailComment);
    }




    [HttpPost]
    [Route("/YorumEkle")]
    public IActionResult YorumEkle(CommentModel model)
    {
		//yorum oluþturma
        model.Created = DateTime.Now;

        using var connection = _helper.ConnectToSql();
		var sql = "INSERT INTO comments (Comment, Created, UserId, MoviesId) VALUES (@Comment, @Created, @UserId, @MoviesId)";
        var comment = "SELECT * FROM comments ";

        var data = new {
			Comment = model.Comment,
			Created = DateTime.Now,
			UserId = model.UserId,
			MoviesId = model.Id
		};

		connection.Execute(sql, data);

		return RedirectToAction("Index");
    }

    [Route("/YorumSil/{id}")]
    public IActionResult CommentDel(int id)
    {
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        return View();
    }

    public IActionResult Team()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        return View();
    }
    public IActionResult Detail()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        return View();
    }

	public IActionResult AdminReviews()
	{
		return View();
	}

	public bool CheckLogin()
	{
		if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
		{
			return false;
		}

		return true;
	}

	public int? KullaniciGetir(string UserName)
	{
		//get user
		using var connection = _helper.ConnectToSql();
		var sql = "SELECT Id FROM users WHERE UserName = @UserName";
		var userId = connection.QueryFirstOrDefault<int?>(sql, new { UserName = UserName });
		return userId;
	}

	[HttpGet]
	public IActionResult Login()
	{
        return View();
	}

	[HttpGet]
	public IActionResult Register(Register? model)
	{
		if (model == null)
		{
			model = new Register();
		}

		return View(model);
	}

	[HttpPost]
	[Route("/Login")]
	public IActionResult Login(UserModel model)
	{
		//Login
		model.Password = _helper.Hash(model.Password);
		using var connection = _helper.ConnectToSql();
		var sql = "SELECT * FROM Users WHERE UserName = @UserName AND Password = @Password";
		var user = connection.QueryFirstOrDefault<UserModel>(sql, new { model.UserName, model.Password });

		if (user != null)
		{
			HttpContext.Session.SetInt32("userId", user.Id);
			HttpContext.Session.SetString("UserName", user.UserName);
			ViewData["UserName"] = HttpContext.Session.GetString("UserName");

			ViewBag.Message = "login Baþarýlý";
			return View("Message");
		}

		TempData["AuthError"] = "Kullanýcý adý veya þifre hatalý";
		return View("Login");
	}

	[HttpPost]
	[Route("/KayitOl")]
	public IActionResult KayitOl(Register model)
	{
		//register
		if (!ModelState.IsValid)
		{
			TempData["AuthError"] = "Form eksik veya hatalý.";
			return View("Register");
		}

		if (model.Password != model.Pwconfirmend)
		{
			TempData["AuthError"] = "Þifreler Uyuþmuyor.";
			return View("Register", model);
		}

		using (var control = _helper.ConnectToSql())
		{
			var cntrl = "SELECT * FROM Users WHERE UserName = @UserName";
			var user = control.QueryFirstOrDefault(cntrl, new { model.UserName });
			if (user != null)
			{
				TempData["AuthError"] = "Bu kullanýcý adý mevcut!.";
				return View("Register", model);
			}
		}

		model.Created = DateTime.Now;
		model.Password = _helper.Hash(model.Password);
		using var connection = _helper.ConnectToSql();
		var sql =
            "INSERT INTO Users (Mail, Password, UserName) VALUES (@Mail, @Password, @UserName)";
		var data = new
		{
			model.UserName,
			model.Password,
			model.Mail,
		};

		var rowAffected = connection.Execute(sql, data);

		ViewBag.Message = "Kayýt Baþarýlý";
		 
		return View("Message");
	}

	public IActionResult Cikis()
	{
		HttpContext.Session.Clear();
		return RedirectToAction("Index");
	}
}
