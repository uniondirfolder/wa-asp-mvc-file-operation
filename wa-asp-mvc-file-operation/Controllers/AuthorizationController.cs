using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wa_asp_mvc_file_operation.Data.Models;
using wa_asp_mvc_file_operation.Data.Repository;

namespace wa_asp_mvc_file_operation.Controllers
{
    public class AuthorizationController : Controller
    {
        // GET: Authorization
        [HttpGet]
        public ActionResult Index()
        {
            if (bool.Parse(Session["authorized"]?.ToString() ?? "false"))
                return RedirectToAction("Index", "Home");
            else
                return View();
        }

        

        [HttpPost]
        public ActionResult Index(CloudUser user)
        {
            if (user!=null && WS.IsGoodString(user.Login) && WS.IsGoodString(user.Passwords))
            {
                try
                {
                    using (var context = new StorageBoxContext())
                    {
                        CloudUser cloudUser = context.Users.FirstOrDefault(
                            x => x.Login.ToLower().Equals(user.Login.ToLower()) 
                            && x.Passwords.Equals(user.Passwords));
                        if (cloudUser != null)
                        {
                            if (WS.IsGoodString(cloudUser.FolderName))
                            {
                                string basePath = Server.MapPath("~/Upload/PrivateRepository");
                                if (Directory.Exists(basePath))
                                {
                                    string path = Path.Combine(basePath, cloudUser.FolderName);
                                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                                    Session["currPuth"] = path;
                                    Session["rootPuth"] = path;
                                    Session["authorized"] = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignOut() 
        {
            Session["currPuth"] = Server.MapPath("~/Upload/PublicRepository");
            Session["rootPuth"] = Server.MapPath("~/Upload/PublicRepository");
            Session["authorized"] = false;

            return RedirectToAction("Index", "Home");
        }
    }
}