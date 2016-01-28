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
    public class TcSetsController : Controller
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
                    var url = urlHelper.Action("Error", "TcSets");

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

        // GET: TcSets
        public ActionResult Index()
        {
            var tcSet = db.TcSet.Include(t => t.DataFormat).Include(t => t.TechnicalCharacteristic);
            return View(tcSet.OrderBy(x=>x.SetName).ToList());
        }

        // GET: TcSets/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function"; 
                return View("Error");
            }
            TcSet tcSet = db.TcSet.Find(id);
            if (tcSet == null)
            {
                ViewBag.Error = "The requested technical Characteristic property does not exist";
                return View("Error");
            }
            return View(tcSet);
        }

        //Copy Properties
      
        public ActionResult CopyProperty(int id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                try
                {

                    //if (id == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    var tclist = new List<int>();
                    foreach (var item in db.TechnicalCharacteristic.Where(x => x.TechnicalCharacteristicID != id).ToList())
                        tclist.Add(item.TechnicalCharacteristicID);
                    var tclistremove = new List<int>();

                    // eliminating set names that are used in a system where the technical characteristic is used.
                    foreach(var item in db.Lsystem.ToList())
                    {
                        if (item.Options.Any(x => x.TechnicalCharacteristicID == id))
                        {


                            foreach (var op in item.Options.ToList())
                            {

                                if (tclist.Contains(op.TechnicalCharacteristicID))
                                {
                                    tclist.Remove(op.TechnicalCharacteristicID);
                                    tclistremove.Add(op.TechnicalCharacteristicID);
                                }
                                    
                            }
                        }
                    }
                    var setlistname = new List<string>();
                    foreach(var item in tclistremove)
                    {
                        var tc = db.TechnicalCharacteristic.Where(x => x.TechnicalCharacteristicID == item);
                        foreach (var t in tc)
                            foreach (var s in t.TcSets)
                                if(!setlistname.Contains(s.SetName))
                                   setlistname.Add(s.SetName);
                    }
                    var TC = new List<TechnicalCharacteristic>();
                    foreach (var item in tclist)
                        TC.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == item));
                    ViewBag.TC = TC;
                    var model = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                    var set = db.TcSet.Where(x => x.TechnicalCharacteristicID == id).ToList();
                    foreach (var item in set)
                        model.TcSets.Add(item);
                    //.TcSet = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                    var filterset = new List<TcSet>();
                    foreach(var item in TC)
                    {
                        foreach (var s in item.TcSets)
                            if (!setlistname.Contains(s.SetName))
                                filterset.Add(s);
                    }

                    ViewBag.TcSet = filterset.OrderBy(x=>x.SetName);

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

        [HttpPost]
        public ActionResult CopyProperty(TechnicalCharacteristic model, int[] setid,int[] tcid)
        {
            try
            {


                if (tcid != null)
                {
                    var tcset = new List<TcSet>();
                    var TC = new List<TechnicalCharacteristic>();

                    foreach (var id in tcid)
                    {
                        TC.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id));
                        
                        foreach (var set1 in db.TcSet.Where(x => x.TechnicalCharacteristicID == id))
                            if (!tcset.Any(x => x.SetName == set1.SetName))
                                tcset.Add(set1);
                    }


                    ViewBag.TC = TC.OrderBy(x => x.TCName);
                    ViewBag.TcSet = tcset.OrderBy(x => x.SetName);
                    return View(model);
                }
                var req_tcset = new List<TcSet>();
                foreach (var item in setid)
                    req_tcset.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item));
                var set = db.TcSet.Where(x => x.TechnicalCharacteristicID == model.TechnicalCharacteristicID).ToList();
                foreach (var item in set)
                    model.TcSets.Add(item);
                foreach (var item in req_tcset)
                {
                    if (!model.TcSets.Any(x => x.SetName == item.SetName))
                    {
                        var add = new TcSet { TechnicalCharacteristic = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == model.TechnicalCharacteristicID), TechnicalCharacteristicID = model.TechnicalCharacteristicID };
                        add.SetName = item.SetName;
                        add.DescriptionDE = item.DescriptionDE;
                        add.PhysicalUnit = item.PhysicalUnit;
                        add.DescriptionEN = item.DescriptionEN;
                        add.DataStatus = item.DataStatus;
                        add.DataFormatID = item.DataFormatID;
                        add.DataFormat = item.DataFormat;
                        add.CreatedBy = User.Identity.Name;
                        add.CreatedOn = DateTime.Now;
                        add.ModifiedBy = User.Identity.Name;
                        add.ModifiedOn = DateTime.Now;
                        add.DataUsage = item.DataUsage;
                        //add.TcSetID = model.TechnicalCharacteristicID;
                        //add.TechnicalCharacteristic = model;
                        db.TcSet.Add(add);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ViewTCSet", "TechnicalCharacteristics", new { id = model.TechnicalCharacteristicID });
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: TcSets/Create
      
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
                    ViewBag.TcSet = db.TcSet.Where(x => x.TcSetID != 0).ToList();

                    ViewBag.id = id;
                    var tc1 = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                    ViewBag.TC = tc1.TCName;
                    var model = new TcSet
                    {
                        TechnicalCharacteristicID = id
                    };
                    var tc = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                    ViewBag.TC = tc.TCName;
                    ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName");
                    //ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic, "TechnicalCharacteristicID", "TCName");
                    if (!db.DataFormat.Any())
                        ViewBag.Message = "There are no Data Formats to display. Please add new Data Formats";
                    return View(model);
                }
                catch(Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View("Error");
                }
            }
            else return View("AuthorizationError");
        }

        // POST: TcSets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TcSetID,SetName,PhysicalUnit,DataUsage,DataStatus,CreatedOn,ModifiedOn,DescriptionDE,DescriptionEN,CreatedBy,ModifiedBy,TechnicalCharacteristicID,DataFormatID")] TcSet tcSet)
        {
            try
            {


                var tc1 = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcSet.TechnicalCharacteristicID);
                ViewBag.TC = tc1.TCName;
                if (ModelState.IsValid)
                {
                    if(db.TcSet.Any(x=>x.SetName==tcSet.SetName))
                    {
                        ModelState.AddModelError("SetName", "Property already exists");
                        ViewBag.TcSet = db.TcSet.Where(x => x.TcSetID != 0).ToList();

                        ViewBag.id = tcSet.TechnicalCharacteristicID;
                       // var tc1 = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == id);
                        ViewBag.TC = tc1.TCName;
                        var model = new TcSet
                        {
                            TechnicalCharacteristicID = tcSet.TechnicalCharacteristicID
                        };
                        var tc = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcSet.TechnicalCharacteristicID);
                        ViewBag.TC = tc.TCName;
                        ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x => x.FormatName), "DataFormatID", "FormatName");
                        //ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic, "TechnicalCharacteristicID", "TCName");
                        if (!db.DataFormat.Any())
                            ViewBag.Message = "There are no Data Formats to display. Please add new Data Formats";
                        return View(model);

                    }
                    tcSet.CreatedBy = User.Identity.Name;
                    tcSet.CreatedOn = DateTime.Now;
                    tcSet.ModifiedOn = DateTime.Now;
                    tcSet.ModifiedBy = User.Identity.Name;
                    db.TcSet.Add(tcSet);
                    db.SaveChanges();
                    return RedirectToAction("ViewTcSet", "TechnicalCharacteristics", new { id = tcSet.TechnicalCharacteristicID });
                }
                if (!db.DataFormat.Any())
                    ViewBag.Message = "There are no Data Formats to display. Please add new Data Formats";
                ViewBag.id = tcSet.TechnicalCharacteristicID;
                ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", tcSet.DataFormatID);
                //ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic, "TechnicalCharacteristicID", "TCName", tcSet.TechnicalCharacteristicID);
                return View(tcSet);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: TcSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                TcSet tcSet = db.TcSet.Find(id);
                if (tcSet == null)
                {
                    ViewBag.Error = "The requested technical Characteristic Property does not exist";
                    return View("Error");
                }
                ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", tcSet.DataFormatID);
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", tcSet.TechnicalCharacteristicID);
                return View(tcSet);
            }
            else return View("AuthorizationError");
        }

        // POST: TcSets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TcSetID,SetName,PhysicalUnit,DataUsage,DataStatus,CreatedOn,ModifiedOn,DescriptionDE,DescriptionEN,CreatedBy,ModifiedBy,TechnicalCharacteristicID,DataFormatID")] TcSet tcSet)
        {
            try
            {


                if (ModelState.IsValid)
                {
                   
                    if (db.TcSet.Any(x => x.SetName.Equals(tcSet.SetName) && x.TechnicalCharacteristicID==tcSet.TechnicalCharacteristicID && x.TcSetID != tcSet.TcSetID))
                    {
                        ModelState.AddModelError("SetName", "Property already Exists");
                        ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", tcSet.DataFormatID);
                        ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", tcSet.TechnicalCharacteristicID);
                        tcSet.TechnicalCharacteristic = db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcSet.TechnicalCharacteristicID);
                        return View(tcSet);
                    }
                    foreach(var set in db.TcSet.Where(x=>x.SetName==tcSet.SetName&&x.TcSetID!=tcSet.TcSetID))
                    {
                        set.ModifiedBy = User.Identity.Name;
                        set.ModifiedOn = DateTime.Now;
                        set.PhysicalUnit = tcSet.PhysicalUnit;
                        set.SetName = tcSet.SetName;
                        set.DataFormatID = tcSet.DataFormatID;
                        set.DataStatus = tcSet.DataStatus;
                        set.DataUsage = tcSet.DataUsage;
                        set.DescriptionDE = tcSet.DescriptionDE;
                        set.DescriptionEN = tcSet.DescriptionEN;
                        
                        db.Entry(set).State = EntityState.Modified;

                        db.Entry(set).Property(x => x.CreatedBy).IsModified = false;
                        db.Entry(set).Property(x => x.CreatedOn).IsModified = false;
                        db.Entry(set).Property(x => x.TechnicalCharacteristicID).IsModified = false;

                    }
                    tcSet.ModifiedOn = DateTime.Now;
                    tcSet.ModifiedBy = User.Identity.Name;
                    db.Entry(tcSet).State = EntityState.Modified;
                    db.Entry(tcSet).Property("CreatedBy").IsModified = false;
                    db.Entry(tcSet).Property("CreatedOn").IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("ViewTcSet", "TechnicalCharacteristics", new { id = tcSet.TechnicalCharacteristicID });
                }
                ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", tcSet.DataFormatID);
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x=>x.TCName), "TechnicalCharacteristicID", "TCName", tcSet.TechnicalCharacteristicID);
                return View(tcSet);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: TcSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                TcSet tcSet = db.TcSet.Find(id);
                if (tcSet == null)
                {
                    ViewBag.Error = "The requested Technical Characteristic does not exist";
                    return View("Error");
                }
                return View(tcSet);
            }
            else return View("AuthorizationError");
        }

        // POST: TcSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                TcSet tcSet = db.TcSet.Find(id);
                var set = db.SetValue.Where(x => x.TcSetID == tcSet.TcSetID);
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
                var viewprop = db.ViewsProperty.Where(x => x.TcSetID == tcSet.TcSetID).ToList();
                foreach (var i in viewprop)
                    db.ViewsProperty.Remove(i);
                db.TcSet.Remove(tcSet);
                db.SaveChanges();
                return RedirectToAction("ViewTcSet", "TechnicalCharacteristics", new { id = tcSet.TechnicalCharacteristicID });
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
