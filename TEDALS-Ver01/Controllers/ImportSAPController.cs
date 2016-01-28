using Excel;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TEDALS_Ver01.DAL;
using TEDALS_Ver01.Models;
using TEDALS_Ver01.ViewModels;
using TEDALS_Ver01.ViewModels.ImportSAP;

namespace TEDALS_Ver01.Controllers
{
    public class ImportSAPController : Controller
    {
        private TedalsContext db = new TedalsContext();

        //Index  page for SAP
        public ActionResult ImportSAP()
        {         
            return View();
        }

        //Interface for upload
        public ActionResult Upload()
        {
            return View();
        }

        //Save content to database (POST method)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;
                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        // upload.SaveAs(Server.MapPath("/App_Data/Excel Files/" + upload.FileName));
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        //string filepath = Server.MapPath("/App_Data/Excel Files/" + upload.FileName);
                        //upload.SaveAs(filepath);
                        //ImportExcel(filepath);                        
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    DataSet result = reader.AsDataSet();
                    reader.Close();

                    DataTable dt = result.Tables[0];
                    bool flag = true;

                    //Delete existing contents of SAP file from database
                    if(db.FromSAP.ToList().Count()!=0)
                        db.Database.ExecuteSqlCommand("TRUNCATE TABLE [FromSAP]");

                    //Saving values of SAP file to database
                    foreach(DataRow dr in dt.Rows)
                    {
                        string filter1 = "R5G_MX_SYS_1";
                        string filter2 = "R5G_MX_SYS_2";
                        string filter3 = "R5G_MX_SYS_3";
                        string filter4 = "R5G_MX_SYS_4";
                        string filter5 = "R5G_MX_SYS_5";
                        string filter6 = "R5G_MX_SYS_6";
                        string filter7 = "R5G_MX_SYS_7";
                        string filter8 = "R5G_MX_SYS_8";
                        string filter9 = "R5G_MX_SYS_9";
                        string filter0 = "R5G_MX_SYS_0";
                        string matno = dr["Column1"].ToString();
                        string optionname = dr["Column3"].ToString();
                        string filter = dr["Column4"].ToString();
                        string optionval = dr["Column5"].ToString();
                        string description = dr["Column6"].ToString();
                        if(String.IsNullOrEmpty(matno)||String.IsNullOrEmpty(optionname)||String.IsNullOrEmpty(filter)||String.IsNullOrEmpty(optionval)||string.IsNullOrEmpty(description)||optionval=="999")
                        {
                            flag = false;
                        }
                        else if (filter.StartsWith(filter1) || filter.StartsWith(filter2) || filter.StartsWith(filter3) || filter.StartsWith(filter4) || filter.StartsWith(filter5) || filter.StartsWith(filter6) || filter.StartsWith(filter7) || filter.StartsWith(filter8) || filter.StartsWith(filter9) || filter.StartsWith(filter0))
                        {
                            var fromsap = new FromSAP{
                                MaterialNumber= matno,
                                Description= description,
                                OptionName = optionname,
                                OptionValue = optionval
                            };
                            db.FromSAP.Add(fromsap);
                        }
                    }
                    db.SaveChanges();                   
                    return View("ImportSAP");
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View("Upload");
        }

        public ActionResult CompareSystem()
        {
            if(db.FromSAP.Count()==0)
            {
                ViewBag.Message = "There are no imported contents in database. Please upload a SAP file";
                return View("Upload");
            }

            var model = new SystemSAP { d_s_match= new List<DB_SAP_Match>(), d_s_mismatch= new List<DB_SAP_Mismatch>(), s_d_mismatch=new List<SAP_DB_mismatch>() };
            var sap_materialno = db.FromSAP.Select(x => x.MaterialNumber).Distinct().ToList();
            var db_materialno = db.Lsystem.Select(x => x.MaterialNumber).Distinct().ToList();
            db_materialno.Remove("##########");
            foreach(var item in sap_materialno.Intersect(db_materialno).ToList())
            {
                var modelitem = new DB_SAP_Match();
                modelitem.MaterialNo = item;
                modelitem.SystemName = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemName;
                modelitem.LsystemID = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemID;
                model.d_s_match.Add(modelitem);
            }

            foreach(var item in db_materialno)
            {
                var modelitem = new DB_SAP_Mismatch();
                if(!sap_materialno.Contains(item))
                {
                    modelitem.MaterialNo = item;
                    modelitem.SystemName = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemName;
                    modelitem.LsystemID = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemID;
                    model.d_s_mismatch.Add(modelitem);
                }
            }

            foreach(var item in sap_materialno)
            {
                var modelitem = new SAP_DB_mismatch();
                if (!db_materialno.Contains(item))
                {
                    modelitem.MaterialNo = item;                    
                    model.s_d_mismatch.Add(modelitem);
                }
            }

            return View(model);
        }

