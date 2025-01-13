using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AVVNL_File_System.Models;

namespace AVVNL_File_System.Controllers
{
    public class HomeController : Controller
    {
        string connString;
        Users _login = new Users();
        Data _doc = new Data();
        private readonly string fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Index(string username, string password)
        {
            ApiResponse apiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(username))
            {
                ClaimsIdentity identity = null;
                bool isAuthenticated = false;
                Users users = new Users();
                users.Username = username;
                users.Password = password;
                users.IsActive = 1;
                apiResponse = await _login.CheckLogin(users);
                ViewBag.Err = apiResponse.StatusMessage;
                if (apiResponse.Status == 200)
                {
                    Session["User"] = apiResponse.UserName;
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.Err = apiResponse.StatusMessage;
                    return View();
                }
            }
            else
            {
                ViewBag.Err = apiResponse.StatusMessage;
            }
            return View();
        }
        public ActionResult Dashboard()
        {
            List<Data> list = new List<Data>();
            list = _doc.GetList();
            ViewBag.Path = fileDirectory;
            return View(list);
        }

        public ActionResult Upload()
        {
            List<Data> list = new List<Data>();
            list = _doc.GetList();
            return View(list);
        }
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo, string header, string desp)
        {
            ApiResponse res = new ApiResponse();
            Data data = new Data();
            var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            // Define allowed file types
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".mp4", ".xls", ".xlsx", ".jpeg", ".png", ".jpg" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                ViewBag.Err = "Invalid file type. Only image and PDF/doc/mp4 files are allowed.";
                return View();
            }
            string _FileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            string _path = Path.Combine(Server.MapPath("~/Files"), _FileName);
            photo.SaveAs(_path);
            data.FileName = _FileName;
            data.Header = header;
            data.Desp = desp;
            data.FileType = fileExtension;
            res = await _doc.AddEditDoc(data);
            if (res.Status != 200)
            {
                ViewBag.Err = res.StatusMessage;
            }
            return RedirectToAction("Upload");
        }

        public async Task<JsonResult> Delete(int id)
        {
            string _path = Server.MapPath("~/Files");
            ApiResponse api = new ApiResponse();
            try
            {
                api = _doc.DeleteDoc(id,_path);
            }
            catch
            {
                api.Status = 0;
            }
            return Json(new { data = api}, JsonRequestBehavior.AllowGet);
        }
    }
}