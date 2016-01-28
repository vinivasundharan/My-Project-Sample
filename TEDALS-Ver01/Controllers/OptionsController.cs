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
    public class OptionsController : Controller
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
                    var url = urlHelper.Action("Error", "Options");

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

        // GET: Options
        public ActionResult Index()
        {
            var option = db.Option.Include(o => o.Lsystem).Include(o => o.TechnicalCharacteristic);
            option = option.OrderBy(o => o.LsystemID);
            return View(option.ToList());
        }

        //Not required
        public ActionResult ViewSetValues (int id)
        {
            var viewmodel = new OptionValue_SetVal();
            var set = db.OptionValue.Include(x => x.SetValue).FirstOrDefault(x => x.OptionValueID == id);
            if (set!=null)
            {
                viewmodel.SetValues = set.SetValue;
            }
            return View(viewmodel);
        }

        //Returns partial view: List option values and associated set values
        public ActionResult ViewOptionValues(int id)
        {
            try
            {

                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}
                var model = new Option();
                var ov = db.Option.Include(x => x.OptionValues).FirstOrDefault(x => x.OptionID == id);
                model.OptionValues = ov.OptionValues;
                //foreach(var item in ov.OptionValues)
                //{
                //    var sv = db.OptionValue.Include(x => x.SetValue).FirstOrDefault(x => x.OptionValueID == item.OptionValueID);
                //    model.OptionValues.FirstOrDefault().SetValue = sv.SetValue;
                //}
                var op = db.Option.FirstOrDefault(x => x.OptionID == id);
                var tc = db.Option.FirstOrDefault(x => x.OptionID == id).TechnicalCharacteristic;
                model.TechnicalCharacteristic = tc;
                ViewBag.Option = op.OptionName;
                ViewBag.Lsystem = op.Lsystem.LsystemName;
                ViewBag.FamilyName = op.Lsystem.LsystemFamily.FamilyName;
                ViewBag.id = op.LsystemID;
                return PartialView(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        public ActionResult test1()
        {
            var model = new test2();
            
            model.Option = db.Option.FirstOrDefault(x => x.OptionID == 24);
            var ov = db.Option.Include(x => x.OptionValues).FirstOrDefault(x => x.OptionID == 24);
            var tc = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == 24);
            model.Option.OptionValues = ov.OptionValues;
            model.SetValue = db.TcSet.Include(x => x.SetValues).FirstOrDefault(x => x.TechnicalCharacteristicID == tc.TechnicalCharacteristicID).SetValues;
            return View(model);
        }

        // GET: Options/Details/5
        public ActionResult Details(int? id)
        {
            try
            {


                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                Option option = db.Option.Find(id);
                if (option == null)
                {
                    ViewBag.Error = "The requested Option does not exist";
                    return View("Error");
                }
                return View(option);
            }
            catch
            {
                return View("Error");
            }
        }

       
        public ActionResult ViewOptions(int id)
        {
            try
            {

                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}
                var op = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
                ViewBag.id = id;
                ViewBag.Lsystem = op.LsystemName;
                ViewBag.FamID = op.LsystemFamilyID;
                ViewBag.FamilyName = op.LsystemFamily.FamilyName;
                var viewModel = new Lsystem_Option();
                if (id != 0)
                {
                    var Op = db.Lsystem.Include(x => x.Options).FirstOrDefault(x => x.LsystemID == id);
                    if (Op != null)
                    {
                        viewModel.Options = Op.Options;
                    }
                }
                return View(viewModel);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Options/Create
       
        public ActionResult Create(int id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {

                    //if (id == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    var model = new Option
                    {
                        LsystemID = id
                    };
                    //ViewBag.LsystemID = new SelectList(db.Lsystem, "LsystemID", "LsystemName");
                    var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
                    ViewBag.Lsystem = sys.LsystemName;
                    ViewBag.FamilyName = sys.LsystemFamily.FamilyName;
                    ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName");
                    var tc = db.TechnicalCharacteristic.Any();
                    if (!tc)
                        ViewBag.Message = "There are no technical Characteristics to display. Please Add new Technical Characteristics";
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

        // POST: Options/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OptionID,OptionName,DescriptionEN,DescriptionDE,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy,TechnicalCharacteristicID,LsystemID")] Option option)
        {
            try
            {


                var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == option.LsystemID);
                ViewBag.Lsystem = sys.LsystemName;
                ViewBag.FamilyName = sys.LsystemFamily.FamilyName;
                option.CreatedBy = User.Identity.Name;
                option.CreatedOn = DateTime.Now;
                option.ModifiedBy = User.Identity.Name;
                option.ModifiedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (db.Option.Any(x => x.OptionName.Equals(option.OptionName) && x.LsystemID == option.LsystemID))
                    {

                        ModelState.AddModelError("OptionName", "Option Already Exists");
                        ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                        ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                        return View(option);
                    }
                    if(db.Option.Any(x=>x.TechnicalCharacteristicID.Equals(option.TechnicalCharacteristicID)&& x.LsystemID==option.LsystemID))
                    {
                        ModelState.AddModelError("TechnicalCharacteristicID", "Technical characteristic Already Exists");
                        ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x => x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                        ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x => x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                        return View(option);
                    }
                    db.Option.Add(option);
                    db.SaveChanges();
                    return RedirectToAction("ViewOptions", "Options", new { id = option.LsystemID });
                }
                ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                return View(option);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Options/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                Option option = db.Option.Find(id);
                if (option == null)
                {
                    ViewBag.Error = "The requested Option does not exist";
                    return View("Error");
                }
                var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == option.LsystemID);
                ViewBag.Lsystem = sys.LsystemName;
                ViewBag.FamilyName = sys.LsystemFamily.FamilyName;
                ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                return View(option);
            }
            else
                return View("AuthorizationError");
        }

        // POST: Options/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OptionID,OptionName,DescriptionEN,DescriptionDE,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy,TechnicalCharacteristicID,LsystemID")] Option option)
        {
            try
            {


                var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == option.LsystemID);
                ViewBag.Lsystem = sys.LsystemName;
                ViewBag.FamilyName = sys.LsystemFamily.FamilyName;
                if (ModelState.IsValid)
                {
                    if (db.Option.Any(x => x.OptionName.Equals(option.OptionName) && x.LsystemID == option.LsystemID && x.OptionID != option.OptionID))
                    {
                        ModelState.AddModelError("OptionName", "Option Already Exists");
                        ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                        ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                        //ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                       // ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic, "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                        return View(option);
                    }

                    if (db.Option.Any(x => x.TechnicalCharacteristicID.Equals(option.TechnicalCharacteristicID) && x.LsystemID == option.LsystemID&&x.OptionID!=option.OptionID))
                    {
                        ModelState.AddModelError("TechnicalCharacteristicID", "Technical characteristic Already Exists");
                        ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x => x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                        ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x => x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                        return View(option);
                    }


                    var oldop = db.Option.Find(option.OptionID);
                    var oldtcid = oldop.TechnicalCharacteristicID;
                    var oldtcset = db.TcSet.Where(x => x.TechnicalCharacteristicID == oldtcid).Select(x=>x.SetName).ToList();
                    var newtcset = db.TcSet.Where(x => x.TechnicalCharacteristicID == option.TechnicalCharacteristicID).Select(x=>x.SetName).ToList();
                    if (oldtcid != option.TechnicalCharacteristicID)
                        ViewBag.warning = "This operation may result in data loss";

                    List<string> duplicate = oldtcset.Intersect(newtcset).ToList();

                    

                    oldop.ModifiedOn = DateTime.Now;
                    oldop.ModifiedBy = User.Identity.Name;
                    oldop.TechnicalCharacteristicID = option.TechnicalCharacteristicID;
                    //oldop.CreatedBy = option.CreatedBy;
                    oldop.OptionName = option.OptionName;
                    oldop.DescriptionDE = option.DescriptionDE;
                    oldop.DescriptionEN = option.DescriptionEN;
                    db.Entry(oldop).State = EntityState.Modified;
                    db.Entry(oldop).Property("CreatedOn").IsModified = false;
                    db.Entry(oldop).Property("CreatedBy").IsModified = false;
                    db.Entry(oldop).Property("LsystemID").IsModified = false;
                    //db.Entry(oldop).Property(x => x.OptionValues).IsModified = true;
                    db.SaveChanges();

                    foreach (var ov in oldop.OptionValues.ToList())
                    {
                        foreach (var set in ov.SetValue.ToList())
                        {

                            if (!duplicate.Contains(set.TcSet.SetName))
                            {
                                db.SetValue.Remove(set);
                            }
                            else
                            {
                                var p = db.SetValue.Find(set.SetValueID);
                                p.TcSetID = db.TcSet.FirstOrDefault(x => x.SetName == set.TcSet.SetName && x.TechnicalCharacteristicID == oldop.TechnicalCharacteristicID).TcSetID;
                                p.ModifiedBy = User.Identity.Name;
                                p.ModifiedOn = DateTime.Now;
                                db.Entry(p).State = EntityState.Modified;
                                db.Entry(p).Property(x => x.OptionValueID).IsModified = false;
                                db.Entry(p).Property(x => x.CreatedOn).IsModified = false;
                                db.Entry(p).Property(x => x.CreatedBy).IsModified = false;
                                
                            }

                        }
                        if (ov.SetValue.Count() == 0)
                            db.OptionValue.Remove(ov);
                    }
                    db.SaveChanges();
                    return RedirectToAction("ViewOptions", new { id = option.LsystemID });
                }
                ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", option.LsystemID);
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", option.TechnicalCharacteristicID);
                return View(option);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Options/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                Option option = db.Option.Find(id);
                if (option == null)
                {
                    ViewBag.Error = "The requested Option does not exist";
                    return View("Error");
                }
                return View(option);
            }
            else
                return View("AuthorizationError");
        }

        // POST: Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                Option option = db.Option.Find(id);
                var opv = db.OptionValue.Where(x => x.OptionID == option.OptionID);
                foreach (var s in opv)
                {
                    var set = db.SetValue.Where(x => x.OptionValueID == s.OptionValueID);
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
                    var cv = db.Config_OptionVal.Where(x => x.OptionValueID == s.OptionValueID);
                    foreach (var cvl in cv)
                        db.Config_OptionVal.Remove(cvl);
                    db.OptionValue.Remove(s);
                }
                db.Option.Remove(option);
                db.SaveChanges();
                return RedirectToAction("ViewOptions", new { id = option.LsystemID });
            }
            catch
            {
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