        public ActionResult CompareOption(int[] id)
        {
            var model = new List<OptionSAP>();
            var sap_materialno = db.FromSAP.Select(x => x.MaterialNumber).Distinct().ToList();
            var db_materialno = db.Lsystem.Select(x => x.MaterialNumber).Distinct().ToList();
            db_materialno.Remove("##########");
            if(id==null)
            {
                foreach(var item in sap_materialno.Intersect(db_materialno).ToList())
                {
                    var modelitem = new OptionSAP { d_s_match = new List<DB_SAP_Match1>(), d_s_mismatch = new List<DB_SAP_Mismatch1>(), s_d_mismatch = new List<SAP_DB_Mismatch1>() };
                    int lsysid = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemID;
                    modelitem = CompareOption1(lsysid);
                    model.Add(modelitem);
                }
            }
            else
            {
                foreach(var item in id)
                {
                    var modelitem = new OptionSAP { d_s_match = new List<DB_SAP_Match1>(), d_s_mismatch = new List<DB_SAP_Mismatch1>(), s_d_mismatch = new List<SAP_DB_Mismatch1>() };                    
                    modelitem = CompareOption1(item);
                    model.Add(modelitem);
                }
            }
            ViewBag.SystemFilter = db.Lsystem.Where(x => x.LsystemID != 1038).ToList();
            
            return View(model);
        }

        public OptionSAP CompareOption1(int id)
        {
            var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
            var modelitem = new OptionSAP { d_s_match = new List<DB_SAP_Match1>(), d_s_mismatch = new List<DB_SAP_Mismatch1>(), s_d_mismatch = new List<SAP_DB_Mismatch1>() };
            var db_oplist = db.Option.Where(x => x.LsystemID == id).Select(x=>x.OptionName).ToList();
            foreach(var item in db.FromSAP.Where(x=>x.MaterialNumber==sys.MaterialNumber).ToList())
            {
                if(db_oplist.Contains(item.OptionName))
                {
                    if (!modelitem.d_s_match.Any(x => x.OptionName == item.OptionName))
                    {
                        var match = new DB_SAP_Match1
                        {
                            LsystemID = id,
                            MaterialNo = sys.MaterialNumber,
                            OptionName = item.OptionName,
                            SystemName = sys.LsystemName,
                            OptionID = db.Option.FirstOrDefault(x => x.LsystemID == id && x.OptionName == item.OptionName).OptionID
                        };
                        modelitem.d_s_match.Add(match);
                    }
                }
                else 
                {
                    if (!modelitem.s_d_mismatch.Any(x => x.OptionName == item.OptionName))
                    {
                        var mismatch = new SAP_DB_Mismatch1
                        {
                            LsystemID = id,
                            MaterialNo = sys.MaterialNumber,
                            OptionName = item.OptionName,
                            SystemName = sys.LsystemName
                        };
                        modelitem.s_d_mismatch.Add(mismatch);
                    }
                }
                    
            }
            foreach(var item in db_oplist)
            {
                if(!db.FromSAP.Where(x=>x.MaterialNumber==sys.MaterialNumber).Select(x=>x.OptionName).ToList().Contains(item) && !modelitem.d_s_mismatch.Any(x=>x.OptionName==item)&&!modelitem.d_s_match.Any(x=>x.OptionName==item))
                {
                    var mismatch = new DB_SAP_Mismatch1
                    {
                        LsystemID = id,
                        MaterialNo = sys.MaterialNumber,
                        OptionID = db.Option.FirstOrDefault(x => x.LsystemID == id && x.OptionName == item).OptionID,
                        OptionName = item,
                        SystemName = sys.LsystemName
                    };
                    modelitem.d_s_mismatch.Add(mismatch);
                }
            }
            return modelitem;
        }

