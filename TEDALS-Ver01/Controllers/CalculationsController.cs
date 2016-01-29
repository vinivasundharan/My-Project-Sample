using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TEDALS_Ver01.DAL;
using TEDALS_Ver01.Models;
using NCalc;
using System.Text.RegularExpressions;
using TEDALS_Ver01.ViewModels.Calculation;
using TEDALS_Ver01.ViewModels.CalculationVM;

namespace TEDALS_Ver01.Controllers
{
    public class CalculationsController : Controller
    {
        private TedalsContext db = new TedalsContext();

        // GET: Calculations
        public ActionResult Index()
        {
            try
            {

                var calculation = db.Calculation.Include(c => c.DataFormat);
                calculation = calculation.OrderBy(x => x.CalculationName);
                return View(calculation.ToList());
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Calculations/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Calculation calculation = db.Calculation.Find(id);
                if (calculation == null)
                {
                    return HttpNotFound();
                }
                return View(calculation);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        

        // GET: Calculations/Create
        public ActionResult Create()
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {


                    ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0 && x.DataFormat.FormatType.Equals("Number"));
                    ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName");
                    return View();
                }
                catch(Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View("Error");
                }
            }
            else return View("AuthorizationError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CalculationID,CalculationName,CalculationFormula,PhysicalUnit,DataStatus,DescriptionEN,DescriptionDE,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,DataFormatID")] Calculation calculation)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.Calculation.Any(x => x.CalculationName.Equals(calculation.CalculationName)))
                    {
                        ModelState.AddModelError("CalculationName", "Calculation Name already exists");
                        ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0 && x.DataFormat.FormatType.Equals("Number"));
                        ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x => x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                        return View(calculation);
                    }
                    
                    var FunctionList = new BasicFunctions();
                    var delimstringlist = new string[] { "Min", "Max", "Abs" ,"Pow","+","-","*","/","(",")","²","³","Length"," ","\r","\n",",","[","]","Sqrt","Cubrt","^"};
                    string[] formulaSplit = calculation.CalculationFormula.Split(delimstringlist,StringSplitOptions.RemoveEmptyEntries);
                    
                    double constant = 0;
                    foreach(var s in formulaSplit)
                    {
                        if(!db.TcSet.Any(x=>x.SetName==s && x.DataFormat.FormatType=="Number")&&!double.TryParse(s,out constant))
                        {
                            ModelState.AddModelError("CalculationFormula", "Formula contains unidentified property names");
                            ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0 && x.DataFormat.FormatType.Equals("Number"));
                            ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x => x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                            return View(calculation);
                        }
                    }

                     
                   
                    calculation.CreatedBy = User.Identity.Name;
                    calculation.CreatedOn = DateTime.Now;
                    calculation.ModifiedBy = User.Identity.Name;
                    calculation.ModifiedOn = DateTime.Now;
                    db.Calculation.Add(calculation);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0);
                ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x => x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                return View(calculation);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        //return list of all available calculations
        public List<Calculation> CalculationList(int id)
        {
            var viewmodel = new List<Calculation>();
            int LsystemID = id;
            var lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
            var delimstringlist = new string[] { "Min", "Max", "Abs", "Pow", "+", "-", "*", "/", "(", ")", "²", "³", " ", "\r", "\n", ",", "[", "]", "Sqrt", "Cubrt", "^" };
            //var setprop = db.SetValue.Where(x => x.OptionValue.Option.LsystemID == id).ToList();
            var setlist = new List<String>();
            var optionlist = db.Option.Where(x => x.LsystemID == id).ToList();
            foreach (var op in optionlist)
            {
                var setprop = db.TcSet.Where(x => x.TechnicalCharacteristicID == op.TechnicalCharacteristicID).ToList();
                foreach (var item in setprop)
                    if (!setlist.Contains(item.SetName))
                        setlist.Add(item.SetName);
            }

            foreach (var cal in db.Calculation.ToList())
            {
                bool contains = true;
                string[] formulaSplit = cal.CalculationFormula.Split(delimstringlist, StringSplitOptions.RemoveEmptyEntries);
                double val = 0;
                foreach (var item in formulaSplit)
                {
                    if (!setlist.Contains(item) && !double.TryParse(item, out val) && item=="Length")
                    {
                        contains = false;
                        break;
                    }
                }
                if (contains)
                    viewmodel.Add(cal);
            }
            return viewmodel;
        }

        //display the available calculations for a system
        public ActionResult ViewCalculations(int id)
        {
            try
            {
                var viewmodel = new List<Calculation>();
                int LsystemID = id;
                var lsystem = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
                var delimstringlist = new string[] { "Min", "Max", "Abs", "Pow", "+", "-", "*", "/", "(", ")", "²", "³", "Length", " ", "\r", "\n", ",", "[", "]", "Sqrt", "Cubrt", "^" };
                //var setprop = db.SetValue.Where(x => x.OptionValue.Option.LsystemID == id).ToList();
                var setlist = new List<String>();
                var optionlist = db.Option.Where(x => x.LsystemID == id).ToList();
                foreach(var op in optionlist)
                {
                    var setprop = db.TcSet.Where(x => x.TechnicalCharacteristicID == op.TechnicalCharacteristicID).ToList();
                    foreach (var item in setprop)
                        if (!setlist.Contains(item.SetName))
                            setlist.Add(item.SetName);
                }
                
                foreach(var cal in db.Calculation.ToList())
                {
                    bool contains = true;
                    string[] formulaSplit = cal.CalculationFormula.Split(delimstringlist, StringSplitOptions.RemoveEmptyEntries);
                    double val = 0;
                    foreach(var item in formulaSplit)
                    {
                        if (!setlist.Contains(item) && !double.TryParse(item,out val))
                        {
                            contains = false;
                            break;
                        }
                            
                        

                    }
                    if (contains)
                        viewmodel.Add(cal);
                }
                @ViewBag.LsystemFamilyID = lsystem.LsystemFamilyID;
                ViewBag.LsystemID = id;
                return View(viewmodel);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }

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
                result = Math.Round(Math.Pow(10, minusn) * value, MidpointRounding.AwayFromZero) * Math.Pow(10, n);
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

        public ActionResult Calculate1(int calcID, int LsystemID, string Length)
        {
            var calc = db.Calculation.FirstOrDefault(x=>x.CalculationID==calcID);
            var model = new List<CalculationResult>();
            double length;
            bool lengthvalid = true;
            if(Length!=null)
                lengthvalid = double.TryParse(Length, out length);

            if (!lengthvalid)
            {
                ViewBag.Message = "Enter a valid value for Length";
                return View();
            }

            if (Length != null && calc.CalculationFormula.Contains("Length"))
            {
                Length = Length.Replace(',', '.');
                calc.CalculationFormula = calc.CalculationFormula.Replace("Length", Length);
            }
            
            if (calc.CalculationFormula.Contains("Length"))
            {
                ViewBag.Message = "Formula takes a value for Length, Enter a value for length to continue";
                ViewBag.calcID = calcID;
                ViewBag.LsystemID = LsystemID;
                return View();
            }
            
            
            var delimstringlist = new string[] { "Min", "Max", "Abs", "Pow", "+", "-", "*", "/", "(", ")", "²", "³", "Length", " ", "\r", "\n", ",", "[", "]", "Sqrt", "Cubrt", "^" };
            var formulaSplit = calc.CalculationFormula.Split(delimstringlist, StringSplitOptions.RemoveEmptyEntries);
            var setlist = new List<string>();
            double d;
            foreach(var item in formulaSplit)
                if (!double.TryParse(item, out d))
                    setlist.Add(item);
            string sb = "";
            string prev = sb;
            var formlist = new List<string>();

            string formula = new string(calc.CalculationFormula.Where(x => !Char.IsWhiteSpace(x)).ToArray());
            for(int i=0;i<formula.Length;i++)
            {
                sb = sb + formula[i];
                double num;
                bool added = false;
                if(double.TryParse(sb,out num))
                {
                    if(formulaSplit.Contains(sb))
                    {
                        prev = sb;
                        if(i+1!=formula.Length)
                        {
                            string next = formula[i + 1].ToString();
                            if (delimstringlist.Contains(next))
                            {
                                added = true;
                                formlist.Add(prev);
                                sb = "";

                            }
                        }
                        else
                        {
                            formlist.Add(prev);
                        }
                        
                    }
                    
                }
                else if(formulaSplit.Contains(sb))
                {
                    formlist.Add(sb);
                    sb = "";
                }
                else if(delimstringlist.Contains(sb))
                {
                    formlist.Add(sb);
                    sb = "";
                }
            }

            var config = db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).OrderBy(x=>x.ConfigurationCollectionID).ToList();
           
            
            foreach(var con in config)
            {
                var modelitem = new CalculationResult { ConfigurationName = con.CollectionName, AnsList = new List<string>() };
                var expresionlist = new List<string>();
                var oplist = new List<Option>();
                var conprop = db.Config_OptionVal.Where(x => x.ConfigurationCollectionID == con.ConfigurationCollectionID).ToList();
                var permutefn = new List<Option_OptionVal>();

                var formulalist = new List<string>();
                foreach(var cp in conprop)
                {
                    var op = db.Option.FirstOrDefault(x=>x.OptionID==cp.OptionID);
                    var tcset = op.TechnicalCharacteristic.TcSets.ToList();
                    
                    foreach(var set in tcset)
                    {
                        if(setlist.Contains(set.SetName))
                        {
                            if (!oplist.Contains(op))
                                oplist.Add(op);
                            break;
                        }
                    }
                }

                foreach(var op in oplist)
                {
                    var inner = new Option_OptionVal { OptionID = op.OptionID, OptionValueID = new List<int>() };
                    foreach (var ov in conprop.Where(x => x.OptionID == op.OptionID).ToList())
                        inner.OptionValueID.Add(ov.OptionValueID);
                    permutefn.Add(inner);
                }

                var permutelist = Permutefun(permutefn);
                permutelist = permutelist.Distinct().ToList();
                var allrow = new List<List<OptionValue>>();
                
                foreach(var pl in permutelist)
                {
                    var eachrow = new List<OptionValue>();
                    string[] oval = pl.Split(new string[] { "delim" }, StringSplitOptions.None);
                    foreach(var o in oval)
                    {
                        int ovid;
                        bool flag = int.TryParse(o, out ovid);
                        eachrow.Add(db.OptionValue.FirstOrDefault(x => x.OptionValueID == ovid));
                    }
                    allrow.Add(eachrow);
                }

                foreach(var ar in allrow)
                {
                    List<string> expr = formlist.ToList();
                    foreach(var ov in ar)
                    {
                        var setval = ov.SetValue.ToList();
                        foreach(var s in setval)
                        {
                            while(expr.Contains(s.TcSet.SetName))
                            {
                                int index = expr.IndexOf(s.TcSet.SetName);
                                expr[index] = s.Value.Replace(',','.');
                            }
                        }
                    }
                    string ex ="";
                    foreach (var e in expr)
                        ex = ex + e;
                    expresionlist.Add(ex);
                }

                foreach (var ex in expresionlist.ToList())
                {
                    var formulasplit = ex.Split(delimstringlist, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var f in formulasplit)
                    {
                        double d1;
                        if (!double.TryParse(f, out d1))
                        {
                            expresionlist.Remove(ex);
                            break;
                        }

                    }
                }

                

                var anslist = new List<string>();
                
                foreach (var item in expresionlist)
                {
                    string expritem = item;
                    while(expritem.Contains("Min"))
                    {
                        int startindex = expritem.IndexOf("Min");
                        var paramlist = new List<double>();
                        sb="";
                        for(int i=startindex+4;;i++)
                        {
                            
                            if(expritem[i]==',')
                            {
                                double p;
                                bool c = double.TryParse(sb, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out p);
                                paramlist.Add(p);
                                sb = "";
                            }
                            else if(expritem[i]==')')
                                {
                                    double p;
                                    bool c = double.TryParse(sb, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out p);
                                    paramlist.Add(p);
                                    sb = "";
                                    break;
                                }
                            else
                            {
                                sb= sb+expritem[i];
                            }
                        }

                        var minval = paramlist.Min();
                        var minvalstr = minval.ToString().Replace(',', '.');
                        for(int i=startindex;;i=startindex)
                        {
                            
                            if (expritem[i] == ')')
                            {
                                expritem = expritem.Remove(i, 1);
                                break;
                            }
                            else
                                expritem = expritem.Remove(i, 1);
                                          
                        }
                        expritem= expritem.Insert(startindex, minvalstr);
                    }

                    while (expritem.Contains("Max"))
                    {
                        int startindex = expritem.IndexOf("Max");
                        var paramlist = new List<double>();
                        sb = "";
                        for (int i = startindex + 4; ; i++)
                        {

                            if (expritem[i] == ',')
                            {
                                double p;
                                bool c = double.TryParse(sb, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out p);
                                paramlist.Add(p);
                                sb = "";
                            }
                            else if (expritem[i] == ')')
                            {
                                double p;
                                bool c = double.TryParse(sb, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out p);
                                paramlist.Add(p);
                                sb = "";
                                break;
                            }
                            else
                            {
                                sb = sb + expritem[i];
                            }
                        }

                        var minval = paramlist.Max();
                        var minvalstr = minval.ToString().Replace(',', '.');
                        for (int i = startindex; ; i = startindex)
                        {

                            if (expritem[i] == ')')
                            {
                                expritem = expritem.Remove(i, 1);
                                break;
                            }
                            else
                                expritem = expritem.Remove(i, 1);

                        }
                        expritem = expritem.Insert(startindex, minvalstr);
                    }

                    while(expritem.Contains("Abs"))
                    {
                        int startindex = expritem.IndexOf("Abs");
                        double absval;
                        string absstr;
                        sb = "";
                        for (int i = startindex + 4; ; i++)
                        {                            
                            if (expritem[i] == ')')
                            {
                                //double p;
                                //bool c = double.TryParse(sb, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out p);
                                //absval=p;
                                //sb = "";
                                //break;
                                Expression abs = new Expression(sb);
                                absstr = abs.Evaluate().ToString();
                                break;
                            }
                            else
                            {
                                sb = sb + expritem[i];
                            }
                        }
                        bool p = double.TryParse(absstr, out absval);
                        absval = Math.Abs(absval);
                        string absvalstr = absval.ToString().Replace(',', '.');
                        for (int i = startindex; ; i = startindex)
                        {

                            if (expritem[i] == ')')
                            {
                                expritem = expritem.Remove(i, 1);
                                break;
                            }
                            else
                                expritem = expritem.Remove(i, 1);

                        }
                        expritem = expritem.Insert(startindex,absvalstr);
                    }

                    while(expritem.Contains("Cubrt"))
                    {
                        sb = "";
                        int startindex = expritem.IndexOf("Cubrt");
                        for(int i=startindex+6;;i++)
                        {
                            if(expritem[i]==')')
                            {

                                break;
                            }
                            else
                            {
                                sb = sb + expritem[i];
                            }
                        }
                        double cubval;
                        bool p = double.TryParse(sb, out cubval);
                        double cubrtans = Math.Pow(cubval, (1.0 / 3.0));
                        string cubans = cubrtans.ToString().Replace(',','.');
                        for (int i = startindex; ; )
                        {
                            if(expritem[i]!=')')
                              expritem = expritem.Remove(i, 1);
                            else
                            {
                                expritem = expritem.Remove(i, 1);
                                break;
                            }

                        }
                        expritem = expritem.Insert(startindex, cubans);

                    }
                    Expression e = new Expression(expritem);
                    string ans = e.Evaluate().ToString();
                    double ans1;
                    bool a = double.TryParse(ans, out ans1);
                    string anstr="";
                    var dataformat = calc.DataFormat;
                    int? precision = dataformat.PrecisionDigits;
                    int? scaling = dataformat.ScalingDigits;
                    if(precision!=null)
                    {
                        int pre = precision ?? default(int);
                        if (pre != 0)
                            anstr = Significant(ans1, pre);
                        else
                            anstr = ans.ToString();
                    }
                    else if(scaling!=null)
                    {
                        int sca = scaling ?? default(int);
                        anstr = Scaling(ans1, sca);
                    }
                    anslist.Add(anstr);
                }
                modelitem.AnsList = anslist.Distinct().ToList();
                model.Add(modelitem);
            }


            ViewBag.SystemName = db.Lsystem.FirstOrDefault(x => x.LsystemID == LsystemID).LsystemName;
            ViewBag.CalcName = calc.CalculationName;
            ViewBag.Formula = calc.CalculationFormula;
            ViewBag.Length = Length;
            return View(model);
        }



        public List<string> Permutefun(List<Option_OptionVal> opovlist)
        {
            var ans = new List<string>();
            bool flag = true;
            foreach(var item in opovlist)
            {
                if(flag)
                {
                    foreach (var ov in item.OptionValueID)
                        ans.Add(ov.ToString());
                    flag = false;
                }
                else
                {
                    var innerlist = new List<string>();
                    for(int i=0;i<ans.Count();i++)
                    {
                        for(int j=0;j<item.OptionValueID.Count();j++)
                        {
                            string inner = ans[i] + "delim" + item.OptionValueID[j].ToString();
                            innerlist.Add(inner);
                        }
                    }
                    ans = innerlist;
                }
            }
            return ans;
        }


        //Display Available calculations for a  Option value : Test
        public ActionResult ViewCalculations1(int id)
        {
            ViewBag.id = db.OptionValue.FirstOrDefault(x => x.OptionValueID == id).OptionID;
            try
            {


                ViewBag.OptionValID = id;
                var model = new List<Calculation>();
                var sv = db.SetValue.Where(x => x.OptionValueID == id);
                var calc = db.Calculation.Where(x => x.CalculationID != 0);
                foreach (var item in calc)
                {
                    int p = 0;
                    char[] delimiter = { '+', '-', '*', '/', '(', ')', '²', '³',',' };
                    string[] set = item.CalculationFormula.Split(delimiter);
                    foreach (string s in set)
                    {
                        double d;
                        if (s != "Length" && !String.IsNullOrEmpty(s) && !double.TryParse(s, out d)&& s!="Pow")
                        {
                            if (!sv.Any(x => x.TcSet.SetName.Equals(s)))
                            {
                                p++;
                                break;
                            }
                        }
                    }
                    if (p == 0)
                        model.Add(item);
                }
                return View(model);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }
        
        //Calculate for option value : Test , Not used
        public ActionResult Calculate(int calcID, int OptionValID,string length)
        {
            ViewBag.id = OptionValID;
            try
            {
                var model = new List<SetValue>();
                ViewBag.Message = null;
                string formula = db.Calculation.FirstOrDefault(x => x.CalculationID == calcID).CalculationFormula;
                ViewBag.CalcName = db.Calculation.FirstOrDefault(x => x.CalculationID == calcID).CalculationName;
                if (formula.Contains("Length") && String.IsNullOrWhiteSpace(length))
                {
                    ViewBag.Message = "Enter a Value for length";
                    return View();
                }
                char[] delimiter = { '+', '-', '*', '/', '(', ')', '²', '³',',' };
                
                formula = formula.Replace("Length", length);
                string[] split = formula.Split(delimiter);
                var sv = db.SetValue.Where(x => x.OptionValueID == OptionValID);
                foreach (string s in split)
                {
                    
                    double d;
                    if (s != "Length" && !String.IsNullOrEmpty(s)&&!double.TryParse(s,out d)&&s!="Pow")
                    {
                        foreach (var item in sv)
                        {
                            if (item.TcSet.SetName.Equals(s))
                            {
                                model.Add(item);
                                string rep = item.Value.Replace(',', '.');
                                string s1 = @"\b" + Regex.Escape(s) + @"\b";
                                var rx1 = new Regex(s1);
                                
                                formula = rx1.Replace(formula, rep,1);
                                //formula = formula.Replace(s, item.Value);
                            }
                        }
                    }
                }
                
                Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

                //DataTable dt = new DataTable();
                //formula = formula.Replace(",", ".");
                //for(int i=0;i<formula.Length;i++)
                //{
                //    int k = i;
                //    string op1 ="", op2="";
                //    int startindex = 0;
                //    int endindex = 0;
                //    string mathpow = "";
                //    if(formula[i]=='²')
                //    {
                        
                //        while(!delimiter.Contains(formula[k--]))
                //        {
                //            op1 = op1 + formula[k];
                //        }
                //        Array.Reverse(op1.ToCharArray());
                //        //op1 = new string (op1);
                //        startindex = k++;
                //        k = i;
                //        while(!delimiter.Contains(formula[k++]))
                //        {
                //            op2 = op2 + formula[k];
                //        }
                //        endindex = k--;
                //        mathpow = "Pow(op1,op2)";
                //    }
                //    formula = formula.Remove(startindex, endindex - startindex);
                //    formula = formula.Insert(startindex, mathpow);
                //    i = startindex + mathpow.Length;
                //}
                //formula = formula.Replace(',', '.');
                //formula = formula.Replace("²", "^2");
                //formula = formula.Replace("³", "^3");
                //if(formula.Contains("Pow"))
                //{
                //    int first = formula.IndexOf("Pow");
                //    string oldstring = "";
                //    string newstring="";
                //    while (formula[first++] != ')')
                //        newstring = newstring + formula[first];
                //    int commacount = newstring.Count(c => c.Equals('.'));
                //    if(commacount>1)
                //    {

                //    }
                //}
                

                ViewBag.formula = formula;
                //try
                //{
                String.Join("", formula.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    ViewBag.ans = Calc_Parse(formula);
                    //ViewBag.ans = dt.Compute(formula, string.Empty);
                    return View(model);
                //}
                //catch
                //{
                //    ViewBag.error = "Invalid parameters found for calculation";
                //    return View(model);
                //}
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        public string Calc_Parse(string formula)
        {
            char[] delimiter = { '+', '-', '*', '/', '(', ')', '^',' '};
            string ans;
            int p = 0;
            int i = 0,j=0;
            double[] operands = new double[formula.Length];
            char[] operators = new char[formula.Length];
            Expression e = new Expression(formula);
            ans = e.Evaluate().ToString();

            //foreach(var item in formula.Split(delimiter))
            //{
            //    if(item!="")
            //    operands[i] = Convert.ToDouble(item);
            //    i++;
            //}
            //for (int k = 0; k < formula.Length;k++ )
            //{
            //    if(formula[k]=='+'||formula[k]=='-'||formula[k]=='*'||formula[k]=='/')
            //    {
            //        operators[j++] = formula[k];
            //    }
            //}
            //ans = operands[0];
            //j = 0;
            //for (i = 1; operands[i]!=0;i++ )
            //{
            //    if(operators[j]=='+')
            //    {
            //        ans = ans + operands[i];
            //    }
            //    else if (operators[j] == '-')
            //    {
            //        ans = ans - operands[i];
            //    }
            //    else if (operators[j] == '*')
            //    {
            //        ans = ans * operands[i];
            //    }
            //    else if (operators[j] == '/')
            //    {
            //        ans = ans / operands[i];
            //    }
            //    j++;

            //}
                return (ans);
        }


        //Calculate for System
        //public ActionResult Calculate1(int calcID,int LsystemID)
        //{
        //    //try
        //    //{


        //    //    var option = new List<Option>();
        //    //    var op = db.Option.Where(x => x.LsystemID == LsystemID);
        //    //    foreach (var item in op)
        //    //    {
        //    //        item.OptionValues = db.OptionValue.Where(x => x.OptionID == item.OptionID).ToList();
        //    //        option.Add(item);
        //    //    }

        //    //    return View(option);
        //    //}
        //    //catch(Exception e)
        //    //{
        //    //    ViewBag.Error = e.Message;
        //    //    return View("Error");
        //    //}

        //    var config = db.ConfigurationCollection.Where(x => x.LsystemID == LsystemID).ToList();
        //    foreach(var con in )
        //}

        // POST: Calculations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        

        // GET: Calculations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {


                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Calculation calculation = db.Calculation.Find(id);
                    if (calculation == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0);
                    ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                    return View(calculation);
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View("Error");
                }
            }
            else
                return View("AuthorizationError");
        }

        // POST: Calculations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CalculationID,CalculationName,CalculationFormula,PhysicalUnit,DataStatus,DescriptionEN,DescriptionDE,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,DataFormatID")] Calculation calculation)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    if (db.Calculation.Any(x => x.CalculationName.Equals(calculation.CalculationName) && x.CalculationID != calculation.CalculationID))
                    {
                        ModelState.AddModelError("CalculationName", "Calculation Name already exists");
                        ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                        return View(calculation);
                    }
                    int p = 0;


                    var delimstringlist = new string[] { "Min", "Max", "Abs", "Pow", "+", "-", "*", "/", "(", ")", "²", "³", "Length", " ", "\r", "\n", ",", "[", "]", "Sqrt", "Cubrt", "^" };
                    string[] formulaSplit = calculation.CalculationFormula.Split(delimstringlist, StringSplitOptions.RemoveEmptyEntries);

                    double constant = 0;
                    foreach (var s in formulaSplit)
                    {
                        if (!db.TcSet.Any(x => x.SetName == s && x.DataFormat.FormatType == "Number") && !double.TryParse(s, out constant))
                        {
                            ModelState.AddModelError("CalculationFormula", "Formula contains unidentified property names");
                            ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0 && x.DataFormat.FormatType.Equals("Number"));
                            ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x => x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                            return View(calculation);
                        }
                    }

                    //char[] delimiter = { '+', '-', '*', '/', '(', ')', '²', '³' };
                    //string[] set = calculation.CalculationFormula.Split(delimiter);
                    //foreach (string s in set)
                    //    if (!db.TcSet.Any(x => x.SetName.Equals(s)) && !String.IsNullOrWhiteSpace(s) && s != "Length")
                    //        p++;

                    //if (p != 0)
                    //{
                    //    ModelState.AddModelError("CalculationFormula", "Formula contains unidentified property names");
                    //    ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0);
                    //    ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                    //    return View(calculation);
                    //}
                    //string formula = calculation.CalculationFormula;
                    //char last = formula[formula.Length - 1];
                    //if (last == '+' || last == '-' || last == '/' || last == '*' || last == '(')
                    //{
                    //    ModelState.AddModelError("CalculationFormula", "Formula cannot end with an operator");
                    //    ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0);
                    //    ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                    //    return View(calculation);
                    //}
                    calculation.ModifiedBy = User.Identity.Name;
                    calculation.ModifiedOn = DateTime.Now;
                    db.Entry(calculation).State = EntityState.Modified;
                    db.Entry(calculation).Property(x => x.CreatedOn).IsModified = false;
                    db.Entry(calculation).Property(x => x.CreatedBy).IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.set = db.TcSet.Where(x => x.TcSetID != 0);
                ViewBag.DataFormatID = new SelectList(db.DataFormat.OrderBy(x=>x.FormatName), "DataFormatID", "FormatName", calculation.DataFormatID);
                return View(calculation);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Error");
            }
        }

        // GET: Calculations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {


                    if (id == null)
                    {
                        ViewBag.Error = "A null parameter was passed to the function";
                        return View("Error");
                    }
                    Calculation calculation = db.Calculation.Find(id);
                    if (calculation == null)
                    {
                        ViewBag.Error = "The requested calculation does not exist";
                        return View("Error");
                    }
                    return View(calculation);
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

        // POST: Calculations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {


                Calculation calculation = db.Calculation.Find(id);
                db.Calculation.Remove(calculation);
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
