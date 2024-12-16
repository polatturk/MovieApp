using Microsoft.AspNetCore.Http;

namespace Model
{
	public class MovieModelResponse
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

		public string UserName { get; set; }
		public List<CommentModelResponse> Comments { get; set; }
		public IFormFile? ImgFile { get; set; }
		public string? CategoryName { get; set; }
		public string AdminComment { get; set; }
	}

	public class CommentModelResponse
	{
		public int Id { get; set; }
		public string Comment { get; set; }
		public DateTime Created { get; set; }
		public int UserId { get; set; }
		public int MoviesId { get; set; }

		public string UserName { get; set; }
	}

	public class CategoryModelResponse
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class UserModelResponse
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Mail { get; set; }
		public string Password { get; set; }
		public string UserName { get; set; }
	}
}
