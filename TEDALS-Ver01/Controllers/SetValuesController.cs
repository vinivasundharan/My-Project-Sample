using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using TEDALS_Ver01.DAL;
using TEDALS_Ver01.Models;

namespace TEDALS_Ver01.Controllers
{

    public class SetValuesController : Controller
    {
        private TedalsContext db = new TedalsContext();

        
        public class IdRequiredAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                object parameter = null;
                filterContext.ActionParameters.TryGetValue("id", out parameter);
                var id = parameter as int?;

                if (id == null)
                {
                    var urlHelper = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext);
                    //ViewBag.Error = "An unhandled exception occured";
                    var url = urlHelper.Action("Error", "SetValues");

                    filterContext.Result = new RedirectResult(url);
                }
            }
        }

        public ActionResult Error ()
        {
            ViewBag.Error = "An unhandled Exception occured with the processing of your request";
            return View("Error");
        }

        
        public void precision()
        {
            int sig = 4;
            int sca = 2;
            string n = "0,2899";
            int numlength = n.Length;
            double n1 = Convert.ToDouble(n) ;
            
            int wholenumcount = n.IndexOf(',');
            string wholenumpart = n.Substring(0, wholenumcount);
            string decimalpart = n.Substring(n.IndexOf(','), numlength - wholenumcount);
            int decimalcount = decimalpart.Length - 1;
            int wholenum = Convert.ToInt16(wholenumpart);
            double decim = Convert.ToDouble(decimalpart);
            if(numlength-1<sig)
            {
                int diff = sig - (numlength-1 );
                while(diff!=0)
                {
                    n = n + "0";
                    diff--;
                }
            }
            //if (wholenumcount == 0)
            //{
            //    n = "0" + n;
            //    wholenumcount++;
            //}
            
            
            else if(wholenumcount>sig)
            {
                int diff = wholenumcount - sig;
                int rem=0;
                while(diff!=0)
                {
                    rem = wholenum % 10;
                    wholenum /= 10;
                    diff--;
                }
                wholenum = Convert.ToInt32(wholenum * Math.Pow(10, wholenumcount - sig));
                if(rem>=5)
                
                    wholenum = Convert.ToInt32(wholenum + Math.Pow(10, wholenumcount - sig));
                n = wholenum.ToString();
                //else 
                  //  wholenum = Convert.ToInt32(wholenum * Math.Pow(10, wholenumcount-sig));
                //else if(decim<Math.Round(decim,MidpointRounding.AwayFromZero))
                //{
                //    wholenum = Convert.ToInt32(wholenum + Math.Pow(10, wholenumcount - sig));
                //}
                //if(sca!=0)
                //{
                //    n = wholenum.ToString()+",";

                //    while (sca != 0)
                //    {
                //        n = n + "0";
                //        sca--;
                //    }
                        
                //}
                
            }
            //else if(wholenumcount==sig)
            //{
                
            //    if (Math.Round(decim,MidpointRounding.AwayFromZero) > decim)
            //        wholenum = wholenum + 1;
            //    n = wholenum.ToString();
            //    //if(sca!=0)
            //    //{
            //    //    n = wholenum.ToString()+",";
            //    //    while(sca!=0)
            //    //    {
            //    //        n = n + "0";
            //    //        sca--;
            //    //    }
                    
            //    //}

            //}
            else if(wholenumcount<=sig)
            {

                decim = Math.Round(decim, sig - wholenumcount, MidpointRounding.AwayFromZero);
                if(Convert.ToInt16(decim)!=0)
                {
                    wholenum = wholenum + 1;
                    n = wholenum.ToString();
                    if(n.Length<sig)
                    {
                        int count = sig - n.Length;
                        n=n+",";
                        
                        while(count!=0)
                        {
                            n = n + "0";
                            count--;
                        }
                    }
                }
                else
                {
                    n = wholenumpart + decim.ToString().Remove(0, 1);
                    if(n.Length-1<sig)
                    {
                        int diff = sig - (n.Length - 1);
                        while(diff!=0)
                        {
                            n = n + "0";
                            diff--;
                        }
                    }
                }
                    
                //while(sca!=0)
                //{
                //    n = n.Remove(n.Length - 1);
                //    sca--;
                    
                //}
            }
            
            
        }

        // GET: SetValues
        
        public ActionResult Index(int id)
        {
            //if(id==null)
            //{
            //    ViewBag.Error = "A null parameters passed to the function";
            //    return View("Error");
            //}
            try
            {

                precision();
                ViewBag.Empty = false;
                var ov = db.OptionValue.Include(x => x.Option).FirstOrDefault(x => x.OptionValueID == id);
                var opid = ov.OptionID;
                var op = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == opid);
                var tcid = op.TechnicalCharacteristicID;
                var tc = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == tcid);
                var tcset = tc.TcSets;
                var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == id);
                ViewBag.OptionValue = opv.OptionVal;
                ViewBag.Option = opv.Option.OptionName;
                ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
                ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
                ViewBag.OptionValID = id;
                ViewBag.OptionID = opv.OptionID;
                var setValue = db.SetValue.Where(x => x.OptionValueID == id).OrderBy(x => x.TcSet.SetName);
                ViewBag.CopyID = new SelectList(db.OptionValue.Where(x => x.OptionID == opid).OrderBy(x => x.OptionVal), "OptionValueID", "OptionVal");
                return View(setValue.ToList());
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }

        }

        

        public ActionResult copy(int CopyID,int OptionValueID)
        {
            try
            {


                if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
                {
                    //if (CopyID == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    if (CopyID == OptionValueID)
                    {
                        ViewBag.id = OptionValueID;
                        ViewBag.Message = "Cannot copy Option value to itself. Select another Option value to copy";
                        return View("Empty");
                    }
                    var ovtobecopied = db.OptionValue.FirstOrDefault(x => x.OptionValueID == CopyID);
                    var ovtobecreated = db.OptionValue.FirstOrDefault(x => x.OptionValueID == OptionValueID);
                    var oldsetval = ovtobecopied.SetValue;
                    
                    
                    if(db.SetValue.Where(x=>x.OptionValueID==OptionValueID).Count()!=0)
                    {
                        ViewBag.id = OptionValueID;
                        ViewBag.Message = "Copy cannot be completed. There are a existing Property values for the Option value you tried to create";
                        return View("Empty");
                    }
                    
                    if (!db.SetValue.Any(x => x.OptionValueID == OptionValueID))
                    {


                        foreach (var item in ovtobecopied.SetValue)
                        {

                            var setVal = new SetValue
                            {
                                OptionValueID = OptionValueID,
                                Value = item.Value,
                                TcSet = item.TcSet,
                                TcSetID = item.TcSetID,
                                OptionValue = db.OptionValue.FirstOrDefault(x => x.OptionValueID == OptionValueID),
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                ModifiedOn = DateTime.Now

                            };
                            db.SetValue.Add(setVal);
                            var rev = new RevisionHistory();
                            rev.CreatedOn = setVal.CreatedOn;
                            rev.ModifiedOn = setVal.ModifiedOn;
                            rev.ModifiedBy = setVal.ModifiedBy;
                            rev.Optionvalue = setVal.OptionValue.OptionVal;
                            rev.Option = setVal.OptionValue.Option.OptionName;
                            rev.SystemName = setVal.OptionValue.Option.Lsystem.LsystemName;
                            var p = db.TcSet.FirstOrDefault(x => x.TcSetID == setVal.TcSetID);
                            rev.TCSetName = p.SetName;
                            rev.InitialValue = setVal.Value;
                            rev.ModifiedValue = setVal.Value; ;
                            rev.SetValueID = setVal.SetValueID;
                            rev.Action = "Created";
                            db.RevisionHistory.Add(rev);
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index", "SetValues", new { id = OptionValueID });
                }



                else
                    return View("AuthorizationError");
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }
        // GET: SetValues/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.id = id;
            if (id == null)
            {
                ViewBag.Error = "A null parameter was passed to the fucntion";
                return View("Error");
            }
            SetValue setValue = db.SetValue.Find(id);
            if (setValue == null)
            {
                ViewBag.Error = "The requested property does not exist";
                return View("Error");
            }
            var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == setValue.OptionValueID);
            ViewBag.OptionValue = opv.OptionVal;
            ViewBag.Option = opv.Option.OptionName;
            ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
            ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
            return View(setValue);
        }

        

        // GET: SetValues/Create
       
        public ActionResult Create(int id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {
                    //if(id==null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the fucntion";
                    //    return View("Error");
                    //}

                    ViewBag.id = id;
                    //var model = new SetValue { OptionValueID = id };
                    var ov = db.OptionValue.Include(x => x.Option).FirstOrDefault(x => x.OptionValueID == id);
                    var opid = ov.OptionID;
                    var op = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == opid);
                    var tcid = op.TechnicalCharacteristicID;
                    var tc = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == tcid);
                    var tcset = tc.TcSets.ToList();
                    if (!tc.TcSets.Any())
                        ViewBag.Message = "There are no Tc Set Properties to be displayed. Add properties in the Technical Characteristics";
                    var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == id);
                    ViewBag.OptionValue = opv.OptionVal;
                    ViewBag.Option = opv.Option.OptionName;
                    ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
                    ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
                    ViewBag.TcSetID = new SelectList(tcset.OrderBy(x=>x.SetName), "TcSetID", "SetName");
                    //ViewBag.OptionValueID = new SelectList(db.OptionValue,"OptionValueID","OptionVal",)
                    var model = new List<SetValue>();
                    var exist = db.SetValue.Where(x => x.OptionValueID == id).ToList();
                    foreach(var e in exist)
                    {
                        if (tcset.Contains(e.TcSet))
                            tcset.Remove(e.TcSet);

                    }
                    if(tcset.Count() == 0)
                    {
                        ViewBag.id = id;
                        ViewBag.Message = "There are no Properties to be added.";
                        return View("Empty");
                    }
                        
                    
                    int count = tcset.Count();
                    for (int i = 0; i < count; i++)
                    {
                        var setval = new SetValue
                        {
                            OptionValueID = id,
                            TcSetID = tcset[i].TcSetID,
                            TcSet = tcset[i],
                            OptionValue = ov
                        };
                        model.Add(setval);
                    }
                    return View(model.OrderBy(x=>x.TcSet.SetName).ToList());
                }
                catch(Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View("Error");
                }
            }
            else return View("AuthorizationError");
        }

        public string Significant(double value, int precision1)
        {
            string valuestring = value.ToString(); ;
            double result;
             if (value == 0)
                {
                    valuestring = "0";
                    if (precision1 != 1)
                        valuestring = valuestring = valuestring + ",";
                    while (precision1 != 1)
                    {
                        valuestring = valuestring + "0";
                        precision1--;
                    }
                }
                else
                {
                    var n = Math.Floor(Math.Log10(value)) + 1 - precision1;
                    var minusn = -n;
                    result = Math.Round(Math.Pow(10, minusn) * value,MidpointRounding.AwayFromZero) * Math.Pow(10, n);
                    bool lesser = false;
                    if (result < 1)
                        lesser = true;
                    valuestring = result.ToString();
                    if (valuestring.Contains(',') && valuestring.Length - 1 <= precision1)
                    {
                        if (lesser)
                            while (precision1 != valuestring.Length - 2)
                                valuestring = valuestring + "0";
                        else
                            while (precision1 != valuestring.Length - 1)
                                valuestring = valuestring + "0";

                    }

                    else if (valuestring.Length < precision1)
                    {
                        valuestring = valuestring + ",";
                        while (valuestring.Length != precision1 + 1)
                            valuestring = valuestring + "0";
                    }
                }
            return valuestring;
        }

        public string Scaling(double value, int sca)
        {
            double result = 0;
            string valuestring = value.ToString();
            
            if (sca != 0)
            {                
                if (value == 0)
                {
                    valuestring = "0";
                    if (sca != 0)
                        valuestring = "0,";
                    while (sca != 0)
                    {
                        valuestring = valuestring + "0";
                        sca--;
                    }
                        //valuestring = valuestring + "0";

                }
                else
                {                   
                    result = Math.Round(value, sca, MidpointRounding.AwayFromZero);
                    valuestring = result.ToString();
                    if (valuestring.Contains(','))
                    {
                        int deccount = valuestring.Length - valuestring.IndexOf(',') - 1;
                        while (sca - deccount != 0)
                        {
                            valuestring = valuestring + "0";
                            deccount++;
                        }
                    }

                    else
                    {
                        valuestring = valuestring + ",";
                        while (sca != 0)
                        {
                            valuestring = valuestring + "0";
                            sca--;
                        }
                    }
                }
            }
            else
            {
                result = Math.Round(value, 0, MidpointRounding.AwayFromZero);
                valuestring = result.ToString();
            }
            return valuestring;
        }



        // POST: SetValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SetValueID,Value,Status,TcSetID,OptionValueID")] List<SetValue> setValue)
        {
            try
            {


                int ovid1 = setValue[0].OptionValueID;
                var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == ovid1);
                ViewBag.OptionValue = opv.OptionVal;
                ViewBag.Option = opv.Option.OptionName;
                ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
                ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
                ViewBag.id = opv.OptionValueID;
                int ovid = 0;
                bool error = false;
                if (ModelState.IsValid)
                {
                    for (int i = 0; i < setValue.Count(); i++)
                    {

                        int setid = setValue[i].TcSetID;
                        if (db.TcSet.FirstOrDefault(x => x.TcSetID == setid).DataFormat.FormatType == "Number" &&setValue[i].Value!="#")
                        {
                            double val;
                            if (!double.TryParse(setValue[i].Value, out val))
                            {

                                error = true;
                                Expression<Func<List<SetValue>, string>> exp = x => x[i].Value;
                                string key = ExpressionHelper.GetExpressionText(exp);
                                ModelState.AddModelError(key, "Value can only be a number or #");
                                int sd = setValue[i].OptionValueID;
                                var ov = db.OptionValue.Include(x => x.Option).FirstOrDefault(x => x.OptionValueID == sd);
                                var opid = ov.OptionID;
                                var op = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == opid);
                                var tcid = op.TechnicalCharacteristicID;
                                var tc = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == tcid);
                                var tcset = tc.TcSets;
                                //ViewBag.OptionValueID = new SelectList(db.OptionValue, "OptionValueID", "OptionVal", setValue.OptionValueID);


                            }

                        }

                    }
                    if (error)
                    {
                        int count = setValue.Count();
                        for (int j = 0; j < count; j++)
                        {
                            int setid = setValue[j].TcSetID;
                            setValue[j].TcSet = db.TcSet.FirstOrDefault(x => x.TcSetID == setid);
                        }
                        return View(setValue);
                    }
                    foreach (var item in setValue)
                    {

                        item.CreatedBy = User.Identity.Name;
                        item.ModifiedBy = User.Identity.Name;
                        item.ModifiedOn = DateTime.Now;
                        item.CreatedOn = DateTime.Now;
                        item.OptionValue = db.OptionValue.FirstOrDefault(x => x.OptionValueID == item.OptionValueID);
                        var dataformid = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID).DataFormatID;
                        var df = db.DataFormat.FirstOrDefault(x => x.DataFormatID == dataformid);
                        int? scaling = df.ScalingDigits;
                        int? precision = df.PrecisionDigits;
                        int count = 0;
                        String newval = item.Value;
                        string valuestring = "";
                        double result;
                        double value=0;
                        bool flag = double.TryParse(item.Value, out value);
                        if(item.Value!="#"&& df.FormatType!="String" )
                        {
                            
                            if(precision!=null)
                            {
                                int precision1 = precision ?? default(int);
                                if (precision1 == 0)
                                    valuestring = item.Value;
                                else
                                    valuestring = Significant(value, precision1);

                            }
                            else if(scaling!=null)
                            {
                                int sca = scaling ?? default(int);
                                valuestring = Scaling(value, sca);
                            }

                            item.Value = valuestring;
                        }


                        
                       
                        db.SetValue.Add(item);
                        ovid = item.OptionValueID;
                        var rev = new RevisionHistory();
                        rev.CreatedOn = item.CreatedOn;
                        rev.ModifiedOn = item.ModifiedOn;
                        rev.ModifiedBy = item.ModifiedBy;
                        rev.Optionvalue = item.OptionValue.OptionVal;
                        rev.Option = item.OptionValue.Option.OptionName;
                        rev.SystemName = item.OptionValue.Option.Lsystem.LsystemName;
                        var p = db.TcSet.FirstOrDefault(x => x.TcSetID == item.TcSetID);
                        rev.TCSetName = p.SetName;
                        rev.InitialValue = item.Value;
                        rev.ModifiedValue = item.Value; ;
                        rev.SetValueID = item.SetValueID;
                        rev.Action = "Created";
                        db.RevisionHistory.Add(rev);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "SetValues", new { id = ovid });
                }
                var ov1 = db.OptionValue.Include(x => x.Option).FirstOrDefault(x => x.OptionValueID == ovid);
                var opid1 = ov1.OptionID;
                var op1 = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == opid1);
                var tcid1 = op1.TechnicalCharacteristicID;
                var tc1 = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == tcid1);
                var tcset1 = tc1.TcSets;
                return View(setValue);

            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: SetValues/Edit/5
        
        public ActionResult Edit(int? id)
        {

            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                SetValue setValue = db.SetValue.Find(id);
                var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == setValue.OptionValueID);
                ViewBag.OptionValue = opv.OptionVal;
                ViewBag.Option = opv.Option.OptionName;
                ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
                ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
                ViewBag.id = setValue.OptionValueID;
                var q1 = db.TcSet.FirstOrDefault(x => x.TcSetID == setValue.TcSetID);
                ViewBag.TcSet = q1.SetName;
                if (setValue == null)
                {
                    ViewBag.Error = "The requested  Property does not exist";
                    return View("Error");
                }
                ViewBag.OptionValueID = new SelectList(db.OptionValue.OrderBy(x=>x.OptionVal), "OptionValueID", "OptionVal", setValue.OptionValueID);
                return View(setValue);
            }
            else return View("AuthorizationError");
        }

        // POST: SetValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SetValueID,Value,Status,TcSetID,OptionValueID,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy")] SetValue setValue)
        {
            try
            {
                var opv = db.OptionValue.FirstOrDefault(x => x.OptionValueID == setValue.OptionValueID);
                ViewBag.OptionValue = opv.OptionVal;
                ViewBag.Option = opv.Option.OptionName;
                ViewBag.Lsystem = opv.Option.Lsystem.LsystemName;
                ViewBag.FamilyName = opv.Option.Lsystem.LsystemFamily.FamilyName;
                ViewBag.id = setValue.OptionValueID;
                var q1 = db.TcSet.FirstOrDefault(x => x.TcSetID == setValue.TcSetID);
                ViewBag.TcSet = q1.SetName;
                if (ModelState.IsValid)
                {
                    //if (db.SetValue.Any(x => x.TcSetID == setValue.TcSetID && x.OptionValueID == setValue.OptionValueID && x.SetValueID != setValue.SetValueID))
                    //{
                    //    ModelState.AddModelError("TcSetID", "Property already exists");
                    //    var ov = db.OptionValue.Include(x => x.Option).FirstOrDefault(x => x.OptionValueID == setValue.OptionValueID);
                    //    var opid = ov.OptionID;
                    //    var op = db.Option.Include(x => x.TechnicalCharacteristic).FirstOrDefault(x => x.OptionID == opid);
                    //    var tcid = op.TechnicalCharacteristicID;
                    //    var tc = db.TechnicalCharacteristic.Include(x => x.TcSets).FirstOrDefault(x => x.TechnicalCharacteristicID == tcid);
                    //    var tcset = tc.TcSets;
                    //    //ViewBag.OptionValueID = new SelectList(db.OptionValue, "OptionValueID", "OptionVal", setValue.OptionValueID);
                    //    ViewBag.TcSetID = new SelectList(tcset, "TcSetID", "SetName");
                    //    return View();
                    //}
                    var q = db.TcSet.FirstOrDefault(x => x.TcSetID == setValue.TcSetID);
                    var data = q.DataFormat.FormatType;
                    var precision = q.DataFormat.PrecisionDigits;
                    var scaling = q.DataFormat.ScalingDigits;
                    double val = 0;

                    ViewBag.val = val;

                    double value;
                    string valuestring ="";
                    bool flag= double.TryParse(setValue.Value,out value);
                    if(setValue.Value!="#" && data!="String")
                    {
                        if(!flag)
                        {
                            ModelState.AddModelError("Value", "Value can only be a number or #");
                            return View(setValue);

                        }
                        if(precision!=null)
                        {
                            int precision1 = precision ?? default(int);
                            if (precision != 0)
                                valuestring = Significant(value, precision1);
                            else
                                valuestring = setValue.Value;
                        }
                        else if(scaling!=null)
                        {
                            int sca = scaling ?? default(int);
                            valuestring = Scaling(value, sca);
                        }
                        
                    }
                    setValue.Value = valuestring;
                    var original = db.SetValue.Find(setValue.SetValueID);
                    bool modified = original.Value != setValue.Value;
                    if (modified)
                    {
                        //var old = db.RevisionHistory.FirstOrDefault(x => x.SetValueID == setValue.SetValueID);
                        var rev = new RevisionHistory();
                        rev.CreatedOn = original.ModifiedOn;
                        rev.ModifiedOn = DateTime.Now;
                        rev.ModifiedBy = User.Identity.Name;
                        rev.InitialValue = original.Value; ;
                        rev.Optionvalue = original.OptionValue.OptionVal;
                        rev.TCSetName = original.TcSet.SetName;
                        rev.Option = original.OptionValue.Option.OptionName;
                        rev.SystemName = original.OptionValue.Option.Lsystem.LsystemName;
                        rev.SetValueID = original.SetValueID;
                        rev.ModifiedValue = setValue.Value;
                        rev.Action = "Modified";
                        db.Entry(original).CurrentValues.SetValues(setValue);
                        db.RevisionHistory.Add(rev);
                    }
                    original.ModifiedOn = DateTime.Now;
                    original.ModifiedBy = User.Identity.Name;
                    db.Entry(original).State = EntityState.Modified;
                    db.Entry(original).Property("CreatedOn").IsModified = false;
                    db.Entry(original).Property("CreatedBy").IsModified = false;
                    db.Entry(original).Property("OptionValueID").IsModified = false;
                    db.Entry(original).Property("TcSetID").IsModified = false;

                    db.SaveChanges();
                    return RedirectToAction("Index", "SetValues", new { id = setValue.OptionValueID });
                }
                ViewBag.OptionValueID = new SelectList(db.OptionValue.OrderBy(x=>x.OptionVal), "OptionValueID", "OptionVal", setValue.OptionValueID);
                return View(setValue);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: SetValues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                if (id == null)
                {
                    ViewBag.Error = "A null parameter was passed to the function";
                    return View("Error");
                }
                SetValue setValue = db.SetValue.Find(id);
                if (setValue == null)
                {
                    ViewBag.Error = "The requested property does not exist";
                    return View("Error");
                }
                return View(setValue);
            }
            else return View("AuthorizationError");
        }

        // POST: SetValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                SetValue setValue = db.SetValue.Find(id);
                var rev = new RevisionHistory();
                rev.Action = "Deleted";
                rev.CreatedOn = setValue.ModifiedOn;
                rev.ModifiedOn = DateTime.Now;
                rev.ModifiedBy = User.Identity.Name;
                rev.SystemName = setValue.OptionValue.Option.Lsystem.LsystemName;
                rev.Option = setValue.OptionValue.Option.OptionName;
                rev.Optionvalue = setValue.OptionValue.OptionVal;
                rev.TCSetName = setValue.TcSet.SetName;
                rev.SetValueID = setValue.SetValueID;
                rev.InitialValue = setValue.Value;
                db.RevisionHistory.Add(rev);
                db.SetValue.Remove(setValue);
                db.SaveChanges();
                return RedirectToAction("Index", "SetValues", new { id = setValue.OptionValueID });
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
