using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Helpers
{
	public class Helper : IHelper
	{
		private readonly IConfiguration _configuration;

		public Helper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Hash(string input)
		{
			using HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(""));
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = hmac.ComputeHash(inputBytes);

			var sb = new StringBuilder();
			foreach (var b in hashBytes)
			{
				sb.Append(b.ToString("X2"));
			}

			return sb.ToString();
		}

		public SqlConnection ConnectToSql()
		{
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			var connection = new SqlConnection(connectionString);
			connection.Open();

			return connection;
		}
	}
}