        public ActionResult CompareOptionValues(int[] id)
        {
            var model = new List<OptionValueSAP>();
            var sap_materialno = db.FromSAP.Select(x => x.MaterialNumber).Distinct().ToList();
            var db_materialno = db.Lsystem.Select(x => x.MaterialNumber).Distinct().ToList();
            db_materialno.Remove("##########");
            if(id==null)
            {
                foreach(var item in sap_materialno.Intersect(db_materialno).ToList())
                {
                    int lsysid = db.Lsystem.FirstOrDefault(x => x.MaterialNumber == item).LsystemID;
                    var modelitem = new OptionValueSAP { d_s_match = new List<DB_SAP_Match2>(), d_s_mismatch = new List<DB_SAP_Mismatch2>(), s_d_mismatch = new List<SAP_DB_Mismatch2>() };
                    modelitem = CompareOptionValues1(lsysid);
                    model.Add(modelitem);
                }
            }
            else
            {
                foreach(var lsysid in id)
                {
                    var modelitem = new OptionValueSAP { d_s_match = new List<DB_SAP_Match2>(), d_s_mismatch = new List<DB_SAP_Mismatch2>(), s_d_mismatch = new List<SAP_DB_Mismatch2>() };
                    modelitem = CompareOptionValues1(lsysid);
                    model.Add(modelitem);
                }
            }
            ViewBag.SystemFilter = db.Lsystem.Where(x => x.LsystemID != 1038).ToList();
            return View(model);
        }

        public OptionValueSAP CompareOptionValues1(int id)
        {
            var modelitem = new OptionValueSAP { d_s_match = new List<DB_SAP_Match2>(), s_d_mismatch = new List<SAP_DB_Mismatch2>(), d_s_mismatch = new List<DB_SAP_Mismatch2>() };
            var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == id);
            var oplist = db.Option.Where(x => x.LsystemID == id).ToList();
            var ovlist = new List<OptionValue>();
            
            foreach(var item in db.FromSAP.Where(x=>x.MaterialNumber==sys.MaterialNumber).ToList())
            {
                bool match = false;
                bool s_d_mis = false;
                
                double ov_sap;
                bool flag = double.TryParse(item.OptionValue, out ov_sap);
                foreach(var op in oplist)
                {
                    foreach(var ov in op.OptionValues)
                    {
                        double ov_db;
                        bool flag_db = double.TryParse(ov.OptionVal, out ov_db);
                        if(flag&&flag_db && ov_db==ov_sap && op.OptionName==item.OptionName)
                        {
                            var p = new DB_SAP_Match2
                            {
                                DescriptionDB = ov.DescriptionEN,
                                DescriptionSAP = item.Description,
                                MaterialNo = item.MaterialNumber,
                                OptionID = op.OptionID,
                                OptionName = op.OptionName,
                                Optionval = ov.OptionVal,
                                OptionValID = ov.OptionValueID,
                                SystemID = id,
                                SystemName = sys.LsystemName
                            };

                            modelitem.d_s_match.Add(p);
                            match = true;
                            break;
                        }
                        else if(!flag&&!flag_db&& item.OptionValue==ov.OptionVal && op.OptionName==item.OptionName)
                        {
                            var p = new DB_SAP_Match2
                            {
                                DescriptionDB = ov.DescriptionEN,
                                DescriptionSAP = item.Description,
                                MaterialNo = item.MaterialNumber,
                                OptionID = op.OptionID,
                                OptionName = op.OptionName,
                                Optionval = ov.OptionVal,
                                OptionValID = ov.OptionValueID,
                                SystemID = id,
                                SystemName = sys.LsystemName
                            };


                            modelitem.d_s_match.Add(p);
                            match = true;
                            break;
                        }

                    }
                }
                if(!match)
                {
                    var p = new SAP_DB_Mismatch2
                    {
                        Description = item.Description,
                        MaterialNo = item.MaterialNumber,
                        OptionName = item.OptionName,
                        Optionval = item.OptionValue,
                        SystemID = id,
                        SystemName = sys.LsystemName
                    };
                    if (db.Option.Any(x => x.LsystemID == id && x.OptionName == item.OptionName))
                        p.OptionID = db.Option.FirstOrDefault(x => x.LsystemID == id && x.OptionName == item.OptionName).OptionID;
                    else
                        p.OptionID = 0;
                    modelitem.s_d_mismatch.Add(p);
                }
            }

