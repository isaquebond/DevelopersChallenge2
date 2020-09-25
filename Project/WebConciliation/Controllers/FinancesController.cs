using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebConciliation.Models;

namespace WebConciliation.Controllers
{
    public class FinancesController : Controller
    {
        // GET: Finances
        public ActionResult Index()
        {
            string file_ = "";
            var listBank_Statement = new List<Bank_Statement>();

            if(Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    if (file.ContentLength > 0)
                    {
                        file_ = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Guid.NewGuid().ToString(), ".ofx"));
                        file.SaveAs(file_);

                        XElement doc = ImportOfx.toXElement(file_);

                        System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(new StringReader(doc.ToString()));
                        reader.Read();

                        DataSet ds = new DataSet();
                        ds.ReadXml(reader, XmlReadMode.Auto);
                        

                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {
                                var m = new Bank_Statement()
                                {
                                    Date = dr["DTPOSTED"].ToString(),
                                    Value = Convert.ToDecimal(dr["TRNAMT"].ToString()),
                                    Description = dr["MEMO"].ToString(),
                                    Type = dr["TRNTYPE"].ToString()
                                };

                                if(!isExist(listBank_Statement, m))
                                {
                                    listBank_Statement.Add(m);
                                }
                            }
                        }

                        TempData["success"] = "File imported successfully.";
                    }
                    else
                    {
                        TempData["error"] = "No file selected.";
                    }
                }
            }

            ViewBag.ITENS_FILE = listBank_Statement;

            return View();
        }

        private static bool isExist(List<Bank_Statement> x, Bank_Statement y)
        {
            bool _result = false;

            var objCons = x.Find(a => a.Date.Equals(y.Date)
                                            && a.Value.Equals(y.Value)
                                            && a.Description.Equals(y.Description)
                                            && a.Type.Equals(y.Type)
                                            );

            if (objCons != null)
                _result = true;

            return _result;
        }


    }
}