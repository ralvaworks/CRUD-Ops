using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace MVC.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            IEnumerable<mvcCustomerModel> custList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Customer").Result;
            custList = response.Content.ReadAsAsync<IEnumerable<mvcCustomerModel>>().Result;
            return View(custList);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new mvcCustomerModel());
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Customer/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<mvcCustomerModel>().Result);
            }

        }
        [HttpPost]
        public ActionResult AddOrEdit(mvcCustomerModel cust)
        {
            if (cust.CustomerID == 0)
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PostAsJsonAsync("Customer", cust).Result;
                TempData["SuccessMessage"] = "Saved Record Successfully";
            }
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("Customer/" + cust.CustomerID, cust).Result;
                TempData["SuccessMessage"] = "Updated Record Successfully";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("Customer/" + id.ToString()).Result;
            TempData["SuccessMessage"] = "Deleted Record Successfully";
            return RedirectToAction("Index");
        }

        public void ExportContentToCSV()
        {
            var sw = new StringWriter();
            sw.WriteLine("\"CustomerID\", \"Name\", \"Phone\", \"Address\", \"InvoiceID\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", string.Format("attachment; filename = CustomerListing_{0}.CSV", DateTime.Now));
            Response.ContentType = "text/csv";

            IEnumerable<mvcCustomerModel> custList;
            HttpResponseMessage resp = GlobalVariables.WebApiClient.GetAsync("Customer").Result;
            custList = resp.Content.ReadAsAsync<IEnumerable<mvcCustomerModel>>().Result;

            var listofCustomers = custList.OrderBy(x => x.CustomerID).ToList();
            foreach (var cust in listofCustomers)
            {
                sw.WriteLine(string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"",
                                            cust.CustomerID, cust.Name, cust.Phone, cust.Address, cust.InvoiceID));
            }

            Response.Write(sw.ToString());
            Response.End();
            TempData["SuccessMessage"] = "Exported Records Successfully";
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;

            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                filePath = path + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(filePath);

                // create a DataTable
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[5] 
                {
                    new DataColumn("custid", typeof(int)),
                    new DataColumn("name", typeof(string)),
                    new DataColumn("fone", typeof(string)),
                    new DataColumn("addr", typeof(string)),
                    new DataColumn("invid", typeof(int))
                });

                // read contents of CSV file
                string csvData = System.IO.File.ReadAllText(filePath);

                // execute loop over the rows
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        // execute loop over the columns
                        foreach (string cell in row.Split(','))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string connString = WebConfigurationManager.ConnectionStrings["dbCS"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connString))
                {
                    using (SqlBulkCopy sqlBulk = new SqlBulkCopy(con))
                    {
                        //set the database table name
                        sqlBulk.DestinationTableName = "dbo.Customer";

                        //[OPTIONAL]: map the datatable columns with that of the database table
                        //sqlBulk.ColumnMappings.Add("custid", "CustomerID");
                        sqlBulk.ColumnMappings.Add("name", "Name");
                        sqlBulk.ColumnMappings.Add("fone", "Phone");
                        sqlBulk.ColumnMappings.Add("addr", "Address");
                        sqlBulk.ColumnMappings.Add("invid", "InvoiceID");
                        
                        con.Open();
                        sqlBulk.WriteToServer(dt);
                        con.Close();

                    }
                }

                TempData["SuccessMessage"] = "Imported Records Successfully";
            }
            
            return RedirectToAction("Index");
        }
    }
}