                foreach(var op in db.Option.Where(x=>x.LsystemID==id))
                {
                    foreach(var ov in op.OptionValues)
                    {
                        //bool d_s_mis = false;
                        //foreach(var item in db.FromSAP.Where(x=>x.MaterialNumber==sys.MaterialNumber&& x.OptionName==op.OptionName))
                        //{
                        //    var p = new DB_SAP_Mismatch2
                        //    {
                        //        Description = ov.DescriptionEN,
                        //        MaterialNo = item.MaterialNumber,
                        //        OptionID = op.OptionID,
                        //        OptionName = op.OptionName,
                        //        Optionval = ov.OptionVal,
                        //        OptionValID = ov.OptionValueID,
                        //        SystemID = id,
                        //        SystemName = sys.LsystemName,
                        //    };
                        //    d_s_mis = true;
                        //    modelitem.d_s_mismatch.Add(p);
                        //    break;
                        //}
                        //if(!d_s_mis)
                        //{
                        //var p = new DB_SAP_Mismatch2
                        //{
                        //    Description = ov.DescriptionEN,
                        //    MaterialNo = sys.MaterialNumber,
                        //    OptionID = op.OptionID,
                        //    OptionName = op.OptionName,
                        //    Optionval = ov.OptionVal,
                        //    OptionValID = ov.OptionValueID,
                        //    SystemID = id,
                        //    SystemName = sys.LsystemName,
                        //};
                        //modelitem.d_s_mismatch.Add(p);
                        //}
                        
                        if(!modelitem.d_s_match.Any(x=>x.OptionName==op.OptionName&& x.Optionval==ov.OptionVal)&&!modelitem.s_d_mismatch.Any(x=>x.OptionName==op.OptionName&&x.Optionval==ov.OptionVal))
                        {
                            var p = new DB_SAP_Mismatch2
                            {
                                Description = ov.DescriptionEN,
                                MaterialNo = sys.MaterialNumber,
                                OptionID = op.OptionID,
                                OptionName = op.OptionName,
                                Optionval = ov.OptionVal,
                                OptionValID = ov.OptionValueID,
                                SystemID = id,
                                SystemName = sys.LsystemName,
                            };
                            modelitem.d_s_mismatch.Add(p);
                        }
                    }
                }
            
