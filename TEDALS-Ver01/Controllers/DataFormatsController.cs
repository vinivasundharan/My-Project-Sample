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
    public class DataFormatsController : Controller
    {
        private TedalsContext db = new TedalsContext();

        // GET: DataFormats
        public ActionResult Index()
        {
            try
            {


                return View(db.DataFormat.OrderBy(x => x.FormatName).ToList());
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: DataFormats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the fucntion";
                return View("Error");
            }
            DataFormat dataFormat = db.DataFormat.Find(id);
            if (dataFormat == null)
            {
                ViewBag.Error = "The requested Data format does not exist";
                return View("Error");
            }
            return View(dataFormat);
        }

        // GET: DataFormats/Create
        public ActionResult Create()
        {
            //var model = new DataFormat
            //{
            //    PrecisionDigits = 0,
            //    ScalingDigits = 0
            //};
            var FormatType = new SelectList(new[]
            {
                new {  ID = "Number",Name = "Number"},
                new { ID= "String", Name = "String"},
            },
            "ID", "Name", 1);
            ViewData["FormatType"] = FormatType;
            return View();
        }

        // POST: DataFormats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DataFormatID,FormatName,FormatType,PrecisionDigits,ScalingDigits,DescriptionDE,DescriptionEN,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy")] DataFormat dataFormat)
        {
            try
            {


                //dataFormat.CreatedBy = User.Identity.Name;
                //dataFormat.CreatedOn = DateTime.Now;
                //dataFormat.ModifiedBy = User.Identity.Name;
                //dataFormat.ModifiedOn = DateTime.Now;
                if (ModelState.IsValid)
                {
                    if (db.DataFormat.Any(x => x.FormatName.Equals(dataFormat.FormatName)))
                    {

                        ModelState.AddModelError("FormatName", "Data format already exists");
                        var FormatType = new SelectList(new[]
                    {
                      new {  ID = "Number",Name = "Number"},
                      new { ID= "String", Name = "String"},
                    },
                        "ID", "Name", 1);
                        ViewData["FormatType"] = FormatType;
                        return View(dataFormat);
                    }
                    if (dataFormat.FormatType == "Number")
                    {
                        if ((dataFormat.ScalingDigits == null && dataFormat.PrecisionDigits == null)||(dataFormat.ScalingDigits!=null&& dataFormat.PrecisionDigits!=null))
                        {
                            ModelState.AddModelError("PrecisionDigits", "Precision digits and scaling digits cannot be empty. Please enter value for either of the one");
                            var FormatType = new SelectList(new[]
                    {
                      new {  ID = "Number",Name = "Number"},
                      new { ID= "String", Name = "String"},
                    },
                        "ID", "Name", 1);
                            ViewData["FormatType"] = FormatType;
                            return View(dataFormat);
                        }
                    //    else if()
                    //    {
                    //        ModelState.AddModelError("PrecisionDigits", "Precision digits and scaling digits cannot have values simultaneously. Please enter value for either of the one");
                    //        var FormatType = new SelectList(new[]
                    //{
                    //  new {  ID = "Number",Name = "Number"},
                    //  new { ID= "String", Name = "String"},
                    //},
                    //    "ID", "Name", 1);
                    //        ViewData["FormatType"] = FormatType;
                    //        return View(dataFormat);
                    //    }
                    }
                    
                    dataFormat.CreatedBy = User.Identity.Name;
                    dataFormat.CreatedOn = DateTime.Now;
                    dataFormat.ModifiedBy = User.Identity.Name;
                    dataFormat.ModifiedOn = DateTime.Now;
                    db.DataFormat.Add(dataFormat);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(dataFormat);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: DataFormats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            DataFormat dataFormat = db.DataFormat.Find(id);
            if (dataFormat == null)
            {
                ViewBag.Error = "The requested Data format do not exist";
                return View("Error");
            }
            var tclist = new List<TcSet>();
            foreach(var item in db.TcSet.ToList())
                if(item.DataFormatID==id)
                    tclist.Add(item);
            if(tclist.Count()!=0)
            {
                ViewBag.Tclist = tclist;
                return View("DataFormatEdit");
            }


            return View(dataFormat);
        }

        // POST: DataFormats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DataFormatID,FormatName,FormatType,PrecisionDigits,ScalingDigits,DescriptionDE,DescriptionEN")] DataFormat dataFormat)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if(db.DataFormat.Any(x=>x.FormatName==dataFormat.FormatName && x.DataFormatID!=dataFormat.DataFormatID))
                    {
                        ModelState.AddModelError("FormatName", "Format name already exists");
                        var FormatType = new SelectList(new[]
                    {
                      new {  ID = "Number",Name = "Number"},
                      new { ID= "String", Name = "String"},
                    },
                        "ID", "Name", 1);
                        ViewData["FormatType"] = FormatType;                      
                        return View(dataFormat);
                    }
                    if (dataFormat.FormatType != "String")
                    {
                        if ((dataFormat.PrecisionDigits != null & dataFormat.ScalingDigits != null) || (dataFormat.ScalingDigits == null && dataFormat.PrecisionDigits == null))
                        {
                            ModelState.AddModelError("PrecisionDigits", "Precision digits and scaling digits cannot be empty. Please enter value for either of the one");
                            var FormatType = new SelectList(new[]
                             {
                                new {  ID = "Number",Name = "Number"},
                                new { ID= "String", Name = "String"},
                             },
                            "ID", "Name", 1);
                            ViewData["FormatType"] = FormatType;
                            return View(dataFormat);
                        }
                    }
                    dataFormat.ModifiedOn = DateTime.Now;
                    dataFormat.ModifiedBy = User.Identity.Name;
                    db.Entry(dataFormat).State = EntityState.Modified;
                    db.Entry(dataFormat).Property("CreatedOn").IsModified = false;
                    db.Entry(dataFormat).Property("CreatedBy").IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(dataFormat);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: DataFormats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the method";
                return View("Error");
            }
            DataFormat dataFormat = db.DataFormat.Find(id);
            if (dataFormat == null)
            {
                ViewBag.Error = "The requested Data format does not exist";
                return View("Error");
            }
            var tclist = new List<TcSet>();
            foreach (var item in db.TcSet.ToList())
                if (item.DataFormatID == id)
                    tclist.Add(item);
            if (tclist.Count() != 0)
            {
                ViewBag.Tclist = tclist;
                ViewBag.Message = "Data Format cannot be deleted";
                return View("DataFormatEdit");
            }
            return View(dataFormat);
        }

        // POST: DataFormats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                DataFormat dataFormat = db.DataFormat.Find(id);
                var tset = db.TcSet.Where(x => x.DataFormatID == dataFormat.DataFormatID);
                foreach (var s in tset)
                {
                    var set = db.SetValue.Where(x => x.TcSetID == s.TcSetID);
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
                    db.TcSet.Remove(s);
                }

                db.DataFormat.Remove(dataFormat);
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
