using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using wa_asp_mvc_file_operation.Data.Models;

namespace wa_asp_mvc_file_operation.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var answer = WS.GetListServerFilesUser(this.HttpContext);

            if (answer != null)
            {
                return View(answer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateFolder(string folderName) 
        {
            if(WS.CreateFolder(folderName, this.HttpContext)) 
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UploadFiles() 
        {
            if (WS.UploadFiles(this.HttpContext)) 
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult RemoveFile(string fileName)
        {
            if (WS.RemoveFile(fileName,this.HttpContext))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult RemoveFolder(string folderName)
        {
            if (WS.RemoveFile(folderName, this.HttpContext))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Rename(string oldName, string fileType, string newName) 
        {
            if (WS.Rename(oldName, fileType, newName, this.HttpContext))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Back()
        {
            var currPath = Session["currPath"].ToString();
            var rootPath = Session["rootPath"].ToString();
            if (WS.IsGoodString(currPath) && WS.IsGoodString(rootPath))
            {
                if (string.Equals(currPath,rootPath, StringComparison.OrdinalIgnoreCase))
                {

                }
                else
                {
                  

                    int index = currPath.LastIndexOf('\\');
                    if (index != -1)
                    {
                        string newPath = currPath.Substring(0, index);

                        Session["currPath"] = newPath;
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult GoToFolder(string folderName)
        {
            if (WS.IsGoodString(folderName))
            {
                string newPath = Session["rootPath"].ToString() + Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(folderName));

                if (Directory.Exists(newPath))
                {
                    Session["currPath"] = newPath;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult MoveFile(string fileName, string fileType, string newPath)
        {
            if (WS.MoveFile(fileName, fileType, newPath, this.HttpContext)) 
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //📢
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult _GetFoldersThreed()
        {
            string basePath = Session["rootPath"].ToString();
            List<string> folderList = null;
            if (WS.IsGoodString(basePath))
            {
               folderList = new List<string> { "\\" };

                if (Directory.Exists(basePath))
                    WS.GetFolderThree(basePath, folderList, this.HttpContext);
                
            }
            return PartialView(folderList);
        }        

        public ActionResult Search(string fileName)
        {
            List<ServerFile> files = null;

            if (WS.IsGoodString(fileName))
            {
                files = new List<ServerFile>();
                string basePath = Session["rootPath"].ToString();
                if (WS.IsGoodString(basePath))
                {
                    WS.FindFile(basePath, files, fileName.ToLower(), this.HttpContext);
                }
            }
            return View(files);
        }       
    }
}