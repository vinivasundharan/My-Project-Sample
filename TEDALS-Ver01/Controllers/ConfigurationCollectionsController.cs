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
using TEDALS_Ver01.ViewModels.ConfigurationCollection;

namespace TEDALS_Ver01.Controllers
{
    public class ConfigurationCollectionsController : Controller
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
                    var url = urlHelper.Action("Error", "ConfigurationCollections");

                    filterContext.Result = new RedirectResult(url);
                }
                if(LsystemID==null)
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

        // GET: ConfigurationCollections
        public ActionResult Index()
        {
            try
            {
                var configurationCollection = db.ConfigurationCollection.Include(c => c.Lsystem);
                return View(configurationCollection.ToList());
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: ConfigurationCollections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(id);
            if (configurationCollection == null)
            {
                ViewBag.Error = "The requested configuration collection do not exist";
                return View("Error");
            }
            return View(configurationCollection);
        }

        //DispalyAll Configurations for a system
        public ActionResult DisplayAll(int id)
        {
            var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
            var config = db.ConfigurationCollection.Where(x => x.LsystemID == id).ToList();

            ViewBag.LsystemID = id;
            ViewBag.LsystemFamilyID = sys.LsystemFamilyID;

            var model = new  List<ConfigAllList>();
            foreach(var con in config)
            {
                var modelitem = new ConfigAllList { ConfigurationName = con.CollectionName, OpOvList = new List<OptionOptionValList>(), ConfigID=con.ConfigurationCollectionID };
                foreach(var op in sys.Options.OrderBy(x=>x.OptionName))
                {
                    var opov = new OptionOptionValList { OptionName = op.OptionName, Optionvalue = new List<string>() };
                    foreach (var ov in db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == con.ConfigurationCollectionID && x.OptionID == op.OptionID))
                        opov.Optionvalue.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov.OptionValueID).OptionVal);
                    if (opov.Optionvalue.Count == 0)
                        opov.Optionvalue.Add("No Value");
                    modelitem.OpOvList.Add(opov);
                }
                foreach(var op in db.Option.Where(x=>x.LsystemID==1038).OrderBy(x=>x.OptionName))
                {
                    var opov = new OptionOptionValList { OptionName = op.OptionName, Optionvalue = new List<string>() };
                    foreach (var ov in db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == con.ConfigurationCollectionID && x.OptionID == op.OptionID))
                        opov.Optionvalue.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov.OptionValueID).OptionVal);
                    if (opov.Optionvalue.Count == 0)
                        opov.Optionvalue.Add("No Value");
                    modelitem.OpOvList.Add(opov);
                }
                model.Add(modelitem);
            }
            ViewBag.Option = db.Lsystem.FirstOrDefault(x => x.LsystemID == id).Options.OrderBy(x=>x.OptionName).ToList();
            ViewBag.Independent = db.Lsystem.FirstOrDefault(x => x.LsystemID == 1038).Options.OrderBy(x=>x.OptionName).ToList();
            return View(model);
        }

        //Display the name of the Configurations available for a particular system : Not required
        
        public ActionResult ViewConfiguration(int id)
        {
            try
            {
                ViewBag.LsystemID = id;
                var configuration = new List<ConfigurationCollection>();
                foreach (var item in db.ConfigurationCollection.Where(x => x.LsystemID == id))
                {
                    configuration.Add(item);
                }
                return View(configuration);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Return configuration details when a ConfigurationID is passed : Called from ListAllConfiguration() : Function not used
        
        public List<ListConfiguration> ListConfiguration(int id)
        {
            var model = new List<ListConfiguration>();
            int i = 0,p=0;
            var opids = new List<int>();
            int opid;
            var system = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == id).Lsystem;
            var Config_OptionVal = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == id).ToList();
            //foreach(var item in Config_OptionVal)
            //{
            //    var op = db.Option.FirstOrDefault(x => x.OptionID == item.OptionID);
            //    op.OptionValues = ;
            //}
            var list = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == id).OrderBy(x=>x.OptionID).ToList();
            foreach(var item in list)
            {
                opid = item.OptionID;
                if(!opids.Contains(opid))
                {
                    opids.Add(opid);
                }
            }
            opids.Sort();
            p=0;
            for (i = 0; i < opids.Count();i++ )
            {
                var listItem = new ListConfiguration
                {
                    Option = new Option(),
                    OptionValue = new List<OptionValue>()
                };
                opid = opids[i];
                listItem.Option = db.Option.FirstOrDefault(x => x.OptionID == opid);
                foreach(var opv in db.Config_OptionVal.Where(x=>x.OptionID==opid&&x.ConfigurationCollectionID==id))
                {
                    if(opv.OptionValue!=null)
                    listItem.OptionValue.Add(opv.OptionValue);
                }
                model.Add(listItem);
            }
                return model;
        }

        //View complete details of a particular Configuration      
        public ActionResult ListConfigurations(int id)
        {
            try
            {
                
                var config = db.ConfigurationCollection.FirstOrDefault(x=>x.ConfigurationCollectionID==id);
                ViewBag.LsystemName = db.Lsystem.FirstOrDefault(x => x.LsystemID == config.LsystemID).LsystemName;
                ViewBag.FamilyName = db.Lsystem.FirstOrDefault(x => x.LsystemID == config.LsystemID).LsystemFamily.FamilyName;
                var model = new ConfigAllList { ConfigID = id, ConfigurationName = config.CollectionName, OpOvList = new List<OptionOptionValList>() };
                foreach(var op in db.Lsystem.FirstOrDefault(x=>x.LsystemID==config.LsystemID).Options.OrderBy(x=>x.OptionName))
                {
                    var opov = new OptionOptionValList { OptionName = op.OptionName, Optionvalue = new List<string>() };
                    foreach(var ov in db.Config_OptionVal.Where(x=>x.ConfigurationCollectionID==id&&x.OptionID==op.OptionID))
                        opov.Optionvalue.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov.OptionValueID).OptionVal);
                    if (opov.Optionvalue.Count == 0)
                        opov.Optionvalue.Add("No Value");
                    model.OpOvList.Add(opov);
                }
                foreach (var op in db.Option.Where(x => x.LsystemID == 1038).OrderBy(x => x.OptionName))
                {
                    var opov = new OptionOptionValList { OptionName = op.OptionName, Optionvalue = new List<string>() };
                    foreach (var ov in db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == config.ConfigurationCollectionID && x.OptionID == op.OptionID))
                        opov.Optionvalue.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov.OptionValueID).OptionVal);
                    if (opov.Optionvalue.Count == 0)
                        opov.Optionvalue.Add("No Value");
                    model.OpOvList.Add(opov);
                }

                ViewBag.Option = db.Lsystem.FirstOrDefault(x => x.LsystemID == config.LsystemID).Options.OrderBy(x => x.OptionName).ToList();
                ViewBag.Independent = db.Lsystem.FirstOrDefault(x => x.LsystemID == 1038).Options.OrderBy(x => x.OptionName).ToList();
                return View(model);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //List all configurations along with the details associated with a particular system : Changed to display all     
        public ActionResult ListAllConfiguration(int id)
        {
            try
            {

                //if (id == null)
                //{
                //    ViewBag.Error = "A null parameter was passed to the function";
                //    return View("Error");
                //}
                ViewBag.LsystemID = id;
                ViewBag.LsystemName = db.Lsystem.FirstOrDefault(x => x.LsystemID == id).LsystemName;
                ViewBag.FamilyName = db.Lsystem.FirstOrDefault(x => x.LsystemID == id).LsystemFamily.FamilyName;
                ViewBag.LsystemFamilyID = db.Lsystem.FirstOrDefault(x => x.LsystemID == id).LsystemFamilyID;
                int[] configID = new int[1000];
                int i = 0;
                var model = new List<ListAllConfiguration>();
                foreach (var item in db.ConfigurationCollection.Where(x => x.LsystemID == id))
                {
                    configID[i++] = item.ConfigurationCollectionID;
                }
                for (i = 0; i < configID.Count() && configID[i] != 0; i++)
                {
                    var listItem = ListConfiguration(configID[i]);
                    var Elem = new ListAllConfiguration
                    {
                        ConfigurationCollection = new ConfigurationCollection(),
                        ListConfiguration = new List<ListConfiguration>()
                    };
                    int conid = configID[i];
                    Elem.ConfigurationCollection = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == conid);
                    Elem.ListConfiguration = listItem;
                    model.Add(Elem);
                }
                ViewBag.count = 0;
                int pqr = configID[0];
                var poption = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == pqr).OrderBy(x=>x.OptionID).ToList();
                var somelist = new List<Option>();
                foreach (var item in poption)
                    somelist.Add(db.Option.FirstOrDefault(x => x.OptionID == item.OptionID));
                ViewBag.Option = somelist.Distinct();
                ViewBag.Optioncount = 0;
                return View(model);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: ConfigurationCollections/Create        
        public ActionResult Create(int LsystemID)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {
                    //if (LsystemID == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    var model = new ConfigurationCollection
                    {
                        LsystemID = LsystemID,
                        Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == LsystemID),
                        Options = db.Option.Where(x=>x.LsystemID==LsystemID || x.LsystemID==1038).OrderBy(x=>x.OptionName).ToList(),
                        OptionValues = new List<OptionValue>()
                    };
                    
                    //ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily, "LsystemFamilyID", "FamilyName");
                    //ViewBag.LsystemID = new SelectList(db.Lsystem, "LsystemID", "LsystemName");
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

        // POST: ConfigurationCollections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ConfigurationCollectionID,CollectionName,DescriptionEN,DescriptionDE,LsystemID")] ConfigurationCollection configurationCollection, int[] OptionValues)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.ConfigurationCollection.Any(x => x.CollectionName.Equals(configurationCollection.CollectionName)))
                    {
                        ModelState.AddModelError("CollectionName", "Collection Name already exists");
                        //ViewBag.LsystemID = new SelectList(db.Lsystem, "LsystemID", "LsystemName", configurationCollection.LsystemID);
                        configurationCollection.Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        configurationCollection.OptionValues = new List<OptionValue>();
                        configurationCollection.Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList();
                        return View(configurationCollection);
                    }
                    if(OptionValues==null)
                    {
                        ModelState.AddModelError("CollectionName", "Configuration Cannot be empty. Select Values for Options");
                        configurationCollection.Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        configurationCollection.OptionValues = new List<OptionValue>();
                        configurationCollection.Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList();
                        return View(configurationCollection);
                    }
                    var opl = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID).Options.ToList();
                    var opcheck = new List<Option>();
                    foreach(var ov in OptionValues)
                        opcheck.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).Option);
                    bool unavailable = false;
                    foreach (var item in opl)
                        if (!opcheck.Contains(item))
                            unavailable = true;

                    if(unavailable)
                    {
                        ModelState.AddModelError("", "Option values for all Options in the system has not been selected");
                        configurationCollection.Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        configurationCollection.OptionValues = new List<OptionValue>();
                        configurationCollection.Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList();
                        return View(configurationCollection);
                    }
                    configurationCollection.CreatedBy = User.Identity.Name;
                    configurationCollection.CreatedOn = DateTime.Now;
                    configurationCollection.ModifiedBy = User.Identity.Name;
                    configurationCollection.ModifiedOn = DateTime.Now;
                    db.ConfigurationCollection.Add(configurationCollection);
                    db.SaveChanges();
                    
                    foreach(var item in OptionValues)
                    {
                        var config_ov = new Config_OptionVal();
                        var con = configurationCollection;
                        var ov = db.OptionValue.Find(item);
                        config_ov.ConfigurationCollection = con;
                        config_ov.OptionValue = ov;
                        config_ov.ConfigurationCollectionID = con.ConfigurationCollectionID;
                        config_ov.OptionValueID = ov.OptionValueID;
                        config_ov.OptionID = ov.OptionID;
                        db.Config_OptionVal.Add(config_ov);
                        db.SaveChanges();

                    }

                    return RedirectToAction("DisplayAll", "ConfigurationCollections", new { id= configurationCollection.LsystemID});
                }

                configurationCollection.Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                configurationCollection.Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList();
                configurationCollection.OptionValues = new List<OptionValue>();
                return View(configurationCollection);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: ConfigurationCollections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            
            {


                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(id);
               
                if (configurationCollection == null)
                {
                    ViewBag.Error = "The requested Configuration collection do not exist";
                    return View("Error");
                }
                var model = new EditConfigVM
                {
                    ConfigCol = configurationCollection,
                    selected = new List<OptionValue>(),
                    Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).OrderBy(x=>x.OptionName).ToList()
                };
                var sel = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == id).ToList();
                var Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                model.ConfigCol.OptionValues = new List<OptionValue>();
                foreach (var item in sel)
                    model.selected.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == item.OptionValueID));
                //ViewBag.LsystemFamilyID = new SelectList(db.LsystemFamily, "LsystemFamilyID", "FamilyName");
                //ViewBag.LsystemID = new SelectList(db.Lsystem, "LsystemID", "LsystemName");

                ViewBag.LsystemID = configurationCollection.LsystemID;
                return View(model);
            }
            else
                return View("AuthorizationError");
        }

        // POST: ConfigurationCollections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConfigCol,selected")] EditConfigVM editVM,int[] OptionValues)
        {
            try
            {
                var con12 = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID);
                ViewBag.LsystemID = con12.LsystemID;
                if (ModelState.IsValid)
                {
                    ConfigurationCollection concol = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID);
                    if (db.ConfigurationCollection.Any(x => x.CollectionName.Equals(editVM.ConfigCol.CollectionName) && x.ConfigurationCollectionID != editVM.ConfigCol.ConfigurationCollectionID))
                    {
                        ModelState.AddModelError("CollectionName", "Colelction Name already exists");
                        ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(editVM.ConfigCol.ConfigurationCollectionID);
                        var model = new EditConfigVM
                        {
                            ConfigCol = configurationCollection,
                            selected = new List<OptionValue>(),
                            Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList()
                        };
                        var sel = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID).ToList();
                        var Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        model.ConfigCol.OptionValues = new List<OptionValue>();
                        foreach (var item in sel)
                            model.selected.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == item.OptionValueID));
                        ViewBag.LsystemID = new SelectList(db.Lsystem.OrderBy(x=>x.LsystemName), "LsystemID", "LsystemName", editVM.ConfigCol.LsystemID);
                        return View(editVM);
                    }
                    if (OptionValues == null)
                    {
                        ModelState.AddModelError("", "Configuration Cannot be empty. Select Values for Options");
                        ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(editVM.ConfigCol.ConfigurationCollectionID);
                        var model = new EditConfigVM
                        {
                            ConfigCol = configurationCollection,
                            selected = new List<OptionValue>(),
                            Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList()
                        };
                        var sel = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID).ToList();
                        var Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        model.ConfigCol.OptionValues = new List<OptionValue>();
                        foreach (var item in sel)
                            model.selected.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == item.OptionValueID));
                        model.ConfigCol.Lsystem = Lsystem;
                        return View(model);
                    }
                    
                    var opl = db.Lsystem.FirstOrDefault(x => x.LsystemID == con12.LsystemID).Options.ToList();
                    var opcheck = new List<Option>();
                    foreach (var ov in OptionValues)
                        opcheck.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).Option);
                    bool unavailable = false;
                    foreach (var item in opl)
                        if (!opcheck.Contains(item))
                            unavailable = true;

                    if (unavailable)
                    {
                        ModelState.AddModelError("", "Option values for all Options in the system has not been selected");
                        ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(editVM.ConfigCol.ConfigurationCollectionID);
                        var model = new EditConfigVM
                        {
                            ConfigCol = configurationCollection,
                            selected = new List<OptionValue>(),
                            Options = db.Option.Where(x => x.LsystemID == configurationCollection.LsystemID || x.LsystemID == 1038).ToList()
                        };
                        var sel = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID).ToList();
                        var Lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == configurationCollection.LsystemID);
                        model.ConfigCol.OptionValues = new List<OptionValue>();
                        foreach (var item in sel)
                            model.selected.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == item.OptionValueID));
                        model.ConfigCol.Lsystem = Lsystem;
                        return View(model);
                    }

                    concol.ModifiedOn = DateTime.Now;
                    concol.ModifiedBy = User.Identity.Name;
                    concol.CollectionName = editVM.ConfigCol.CollectionName;
                    concol.DescriptionDE = editVM.ConfigCol.DescriptionDE;
                    concol.DescriptionEN = editVM.ConfigCol.DescriptionEN;
                    
                    db.Entry(concol).State = EntityState.Modified;
                    db.Entry(concol).Property(x => x.CreatedOn).IsModified = false;
                    db.Entry(concol).Property(x => x.CreatedBy).IsModified = false;
                    db.SaveChanges();
                    var dellist = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == editVM.ConfigCol.ConfigurationCollectionID).ToList();
                    foreach (var sel in dellist)
                        db.Config_OptionVal.Remove(sel);

                    foreach (var item in OptionValues)
                    {
                        var config_ov = new Config_OptionVal();
                        var con = concol;
                        var ov = db.OptionValue.Find(item);
                        config_ov.ConfigurationCollection = con;
                        config_ov.OptionValue = ov;
                        config_ov.ConfigurationCollectionID = con.ConfigurationCollectionID;
                        config_ov.OptionValueID = ov.OptionValueID;
                        config_ov.OptionID = ov.OptionID;
                        db.Config_OptionVal.Add(config_ov);
                        db.SaveChanges();

                    }
                    
                    return RedirectToAction("DisplayAll", "ConfigurationCollections", new { id= con12.LsystemID});
                }
                
                editVM.selected = new List<OptionValue>();
                editVM.Options = db.Option.Where(x=>x.LsystemID==editVM.ConfigCol.LsystemID||x.LsystemID==1038).ToList();
                return View(editVM);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: ConfigurationCollections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(id);
                if (configurationCollection == null)
                {
                    ViewBag.Error = "The requested COnfiguration Collection do not exist";
                    return View("Error");
                }
                return View(configurationCollection);
            }
            else return View("AuthorizationError");
        }

        // POST: ConfigurationCollections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ConfigurationCollection configurationCollection = db.ConfigurationCollection.Find(id);
                var cv = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == id);
                foreach(var item in cv)
                
                    db.Config_OptionVal.Remove(item);
                

                db.ConfigurationCollection.Remove(configurationCollection);
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


        public void GenerateView(int ConfigID)
        {
            
        }



    }
}
