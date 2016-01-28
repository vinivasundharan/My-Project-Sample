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
    public class LsystemsController : Controller
    {
        private TedalsContext db = new TedalsContext();


        public class IdRequiredAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                object parameter = null;
                filterContext.ActionParameters.TryGetValue("id", out parameter);
                var id = parameter as int?;

                if (id == null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "Lsystems");

                    filterContext.Result = new RedirectResult(url);
                }
            }
        }

        public ActionResult Error()
        {
            ViewBag.Error = "An unhandled Exception occured with the processing of your request";
            return View("Error");
        }
        // GET: Lsystems
        public ActionResult Index()
        {
            try
            {


                var lsystem = db.Lsystem.Include(l => l.LsystemFamily).Where(x => x.LsystemFamilyID != 62);

                lsystem = lsystem.OrderBy(x => x.LsystemFamilyID);

                return View(lsystem.ToList());
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }


        }

        // GET: Lsystems/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            Lsystem lsystem = db.Lsystem.Find(id);
            if (lsystem == null)
            {
                ViewBag.Error = "The requested System does not exist";
                return View("Error");
            }
            return View(lsystem);
        }

        // GET: Lsystems/Create
        
        public ActionResult Create(int id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {


                try
                {


                    var model = new Lsystem
                    {
                        LsystemFamilyID = id
                    };
                    var fam = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == id);
                    ViewBag.FamilyName = fam.FamilyName;
                    return View(model);
                }
                catch(Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View("Error");
                }
            }
            else
                return View("AuthorizationError");
        }

        // POST: Lsystems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LsystemID,LsystemName,MaterialNumber,DescriptionEN,DescriptionDE,LsystemFamilyID")] Lsystem lsystem)
        {
            try
            {


                var fam = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == lsystem.LsystemFamilyID);
                ViewBag.FamilyName = fam.FamilyName;
                if (ModelState.IsValid)
                {
                    if (db.Lsystem.Any(x => x.LsystemName.Equals(lsystem.LsystemName) && x.LsystemFamilyID == lsystem.LsystemFamilyID))
                    {
                        ModelState.AddModelError("LsystemName", "System Name already exists");
                        ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily.OrderBy(x=>x.FamilyName), "LsystemFamilyID", "FamilyName", lsystem.LsystemFamilyID);
                        return View(lsystem);
                    }
                    lsystem.CreatedOn = DateTime.Now;
                    lsystem.CreatedBy = User.Identity.Name;
                    lsystem.ModifiedOn = DateTime.Now;
                    lsystem.ModifiedBy = User.Identity.Name;
                    db.Lsystem.Add(lsystem);
                    db.SaveChanges();
                    return RedirectToAction("ViewSystems", "LsystemFamilies", new { id = lsystem.LsystemFamilyID });
                }

                ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily.OrderBy(x=>x.FamilyName), "LsystemFamilyID", "FamilyName", lsystem.LsystemFamilyID);
                return View(lsystem);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Lsystems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                Lsystem lsystem = db.Lsystem.Find(id);

                if (lsystem == null)
                {
                    ViewBag.Error = "The requested System does not exist";
                    return View("Error");
                }
                ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily.OrderBy(x=>x.FamilyName), "LsystemFamilyID", "FamilyName", lsystem.LsystemFamilyID);
                return View(lsystem);
            }
            else
                return View("AuthorizationError");
        }

        // POST: Lsystems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LsystemID,LsystemName,MaterialNumber,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy,DescriptionEN,DescriptionDE,LsystemFamilyID")] Lsystem lsystem)
        {
            try
            {


                //var fam = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == lsystem.LsystemFamilyID);
                //ViewBag.FamilyName = fam.FamilyName;
                if (ModelState.IsValid)
                {
                    if (db.Lsystem.Any(x => x.LsystemName.Equals(lsystem.LsystemName) && x.LsystemFamilyID == lsystem.LsystemFamilyID && x.LsystemID != lsystem.LsystemID))
                    {
                        ModelState.AddModelError("LsystemName", "System Name already exists");
                        ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily.OrderBy(x=>x.FamilyName), "LsystemFamilyID", "FamilyName", lsystem.LsystemFamilyID);
                        return View(lsystem);
                    }
                    lsystem.ModifiedOn = DateTime.Now;
                    lsystem.ModifiedBy = User.Identity.Name;
                    db.Entry(lsystem).State = EntityState.Modified;
                    db.Entry(lsystem).Property("CreatedOn").IsModified = false;
                    db.Entry(lsystem).Property("CreatedBy").IsModified = false;
                    //db.Entry(lsystem).Property("LsystemCount").IsModified = false;

                    db.SaveChanges();
                    return RedirectToAction("ViewSystems", "LsystemFamilies", new { id = lsystem.LsystemFamilyID });
                }
                ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily.OrderBy(x=>x.FamilyName), "LsystemFamilyID", "FamilyName", lsystem.LsystemFamilyID);
                return View(lsystem);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        public ActionResult ViewSystemProperties()
        {
            Lsystem model = db.Lsystem.Find(22);

            return View(model);
        }

        // GET: Lsystems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                Lsystem lsystem = db.Lsystem.Find(id);
                if (lsystem == null)
                {
                    ViewBag.Error = "The requested System does not exist";
                    return View("Error");
                }
                return View(lsystem);
            }
            else
                return View("AuthorizationError");
        }

        // POST: Lsystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                Lsystem lsystem = db.Lsystem.Find(id);
                var cc = db.ConfigurationCollection.Where(x => x.LsystemID == lsystem.LsystemID);
                foreach (var c in cc)
                {
                    var cv = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == c.ConfigurationCollectionID);
                    foreach (var item in cv)
                        db.Config_OptionVal.Remove(item);
                    db.ConfigurationCollection.Remove(c);
                }
                var op = db.Option.Where(x => x.LsystemID == lsystem.LsystemID);
                foreach (var o in op)
                {
                    var opv = db.OptionValue.Where(x => x.OptionID == o.OptionID);
                    foreach (var ov in opv)
                    {
                        var set = db.SetValue.Where(x => x.OptionValueID == ov.OptionValueID);
                        foreach (var item in set)
                        {
                            var rev = new RevisionHistory();
                            rev.Action = "Deleted";
                            rev.CreatedOn = item.ModifiedOn;
                            rev.ModifiedOn = DateTime.Now;
                            rev.ModifiedBy = User.Identity.Name;
                            rev.SystemName = item.OptionValue.Option.Lsystem.LsystemName;
                            rev.Option = item.OptionValue.Option.OptionName;
                            rev.Optionvalue = item.OptionValue.OptionVal;
                            rev.TCSetName = item.TcSet.SetName;
                            rev.SetValueID = item.SetValueID;
                            rev.InitialValue = item.Value;
                            db.RevisionHistory.Add(rev);
                            db.SetValue.Remove(item);
                        }
                        db.OptionValue.Remove(ov);
                    }
                    db.Option.Remove(o);
                }
                
                db.Lsystem.Remove(lsystem);
                db.SaveChanges();
                return RedirectToAction("ViewSystems", "LsystemFamilies", new { id = lsystem.LsystemFamilyID });
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