            return modelitem;
        }


        public ActionResult OptionCreate(string OptionName, int lsysid)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                

                var model = new Option { OptionName = OptionName, LsystemID = lsysid };
                var sys = db.Lsystem.FirstOrDefault(x => x.LsystemID == lsysid);
                ViewBag.Lsystem = sys.LsystemName;
                ViewBag.FamilyName = sys.LsystemFamily.FamilyName;
                ViewBag.TechnicalCharacteristicID = new SelectList(db.TechnicalCharacteristic.OrderBy(x => x.TCName), "TechnicalCharacteristicID", "TCName");
                var tc = db.TechnicalCharacteristic.Any();
                if (!tc)
                    ViewBag.Message = "There are no technical Characteristics to display. Please Add new Technical Characteristics";
                return View(model);
            }
            else
                return View("AuthorizationError");
        }


        public ActionResult OptionValueCreate(string OptionValue, int opid, string desc)
        {
            if (db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsAdmin || db.UserRight.FirstOrDefault(x => x.UserCode == User.Identity.Name).IsEditor)
            {
                try
                {
                    //if (OptionID == null)
                    //{
                    //    ViewBag.Error = "A null parameter was passed to the function";
                    //    return View("Error");
                    //}
                    var model = new OptionValue
                    {
                        OptionID = opid, OptionVal = OptionValue, DescriptionDE=desc, DescriptionEN=desc

                    };
                    var op = db.Option.FirstOrDefault(x => x.OptionID == opid);
                    ViewBag.Option = op.OptionName;
                    ViewBag.Lsystem = op.Lsystem.LsystemName;
                    ViewBag.FamilyName = op.Lsystem.LsystemFamily.FamilyName;
                    //ViewBag.OptionID = new SelectList(db.Option.OrderBy(x => x.OptionName), "OptionID", "OptionName");
                    return View(model);
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
        
        //[HttpPost]
        //public ActionResult FileUpload(HttpPostedFileBase file)
        //{
        //    DataSet ds = new DataSet();
        //    if (Request.Files["file"].ContentLength > 0)
        //    {
        //        string fileExtension =
        //                             System.IO.Path.GetExtension(Request.Files["file"].FileName);

        //        if (fileExtension == ".xls" || fileExtension == ".xlsx")
        //        {
        //            string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
        //            if (System.IO.File.Exists(fileLocation))
        //            {

        //                System.IO.File.Delete(fileLocation);
        //            }
        //            Request.Files["file"].SaveAs(fileLocation);
        //            string excelConnectionString = string.Empty;
        //            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
        //            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //            //connection String for xls file format.
        //            if (fileExtension == ".xls")
        //            {
        //                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
        //                fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //            }
        //            //connection String for xlsx file format.
        //            else if (fileExtension == ".xlsx")
        //            {
        //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
        //                fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //            }
        //            //Create Connection to Excel work book and add oledb namespace
        //            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //            excelConnection.Open();
        //            DataTable dt = new DataTable();

        //            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //            if (dt == null)
        //            {
        //                return null;
        //            }

        //            String[] excelSheets = new String[dt.Rows.Count];
        //            int t = 0;
        //            //excel data saves in temp file here.
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                excelSheets[t] = row["TABLE_NAME"].ToString();
        //                t++;
        //            }
        //            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


        //            string query = string.Format("Select * from [{0}]", excelSheets[0]);
        //            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        //            {
        //                dataAdapter.Fill(ds);
        //            }
        //        }
        //        if (fileExtension.ToString().ToLower().Equals(".xml"))
        //        {
        //            string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
        //            if (System.IO.File.Exists(fileLocation))
        //            {
        //                System.IO.File.Delete(fileLocation);
        //            }

        //            Request.Files["FileUpload"].SaveAs(fileLocation);
        //            XmlTextReader xmlreader = new XmlTextReader(fileLocation);
        //            // DataSet ds = new DataSet();
        //            ds.ReadXml(xmlreader);
        //            xmlreader.Close();
        //        }

        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            string conn = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        //            SqlConnection con = new SqlConnection(conn);
        //            string query = "Insert into Person(Name,Email,Mobile) Values('" +
        //            ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() +
        //            "','" + ds.Tables[0].Rows[i][2].ToString() + "')";
        //            con.Open();
        //            SqlCommand cmd = new SqlCommand(query, con);
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }
        //    return View();
        //}

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            var model = new List<DisplayFromFile>();
            // Verify that the user selected a file
            //if (file != null && file.ContentLength > 0)
            //{
            //    // extract only the filename
            //    var fileName = Path.GetFileName(file.FileName);
            //    // store the file inside ~/App_Data/uploads folder
            //    var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
            //    file.SaveAs(path);
            //}
            //// redirect back to the index action to show the form once again
            //return RedirectToAction("Index");
            //FileUpload fu = file;
            //if (fu.HasFile)
            //{
            //    StreamReader reader = new StreamReader(fu.FileContent);
            //    do
            //    {
            //        string textLine = reader.ReadLine();

            //        // do your coding 
            //        //Loop trough txt file and add lines to ListBox1  

            //    } while (reader.Peek() != -1);
            //    reader.Close();
            //}
            //return View();


            string result = new StreamReader(file.InputStream).ReadToEnd();
            string[] multiline = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach(var item in multiline)
            {
                string[] split1 = item.Split(';');
                var modelitem = new DisplayFromFile();
                if (split1.Length == 3)
                {
                    var mat_opsplit = split1[0].Split('_');
                    
                    if (mat_opsplit[3].Any(c => char.IsDigit(c)))
                    {
                        modelitem.MaterialNumber = mat_opsplit[3];
                        modelitem.OptionName = "";
                        for (int i = 4; i < mat_opsplit.Length; i++)
                            modelitem.OptionName = modelitem.OptionName + mat_opsplit[i];
                    }
                        
                    else
                    {
                        modelitem.MaterialNumber = "";
                        modelitem.OptionName = "";
                        for (int i = 3; i < mat_opsplit.Length; i++)
                            modelitem.OptionName = modelitem.OptionName + mat_opsplit[i];
                    }
                        

                    
                    modelitem.OptionValue = split1[1];
                    modelitem.Description = split1[2];
                }

                
                //else
                //{
                //    modelitem.MaterialNumber = 0;
                //    modelitem.OptionValue=split1[]
                //}
                model.Add(modelitem);
                
               
            }
            //foreach (var item in db.FromSAP.ToList())
            //    db.FromSAP.Remove(item);
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [FromSAP]");
            foreach (var item1 in model)
            {
                var fromsap = new FromSAP
                {
                    MaterialNumber = item1.MaterialNumber,
                    OptionName = item1.OptionName,
                    OptionValue = item1.OptionValue,
                    Description = item1.Description
                };
                db.FromSAP.Add(fromsap);
            }
            db.SaveChanges();
            return View("DisplayFromFile",model);
        }




        public ActionResult Compare()
        {
            var model = new SAP_Table_View { FromSAP = new List<FromSAP>(), SAP_Existing = new List<SAP_Existing>() };
            model.FromSAP = db.FromSAP.ToList();
            foreach(var ov in db.OptionValue.ToList())
            {
                var mo = new SAP_Existing();
                mo.Description = ov.DescriptionEN;
                mo.OptionValues = ov.OptionVal;
                mo.OptionName = ov.Option.OptionName;
                mo.MaterialNumber = ov.Option.Lsystem.MaterialNumber.Remove(0,1);
                model.SAP_Existing.Add(mo);
            }


            return View(model);
        }
    }
}