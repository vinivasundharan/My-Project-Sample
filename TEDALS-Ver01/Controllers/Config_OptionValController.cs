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
    public class Config_OptionValController : Controller
    {
        private TedalsContext db = new TedalsContext();

        // GET: Config_OptionVal
        public ActionResult Index()
        {
            var config_OptionVal = db.Config_OptionVal.Include(c => c.ConfigurationCollection).Include(c => c.OptionValue);
            return View(config_OptionVal.ToList());
        }

        // GET: Config_OptionVal/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Config_OptionVal config_OptionVal = db.Config_OptionVal.Find(id);
            if (config_OptionVal == null)
            {
                return HttpNotFound();
            }
            return View(config_OptionVal);
        }

        // GET: Config_OptionVal/Create
        public ActionResult Create(int id)
        {
            var model = new Config_OptionVal
            {
                ConfigurationCollectionID=id,
                ConfigurationCollection=db.ConfigurationCollection.FirstOrDefault(x=>x.ConfigurationCollectionID==id)
            };
            //var lsysid = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == id).LsystemID;
            ////ViewBag.ConfigurationCollectionID = new SelectList(db.ConfigurationCollection, "ConfigurationCollectionID", "CollectionName");
            ////ViewBag.OptionValueID = new SelectList(db.OptionValue, "OptionValueID", "OptionVal");
            //ViewBag.Lsystem = db.Lsystem.Include(x => x.Options).FirstOrDefault(x => x.LsystemID == lsysid);
            return View(model);
        }

        // POST: Config_OptionVal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<Config_OptionVal> Config_OptionVal)
        {
            if (ModelState.IsValid)
            {
                //db.Config_OptionVal.Add(config_OptionVal);
                //db.SaveChanges();
                //return RedirectToAction("Index");
                foreach(var item in Config_OptionVal)
                {
                    db.Config_OptionVal.Add(item);

                }
                db.SaveChanges();
                return View();
            }
            return View(Config_OptionVal);
            //ViewBag.ConfigurationCollectionID = new SelectList(db.ConfigurationCollection, "ConfigurationCollectionID", "CollectionName", config_OptionVal.ConfigurationCollectionID);
            //ViewBag.OptionValueID = new SelectList(db.OptionValue, "OptionValueID", "OptionVal", config_OptionVal.OptionValueID);
            //return View(config_OptionVal);
        }

        // GET: Config_OptionVal/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Config_OptionVal config_OptionVal = db.Config_OptionVal.Find(id);
            if (config_OptionVal == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConfigurationCollectionID = new SelectList(db.ConfigurationCollection.OrderBy(x=>x.CollectionName), "ConfigurationCollectionID", "CollectionName", config_OptionVal.ConfigurationCollectionID);
            ViewBag.OptionValueID = new SelectList(db.OptionValue.OrderBy(x=>x.OptionVal), "OptionValueID", "OptionVal", config_OptionVal.OptionValueID);
            return View(config_OptionVal);
        }

        // POST: Config_OptionVal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Config_OptionValID,OptionValueID,ConfigurationCollectionID")] Config_OptionVal config_OptionVal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(config_OptionVal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConfigurationCollectionID = new SelectList(db.ConfigurationCollection.OrderBy(x=>x.CollectionName), "ConfigurationCollectionID", "CollectionName", config_OptionVal.ConfigurationCollectionID);
            ViewBag.OptionValueID = new SelectList(db.OptionValue.OrderBy(x=>x.OptionVal), "OptionValueID", "OptionVal", config_OptionVal.OptionValueID);
            return View(config_OptionVal);
        }

        // GET: Config_OptionVal/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Config_OptionVal config_OptionVal = db.Config_OptionVal.Find(id);
            if (config_OptionVal == null)
            {
                return HttpNotFound();
            }
            return View(config_OptionVal);
        }

        // POST: Config_OptionVal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Config_OptionVal config_OptionVal = db.Config_OptionVal.Find(id);
            db.Config_OptionVal.Remove(config_OptionVal);
            db.SaveChanges();
            return RedirectToAction("Index");
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
