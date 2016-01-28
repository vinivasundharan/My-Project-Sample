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
using TEDALS_Ver01.ViewModels;

namespace TEDALS_Ver01.Controllers
{
    public class TechnicalCharacteristicsController : Controller
    {
        private TedalsContext db = new TedalsContext();

        public class IdRequiredAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                object parameter = null;
                filterContext.ActionParameters.TryGetValue("id", out parameter);
                var id = parameter as int?;
                var LsystemID = parameter as int?;
                if (id == null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "TechnicalCharacteristics");

                    filterContext.Result = new RedirectResult(url);
                }
                if (LsystemID == null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "SetValues");

                    filterContext.Result = new RedirectResult(url);
                }
            }
        }

        public ActionResult Error()
        {
            ViewBag.Error = "An unhandled Exception occured with the processing of your request";
            return View("Error");
        }

        // GET: TechnicalCharacteristics
        public ActionResult Index(string SearchString)
        {
            var tc = from t in db.TechnicalCharacteristic select t;
            if(!String.IsNullOrEmpty(SearchString))
            {
                tc = tc.Where(x=>x.TCName.Contains(SearchString));
            }
            return View(tc.OrderBy(x=>x.TCName));
        }

        // GET: TechnicalCharacteristics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            TechnicalCharacteristic technicalCharacteristic = db.TechnicalCharacteristic.Find(id);
            if (technicalCharacteristic == null)
            {
                ViewBag.Error = "The requested technical Characteristic does  not exist";
                return View("Error");
            }
            return View(technicalCharacteristic);
        }

        //returns list of all TcSet under a particular technical characteristic
        public ActionResult ViewTCset (int id)
        {
            try
            {
                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}

                var viewmodel = new TC_TcSet();
                if (id != 0)
                {
                    var tcset = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                    if (tcset != null)
                    {
                        viewmodel.TcSet = tcset.TcSets.OrderBy(x => x.SetName);
                    }
                }
                var tc = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                ViewBag.TC = tc.TCName;
                ViewBag.id = id;
                return View(viewmodel);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //public ActionResult AddProperty(int[] TcSetID, int T)
        //{
        //    foreach(var item in )
        //    return RedirectToAction("Index");
        //}

        // GET: TechnicalCharacteristics/Create
        public ActionResult Create()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                //ViewBag.TcSet = db.TcSet.Where(x => x.TcSetID != 0).ToList();
                return View();
            }
            else return View("AuthorizationError");
        }

        // POST: TechnicalCharacteristics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TechnicalCharacteristicID,TCName,DescriptionEN,DescriptionDE,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy")] TechnicalCharacteristic technicalCharacteristic)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.TechnicalCharacteristic.Any(x => x.TCName.Equals(technicalCharacteristic.TCName)))
                    {
                        ModelState.AddModelError("TCName", "Technical Characteristic already exists");
                        return View(technicalCharacteristic);
                    }
                    technicalCharacteristic.CreatedOn = DateTime.Now;
                    technicalCharacteristic.CreatedBy = User.Identity.Name;
                    technicalCharacteristic.ModifiedBy = User.Identity.Name;
                    technicalCharacteristic.ModifiedOn = DateTime.Now;
                    db.TechnicalCharacteristic.Add(technicalCharacteristic);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(technicalCharacteristic);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
          
        }

        // GET: TechnicalCharacteristics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                TechnicalCharacteristic technicalCharacteristic = db.TechnicalCharacteristic.Find(id);
                if (technicalCharacteristic == null)
                {
                    ViewBag.Error = "The requested Technical Characteristic does not exist";
                    return View("Error");
                }
                return View(technicalCharacteristic);
            }
            else return View("AuthorizationError");
        }

        // POST: TechnicalCharacteristics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TechnicalCharacteristicID,TCName,DescriptionEN,DescriptionDE,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy")] TechnicalCharacteristic technicalCharacteristic)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.TechnicalCharacteristic.Any(x => x.TCName == technicalCharacteristic.TCName && x.TechnicalCharacteristicID != technicalCharacteristic.TechnicalCharacteristicID))
                    {
                        ModelState.AddModelError("TCName", "Technical Characteristic already exists");
                        return View(technicalCharacteristic);
                    }
                    technicalCharacteristic.ModifiedOn = DateTime.Now;
                    technicalCharacteristic.ModifiedBy = User.Identity.Name;
                    db.Entry(technicalCharacteristic).State = EntityState.Modified;
                    db.Entry(technicalCharacteristic).Property("CreatedOn").IsModified = false;
                    db.Entry(technicalCharacteristic).Property("CreatedBy").IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(technicalCharacteristic);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: TechnicalCharacteristics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                TechnicalCharacteristic technicalCharacteristic = db.TechnicalCharacteristic.Find(id);
                if (technicalCharacteristic == null)
                {
                    ViewBag.Error = "The requested Technical Characteristic does not exist";
                    return View("Error");
                }
                return View(technicalCharacteristic);
            }
            else return View("AuthorizationError");
        }

        // POST: TechnicalCharacteristics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                TechnicalCharacteristic technicalCharacteristic = db.TechnicalCharacteristic.Find(id);
                var tset = db.TcSet.Where(x => x.TechnicalCharacteristicID == technicalCharacteristic.TechnicalCharacteristicID);

                var op = db.Option.Where(x => x.TechnicalCharacteristicID == technicalCharacteristic.TechnicalCharacteristicID);
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
                        var con = db.Config_OptionVal.Where(x => x.OptionValueID == ov.OptionValueID).ToList();
                        foreach (var c in con)
                            db.Config_OptionVal.Remove(c);
                        db.OptionValue.Remove(ov);
                    }
                    db.Option.Remove(o);
                }
                foreach (var s in tset)
                {
                    var viewprop = db.ViewsProperty.Where(x => x.TcSetID == s.TcSetID);
                    foreach (var vp in viewprop)
                        db.ViewsProperty.Remove(vp);
                    db.TcSet.Remove(s);
                }
                    
                db.TechnicalCharacteristic.Remove(technicalCharacteristic);
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
