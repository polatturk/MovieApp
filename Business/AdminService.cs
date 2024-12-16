using Dapper;
using Helpers;
using Model;
using System.Data.SqlClient;

namespace Business;

public class AdminService : IAdminService
{
	private readonly IHelper _helper;
	private readonly SqlConnection _sqlConnection;

	public AdminService(IHelper helper, SqlConnection sqlConnection)
	{
		_helper = helper;
		_sqlConnection = sqlConnection;
	}

	public async Task<string> DeleteMovie(int id)
	{
		using var connection = _helper.ConnectToSql();
		var sql = "DELETE FROM Movies WHERE Id = @Id";
		var rowsAffected = connection.Execute(sql, new { Id = id });
		return "Success";
	}

	public async Task<List<MovieModelResponse>> GetAllMovieForAdminDashboard()
	{
		var query = @"SELECT m.*, c.Name AS CategoryName 
                FROM Movies m 
                INNER JOIN Categories c ON m.CategoryId = c.Id";

		var movieList = _sqlConnection.Query<MovieModelResponse>(query).ToList();
		return movieList;
	}

	public async Task<MovieModelResponse> GetMovie(int id)
	{
		using var connection = _helper.ConnectToSql();
		var movies = connection.QuerySingleOrDefault<MovieModelResponse>("SELECT * FROM Movies WHERE Id = @Id", new { Id = id });
		return movies;
	}
}
