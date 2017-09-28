/*
 * Nhóm: Huỳnh Ngọc Thanh Xuân - Trịnh Văn Công
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace ContosoUniversity.Controllers
{
	public class TestController : Controller
	{
		// GET: Tét
		public ActionResult Index()//Action này trả về form để điền các thông tin (IP Address, Username, password) để kết nối đến CSDL
		{
			return View();
		}
		[HttpPost]//Khi nhấn submit ==> Đẩy dữ liệu lên server để tạo connectionString và kết nối tới cơ sở dữ liệu (khác với HttpGet là lấy dữ liệu từ server về)
		public ActionResult Show(string ip, string usr, string pwd) //Phương thức này show ra những database đã được lấy ra khi kết nối server thành công
		{
			//Lưu các thông tin để khi xuống View dùng lại bằng cách sử dụng ViewBag
			ViewBag.ip = ip;
			ViewBag.usr = usr;
			ViewBag.pwd = pwd;
			List<string> list = new List<string>();
			//Truyền các tham số lấy được từ form vào trong file Web.config theo thứ tự nhất định với db là giá trị của name trong thẻ connectionString 
			string connectionString = string.Format(ConfigurationManager.ConnectionStrings["db"].ConnectionString, ip, usr, pwd);
			using (SqlConnection cn = new SqlConnection(connectionString))//Tạo một đối tượng SqlConnection với connectionString
			{
				//Câu query này sẽ lấy ra tên của các database trong server
				using (SqlCommand cmd = new SqlCommand("SELECT name FROM master.dbo.sysdatabases", cn))
				{
					cn.Open();//mở kết nối tới server
					using (SqlDataReader reader = cmd.ExecuteReader())//Tạo đối tượng SqlDataReader để lấy ra những dòng trong datasource
					{
						while (reader.Read())//trả về true nếu vẫn còn dòng tiếp theo trong datasource, ngược lại trả về false 
						{
							list.Add((string)reader["name"]);//lấy dữ liệu được định dạng trong cột tên là name (vì lúc trước chúng ta select name)
						}
					}
				}
			}
			return View(list);//Trả về view để hiển thị các database trên browser với dữ liệu truyền vào biến list kiểu List
		}
		public ActionResult detail(string ip, string usr, string pwd, string dbName)//Phương thức này show ra những table với database tương ứng được click vào
		{
			List<string> list = new List<string>();
			//Truyền các tham số lấy được từ form vào trong file Web.config theo thứ tự nhất định với test là giá trị của name trong thẻ connectionString 
			string connectionString = string.Format(ConfigurationManager.ConnectionStrings["test"].ConnectionString, ip, dbName, usr, pwd);
			using (SqlConnection cnn = new SqlConnection(connectionString))
			{
				//Câu truy vấn này sẽ lấy ra tên bảng của database được truyền vào (biến dbName trên tham số lưu giữ tên của database khi nhấn vào trên web)
				using (SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + dbName + "'", cnn))
				{
					cnn.Open();
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							list.Add((string)reader["TABLE_NAME"]);//Tương tự ở trên
						}
					}
				}
			}
			return View(list);
		}
	}
}