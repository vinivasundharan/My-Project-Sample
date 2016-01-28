using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using TEDALS_Ver01.DAL;
using TEDALS_Ver01.Models;
using TEDALS_Ver01.ViewModels;
using TEDALS_Ver01.ViewModels.ExportClass;

namespace TEDALS_Ver01.Controllers
{
    public class LsystemFamiliesController : Controller
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
                    var url = urlHelper.Action("Error", "SetValues");

                    filterContext.Result = new RedirectResult(url);
                }
                if (LsystemID == null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "LsystemFamilies");

                    filterContext.Result = new RedirectResult(url);
                }
            }
        }
        public ActionResult Error()
        {
            ViewBag.Error = "An unhandled Exception occured with the processing of your request";
            return View("Error");
        }
        // GET: LsystemFamilies
        public ActionResult Index(string SearchString)
        {
            try
            {


                //var vm = new Fam_FamList();
                var fam = from lfam in db.LsystemFamily.Where(x=>x.LsystemFamilyID!=62) select lfam;
                //fam = db.LsystemFamily.ToList();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    fam = fam.Where(x => x.FamilyName.Contains(SearchString));
                }
                foreach (var item in fam)
                {
                    int id = item.LsystemFamilyID;
                    int count = db.Lsystem.Where(x => x.LsystemFamilyID == id).Count();
                    item.LsystemCount = count;
                }
                //vm.LsystemFamilies = vm.LsystemFamilies.OrderBy(x => x.FamilyName).ToList();
                //vm.LsystemFamily = new LsystemFamily();
                return View(fam.OrderBy(x => x.FamilyName));
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }


        // GET: LsystemFamilies/Details/5
        public ActionResult Details(int? id)
        {
            try
            {


                if (id == null)
                {
                    return View("Error");
                }
                LsystemFamily lsystemFamily = db.LsystemFamily.Find(id);
                lsystemFamily.LsystemCount = db.Lsystem.Where(x => x.LsystemFamilyID == id).Count();
                if (lsystemFamily == null)
                {
                    return View("Error");
                }
                return View(lsystemFamily);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: LsystemFamilies/Create
        public ActionResult Create()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
                return View();
            else
                return View("AuthorizationError");
        }

        // POST: LsystemFamilies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LsystemFamilyID,FamilyName,LsystemCount,DescriptionEN,DescriptionDE,CreatedOn,ModifiedOn,CreatedBy,ModifiedBy")] LsystemFamily lsystemFamily)
        {
            try
            {


                lsystemFamily.CreatedOn = DateTime.Now;
                lsystemFamily.CreatedBy = User.Identity.Name;
                lsystemFamily.ModifiedOn = DateTime.Now;
                lsystemFamily.ModifiedBy = User.Identity.Name;
                lsystemFamily.LsystemCount = 0;
                if (ModelState.IsValid)
                {
                    if (db.LsystemFamily.Any(x => x.FamilyName.Equals(lsystemFamily.FamilyName)))
                    {
                        ModelState.AddModelError("FamilyName", "Family Name already Exists");
                        return View(lsystemFamily);
                    }

                    db.LsystemFamily.Add(lsystemFamily);
                    db.SaveChanges();
                    return RedirectToAction("Index", db.LsystemFamily.ToList());
                }

                return View(lsystemFamily);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //returns list off all system under a particular family
        public ActionResult ViewSystems (int id)
        {
            try
            {
                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}
                if(id==62)
                   return View("SystemIndependentOptions");

                ViewBag.id = id;
                var viewModel = new LsystemFamily_Lsystem();
                var fam = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == id);
                ViewBag.FamilyName = fam.FamilyName;
                if (id != 0)
                {
                    var system = db.LsystemFamily.Include(x => x.Lsystems).FirstOrDefault(x => x.LsystemFamilyID == id);
                    if (system != null)
                    {
                        viewModel.Lsystems = system.Lsystems.OrderBy(x => x.LsystemName);

                    }
                }
                viewModel.Lsystems.OrderBy(x => x.LsystemName);
                return View(viewModel);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: LsystemFamilies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {


                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                LsystemFamily lsystemFamily = db.LsystemFamily.Find(id);
                if (lsystemFamily == null)
                {
                    ViewBag.Error = "The requested System Family does not exist";
                    return View("Error");
                }
                return View(lsystemFamily);
            }
            else
                return View("AuthorizationError");
        }

        // POST: LsystemFamilies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LsystemFamilyID,FamilyName,LsystemCount,DescriptionEN,DescriptionDE")] LsystemFamily lsystemFamily)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.LsystemFamily.Any(x => x.FamilyName.Equals(lsystemFamily.FamilyName) && x.LsystemFamilyID != lsystemFamily.LsystemFamilyID))
                    {
                        ModelState.AddModelError("FamilyName", "Family Name already exists");
                        return View(lsystemFamily);
                    }
                    //lsystemFamily.LsystemCount = db.Lsystem.Where(x => x.LsystemFamilyID == lsystemFamily.LsystemFamilyID).Count();
                    lsystemFamily.ModifiedOn = DateTime.Now;
                    lsystemFamily.ModifiedBy = User.Identity.Name;
                    db.Entry(lsystemFamily).State = EntityState.Modified;
                    db.Entry(lsystemFamily).Property("CreatedOn").IsModified = false;
                    db.Entry(lsystemFamily).Property("CreatedBy").IsModified = false;
                    db.Entry(lsystemFamily).Property("LsystemCount").IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(lsystemFamily);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: LsystemFamilies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {


                try
                {


                    if (id == null)
                    {
                        return View("Error");
                    }
                    LsystemFamily lsystemFamily = db.LsystemFamily.Find(id);
                    if (lsystemFamily == null)
                    {
                        return View("Error");
                    }
                    return View(lsystemFamily);
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

        // POST: LsystemFamilies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                LsystemFamily lsystemFamily = db.LsystemFamily.Find(id);
                var sys = db.Lsystem.Where(x => x.LsystemFamilyID == lsystemFamily.LsystemFamilyID);
                foreach (var sy in sys)
                {
                    var cc = db.ConfigurationCollection.Where(x => x.LsystemID == sy.LsystemID);
                    foreach (var c in cc)
                    {
                        var cv = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == c.ConfigurationCollectionID);
                        foreach (var item in cv)
                            db.Config_OptionVal.Remove(item);
                        db.ConfigurationCollection.Remove(c);
                    }
                    var op = db.Option.Where(x => x.LsystemID == sy.LsystemID);
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
                    
                        
                    db.Lsystem.Remove(sy);
                }

                db.LsystemFamily.Remove(lsystemFamily);
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

        //Not used
        public ActionResult XmlExportFile(int id)
        {
            var model = new ExportFileName
            {
                FamilyID = id
            };
            return View(model);
        }

        //public ActionResult XmlExport()
        //{
        //    var model = db.LsystemFamily.ToList();
        //    return View(model);
        //}

        //Export a particular system family
        public ActionResult XmlExport(int id)
        {

            // var model = new LsystemFamily();
            // model = db.LsystemFamily.FirstOrDefault(x => x.LsystemCount == 0);
            //// model.Lsystems = db.Lsystem.Where(x => x.LsystemFamilyID == model.LsystemFamilyID);
            // var serializer = new XmlSerializer(typeof(export_test));
            // //XmlSerializer serializer = new XmlSerializer(p.GetType());
            // StreamWriter writer = new StreamWriter(@"C:\Users\vav9sw\Documents\Visual Studio 2013\Projects\TEDALS-Ver01.4\xml\newxml.xml");
            // serializer.Serialize(writer.BaseStream, model);

            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsExporter || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {
                try
                {
                    //if (id == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}



                var model = new XMLSystemFamily();
                var lsystemfamily = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == id);
                model.FamilyName = lsystemfamily.FamilyName;
                model.SystemCount = lsystemfamily.Lsystems.Count();
                model.DescriptionDE = lsystemfamily.DescriptionDE;
                model.DescriptionEN = lsystemfamily.DescriptionEN;

                model.ModifiedOn = lsystemfamily.ModifiedOn;
                model.Systems = new List<XMLSystems>();
                foreach (var item in lsystemfamily.Lsystems)
                {
                    XMLSystems sys = new XMLSystems
                    {
                        SystemName = item.LsystemName,
                        MaterialNumber = item.MaterialNumber,
                        DescriptionDE = item.DescriptionDE,
                        DescriptionEN = item.DescriptionEN,

                        ModifiedOn = item.ModifiedOn,
                        Options = new List<XMLOptions>()
                    };
                    foreach (var op in item.Options)
                    {
                        XMLOptions option = new XMLOptions
                        {
                            Option = op.OptionName,
                            TechnicalCharacteristic = op.TechnicalCharacteristic.TCName,

                            ModifiedOn = op.ModifiedOn,
                            DescriptionDE = op.DescriptionDE,
                            DescriptionEN = op.DescriptionEN,
                            OptionValues = new List<XMLOptionValues>()
                        };
                        foreach (var ov in op.OptionValues)
                        {
                            XMLOptionValues optionvalues = new XMLOptionValues
                            {
                                OptionValue = ov.OptionVal,

                                ModifiedOn = ov.ModifiedOn,
                                DescriptionDE = ov.DescriptionDE,
                                DescriptionEN = ov.DescriptionEN,
                                SetValues = new List<XMLSetValues>()
                            };
                            foreach (var setvalue in ov.SetValue)
                            {
                                XMLSetValues setvalues = new XMLSetValues
                                {

                                    ModifiedOn = setvalue.ModifiedOn,
                                    Property = setvalue.TcSet.SetName,
                                    Value = setvalue.Value,
                                    Unit = setvalue.TcSet.PhysicalUnit,
                                    Status = setvalue.TcSet.DataStatus.ToString()
                                };
                                optionvalues.SetValues.Add(setvalues);
                            }
                            option.OptionValues.Add(optionvalues);
                        }
                        sys.Options.Add(option);
                    }
                    model.Systems.Add(sys);
                }

                var serializer = new XmlSerializer(typeof(XMLSystemFamily));
                var path = String.Format("{0}xmlfiles", AppDomain.CurrentDomain.BaseDirectory);
                FileStream fs = new FileStream("\\\\FE0VMC0643\\TeDaLS\\xmlfiles\\sample.xml", FileMode.OpenOrCreate);
                fs.Close();
                StreamWriter writer = new StreamWriter(fs.Name);
                serializer.Serialize(writer.BaseStream, model);

                writer.Close();
                //System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                //response.StatusCode = 200;

                //response.AddHeader("content-disposition", "attachment; filename=" + fs.Name);
                //response.AddHeader("Content-Transfer-Encoding", "binary");
                //response.AddHeader("Content-Length", _Buffer.Length.ToString());

                // response.ContentType = "application-download";
                //response.TransmitFile(fs.Name);
                string filename = model.FamilyName + "_" + DateTime.Now + ".xml";
                return File(fs.Name, "text/xml", filename);
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

        public ActionResult ExportAll ()
        {
            var model = new XML { XMLFamilyGroup = new List<XMLSystemFamily>() };
            foreach(var fam in db.LsystemFamily.Where(x=>x.LsystemFamilyID!=62).ToList())
            {
                var systemfamily = new XMLSystemFamily();
                systemfamily.FamilyName = fam.FamilyName;
                systemfamily.SystemCount = fam.Lsystems.Count();
                systemfamily.Systems = new List<XMLSystems>();
                systemfamily.DescriptionDE = fam.DescriptionDE;
                systemfamily.DescriptionEN = fam.DescriptionEN;
                systemfamily.ModifiedOn = fam.ModifiedOn;
                foreach(var sys in fam.Lsystems)
                {
                    var system = new XMLSystems();
                    system.SystemName = sys.LsystemName;
                    system.MaterialNumber = sys.MaterialNumber;
                    system.ModifiedOn = sys.ModifiedOn;
                    system.DescriptionEN = sys.DescriptionEN;
                    system.DescriptionDE = sys.DescriptionDE;
                    system.Options = new List<XMLOptions>();
                    foreach(var op in sys.Options)
                    {
                        var opt = new XMLOptions();
                        opt.Option = op.OptionName;
                        opt.TechnicalCharacteristic = op.TechnicalCharacteristic.TCName;
                        opt.ModifiedOn = op.ModifiedOn;
                        opt.DescriptionDE = op.DescriptionDE;
                        opt.DescriptionEN = op.DescriptionEN;
                        opt.OptionValues = new List<XMLOptionValues>();
                        foreach(var ov in op.OptionValues)
                        {
                            var opv = new XMLOptionValues();
                            opv.OptionValue = ov.OptionVal;
                            opv.DescriptionDE= ov.DescriptionDE;
                            opv.DescriptionEN = ov.DescriptionEN;
                            opv.ModifiedOn = ov.ModifiedOn;
                            opv.SetValues = new List<XMLSetValues>();
                            foreach(var sv in ov.SetValue)
                            {
                                var set = new XMLSetValues();
                                set.Property = sv.TcSet.SetName;
                                set.Status = sv.TcSet.DataStatus.ToString();
                                set.Unit = sv.TcSet.PhysicalUnit;
                                set.ModifiedOn = sv.ModifiedOn;
                                set.Value = sv.Value;
                                opv.SetValues.Add(set);
                            }
                            opt.OptionValues.Add(opv);
                        }
                        system.Options.Add(opt);
                    }
                    systemfamily.Systems.Add(system);
                }
                model.XMLFamilyGroup.Add(systemfamily);
            }
            var serializer = new XmlSerializer(typeof(XML));
            var path = String.Format("{0}xmlfiles", AppDomain.CurrentDomain.BaseDirectory);
            FileStream fs = new FileStream("\\\\FE0VMC0643\\TeDaLS\\xmlfiles\\sample.xml", FileMode.OpenOrCreate);
            fs.Close();
            StreamWriter writer = new StreamWriter(fs.Name);
            serializer.Serialize(writer.BaseStream, model);

            writer.Close();
            //System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            //response.StatusCode = 200;

            //response.AddHeader("content-disposition", "attachment; filename=" + fs.Name);
            //response.AddHeader("Content-Transfer-Encoding", "binary");
            //response.AddHeader("Content-Length", _Buffer.Length.ToString());

            // response.ContentType = "application-download";
            //response.TransmitFile(fs.Name);
            string filename = "ExportAll _ " + DateTime.Now + ".xml";
            return File(fs.Name, "text/xml", filename);
            
        }

        //public ActionResult XmlExport(int id)
        //{

        //    // var model = new LsystemFamily();
        //    // model = db.LsystemFamily.FirstOrDefault(x => x.LsystemCount == 0);
        //    //// model.Lsystems = db.Lsystem.Where(x => x.LsystemFamilyID == model.LsystemFamilyID);
        //    // var serializer = new XmlSerializer(typeof(export_test));
        //    // //XmlSerializer serializer = new XmlSerializer(p.GetType());
        //    // StreamWriter writer = new StreamWriter(@"C:\Users\vav9sw\Documents\Visual Studio 2013\Projects\TEDALS-Ver01.4\xml\newxml.xml");
        //    // serializer.Serialize(writer.BaseStream, model);

        //    if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsExporter)
        //    {
        //        try
        //        {

        //            var model = new XMLSystemFamily();
        //            var lsystemfamily = db.LsystemFamily.FirstOrDefault(x => x.LsystemFamilyID == id);
        //            model.FamilyName = lsystemfamily.FamilyName;
        //            model.SystemCount = lsystemfamily.Lsystems.Count();
        //            model.DescriptionDE = lsystemfamily.DescriptionDE;
        //            model.DescriptionEN = lsystemfamily.DescriptionEN;

        //            model.ModifiedOn = lsystemfamily.ModifiedOn;
        //            model.Systems = new List<XMLSystems>();
        //            foreach (var item in lsystemfamily.Lsystems)
        //                    {
        //                        XMLSystems sys = new XMLSystems
        //                        {
        //                            SystemName = item.LsystemName,
        //                            MaterialNumber = item.MaterialNumber,
        //                            DescriptionDE = item.DescriptionDE,
        //                            DescriptionEN = item.DescriptionEN,

        //                            ModifiedOn = item.ModifiedOn,
        //                            Options = new List<XMLOptions>()
        //                        };
        //                        foreach (var op in item.Options)
        //                        {
        //                            XMLOptions option = new XMLOptions
        //                            {
        //                                Option = op.OptionName,
        //                                TechnicalCharacteristic = op.TechnicalCharacteristic.TCName,

        //                                ModifiedOn = op.ModifiedOn,
        //                                DescriptionDE = op.DescriptionDE,
        //                                DescriptionEN = op.DescriptionEN,
        //                                OptionValues = new List<XMLOptionValues>()
        //                            };
        //                            foreach (var ov in op.OptionValues)
        //                            {
        //                                XMLOptionValues optionvalues = new XMLOptionValues
        //                                {
        //                                    OptionValue = ov.OptionVal,

        //                                    ModifiedOn = ov.ModifiedOn,
        //                                    DescriptionDE = ov.DescriptionDE,
        //                                    DescriptionEN = ov.DescriptionEN,
        //                                    SetValues = new List<XMLSetValues>()
        //                                };
        //                                foreach (var setvalue in ov.SetValue)
        //                                {
        //                                    XMLSetValues setvalues = new XMLSetValues
        //                                    {

        //                                        ModifiedOn = setvalue.ModifiedOn,
        //                                        Property = setvalue.TcSet.SetName,
        //                                        Value = setvalue.Value,
        //                                        Unit = setvalue.TcSet.PhysicalUnit,
        //                                        Status = setvalue.TcSet.DataStatus.ToString()
        //                                    };
        //                                    optionvalues.SetValues.Add(setvalues);
        //                                }
        //                                option.OptionValues.Add(optionvalues);
        //                            }
        //                            sys.Options.Add(option);
        //                        }
        //                        model.Systems.Add(sys);
        //                    }
                            

                           
                        
        //                var serializer = new XmlSerializer(typeof(XMLSystemFamily));
        //                var path = String.Format("{0}xmlfiles", AppDomain.CurrentDomain.BaseDirectory);
        //                FileStream fs = new FileStream("\\\\FE0VMC0643\\TeDaLS\\xmlfiles\\sample.xml", FileMode.OpenOrCreate);
        //                fs.Close();
        //                StreamWriter writer = new StreamWriter(fs.Name);
        //                serializer.Serialize(writer.BaseStream, model);

        //                writer.Close();
        //                //System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
        //                //response.StatusCode = 200;

        //                //response.AddHeader("content-disposition", "attachment; filename=" + fs.Name);
        //                //response.AddHeader("Content-Transfer-Encoding", "binary");
        //                //response.AddHeader("Content-Length", _Buffer.Length.ToString());

        //                // response.ContentType = "application-download";
        //                //response.TransmitFile(fs.Name);
        //                string filename = model.FamilyName + "_" + DateTime.Now + ".xml";
        //                return File(fs.Name, "text/xml", filename);
        //            }
                

        //        catch
        //        {
        //            return View("Error");
        //        }
        //    }
        //    else
        //        return View("AuthorizationError");
        //}

    }
}
