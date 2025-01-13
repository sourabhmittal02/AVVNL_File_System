using System.Collections.Generic;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using AVVNL_File_System;

namespace AVVNL_File_System.Models
{
	public class Users
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int IsActive { get; set; }

		public async Task<ApiResponse> CheckLogin(Users login)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				Users lst = new Users();
				using (SqlConnection conn = new SqlConnection(Utility.Connection))
				{
					lst = conn.Query<Users>("SELECT * FROM Users WHERE username= '" + login.Username + "' AND Password = '" + login.Password + "' and isActive=" + login.IsActive).FirstOrDefault();
					if (lst != null && lst.Username != null)
					{
						apiResponse.UserName = lst.Username;
						apiResponse.Name = lst.Username;
						apiResponse.UserId = lst.Id;
						apiResponse.Status = 200;
						apiResponse.StatusMessage = "Valid User";
					}
					else
					{
						apiResponse.Status = 400;
						apiResponse.StatusMessage = "Invalid Username or Password";
					}
				}
			}
			catch (Exception ex)
			{
				apiResponse.Status = 400;
				apiResponse.StatusMessage = ex.Message;
			}
			return apiResponse;
		}
	}
}
