using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TEDALS_Ver01.DAL;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.Controllers
{
    public class UserRightsController : Controller
    {
        private TedalsContext db = new TedalsContext();
        public UserRight UserRight = new UserRight();

        // GET: UserRights
        public ActionResult Index()
        {
            try
            {


                return View(db.UserRight.ToList());
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }

              
        }

        // GET: UserRights/Details/5
        public ActionResult Details(int? id)
        {
            try
            {


                if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
                {


                    if (id == null)
                    {
                        ViewBag.Error = "A null parameter was passed to the function";
                        return View("Error");
                    }
                    UserRight userRight = db.UserRight.Find(id);
                    if (userRight == null)
                    {
                        ViewBag.Error = "The requested User account does not exist";
                        return View("Error");
                    }
                    return View(userRight);
                }
                else
                    return View("AuthorizationError");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: UserRights/Create
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            try
            {


                if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
                {
                    return View();
                }
                return View("AuthorizationError");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        public ActionResult ViewReaders()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                return View(db.UserRight.ToList());
            }
            return View("AuthorizationError");
        }
        public ActionResult ViewExporters()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                return View(db.UserRight.ToList());
            }
            return View("AuthorizationError");
        }
        public ActionResult ViewEditors()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                return View(db.UserRight.ToList());
            }
            return View("AuthorizationError");
        }
        public ActionResult ViewAdmins()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                return View(db.UserRight.ToList());
            }
            return View("AuthorizationError");
        }

        // POST: UserRights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserRightID,UserName,UserCode,IsReader,IsEditor,IsExporter,IsAdmin,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] UserRight userRight)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.UserRight.Any(x => x.UserCode.Equals(userRight.UserCode)))
                    {
                        ModelState.AddModelError("UserCode", "User Code already exists");
                        return View(userRight);
                    }
                    userRight.CreatedBy = User.Identity.Name;
                    userRight.CreatedOn = DateTime.Now;
                    userRight.ModifiedBy = User.Identity.Name;
                    userRight.ModifiedOn = DateTime.Now;

                    db.UserRight.Add(userRight);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(userRight);
            }
            catch(Exception  e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: UserRights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            UserRight userRight = db.UserRight.Find(id);
            if (userRight == null)
            {
                ViewBag.Error = "The requested user account does not exist";
                return View("Error");
            }
            return View(userRight);
        }

        // POST: UserRights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserRightID,UserName,UserCode,IsReader,IsEditor,IsExporter,IsAdmin,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] UserRight userRight)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.UserRight.Any(x => x.UserCode.Equals(userRight.UserCode) && x.UserRightID != userRight.UserRightID))
                    {
                        ModelState.AddModelError("UserCode", "User Code already exists");
                        return View(userRight);
                    }
                    userRight.ModifiedBy = User.Identity.Name;
                    userRight.ModifiedOn = DateTime.Now;
                    db.Entry(userRight).State = EntityState.Modified;
                    db.Entry(userRight).Property(x => x.CreatedBy).IsModified = false;
                    db.Entry(userRight).Property(x => x.CreatedOn).IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(userRight);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: UserRights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            UserRight userRight = db.UserRight.Find(id);
            if (userRight == null)
            {
                ViewBag.Error = "The requested User account does not exist";
                return View("Error");
            }
            return View(userRight);
        }

        // POST: UserRights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                UserRight userRight = db.UserRight.Find(id);
                db.UserRight.Remove(userRight);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
