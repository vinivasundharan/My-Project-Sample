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
using PagedList;

namespace TEDALS_Ver01.Controllers
{
    public class RevisionHistoriesController : Controller
    {
        private TedalsContext db = new TedalsContext();

        // GET: RevisionHistories
        public ActionResult Index(string SystemName,string Option, string OptionValue, string TCSetName,string Action12,DateTime? from,DateTime? to)
        {
            try
            {


                var rev = from re in db.RevisionHistory select re;
                var cutoff = DateTime.Now.AddDays(-30);
                rev = rev.Where(x => x.ModifiedOn > cutoff).OrderByDescending(x => x.ModifiedOn);

                if (!String.IsNullOrEmpty(SystemName) || !String.IsNullOrEmpty(Option) || !String.IsNullOrEmpty(OptionValue) || !String.IsNullOrEmpty(TCSetName) || !String.IsNullOrEmpty(Action12) || from != null || to != null)
                {
                    if (from != null && to != null)
                    {
                        if (from >= to)
                        {
                            return View("DateMismatchError");
                        }
                        else
                        {
                            rev = db.RevisionHistory.Where(x => x.ModifiedOn >= from && x.ModifiedOn <= to);
                        }
                    }
                    rev = rev.Where(x => x.SystemName.Contains(SystemName));
                    rev = rev.Where(x => x.TCSetName.Contains(TCSetName));
                    rev = rev.Where(x => x.Optionvalue.Contains(OptionValue));
                    rev = rev.Where(x => x.Option.Contains(Option));
                    if (Action12 == "1")
                        rev = rev.Where(x => x.Action == "Created");
                    else if (Action12 == "2")
                        rev = rev.Where(x => x.Action == "Modified");
                    else if (Action12 == "3")
                        rev = rev.Where(x => x.Action == "Deleted");
                    //else
                    //    rev = rev;

                    //if (Modified == "1")
                    //    rev = from re in db.RevisionHistory where Convert.ToDateTime(re.ModifiedOn.AddMonths(1)) > DateTime.Now select re;

                }
                //int pageSize = 5;
                //int PageNo = (Page_No ?? 1);

                return View(rev);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: RevisionHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            RevisionHistory revisionHistory = db.RevisionHistory.Find(id);
            if (revisionHistory == null)
            {
                return View("Error");
            }
            return View(revisionHistory);
        }

        // GET: RevisionHistories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RevisionHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RevisionHistoryID,SystemName,Option,Optionvalue,TCSetName,InitialValue,ModifiedValue,CreatedOn,ModifiedOn,ModifiedBy")] RevisionHistory revisionHistory)
        {
            if (ModelState.IsValid)
            {
                db.RevisionHistory.Add(revisionHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(revisionHistory);
        }

        // GET: RevisionHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionHistory revisionHistory = db.RevisionHistory.Find(id);
            if (revisionHistory == null)
            {
                return HttpNotFound();
            }
            return View(revisionHistory);
        }

        // POST: RevisionHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RevisionHistoryID,SystemName,Option,Optionvalue,TCSetName,InitialValue,ModifiedValue,CreatedOn,ModifiedOn,ModifiedBy")] RevisionHistory revisionHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revisionHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(revisionHistory);
        }

        // GET: RevisionHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevisionHistory revisionHistory = db.RevisionHistory.Find(id);
            if (revisionHistory == null)
            {
                return HttpNotFound();
            }
            return View(revisionHistory);
        }

        // POST: RevisionHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RevisionHistory revisionHistory = db.RevisionHistory.Find(id);
            db.RevisionHistory.Remove(revisionHistory);
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
