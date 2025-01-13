using AVVNL_File_System;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AVVNL_File_System.Models
{
    public class Data
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Desp { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }

        public async Task<ApiResponse> AddEditDoc(Data data)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                using (SqlConnection conn = new SqlConnection(Utility.Connection))
                {
                    int res = conn.Execute("insert into Data(Header,Desp,FileName,FileType) values (@Header,@Desp,@FileName,@FileType)", data);
                    if (res == 1)
                    {
                        apiResponse.Status = 200;
                        apiResponse.StatusMessage = "OK";
                    }
                    else
                    {
                        apiResponse.Status = 400;
                        apiResponse.StatusMessage = "Error";
                    }
                }
            }
            catch(Exception ex)
            {
                apiResponse.Status = 400;
                apiResponse.StatusMessage = ex.Message;
            }
            return apiResponse;
        }

        public List<Data> GetList()
        {
            List<Data> list = new List<Data>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Utility.Connection))
                {
                    list = conn.Query<Data>("select * from Data").ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public ApiResponse DeleteDoc(int Id,string path)
        {
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				using (SqlConnection conn = new SqlConnection(Utility.Connection))
				{
                    Data data = conn.Query<Data>("select * from Data where id=" + Id).First();
                    string _path = Path.Combine(path, data.FileName);
					System.IO.File.Delete(_path);
                    apiResponse.Status = conn.Execute("delete from Data where id=" + Id);
				}
			}
			catch (Exception ex)
			{
				apiResponse.Status = 0;
				apiResponse.StatusMessage = ex.Message;
			}
			return apiResponse;
		}
	}
}
