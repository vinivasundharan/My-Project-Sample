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
using TEDALS_Ver01.ViewModels.Views123;

namespace TEDALS_Ver01.Controllers
{
    public class Views123Controller : Controller
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
                var ViewsID = parameter as int?;
                if (id == null || ViewsID==null||LsystemID==null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "Views123");

                    filterContext.Result = new RedirectResult(url);
                }
                //if (LsystemID == null)
                //{
                //    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                //    //ViewBag.Error = "An unhandled exception occured";
                //    var url = urlHelper.Action("Error", "SetValues");

                //    filterContext.Result = new RedirectResult(url);
                //}
            }
        }

        public ActionResult Error()
        {
            ViewBag.Error = "An unhandled Exception occured with the processing of your request";
            return View("Error");
        }

        // GET: Views
        public ActionResult Index()
        {
            try
            {
                return View(db.Views.OrderBy(x => x.ViewsName).ToList());
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Get the list of systems to be used in the view Both Get and Post method
        public ActionResult UseView (int id, int[] Lsystem, bool required=false)
        {
            try
            {


                if (Lsystem != null && required == true)
                {
                    TempData["lsys"] = Lsystem;

                    return RedirectToAction("ViewSummarize", "Views123", new { ViewsID = id });
                }
                else if (Lsystem != null && required == false)
                {
                    TempData["lsys"] = Lsystem;

                    return RedirectToAction("ViewsResultsWithout", "Views123", new { ViewsID = id });
                }
                ViewBag.ViewsID = id;
                ViewBag.ViewName = db.Views.FirstOrDefault(x => x.ViewsID == id).ViewsName;
                ViewBag.Lsystem = new MultiSelectList(db.Lsystem.Where(x => x.LsystemFamilyID != 62).OrderBy(x => x.LsystemName).ToList(), "LsystemID", "LsystemName");
                ViewBag.Required = "Summarize Option values";
                ViewBag.size = db.Lsystem.Count() - 1;
                return View();
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Finds all combinations of Option values required to generate the View(all option values not required are not permuted)
        public List<string> PermuteConfig (int ConfigID, int ViewsID)
        {
            var Views = db.Views.FirstOrDefault(x => x.ViewsID == ViewsID);
            var Viewprop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
            var reqprop = new List<string>();
            foreach (var vp in Viewprop)
                if (!reqprop.Contains(db.TcSet.FirstOrDefault(x => x.TcSetID == vp.TcSetID).SetName))
                    reqprop.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == vp.TcSetID).SetName);

            var tclist = new List<TechnicalCharacteristic>();
            foreach(var tc in db.TechnicalCharacteristic)
            {
                foreach(var tcset in tc.TcSets)
                {
                    if(reqprop.Contains(tcset.SetName))
                    {
                        tclist.Add(tc);
                        break;
                    }
                }
            }
            var configproplist = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == ConfigID).ToList();
            var oplist = new List<int>();
            foreach (var c in configproplist)
                if (!oplist.Contains(c.OptionID))
                    oplist.Add(c.OptionID);
            var op_ovlist = new List<Tuple<int, List<int>>>();
            foreach (var op in oplist.ToList())
                if (!tclist.Contains(db.Option.FirstOrDefault(x => x.OptionID == op).TechnicalCharacteristic))
                    oplist.Remove(op);
            foreach(var opid in oplist.OrderBy(x=>x).ToList())
            {
                var ovlist = new List<int>();
                foreach(var c in configproplist)
                {
                    if (opid == c.OptionID)
                        ovlist.Add(c.OptionValueID);
                }
                op_ovlist.Add(new Tuple<int, List<int>>(opid, ovlist));
            }
            var result = new List<int>();
            var first = new List<int>();
            first = op_ovlist[0].Item2;
            var ans = new List<string>();
            bool flag = false;
            foreach(var item in op_ovlist)
            {
                if (!flag)
                {
                    foreach(var o in item.Item2)
                        ans.Add(o.ToString());
                    flag = true;
                }
                else
                {
                    var innerlist = new List<string>();
                    for(int i=0;i<ans.Count();i++)
                    {
                        for(int j=0;j<item.Item2.Count();j++)
                        {
                            string inner = ans[i] + "delim" + item.Item2[j].ToString();
                            innerlist.Add(inner);
                        }
                    }
                    ans = innerlist;
                }
            }
            return ans;

        }

        //ViewsResult without required attribue (without summarize)
        public ActionResult ViewsResultsWithout (int ViewsID)
        {
            try
            {


                var model = new List<View_Without>();

                //tuple <tcsetid, tcid>
                var reqprop = new List<Tuple<int, int>>();
                var reqpropname = new List<string>();
                var viewprop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
                foreach (var item in viewprop)
                {
                    string setname = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName;
                    if (!reqpropname.Contains(setname))
                        reqpropname.Add(setname);
                    foreach (var s in db.TcSet.Where(x => x.SetName.Equals(setname)))
                        reqprop.Add(new Tuple<int, int>(s.TcSetID, s.TechnicalCharacteristicID));
                }
                var TechList = new List<TechnicalCharacteristic>();
                foreach (var tc in db.TechnicalCharacteristic.ToList())
                {
                    foreach (var tcset in tc.TcSets)
                    {
                        if (reqpropname.Contains(tcset.SetName))
                        {
                            TechList.Add(tc);
                            break;
                        }
                    }
                }
                var tclist = new List<TechnicalCharacteristic>();
                var reqproplist = new List<TcSet>();
                foreach (var item in reqprop)
                {
                    if (!tclist.Contains(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == item.Item2)))
                        tclist.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == item.Item2));
                    if (!reqproplist.Contains(db.TcSet.FirstOrDefault(x => x.TcSetID == item.Item1)))
                        reqproplist.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.Item1));
                }
                reqproplist = reqproplist.OrderBy(x => x.TechnicalCharacteristicID).ToList();
                tclist = tclist.OrderBy(x => x.TechnicalCharacteristicID).ToList();
                foreach (var item in TempData["lsys"] as int[])
                {

                    var configlist = db.ConfigurationCollection.Where(x => x.LsystemID == item).OrderBy(x=>x.ConfigurationCollectionID).ToList();

                    foreach (var con in configlist)
                    {
                        var configmodel = new config_combi_VM { configcombi = new List<string>(), ConfigurationCollection = con };
                        var ans = PermuteConfig(con.ConfigurationCollectionID, ViewsID);
                        var ov_row_list = new List<List<int>>();

                        foreach (var s in ans)
                        {
                            var ovsplit = s.Split(new string[] { "delim" }, StringSplitOptions.None);
                            var innerlist = new List<int>();
                            foreach (var split in ovsplit)
                                innerlist.Add(Convert.ToInt32(split));
                            ov_row_list.Add(innerlist);
                        }

                        foreach (var ovrow in ov_row_list.ToList())
                        {

                            foreach (var ov in ovrow.ToList())
                            {
                                var tc = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).Option.TechnicalCharacteristic;
                                if (!TechList.Contains(tc))
                                    ovrow.Remove(ov);
                            }
                        }

                        foreach (var ovrow in ov_row_list.Distinct().ToList())
                        {
                            var modelitem = new View_Without { Option_Opval = new List<Option_Opval>(), PropValues = new List<string>() , CalcAns = new List<Calc_Ans>()};
                            modelitem.Systemname = db.Lsystem.FirstOrDefault(x => x.LsystemID == item).LsystemName;
                            var propresult = new List<Tuple<string, string>>();
                            foreach (var ov in ovrow.ToList())
                            {
                                var op_opv = new Option_Opval();
                                op_opv.OptionVal = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).OptionVal;
                                op_opv.Option = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).Option.OptionName;
                                foreach (var setval in db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov).SetValue)
                                {
                                    if (reqpropname.Contains(setval.TcSet.SetName))
                                    {
                                        propresult.Add(new Tuple<string, string>(setval.TcSet.SetName, setval.Value));
                                    }
                                }
                                modelitem.Option_Opval.Add(op_opv);
                            }
                            var insertedlist = new List<string>();
                            for (int v = 0; v < propresult.Count(); v++)
                            {
                                if (!insertedlist.Contains(propresult[v].Item1))
                                    insertedlist.Add(propresult[v].Item1);

                            }

                            foreach (var l in reqpropname)
                                if (!insertedlist.Contains(l))
                                    propresult.Add(new Tuple<string, string>(l, "no value"));

                            propresult = propresult.OrderBy(x => x.Item1).ToList();
                            foreach (var p in propresult)
                                modelitem.PropValues.Add(p.Item2);
                            model.Add(modelitem);
                        }

                    }

                }
                if (model.Count() == 0)
                {
                    return View("Empty");
                }

                ViewBag.Property = reqpropname.OrderBy(q => q).ToList();
                model.OrderByDescending(x => x.Option_Opval.Count());
                var maxcount = model[0].Option_Opval.Count();
                foreach (var item in model)
                {
                    while (item.Option_Opval.Count() != maxcount)
                    {
                        var p = new Option_Opval();
                        p.Option = "null";
                        p.OptionVal = "null";
                        item.Option_Opval.Add(p);
                    }
                }
                ViewBag.count = model[0].Option_Opval.Count();
                return View(model);
            }

            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //View result with required attribute (use summarize) : Not required (Changed to ViewsSummarize).
        public void ViewsResultSummarize()
        {
            
            int ViewsID = 25;
            var viewprop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();

            //Populating property names in the view
            var reqpropname = new List<string>();
            foreach(var item in viewprop)
                if (!reqpropname.Contains(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName))
                    reqpropname.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName);

            //Populating technical characteristic associated with each property name
            var tclist = new List<TechnicalCharacteristic>();
            foreach(var item in db.TechnicalCharacteristic.ToList())
            {
                foreach(var s in item.TcSets)
                {
                    if (reqpropname.Contains(s.SetName))
                    {
                        tclist.Add(item);
                        break;
                    }
                        
                }
            }

            foreach(var sysid in TempData["lsys"] as int[])
            {
                string SystemName = db.Lsystem.FirstOrDefault(x => x.LsystemID == sysid).LsystemName;
                var oplist = db.Lsystem.FirstOrDefault(x => x.LsystemID == sysid).Options.ToList();
                
                var optclist = new List<TechnicalCharacteristic>();
                foreach (var op in oplist)
                    optclist.Add(op.TechnicalCharacteristic);

                var configcol = db.ConfigurationCollection.Where(x => x.LsystemID == sysid).ToList();
                var reducelist = new List<Reduce_OVlist>();
                foreach(var con in configcol)
                {
                    var configprop = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == con.ConfigurationCollectionID).ToList();
                    
                    //get list of all options in configuration collection
                    var temp = new List<int>();
                    foreach (var cp in configprop.OrderBy(x => x.OptionID))
                        if (!temp.Contains(cp.OptionID))
                            temp.Add(cp.OptionID);

                    //get list of all option and associated option values : eg: {op1,<ov1,ov2,ov3>},{op2,<ov1,ov2>}
                    var opovlist = new List<Tuple<int, List<int>>>();
                    foreach(var t in temp)
                    {
                        var tempov = new List<int>();
                        foreach(var cp in configprop.Where(x=>x.OptionID==t).ToList())
                            tempov.Add(cp.OptionValueID);
                        opovlist.Add(new Tuple<int, List<int>>(t, tempov));
                    }
                    
                    foreach(var item in opovlist)
                    {
                        var prop_val_string = new List<string>();
                        var reduceovlist = new List<List<int>>();
                        var opid = item.Item1;
                        var ovidlist = item.Item2;
                        var tc = db.Option.FirstOrDefault(x => x.OptionID == opid).TechnicalCharacteristic;
                        if(tclist.Contains(tc))
                        {
                            var tempans = new List<Tuple<int,string, string>>();
                            var ov_set_val = new List<VRS_Ov_setname_val>();
                            foreach(var ov in ovidlist)
                            {
                                var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ov);
                                var listitem = new VRS_Ov_setname_val { OptionValueID = ov, Set_val_list = new List<Set_Val>() };
                                foreach(var s in opv.SetValue)
                                {
                                    
                                    if(reqpropname.Contains(s.TcSet.SetName))
                                    {
                                        var setvalitem = new Set_Val { setname = s.TcSet.SetName, value = s.Value };
                                        listitem.Set_val_list.Add(setvalitem);
                                    }
                                }
                                ov_set_val.Add(listitem);
                            }
                            ov_set_val.OrderBy(x => x.Set_val_list.OrderBy(y => y.setname));
                            
                            foreach(var o in ov_set_val)
                            {
                                string str="";
                                foreach (var s in o.Set_val_list)
                                    str = str + s.setname + s.value;
                                prop_val_string.Add(str);
                            }
                            if(prop_val_string.Distinct().Count()!= prop_val_string.Count())
                            {
                                var reduceitem = new Reduce_OVlist { Optionid = opid, OptionvalID = new List<int>() };
                                var seenlist = new List<string>();

                                for(int i = 0;i<prop_val_string.Count();i++)
                                {
                                    var ovlist = new List<int>();
                                    if(!seenlist.Contains(prop_val_string[i]))
                                    {
                                        seenlist.Add(prop_val_string[i]);
                                        
                                        ovlist.Add(i);
                                        for(int j=i+1;j<prop_val_string.Count();j++)
                                        {
                                            if (prop_val_string[i] == prop_val_string[j])
                                                ovlist.Add(j);
                                        }
                                    }
                                    reduceitem.OptionvalID = ovlist;
                                }
                                reducelist.Add(reduceitem);

                            }
                            else
                            {
                                
                                foreach(var r in  ovidlist)
                                {
                                    var reduceitem = new Reduce_OVlist { Optionid = opid, OptionvalID = new List<int>() };
                                    reduceitem.OptionvalID.Add(r);
                                    reducelist.Add(reduceitem);
                                }
                            }
                        }                       
                    }
                }
            }
        }

        //View result with summarize
        public ActionResult ViewSummarize(int ViewsID)
        {
            try
            {


                var model = new List<ViewSummarize_FinalResult>();
                var ViewProp = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
                var reqprop = new List<string>();
                foreach (var item in ViewProp)
                {
                    var setname = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName;
                    if (!reqprop.Contains(setname))
                        reqprop.Add(setname);
                }

                var tclist = new List<TechnicalCharacteristic>();
                foreach (var item in db.TechnicalCharacteristic)
                {
                    foreach (var tcset in item.TcSets)
                    {
                        if (reqprop.Contains(tcset.SetName))
                        {
                            tclist.Add(item);
                            break;
                        }
                    }
                }

                reqprop = reqprop.OrderBy(x => x).ToList();
                foreach (var sysid in TempData["lsys"] as int[])
                {
                    var reductionlist = new List<ViewSummarize_ConfigPermute>();
                    var con = db.ConfigurationCollection.Where(x => x.LsystemID == sysid).ToList();
                    foreach (var c in con)
                    {
                        var configpermute = new ViewSummarize_ConfigPermute { ConfigID = c.ConfigurationCollectionID, OptionValCombi = new List<string>() };
                        configpermute.OptionValCombi = PermuteConfig(c.ConfigurationCollectionID, ViewsID);

                        var OptionvalueList = new List<ViewSummarize_OptionValueCombi>();
                        foreach (var s in configpermute.OptionValCombi)
                        {
                            var ovsplit = s.Split(new string[] { "delim" }, StringSplitOptions.None);
                            var inner = new ViewSummarize_OptionValueCombi { ConfigID = c.ConfigurationCollectionID, OptionValue = new List<OptionValue>() };
                            foreach (var p in ovsplit)
                            {
                                int id = Convert.ToInt32(p);
                                inner.OptionValue.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == id));
                            }
                            OptionvalueList.Add(inner);
                        }

                        var CompleteResult = new List<ViewSummarize_CompleteResult>();
                        foreach (var ovl in OptionvalueList)
                        {
                            var inner = new ViewSummarize_CompleteResult { EachRow = new List<Result_EachRow>() };
                            foreach (var ov in ovl.OptionValue)
                            {
                                foreach (var s in ov.SetValue)
                                {
                                    if (reqprop.Contains(s.TcSet.SetName))
                                    {
                                        var eachrow = new Result_EachRow { setname = s.TcSet.SetName, value = s.Value, OptionValID = ov.OptionValueID };
                                        inner.EachRow.Add(eachrow);
                                    }
                                }
                                inner.EachRow.OrderBy(x => x.setname).ToList();
                            }
                            CompleteResult.Add(inner);
                        }
                        var allrowstring = new List<string>();
                        foreach (var cr in CompleteResult)
                        {
                            string eachrowval = "";

                            foreach (var er in cr.EachRow.OrderBy(x => x.setname))
                            {
                                eachrowval = eachrowval + "delim" + er.value;
                            }
                            allrowstring.Add(eachrowval);
                        }

                        var seenlist = new List<string>();

                        for (int i = 0; i < allrowstring.Count(); i++)
                        {
                            var temp = new List<Tuple<int, int>>();
                            if (!seenlist.Contains(allrowstring[i]))
                            {
                                seenlist.Add(allrowstring[i]);
                                bool summarize = false;
                                var ovsplit = configpermute.OptionValCombi[i].Split(new string[] { "delim" }, StringSplitOptions.None);
                                foreach (var x in ovsplit)
                                {
                                    int ovid = Convert.ToInt32(x);
                                    var ov = db.OptionValue.FirstOrDefault(y => y.OptionValueID == ovid);
                                    var tc = ov.Option.TechnicalCharacteristic;
                                    if (tclist.Contains(tc))
                                        temp.Add(new Tuple<int, int>(ov.OptionID, ov.OptionValueID));
                                }

                                for (int j = i + 1; j < allrowstring.Count(); j++)
                                {
                                    if (allrowstring[i] == allrowstring[j])
                                    {
                                        summarize = true;
                                        var osplit = configpermute.OptionValCombi[j].Split(new string[] { "delim" }, StringSplitOptions.None);
                                        foreach (var x in osplit)
                                        {
                                            int ovid = Convert.ToInt32(x);
                                            var ov = db.OptionValue.FirstOrDefault(y => y.OptionValueID == ovid);
                                            var tc = ov.Option.TechnicalCharacteristic;
                                            if (tclist.Contains(tc))
                                                temp.Add(new Tuple<int, int>(ov.OptionID, ov.OptionValueID));
                                        }
                                    }
                                }


                                var seenitem = new List<int>();
                                var op_ov_sum_list = new List<Option_Optionvalue_Summarize>();
                                foreach (var item in temp.OrderBy(x => x.Item1))
                                {

                                    if (!seenitem.Contains(item.Item1))
                                    {
                                        seenitem.Add(item.Item1);
                                        var op_ov_sum = new Option_Optionvalue_Summarize { OptionValues = new List<string>(), OptionName = db.Option.FirstOrDefault(x => x.OptionID == item.Item1).OptionName };
                                        foreach (var t in temp.Where(x => x.Item1 == item.Item1).ToList())
                                        {
                                            var ovname = db.OptionValue.FirstOrDefault(x => x.OptionValueID == t.Item2).OptionVal;
                                            if (!op_ov_sum.OptionValues.Contains(ovname))
                                                op_ov_sum.OptionValues.Add(ovname);
                                        }
                                        op_ov_sum_list.Add(op_ov_sum);
                                    }

                                }

                                var modelitem = new ViewSummarize_FinalResult
                                {
                                    Op_ov_summarize = new List<Option_Optionvalue_Summarize>(),
                                    Property = new List<string>(),
                                    SystemName = db.Lsystem.FirstOrDefault(x => x.LsystemID == sysid).LsystemName,
                                    Values = new List<string>()
                                };
                                modelitem.Op_ov_summarize = op_ov_sum_list;
                                modelitem.Property = reqprop.OrderBy(x => x).ToList();
                                for (int cr = 0; cr < reqprop.Count(); cr++)
                                {
                                    bool found = false;
                                    foreach (var p in CompleteResult[i].EachRow)
                                    {

                                        if (p.setname == reqprop[cr])
                                        {
                                            modelitem.Values.Add(p.value);
                                            found = true;
                                            break;
                                        }

                                    }
                                    if (found == false)
                                        modelitem.Values.Add("No value");
                                }
                                model.Add(modelitem);
                                //an instance of the final model should be added here

                            }

                        }
                    }
                }
                model = model.OrderByDescending(x => x.Op_ov_summarize.Count()).ToList();
                var opcount = model[0].Op_ov_summarize.Count();
                foreach (var item in model.Where(x => x.Op_ov_summarize.Count() != opcount))
                {
                    while (opcount - item.Op_ov_summarize.Count() != 0)
                    {
                        var tempitem = new Option_Optionvalue_Summarize { OptionName = "No value", OptionValues = new List<string>() };
                        tempitem.OptionValues.Add("No value");
                        item.Op_ov_summarize.Add(tempitem);
                    }
                }
                ViewBag.count = model[0].Op_ov_summarize.Count();
                ViewBag.reqprop = reqprop.ToList();
                return View(model);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }

        }

        // GET: Views/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            Views views = db.Views.Find(id);
            if (views == null)
            {
                ViewBag.Error = " The requested View does not exist";
                return View("Error");
            }
            return View(views);
        }

        // GET: Views/Create
        public ActionResult Create()
        {
            try
            {
                if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
                {
                    ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemFamilyID != 62);
                    var model = new Views();
                    ViewBag.Calculation = db.Calculation.Where(x=>!x.CalculationFormula.Contains("Length")).ToList();
                    ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
                    return View();
                }
                else
                    return View("AuthorizationError");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

       

        // POST: Views/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ViewsID,ViewsName,DescriptionEN,DescriptionDE")] Views views, int[] lsystemid, int[] TcSetID, int[] CalcID)
        {            
            try
            {
                if (lsystemid != null)
                {
                    ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemFamilyID != 62);
                    var model = new Views { ViewsName= views.ViewsName };
                    var calcmodel = new List<Calculation>();
                    var tclist = new List<TechnicalCharacteristic>();
                    foreach (var item in lsystemid)
                    {
                        var op = db.Option.Where(x => x.LsystemID == item);
                        foreach (var opid in op)
                        {
                            int tcid = opid.TechnicalCharacteristicID;
                            if (!tclist.Contains(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcid)))
                                tclist.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcid));
                            //if (!model.TechnicalCharacteristic.Any(x => x.TechnicalCharacteristicID == tcid))
                            //    model.TechnicalCharacteristic.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == tcid));
                        }
                        var calcon = new CalculationsController();
                        var calclist = calcon.CalculationList(item);
                        foreach (var c in calclist)
                            calcmodel.Add(c);
                    }
                    //if ( == null)
                    //    ViewBag.Message = "No technical Characteristics to display";
                    
                    ViewBag.Calculation = calcmodel;
                    ViewBag.TC = tclist.OrderBy(x=>x.TCName).ToList();
                    return View(model);
                }
                if (ModelState.IsValid)
                {
                    if (db.Views.Any(x => x.ViewsName.Equals(views.ViewsName)))
                    {
                        ModelState.AddModelError("ViewsName", "View Name already exists");
                        ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemID != 0);
                        ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
                        ViewBag.Calculation = db.Calculation.Where(x => !x.CalculationFormula.Contains("Length")).ToList();
                        return View(views);
                    }
                    if (TcSetID == null)
                    {
                        ModelState.AddModelError("", "You have not selected any properties in the View");
                        ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemID != 0);
                        

                        return View(views);
                    }
                    views.CreatedBy = User.Identity.Name;
                    views.ModifiedBy = User.Identity.Name;
                    views.CreatedOn = DateTime.Now;
                    views.ModifiedOn = DateTime.Now;
                    //views.TechnicalCharacteristic = new List<TechnicalCharacteristic>();
                    var tcidlist = new List<int>();
                    foreach (var item in TcSetID)
                    {
                        tcidlist.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item).TechnicalCharacteristicID);

                    }
                    //foreach (var item in tcidlist.Distinct())
                    //    views.TechnicalCharacteristic.Add(db.TechnicalCharacteristic.FirstOrDefault(x => x.TechnicalCharacteristicID == item));

                    db.Views.Add(views);
                    db.SaveChanges();

                    foreach (var setid in TcSetID)
                    {
                        var x = new ViewsProperty();
                        x.TcSetID = setid;
                        x.ViewsID = views.ViewsID;
                        db.ViewsProperty.Add(x);
                    }

                    foreach(var cal in CalcID)
                    {
                        var c = new ViewsCalculation
                        {
                            CalculationID = cal,
                            Calculation = db.Calculation.FirstOrDefault(x => x.CalculationID == cal),
                            ViewsID = views.ViewsID,
                            Views = views
                        };
                        db.ViewsCalculation.Add(c);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemID != 0);
                ViewBag.Calculation = db.Calculation.Where(x => !x.CalculationFormula.Contains("Length")).ToList();
                ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
                //var model = new Views { TechnicalCharacteristic = db.TechnicalCharacteristic.ToList() };
                return View(views);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }


        //To View the different values for the system in the View : Not required    
        public ActionResult ViewSystemValue (int LsystemID,int ViewsID)
        {
            try
            {


                var model = new List<ViewSystemValuesVM>();
                var lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == LsystemID);
                ViewBag.LsystemName = lsystem.LsystemName;
                ViewBag.ViewsName = db.Views.FirstOrDefault(x => x.ViewsID == ViewsID).ViewsName;
                var viewslist = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
                var tclist = new List<TcSet>();
                var tcid = new List<int>();
                foreach (var item in viewslist)
                {
                    tclist.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID));
                    tcid.Add(item.TcSetID);
                }


                var optionlist = new List<Option>();
                foreach (var item in tclist)
                {
                    foreach (var op in db.Option.ToList())
                    {
                        if (op.TechnicalCharacteristicID == item.TechnicalCharacteristicID && op.LsystemID == LsystemID)
                            if (!optionlist.Contains(op))
                                optionlist.Add(op);
                    }

                }
                foreach (var item in optionlist)
                {
                    foreach (var ov in item.OptionValues)
                    {
                        foreach (var sv in ov.SetValue)
                        {
                            if (tclist.Any(p => p.TcSetID == sv.TcSetID))
                            {
                                var VM = new ViewSystemValuesVM();
                                VM.TcSetID = sv.TcSetID;
                                VM.Value = sv.Value;
                                model.Add(VM);
                            }
                        }
                    }
                }
                string[] myArray = new string[tclist.Count()];
                var model1 = new List<String[]>();
                int tclistcount = tclist.Count();
                int[] tcidcopy = new int[tclistcount];
                var serialize = new List<Tuple<int, List<string>>>();

                for (int i = 0; i < tcid.Count(); i++)
                {
                    var val = new List<string>();
                    foreach (var item in model)
                    {
                        if (tcid[i] == item.TcSetID)
                            val.Add(item.Value);

                    }
                    serialize.Add(new Tuple<int, List<string>>(tcid[i], val));
                }
                var rowval = new List<string>();
                int j = 0;
                var A = new List<String>();
                foreach (var item in serialize)
                {
                    if (j == 0)
                    {
                        A = item.Item2;
                        j = 1;

                    }
                    else
                        A = Cartesian(A, item.Item2);
                }

                ViewBag.A = A;

                var distinctlist = new List<String>();
                distinctlist = A.Distinct().ToList();

                var ViewsVM = new List<ViewsVM>();
                foreach (var item in distinctlist)
                {
                    var row = new ViewsVM();
                    row.row = item.Split(new string[] { "delim" }, StringSplitOptions.None);
                    ViewsVM.Add(row);

                }


                var modeltobepassed = new ModelToBePassed
                {
                    ViewsVM = ViewsVM,
                    TcSet = tclist
                };

                return View(modeltobepassed);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // Not  used : changed to Views Result
        public ActionResult ViewSystemValues(int LsystemID,int ViewsID)
        {
            try
            {
                Lsystem lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == LsystemID);
                ViewBag.LsystemName = lsystem.LsystemName;
                ViewBag.ViewsName = db.Views.FirstOrDefault(x => x.ViewsID == ViewsID).ViewsName;
                Views views = db.Views.FirstOrDefault(x => x.ViewsID == ViewsID);
                var req_prop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList().OrderBy(x => x.TcSetID);
                var req_set = new List<TcSet>();
                foreach (var item in req_prop)
                    req_set.Add(item.TcSet);
                var configlist = db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList();
                var tcidlist = new List<int>();
                //foreach (var item in views.TechnicalCharacteristic)
                //    tcidlist.Add(item.TechnicalCharacteristicID);
                var opidlist = new List<int>();
                foreach (var item in tcidlist)
                {
                    foreach (var op in lsystem.Options)
                        if (op.TechnicalCharacteristicID == item)
                            opidlist.Add(op.OptionID);
                }
                var covlist = new List<Config_OptionVal>();

                foreach (var item in configlist)
                {
                    foreach (var opid in opidlist)
                    {
                        foreach (var c in db.Config_OptionVal)
                        {
                            if (c.OptionID == opid && c.ConfigurationCollectionID == item.ConfigurationCollectionID && !covlist.Contains(c))
                                covlist.Add(c);
                        }
                    }

                }


                var ovcombi = new List<List<int>>();

                var op_ov_list = new List<Tuple<int, int, List<int>>>();
                foreach (var con in configlist)
                {

                    foreach (var op in opidlist)
                    {
                        var ovidlist = new List<int>();
                        foreach (var cov in covlist)
                        {
                            if (cov.ConfigurationCollectionID == con.ConfigurationCollectionID && cov.OptionID == op)
                            {
                                ovidlist.Add(cov.OptionValueID);
                            }
                        }
                        op_ov_list.Add(new Tuple<int, int, List<int>>(con.ConfigurationCollectionID, op, ovidlist));
                    }

                }
                var ovrows = new List<List<String>>();

                var B = new List<String>();
                foreach (var config in configlist)
                {
                    var A = new List<String>();
                    int count = 0;
                    foreach (var item in op_ov_list)
                    {
                        if (item.Item1 == config.ConfigurationCollectionID)
                        {
                            if (count == 0)
                            {
                                foreach (var ov in item.Item3)

                                    A.Add(ov.ToString());
                                count++;
                            }
                            else
                            {
                                foreach (var ov in item.Item3)
                                    B.Add(ov.ToString());
                                A = Cartesian(A, B);
                            }

                        }
                    }
                    ovrows.Add(A.Distinct().ToList());

                }
                var distinctrows = new List<String>();
                foreach (var item in ovrows)
                {
                    foreach (var a in item)
                    {
                        if (!distinctrows.Contains(a))
                            distinctrows.Add(a);
                    }
                }

                var rowovidlist = new List<int[]>();
                foreach (var item in distinctrows)
                {
                    int i = 0;
                    string[] itemsplit = item.Split(new string[] { "delim" }, StringSplitOptions.None);
                    int[] opid = new int[itemsplit.Count()];
                    foreach (var x in itemsplit)
                        opid[i++] = int.Parse(x);
                    rowovidlist.Add(opid);
                }


                var model = new ModelToBePassed { TcSet = req_set, ViewsVM = new List<ViewsVM>() };
                model.TcSet = req_set;
                var r = new List<String[]>();
                var distinctr = new List<String[]>();
                foreach (var item in rowovidlist)
                {
                    var row = new String[req_set.Count()];
                    int i = 0;


                    foreach (var ovid in item)
                    {
                        var ov = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ovid);

                        int count = 0;
                        foreach (var set in ov.SetValue.OrderBy(x => x.TcSet.TcSetID))
                        {
                            count++;
                            //Important part is commented
                            if (req_set.Any(x => x.TcSetID == set.TcSetID))
                            {
                                row[i++] = set.Value;

                                break;
                            }

                            //if (!req_set.Any(x => x.TcSetID == set.TcSetID))
                            //    row[i++] = "null";

                            //if (a.TcSetID == set.TcSetID)
                            //    row[i++] = set.Value;
                            else if (count == ov.SetValue.Count())
                                row[i++] = "no value";
                        }
                    }

                    r.Add(row);

                }

                foreach (var item in op_ov_list)
                {
                    var ov_list = new List<int>();

                }

                var stringlist = new List<String>();
                foreach (var item in r)
                {
                    string s = "delim";
                    foreach (var x in item)
                    {
                        if (x == null)
                            s = s + "no value" + "delim";
                        else
                            s = s + x + "delim";
                    }
                    s = string.Join("", s.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                    stringlist.Add(s);
                }
                var distinctstringlist = stringlist.Distinct().ToList();



                distinctr = r.Distinct().ToList();

                foreach (var item in distinctstringlist)
                {
                    var row1 = new String[req_set.Count()];
                    row1 = item.Split(new string[] { "delim" }, StringSplitOptions.RemoveEmptyEntries);
                    var viewvm = new ViewsVM();
                    viewvm.row = row1;
                    model.ViewsVM.Add(viewvm);
                }

                return View(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Permutation of all option values: Not required (Changed to PermuteConfig)
        public void ConfigCombiModel(int LsystemID)
        {

            var onerow = new List<int>();
            var combined = new List<List<int>>();
            var configlist = new List<ConfigurationCollection>();
            var opovcombolist = new List<string>();
            
            var model = new List<Configcombi>();
            foreach (var item in db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList())
                configlist.Add(item);
            foreach (var item in configlist)
            {
               

                //List of option id
                var oplist = new List<int>();
                //list of option value id
                var ovlist = new List<int>();
                //list optionid and optionvalueid combination (eg : (opid,ovid))
                var opov = new List<Tuple<int, int>>();
                //list of optionid with all corresponding optionvalueid (eg : (opid1,<ovid11,ovid12>) , (opid2,<ovid21,ovid22,ovid23>))
                var opovlist = new List<Tuple<int, List<int>>>();
                foreach (var p in db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == item.ConfigurationCollectionID))
                {
                    if (!oplist.Contains(p.OptionID))
                    {
                        oplist.Add(p.OptionID);
                    }

                    //if (!ovlist.Contains(p.OptionValueID))
                    //    ovlist.Add(p.OptionValueID);
                    opov.Add(new Tuple<int, int>(p.OptionID, p.OptionValueID));
                }
                foreach (var op in oplist)
                {
                    var ov = new List<int>();
                    foreach (var x in opov)
                    {
                        if (x.Item1 == op)
                        {
                            ov.Add(x.Item2);
                        }
                    }
                    opovlist.Add(new Tuple<int, List<int>>(op, ov));
                }
                var A = new List<string>();
                foreach (var l in opovlist[0].Item2)
                    A.Add(l.ToString());
                int count = 0;
                var result = new List<List<string>>();
                foreach (var f in opovlist)
                {
                    if (count == 0)
                        count = 1;
                    else
                    {
                        var B = f.Item2;
                        var inner = new List<string>();
                        for (int i = 0; i < A.Count(); i++)
                        {
                            for (int j = 0; j < B.Count(); j++)
                            {

                                string innerString = A[i] + "delim" + B[j].ToString();
                                inner.Add(innerString);
                            }
                        }
                        A = inner;
                    }
                }
                foreach (var t in A)
                    opovcombolist.Add(t);
            }
            var ovidlist = new List<List<int>>();
            foreach (var item in opovcombolist)
            {
                var ovsplitlist = item.Split(new string[] { "delim" }, StringSplitOptions.None);
                var inner = new List<int>();
                foreach (var ovid in ovsplitlist)
                {
                    inner.Add(Convert.ToInt16(ovid));
                }
                ovidlist.Add(inner);
            }
            //return ovidlist;
        }

        //Returns list of all permutations of option values : Not required(Changed to PermuteConfig)
        public List<List<int>> configcombi(int LsystemID)
        {
            
            var onerow = new List<int>();
            var combined = new List<List<int>>();
            var configlist = new List<ConfigurationCollection>();
            var opovcombolist = new List<string>();
            foreach (var item in db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList())
                configlist.Add(item);
            foreach(var item in configlist)
            {
                //List of option id
                var oplist = new List<int>();
                //list of option value id
                var ovlist = new List<int>();
                //list optionid and optionvalueid combination (eg : (opid,ovid))
                var opov = new List<Tuple< int, int>>();
                //list of optionid with all corresponding optionvalueid (eg : (opid1,<ovid11,ovid12>) , (opid2,<ovid21,ovid22,ovid23>))
                var opovlist = new List<Tuple<int, List<int>>>();
                foreach(var p in db.Config_OptionVal.Where(x=>x.ConfigurationCollectionID==item.ConfigurationCollectionID))
                {
                    if (!oplist.Contains(p.OptionID))
                    {
                        oplist.Add(p.OptionID);
                    }

                    //if (!ovlist.Contains(p.OptionValueID))
                    //    ovlist.Add(p.OptionValueID);
                    opov.Add(new Tuple<int, int>(p.OptionID, p.OptionValueID));
                }
                foreach(var op in oplist)
                {
                    var ov = new List<int>();
                    foreach(var x in opov)
                    {
                         if(x.Item1==op)
                         {
                             ov.Add(x.Item2);
                         }
                    }
                    opovlist.Add(new Tuple<int, List<int>>(op, ov));
                }
                var A = new List<string>();
                foreach (var l in opovlist[0].Item2)
                    A.Add(l.ToString());
                int count = 0;
                var result = new List<List<string>>();
                foreach(var f in opovlist)
                {
                    if (count == 0)
                        count = 1;
                    else
                    {
                        var B = f.Item2;
                        var inner = new List<string>();
                        for(int i=0;i<A.Count();i++)
                        {
                            for(int j=0;j<B.Count();j++)
                            {
                                
                                string innerString = A[i] + "delim" + B[j].ToString();
                                inner.Add(innerString);
                            }
                        }
                        A = inner;
                    }
                }
                foreach (var t in A)
                    opovcombolist.Add(t);
            }
            var ovidlist = new List<List<int>>();
            foreach(var item in opovcombolist)
            {
                var ovsplitlist = item.Split(new string[] { "delim" }, StringSplitOptions.None);
                var inner = new List<int>();
                foreach(var ovid in ovsplitlist)
                {
                    inner.Add(Convert.ToInt16(ovid));
                }
                ovidlist.Add(inner);
            }
            return ovidlist;
        }
  
        //Not required
        public List<Configcombi> ConfigCombiModel(int ViewsID, int LsystemID)
        {
            //int ViewsID = 25;
            //int LsystemID = 1032;
            var viewprop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
            var reqprop = new List<string>();
            var reqpropall = new List<int>();

            //list of name of all properties used in the view
            foreach(var item in viewprop)
                reqprop.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName);

            //List of TcSetID  of all TcSet with the name in reqprop
            foreach (var item in reqprop)
                foreach (var elem in db.TcSet.ToList())
                    if (item == elem.SetName)
                        reqpropall.Add(elem.TcSetID);


            var config = db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList();
            var model = new List<Configcombi>();
            foreach(var c in config)
            {
                var con_op = new Configcombi { OptionValues = new List<Tuple<string,string>>() };
                var concol = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == c.ConfigurationCollectionID).ToList();
                con_op.ConfigurationName = c.CollectionName;
                foreach(var item in concol)
                    con_op.OptionValues.Add(new Tuple<string,string>( item.OptionValueID.ToString(),item.OptionID.ToString()));
                model.Add(con_op);
            }
            foreach(var item in model)
            {
                foreach(var ov in item.OptionValues.ToList())
                {
                    var ovid = Convert.ToInt32(ov.Item1);
                    var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ovid);
                    var setlist = new List<int>();
                    foreach (var set in opv.SetValue)
                        setlist.Add(set.TcSetID);
                    if(!reqpropall.Intersect(setlist).Any())
                    {
                        item.OptionValues.Remove(ov);
                    }
                    
                }
            }
            var modelVm = new List<Configcombi>();

            foreach(var item in model.ToList())
            {
                var one = new Configcombi { OptionValues = new List<Tuple<string, string>>() };

                if (item.OptionValues.Count() == 0)
                    model.Remove(item);
                else
                {
                    one.ConfigurationName = item.ConfigurationName;
                    for (int i = 0; i < item.OptionValues.Count();i++ )
                    {
                        int opvid = Convert.ToInt32(item.OptionValues[i].Item1);
                        int opid = Convert.ToInt32(item.OptionValues[i].Item2);
                        var opval = db.OptionValue.FirstOrDefault(x => x.OptionValueID == opvid).OptionVal;
                        var opt = db.Option.FirstOrDefault(x => x.OptionID == opid).OptionName;
                        one.OptionValues.Add(new Tuple<string, string>(opval, opt));
                    }
                    modelVm.Add(one);
                }
            }
            var optionnamelist = new List<string>();
            foreach(var item in modelVm)
            {
                foreach(var ov in item.OptionValues)
                {
                    if (!optionnamelist.Contains(ov.Item2))
                        optionnamelist.Add(ov.Item2);
                }
            }
            foreach(var item in modelVm)
            {
                var oplist = new List<string>();
                foreach (var op in item.OptionValues)
                    oplist.Add(op.Item2);
                foreach(var l in optionnamelist)
                {
                    if (!oplist.Contains(l))
                    {
                        item.OptionValues.Add(new Tuple<string, string>("no value", l));
                    }
                }
                item.OptionValues= item.OptionValues.OrderBy(x => x.Item2).ToList();
            }
            modelVm = modelVm.OrderBy(x => x.ConfigurationName).ToList();
            return modelVm;
        }

        //Not required (Displays the list of property values and eleiminate duplicate rows)
        public ActionResult ViewsResult (int ViewsID)
        {
            var allovidlist = new List<List<int>>();
            
            var allrow = new List<List<string>>();
            var ViewModel = new ViewsResult_Complete { ConfigCombi = new List<List<Configcombi>>() };
            var Systemname = new List<string>();
            foreach (var item in TempData["lsys"] as int[])
            {
                Systemname.Add(db.Lsystem.FirstOrDefault(x => x.LsystemID == item).LsystemName);
                var listconfigcombi = ConfigCombiModel(ViewsID, item);
                ViewModel.ConfigCombi.Add(listconfigcombi);
                var ovidlist = configcombi(item);
                foreach (var ov in ovidlist)
                    allovidlist.Add(ov);
            }
            ViewBag.SystemName = Systemname;
            //var ovidlist = configcombi(1032);
            //foreach (var ov in ovidlist)
            //    allovidlist.Add(ov);

            var optionnamelist = new List<string>();
            foreach(var item in ViewModel.ConfigCombi)
            {
                foreach(var l in item)
                {
                    foreach(var o in l.OptionValues)
                    {
                        if (!optionnamelist.Contains(o.Item2))
                            optionnamelist.Add(o.Item2);
                    }
                }
            }

            foreach(var item in ViewModel.ConfigCombi)
            {
                foreach(var l in item)
                {
                    var oplist = new List<string>();
                    foreach( var o in l.OptionValues)
                    {
                        oplist.Add(o.Item2);

                    }
                    foreach(var p in optionnamelist)
                    {
                        if (!oplist.Contains(p))
                            l.OptionValues.Add(new Tuple<string, string>("no value", p));
                    }
                    l.OptionValues = l.OptionValues.OrderBy(x => x.Item2).ToList();
                }
            }
            ViewBag.OptionName = optionnamelist;
            ViewModel.ConfigCombi.OrderBy(x => x.OrderBy(y => y.ConfigurationName));
            var reqprop = new List<TcSet>();
            foreach (var t in db.ViewsProperty.Where(x => x.ViewsID == ViewsID))
                reqprop.Add(db.TcSet.FirstOrDefault(x=>x.TcSetID==t.TcSetID));
            reqprop = reqprop.OrderBy(x => x.SetName).ToList();

            var setlist = new List<string>();
            foreach (var item in reqprop)
                setlist.Add(item.SetName);
            var newlist = new List<List<Tuple<string, string>>>();
            foreach(var ovidrow in allovidlist)
            {
                var eachrow = new List<Tuple<string,string>>();
                foreach(var ov in ovidrow)
                {
                    foreach(var set in db.OptionValue.FirstOrDefault(x=>x.OptionValueID==ov).SetValue)
                    {
                        if(setlist.Contains(set.TcSet.SetName))
                        {
                            eachrow.Add(new Tuple<string, string>(set.Value, set.TcSet.SetName));
                        }
                    }
                }
                newlist.Add(eachrow.OrderBy(x=>x.Item2).ToList());
            }

            var neweachrow = new List<string>();
            var newallrow = new List<List<string>>();
            for (int i = 0; i < newlist.Count();i++ )
            {
                var inserttc = new List<string>();
                foreach(var tuple in newlist[i].ToList())
                {
                    if (!inserttc.Contains(tuple.Item2))
                        inserttc.Add(tuple.Item2);
                    else
                        newlist[i].Remove(tuple);
                }
                foreach(var item in reqprop)
                {
                    if(!inserttc.Contains(item.SetName))
                    {
                        newlist[i].Add(new Tuple<string, string>("no value", item.SetName));
                    }
                }
                newlist[i] = newlist[i].OrderBy(x => x.Item2).ToList();
            }

            foreach (var item in newlist)
            {

                 neweachrow = new List<string>();
                foreach (var i in item)
                    neweachrow.Add(i.Item1);
                if(!newallrow.Contains(neweachrow))
                    newallrow.Add(neweachrow);
            }
            var model = new ViewsResult
            {
                allrow = new List<List<string>>(),
                reqprop = new List<string>()
            };

            model.allrow = newallrow.GroupBy(c => String.Join(",", c)).Select(c => c.First().ToList()).ToList();
            foreach (var item in reqprop)
                model.reqprop.Add(item.SetName);
            ViewModel.ViewsResult = model;
            return View(ViewModel);
        }

        // not used: changed to ViewsResult
        public ActionResult TestView(int ViewsID)
        {
            try
            {
                var xy = configcombi(1032);
                var configlist = new List<ConfigurationCollection>();
                foreach(var item in TempData["lsys"] as int[])
                {
                    foreach (var c in db.ConfigurationCollection.Where(x => x.LsystemID == item).ToList())
                        configlist.Add(c);
                }
                
                //configlist = db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList();
                var reqprop = db.ViewsProperty.Where(x => x.ViewsID == ViewsID).ToList();
                var reqset = new List<TcSet>();
                foreach (var item in reqprop)
                    reqset.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID));
                var model = new List<testmain>();
                foreach (var item in configlist)
                {
                    testmain val = new testmain
                    {
                        ConfigurationCollectionID = item.ConfigurationCollectionID,
                        ConfigurationCollection = db.ConfigurationCollection.FirstOrDefault(x => x.ConfigurationCollectionID == item.ConfigurationCollectionID),
                        subset = new List<subtest>()
                    };
                    var conoplist = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == item.ConfigurationCollectionID).ToList();
                    var opidlist = new List<OptionValue>();
                    foreach (var con in conoplist)
                        opidlist.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == con.OptionValueID));
                    foreach (var set in reqset)
                    {
                        var subsetval = new subtest { TcSetID = set.TcSetID, TcSet = db.TcSet.FirstOrDefault(x => x.TcSetID == set.TcSetID), Values = new List<string>() };
                        var setlist = new List<SetValue>();
                        foreach (var ov in opidlist)
                        {
                            foreach (var s in ov.SetValue)
                            {
                                if (s.TcSetID == set.TcSetID)
                                    setlist.Add(s);
                            }
                            //if (ov.SetValue.Any(x => x.TcSetID == set.TcSetID))
                            //    setlist = ov.SetValue.Where(x=>x.TcSetID==set.TcSetID).ToList();

                            // subsetval.Values.Add(ov.SetValue.Where(x => x.TcSetID == set.TcSetID).Select(x=>x.Value).ToList());
                        }
                        foreach (var li in setlist.Distinct())
                            subsetval.Values.Add(li.Value);
                        val.subset.Add(subsetval);
                    }
                    model.Add(val);
                    ViewBag.count = 0;
                }
                return View(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //returns  a string list of cartesian products(to get all possible combinations of the values for the View): Not used
        public List<String> Cartesian (List<String> A, List<String> B)
        {
            int Alenght = A.Count();
            int Blength = B.Count();
            var S = new List<String>();
            for (int i = 0; i < Alenght;i++ )
            {
                
                for(int j=0;j<Blength;j++)
                {
                    S.Add(A[i] + "delim" + B[j]);

                }
            }
                return S;
        }

        //To display the properties in the View (Not values just the name of the properties that make the View)
        public ActionResult DisplayViews(int id)
        {
            try
            {


                ViewBag.id = id;
                var model = new DispalyViewsVM { tcSet = new List<TcSet>() , Calculation = new List<Calculation>()};
                model.Views = db.Views.FirstOrDefault(x => x.ViewsID == id);
                var set = db.ViewsProperty.Where(x => x.ViewsID == id).ToList();
                var setlist = new List<string>();
                foreach (var item in set)
                {
                    var name = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).SetName;
                    if(!setlist.Contains(name))
                    {
                        setlist.Add(name);
                        model.tcSet.Add(item.TcSet);
                    }

                }
                foreach(var item in db.ViewsCalculation.Where(x=>x.ViewsID==id).ToList())
                {
                    model.Calculation.Add(db.Calculation.FirstOrDefault(x => x.CalculationID == item.CalculationID));
                }
                    
                return View(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //Copy a view : GET method
        public ActionResult CopyView()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin)
            {


                ViewBag.CopyID = new SelectList(db.Views.OrderBy(x=>x.ViewsName), "ViewsID", "ViewsName");
                return View();
            }
            else
                return View("Authorization Error");
        }

        [HttpPost]
        public ActionResult CopyView([Bind(Include = "ViewsID,ViewsName,DescriptionEN,DescriptionDE")] Views model, int CopyID)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.Views.Any(x => x.ViewsName.Equals(model.ViewsName)))
                    {
                        ModelState.AddModelError("ViewsName", "Views name already Exists");
                        ViewBag.CopyID = new SelectList(db.Views.OrderBy(x=>x.ViewsName), "ViewsID", "ViewsName");
                        return View(model);
                    }
                    if (CopyID == 0)
                    {
                        ModelState.AddModelError("", "Select a View to be copied");
                        ViewBag.CopyID = new SelectList(db.Views.OrderBy(x=>x.ViewsName), "ViewsID", "ViewsName");
                        return View(model);
                    }
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedOn = DateTime.Now;
                    model.ModifiedBy = User.Identity.Name;
                    model.ModifiedOn = DateTime.Now;

                    //model.TechnicalCharacteristic = new List<TechnicalCharacteristic>();
                    var copy = db.ViewsProperty.Where(x => x.ViewsID == CopyID).ToList();
                    //foreach (var item in copy)

                    //    model.TechnicalCharacteristic.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).TechnicalCharacteristic);


                    db.Views.Add(model);
                    db.SaveChanges();

                    foreach (var item in copy)
                    {
                        ViewsProperty vp = new ViewsProperty
                        {
                            ViewsID = model.ViewsID,
                            TcSetID = item.TcSetID,
                            TcSet = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID),
                            Views = db.Views.FirstOrDefault(x => x.ViewsID == model.ViewsID)
                        };


                        db.ViewsProperty.Add(vp);

                    }

                    foreach(var item in db.ViewsCalculation.Where(x=>x.ViewsID==CopyID).ToList())
                    {
                        ViewsCalculation vc = new ViewsCalculation
                        {
                             ViewsID = model.ViewsID,
                              CalculationID = item.CalculationID,
                               Views = db.Views.FirstOrDefault(x=>x.ViewsID==model.ViewsID),
                                Calculation = db.Calculation.FirstOrDefault(x=>x.CalculationID==item.CalculationID)
                        };
                        db.ViewsCalculation.Add(vc);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CopyID = new SelectList(db.Views.OrderBy(x=>x.ViewsName), "ViewsID", "ViewsName");
                return View();
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Views/Edit/5
        public ActionResult Edit(int? id)
        {
            //ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemID != 0);
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            Views views = db.Views.Find(id);
            var model = new EditViewsVM
            {
                Views12 = views,
                TcSet = new List<TcSet>(),
                 Calculation = new List<Calculation>()
            };
            var set = db.ViewsProperty.Where(x => x.ViewsID == id).ToList();
            foreach (var item in set)
                model.TcSet.Add(item.TcSet);
            model.Calculation = db.ViewsCalculation.Where(x => x.ViewsID == id).Select(x=>x.Calculation).ToList();
            ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
            ViewBag.Calculation = db.Calculation.Where(x => !x.CalculationFormula.Contains("Length")).ToList();
            if (views == null)
            {
                ViewBag.Error = "The requested View does not exist";
                return View("Error");
            }
            return View(model);
        }

        // POST: Views/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Views12,TcSet")] EditViewsVM editVM, int[] TcSetID, int[]lsystemid, int[] CalcID)
        {
            ViewBag.Calculation = db.Calculation.Where(x => !x.CalculationFormula.Contains("Length")).ToList();

            if (ModelState.IsValid)
            {
                var views = db.Views.Where(x => x.ViewsID == editVM.Views12.ViewsID).FirstOrDefault();
                if(db.Views.Any(x=>x.ViewsName.Equals(editVM.Views12.ViewsName)&&(x.ViewsID!=editVM.Views12.ViewsID)))
                {
                    ModelState.AddModelError("", "View name already exists");
                    Views views12 = db.Views.Find(editVM.Views12.ViewsID);
                    var model = new EditViewsVM
                    {
                        Views12 = views12,
                        TcSet = new List<TcSet>(), Calculation = new List<Calculation>()
                    };
                    var set = db.ViewsProperty.Where(x => x.ViewsID == views12.ViewsID).ToList();
                    foreach (var item in set)
                        model.TcSet.Add(item.TcSet);
                    ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
                    model.Calculation = db.ViewsCalculation.Where(x => x.ViewsID == views.ViewsID).Select(x => x.Calculation).ToList();
                    
                    return View(model);
                }
                if (TcSetID == null)
                {
                    ModelState.AddModelError("", "You have not selected any properties in the View");
                    //ViewBag.Lsystem = db.Lsystem.Where(x => x.LsystemID != 0);
                    Views views12 = db.Views.Find(views.ViewsID);
                    var model = new EditViewsVM
                    {
                        Views12 = views12,
                        TcSet = new List<TcSet>(),
                        Calculation =  new List<Calculation>()
                    };
                    var set = db.ViewsProperty.Where(x => x.ViewsID == views12.ViewsID).ToList();
                    foreach (var item in set)
                        model.TcSet.Add(db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID));
                    ViewBag.TC = db.TechnicalCharacteristic.OrderBy(x=>x.TCName).ToList();
                    model.Calculation = db.ViewsCalculation.Where(x => x.ViewsID == views.ViewsID).Select(x => x.Calculation).ToList();


                    return View(model);
                }
                views.ViewsName = editVM.Views12.ViewsName;
                views.DescriptionDE = editVM.Views12.DescriptionDE;
                views.DescriptionEN = editVM.Views12.DescriptionEN;
                views.ModifiedBy = User.Identity.Name;
                views.ModifiedOn = DateTime.Now;
                
                
                db.Entry(views).State = EntityState.Modified;
                db.Entry(views).Property(x => x.CreatedBy).IsModified = false;
                db.Entry(views).Property(x => x.CreatedOn).IsModified = false;
                

               
                
                var viewprop = db.ViewsProperty.Where(x => x.ViewsID == views.ViewsID).ToList();
                foreach (var item in viewprop)
                    db.ViewsProperty.Remove(item);
                var setname = new List<string>();
                foreach(var setid in TcSetID)
                {
                    
                    var set = db.TcSet.FirstOrDefault(y => y.TcSetID == setid).SetName;
                    if(!setname.Contains(set))
                    {
                        setname.Add(set);
                        var x = new ViewsProperty();
                        x.TcSetID = setid;
                        x.ViewsID = views.ViewsID;
                        db.ViewsProperty.Add(x);
                    }
                   
                }

                foreach (var item in db.ViewsCalculation.Where(x => x.ViewsID == views.ViewsID).ToList())
                    db.ViewsCalculation.Remove(item);
                foreach(var item in CalcID)
                {
                    var c = new ViewsCalculation
                    {
                        ViewsID = views.ViewsID,
                        CalculationID = item

                    };
                    db.ViewsCalculation.Add(c);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            
            }
            return View("Error");
        }

        // GET: Views/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the function";
                return View("Error");
            }
            Views views = db.Views.Find(id);
            if (views == null)
            {
                ViewBag.Error = "The requested View does not exist";
                return View("Error");
            }
            return View(views);
        }

        // POST: Views/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Views views = db.Views.Find(id);
            var del = db.ViewsProperty.Where(x => x.ViewsID == id);
            foreach (var item in del)
                db.ViewsProperty.Remove(item);
            foreach (var item in db.ViewsCalculation.Where(x => x.ViewsID == id))
                db.ViewsCalculation.Remove(item);
            //var tc = db.TechnicalCharacteristic.Where(x => x.Views_ViewsID == id || x.Views_ViewsID==id).ToList();
            //foreach(var t in tc)
            //{
            //    t.Views_ViewsID = null;
            //    db.Entry(t).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            db.Views.Remove(views);
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
