using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wa_asp_mvc_file_operation.Data.Models;
using wa_asp_mvc_file_operation.Data.Repository;

namespace wa_asp_mvc_file_operation.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CloudUser cloudUser) 
        {
            if (cloudUser.IsGoodInfo()) 
            {
                try
                {
                    using(var context = new StorageBoxContext()) 
                    {
                        using(var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                cloudUser.SetPersonalFolder();
                                context.Users.Add(cloudUser);
                                context.SaveChanges();
                                transaction.Commit();
                                
                            }
                            catch
                            {

                                transaction.Rollback();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex);
                }
            }

            return RedirectToAction("Index", "Authorization");
        }
    }

}