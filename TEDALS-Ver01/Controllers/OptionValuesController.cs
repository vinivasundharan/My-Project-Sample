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
    public class OptionValuesController : Controller
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
                    var url = urlHelper.Action("Error", "OptionValues");

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

        // GET: OptionValues
        public ActionResult Index()
        {
            var optionValue = db.OptionValue.Include(o => o.Option);
            optionValue = optionValue.OrderBy(o => o.Option.LsystemID);
            return View(optionValue.ToList());
        }

        // GET: OptionValues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter passed to the fucntion";
                return View("Error");
            }
            OptionValue optionValue = db.OptionValue.Find(id);
            if (optionValue == null)
            {
                ViewBag.Error = "The requested Option Value does not exist";
                return View("Error");
            }
            return View(optionValue);
        }

        
        public ActionResult ViewOptionValues (int id)
        {
            try
            {
                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}
                ViewBag.id = id;
                var model = new Option();
                model.OptionValues = new List<OptionValue>();
                model = db.Option.Include(x=>x.OptionValues).FirstOrDefault(x => x.OptionID == id);
                var ov = db.OptionValue.Where(x => x.OptionID == id);
                ViewBag.Option = model.OptionName;
                ViewBag.Lsystem = model.Lsystem.LsystemName;
                ViewBag.FamilyName = model.Lsystem.LsystemFamily.FamilyName;
                ViewBag.LsystemID = model.LsystemID;
                model.OptionValues = ov.OrderBy(x => x.OptionVal).ToList();
                //model.OptionValues = ((from ov in model.OptionValues orderby ov.OptionVal ascending select ov).ToList<OptionValue>());
                return View(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Not required
        public ActionResult AddOptionValue (int OptionID, int TCID)
        {
            try
            {
                var viewModel = new AddOptionValue();
                var option = db.Option.FirstOrDefault(x => x.OptionID == OptionID);
                var tc = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == TCID);
                var TcSet = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == TCID);
                if (option != null && TcSet != null)
                {
                    viewModel.Option = option;
                    viewModel.TechnicalCharacteristic = tc;
                    viewModel.TcSets = TcSet.TcSets;
                }
                return View(viewModel);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: OptionValues/Create
     
        public ActionResult Create(int id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {
                    //if (OptionID == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    var model = new OptionValue
                    {
                        OptionID = id,

                    };
                    var op = db.Option.FirstOrDefault(x => x.OptionID == id);
                    ViewBag.Option = op.OptionName;
                    ViewBag.Lsystem = op.Lsystem.LsystemName;
                    ViewBag.FamilyName = op.Lsystem.LsystemFamily.FamilyName;
                    ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName");
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

        // POST: OptionValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OptionValueID,OptionVal,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,OptionID,DescriptionEN,DescriptionDE")] OptionValue optionValue)
        {
            try
            {
                var op1 = db.Option.FirstOrDefault(x => x.OptionID == optionValue.OptionID);
                ViewBag.Option = op1.OptionName;
                ViewBag.Lsystem = op1.Lsystem.LsystemName;
                ViewBag.FamilyName = op1.Lsystem.LsystemFamily.FamilyName;
                if (ModelState.IsValid)
                {
                    var opv = db.OptionValue.Any(x => x.OptionVal.Equals(optionValue.OptionVal) && x.OptionID == optionValue.OptionID);
                    if (opv)
                    {
                        ModelState.AddModelError("OptionVal", "Option Value already exists");
                        ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName", optionValue.OptionID);
                        return View(optionValue);
                    }
                    optionValue.CreatedBy = User.Identity.Name;
                    optionValue.ModifiedBy = User.Identity.Name;
                    optionValue.CreatedOn = DateTime.Now;
                    optionValue.ModifiedOn = DateTime.Now;
                    db.OptionValue.Add(optionValue);
                    db.SaveChanges();
                    return RedirectToAction("Index", "SetValues", new { id = optionValue.OptionValueID });
                }

                ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName", optionValue.OptionID);
                return View(optionValue);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: OptionValues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                OptionValue optionValue = db.OptionValue.Find(id);
                if (optionValue == null)
                {
                    ViewBag.Error = "Requested Option value do not exist";
                    return View("Error");
                }
                ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName", optionValue.OptionID);
                return View(optionValue);
            }
            else
                return View("AuthorizationError");
        }

        // POST: OptionValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OptionValueID,OptionVal,OptionID,DescriptionDE,DescriptionEN")] OptionValue optionValue)
        {
            try
            {

                //OptionValue old = db.OptionValue.Find(optionValue.OptionValueID);
                if (ModelState.IsValid)
                {
                    if (db.OptionValue.Any(x => x.OptionVal.Equals(optionValue.OptionVal) && x.OptionID == optionValue.OptionID && x.OptionValueID != optionValue.OptionValueID))
                    {
                        ModelState.AddModelError("OptionVal", "Option Value already exists");
                        ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName", optionValue.OptionID);
                        return View(optionValue);
                    }
                    //OptionValue old = db.OptionValue.Find(optionValue.OptionValueID);


                    optionValue.ModifiedBy = User.Identity.Name;
                    //db.Entry(optionValue).Property("T")
                    db.Entry(optionValue).State = EntityState.Modified;
                    db.Entry(optionValue).Property(o => o.OptionID).IsModified = false;
                    db.Entry(optionValue).Property(o => o.CreatedOn).IsModified = false;
                    db.Entry(optionValue).Property(o => o.CreatedBy).IsModified = false;
                    optionValue.ModifiedOn = DateTime.Now;
                    db.SaveChanges();

                    //if (old.OptionVal != optionValue.OptionVal)
                    //{
                    //    var rev = new RevisionHistory();
                    //    rev.CreatedOn = optionValue.CreatedOn;
                    //    rev.ModifiedOn = optionValue.ModifiedOn;
                    //    rev.ModifiedBy = optionValue.ModifiedBy;
                    //    var op = db.Option.FirstOrDefault(x => x.OptionID == optionValue.OptionID);
                    //    rev.Option = op.OptionName;
                    //    rev.SystemName = op.Lsystem.LsystemName;
                    //    rev.InitialValue = old.OptionVal;
                    //    rev.ModifiedValue = optionValue.OptionVal;
                    //    db.RevisionHistory.Add(rev);
                    //    db.SaveChanges();
                    //}

                    //var rev = new RevisionHistory();
                    //rev.CreatedOn = optionValue.CreatedOn;
                    //rev.ModifiedOn = optionValue.ModifiedOn;
                    //rev.ModifiedBy = optionValue.ModifiedBy;
                    //var op = db.Option.FirstOrDefault(x => x.OptionID == optionValue.OptionID);
                    //rev.Option = op.OptionName;
                    //rev.SystemName = op.Lsystem.LsystemName;

                    return RedirectToAction("ViewOptionValues", "Optionvalues", new { id = optionValue.OptionID });
                }
                ViewBag.OptionID = new SelectList(db.Option.OrderBy(x=>x.OptionName), "OptionID", "OptionName", optionValue.OptionID);
                return View(optionValue);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: OptionValues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                OptionValue optionValue = db.OptionValue.Find(id);
                if (optionValue == null)
                {
                    ViewBag.Error = "The requested Option value does not exist";
                    return View("Error");
                }
                return View(optionValue);
            }
            else
                return View("AuthorizationError");
        }

        // POST: OptionValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                OptionValue optionValue = db.OptionValue.Find(id);
                var set = db.SetValue.Where(x => x.OptionValueID == optionValue.OptionValueID);
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
                    
                var cv = db.Config_OptionVal.Where(x => x.OptionValueID == id);
                foreach (var cvl in cv)
                    db.Config_OptionVal.Remove(cvl);
                db.OptionValue.Remove(optionValue);
                db.SaveChanges();
                return RedirectToAction("ViewOptionValues", "Optionvalues", new { id = optionValue.OptionID });
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
