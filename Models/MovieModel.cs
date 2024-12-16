using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class MovieModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public int CategoryId { get; set; }
    public int Imdb { get; set; }
    public string Actors { get; set; }
    public string Director { get; set; }
    public int CommentId { get; set; }
    public int Year { get; set; }
    public string Type { get; set; }
    public string Image { get; set; }
    public string? ImgUrl { get; set; }

    public string UserName {  get; set; }
    public MovieModel? Movie { get; set; }
    public List<CommentModel> Comments { get; set; }
    public IFormFile? ImgFile { get; set; }
    public string? CategoryName { get; set; }
    public string AdminComment { get; set; }
}
public class MoviesViewModel
{
	public List<MovieModel> CarouselMovies { get; set; }
	public List<MovieModel> BannerMovies { get; set; }
	public List<MovieModel> FeatureMovies { get; set; }
	public List<MovieModel> TrendingMovies { get; set; }
}

public class CommentModel
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public DateTime Created { get; set; }
    public int UserId { get; set; }
    public int MoviesId { get; set; }

    public string UserName { get; set; }
}

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}

public class Register
{
	public int Id { get; set; }
	[Required]
	public string UserName { get; set; }
	[Required]
	public string Password { get; set; }
	[Required]
	public string Pwconfirmend { get; set; }
	[Required]
	public string Mail { get; set; }
	public DateTime Created { get; set; }
}


public class YorumViewModel
{
    public MovieModel Movie { get; set; }
    public List<CommentModel> Comments { get; set; }
}