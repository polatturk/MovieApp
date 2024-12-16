using System.Data.SqlClient;
namespace Helpers;

public interface IHelper
{
	SqlConnection ConnectToSql();
	string Hash(string input);

}
