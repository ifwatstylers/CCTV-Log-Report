using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using Read_Excel_OLEDB_MVC.Models;


namespace Import_CSV_SqlDB_MVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(clsNonUpdatedCCTV modelData)
        {
            ViewBag.Title = "PLUS CCTV";
            //if (!string.IsNullOrEmpty(Session["validsession"] as string))
            //{
                #region READ CSV FILE FROM URL CALL
                //read csv file online
                string SAMPLEurlAzureFile = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/testcsv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";
                string urlAzureNonUpdatedCCTV = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/notupdated_cctv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";
                string urlAzureUpdatedCCTV = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/updated_cctv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";

                HttpWebRequest reqNonUpdatedCCTV = (HttpWebRequest)WebRequest.Create(urlAzureNonUpdatedCCTV);
                HttpWebResponse resp = (HttpWebResponse)reqNonUpdatedCCTV.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string resultsNonUpdatedCCTV = sr.ReadToEnd();

                HttpWebRequest reqUpdatedCCTV = (HttpWebRequest)WebRequest.Create(urlAzureUpdatedCCTV);
                HttpWebResponse respUpdated = (HttpWebResponse)reqUpdatedCCTV.GetResponse();
                StreamReader srUpdated = new StreamReader(respUpdated.GetResponseStream());
                string resultsUpdatedCCTV = srUpdated.ReadToEnd();
                //end read csv file online
                #endregion

                #region read NONUPDATED CCTV csv and store into DB
                //Create a DataTable.
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Name", typeof(string)),
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Ip_Add",typeof(string)) });


                //Read the contents of CSV file for NONUPDATED.
                string csvDataNonUpdated = resultsNonUpdatedCCTV;

                //Execute a loop over the rows.
                foreach (string row in csvDataNonUpdated.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {

                        con.Open();
                        string sql = @"DELETE NONUPDATED_CCTV;";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "NONUPDATED_CCTV";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Name", "NAME");
                        sqlBulkCopy.ColumnMappings.Add("Date", "DATE");
                        sqlBulkCopy.ColumnMappings.Add("Ip_Add", "IP_ADD");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
                #endregion

                #region read UPDATED CCTV csv and store into DB
                //Create a DataTable.
                DataTable dtUp = new DataTable();
                dtUp.Columns.AddRange(new DataColumn[3] { new DataColumn("Name", typeof(string)),
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Ip_Add",typeof(string)) });


                //Read the contents of CSV file for NONUPDATED.
                string csvDataUpdated = resultsUpdatedCCTV;

                //Execute a loop over the rows.
                foreach (string row in csvDataUpdated.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dtUp.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dtUp.Rows[dtUp.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string conStringUpdated = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {

                        con.Open();
                        string sql = @"DELETE UPDATED_CCTV;";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "UPDATED_CCTV";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Name", "NAME");
                        sqlBulkCopy.ColumnMappings.Add("Date", "DATE");
                        sqlBulkCopy.ColumnMappings.Add("Ip_Add", "IP_ADD");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dtUp);
                        con.Close();
                    }
                }
                #endregion

                #region COUNT CCTV
                String connectionString = "Server=tcp:festive.database.windows.net,1433;Initial Catalog=FestiveDB;Persist Security Info=False;User ID=admin_festive;Password=P@55w0rd2018;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                SqlConnection connCountCCTV = new SqlConnection(connectionString);
                String sqlList = "SELECT * FROM NONUPDATED_CCTV order by DATE desc";
                String sqlListUpdated = "SELECT * FROM UPDATED_CCTV order by DATE desc";
                SqlCommand cmd2 = new SqlCommand(sqlList, connCountCCTV);
                SqlCommand cmd3 = new SqlCommand(sqlListUpdated, connCountCCTV);

                var model2 = modelData.clsNonUpdatedName.ToList();
                var modelUpdated = modelData.clsUpdatedName.ToList();
                //var modelForNameRSa = modelData.clsNonUpdatedName.ToList();

                using (connCountCCTV)
                {
                    connCountCCTV.Open();
                    SqlDataReader rdr = cmd2.ExecuteReader();

                    while (rdr.Read())
                    {
                        var customer2 = new Student();

                        customer2.FirstName = rdr["NAME"].ToString() + " (" + rdr["DATE"].ToString() + ")" + " - " + rdr["IP_ADD"].ToString();
                        model2.Add(customer2.FirstName.ToString());

                    }
                    connCountCCTV.Close();
                    connCountCCTV.Open();
                    SqlDataReader rdrUpdated = cmd3.ExecuteReader();

                    while (rdrUpdated.Read())
                    {
                        var customer3 = new Student();

                        customer3.LastName = rdrUpdated["NAME"].ToString() + " (" + rdrUpdated["DATE"].ToString() + ")" + " - " + rdrUpdated["IP_ADD"].ToString();
                        modelUpdated.Add(customer3.LastName.ToString());

                    }
                    connCountCCTV.Close();


                    connCountCCTV.Open();
                    String sqlCount = "SELECT COUNT(NAME) FROM NONUPDATED_CCTV";
                    SqlCommand cmdCount = new SqlCommand(sqlCount, connCountCCTV);
                    Int32 count = (Int32)cmdCount.ExecuteScalar();
                    ViewBag.NonUpdatedCount = count.ToString();
                    ViewBag.NonUpdatedCountInt = count;
                    connCountCCTV.Close();
                    connCountCCTV.Open();
                    String sqlCountUpdated = "SELECT COUNT(NAME) FROM UPDATED_CCTV";
                    SqlCommand cmdCountUpdated = new SqlCommand(sqlCountUpdated, connCountCCTV);
                    Int32 countUpdated = (Int32)cmdCountUpdated.ExecuteScalar();
                    ViewBag.UpdatedCount = countUpdated.ToString();
                    ViewBag.UpdatedCountInt = countUpdated;
                    connCountCCTV.Close();
                }


                modelData.clsNonUpdatedName = model2;
                modelData.clsUpdatedName = modelUpdated;
                #endregion

                return View(modelData);
            //}
            //else
            //{


            //    //ViewBag.NonUpdatedCountInt = 0;
            //    //ViewBag.UpdatedCountInt = 0;
            //    //return View();
            //    return RedirectToAction("SessionClosed");
            //}
        }
        public ActionResult DownloadFile()
        {
            return View();
        }

        public ActionResult PlayVideo()
        {
            return View();
        }


        public ActionResult piechart()
        {
            ViewBag.NonUpdatedCountInt = 2;
            ViewBag.UpdatedCountInt = 10;
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            //BUTTON FUNCTION FOR MANULLY REFRESH DATABASE

            #region browse file csv on local to read - ORIGINAL
            //string filePath = string.Empty;
            //if (postedFile != null)
            //{
            //string path = Server.MapPath("~/Uploads/");
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //filePath = path + Path.GetFileName(postedFile.FileName);
            //string extension = Path.GetExtension(postedFile.FileName);
            //postedFile.SaveAs(filePath);
            //}
            #endregion

            #region READ CSV FILE FROM URL CALL
            //read csv file online
            string SAMPLEurlAzureFile = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/testcsv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";
                string urlAzureNonUpdatedCCTV = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/notupdated_cctv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";
                string urlAzureUpdatedCCTV = "https://plusappcctvuat.file.core.windows.net/cctvfeedfs/log/updated_cctv.csv?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2099-10-04T09:06:59Z&st=2018-10-04T01:06:59Z&spr=https&sig=%2Fb%2BrXtUP5V%2F9%2BSXzpSauyugpG%2BvXOfn9GqLfdf1EOUE%3D";

                HttpWebRequest reqNonUpdatedCCTV = (HttpWebRequest)WebRequest.Create(urlAzureNonUpdatedCCTV);
                HttpWebResponse resp = (HttpWebResponse)reqNonUpdatedCCTV.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string resultsNonUpdatedCCTV = sr.ReadToEnd();

                HttpWebRequest reqUpdatedCCTV = (HttpWebRequest)WebRequest.Create(urlAzureUpdatedCCTV);
                HttpWebResponse respUpdated = (HttpWebResponse)reqUpdatedCCTV.GetResponse();
                StreamReader srUpdated = new StreamReader(respUpdated.GetResponseStream());
                string resultsUpdatedCCTV = srUpdated.ReadToEnd();
                //end read csv file online
                #endregion

            #region read NONUPDATED CCTV csv and store into DB
                //Create a DataTable.
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Name", typeof(string)),
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Ip_Add",typeof(string)) });

                
                //Read the contents of CSV file for NONUPDATED.
                string csvDataNonUpdated = resultsNonUpdatedCCTV;

                //Execute a loop over the rows.
                foreach (string row in csvDataNonUpdated.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        
                        con.Open();
                        string sql = @"DELETE NONUPDATED_CCTV;";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "NONUPDATED_CCTV";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Name", "NAME");
                        sqlBulkCopy.ColumnMappings.Add("Date", "DATE");
                        sqlBulkCopy.ColumnMappings.Add("Ip_Add", "IP_ADD");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
                #endregion

            #region read UPDATED CCTV csv and store into DB
                //Create a DataTable.
                DataTable dtUp = new DataTable();
                dtUp.Columns.AddRange(new DataColumn[3] { new DataColumn("Name", typeof(string)),
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Ip_Add",typeof(string)) });


                //Read the contents of CSV file for NONUPDATED.
                string csvDataUpdated = resultsUpdatedCCTV;

                //Execute a loop over the rows.
                foreach (string row in csvDataUpdated.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dtUp.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dtUp.Rows[dtUp.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string conStringUpdated = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {

                        con.Open();
                        string sql = @"DELETE UPDATED_CCTV;";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "UPDATED_CCTV";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Name", "NAME");
                        sqlBulkCopy.ColumnMappings.Add("Date", "DATE");
                        sqlBulkCopy.ColumnMappings.Add("Ip_Add", "IP_ADD");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dtUp);
                        con.Close();
                    }
                }
            #endregion

            #region COUNT CCTV

            clsNonUpdatedCCTV modelData = new clsNonUpdatedCCTV();
            String connectionString = "Server=tcp:festive.database.windows.net,1433;Initial Catalog=FestiveDB;Persist Security Info=False;User ID=admin_festive;Password=P@55w0rd2018;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection connCountCCTV = new SqlConnection(connectionString);
            String sqlList = "SELECT * FROM NONUPDATED_CCTV order by DATE desc";
            String sqlListUpdated = "SELECT * FROM UPDATED_CCTV order by DATE desc";
            SqlCommand cmd2 = new SqlCommand(sqlList, connCountCCTV);
            SqlCommand cmd3 = new SqlCommand(sqlListUpdated, connCountCCTV);

            var model2 = modelData.clsNonUpdatedName.ToList();
            var modelUpdated = modelData.clsUpdatedName.ToList();
            //var modelForNameRSa = modelData.clsNonUpdatedName.ToList();

            using (connCountCCTV)
            {
                connCountCCTV.Open();
                SqlDataReader rdr = cmd2.ExecuteReader();

                while (rdr.Read())
                {
                    var customer2 = new Student();

                    customer2.FirstName = rdr["NAME"].ToString() + " (" + rdr["DATE"].ToString() + ")" + " - " + rdr["IP_ADD"].ToString();
                    model2.Add(customer2.FirstName.ToString());

                }
                connCountCCTV.Close();
                connCountCCTV.Open();
                SqlDataReader rdrUpdated = cmd3.ExecuteReader();

                while (rdrUpdated.Read())
                {
                    var customer3 = new Student();

                    customer3.LastName = rdrUpdated["NAME"].ToString() + " (" + rdrUpdated["DATE"].ToString() + ")" + " - " + rdrUpdated["IP_ADD"].ToString();
                    modelUpdated.Add(customer3.LastName.ToString());

                }
                connCountCCTV.Close();


                connCountCCTV.Open();
                String sqlCount = "SELECT COUNT(NAME) FROM NONUPDATED_CCTV";
                SqlCommand cmdCount = new SqlCommand(sqlCount, connCountCCTV);
                Int32 count = (Int32)cmdCount.ExecuteScalar();
                ViewBag.NonUpdatedCount = count.ToString();
                ViewBag.NonUpdatedCountInt = count;
                connCountCCTV.Close();
                connCountCCTV.Open();
                String sqlCountUpdated = "SELECT COUNT(NAME) FROM UPDATED_CCTV";
                SqlCommand cmdCountUpdated = new SqlCommand(sqlCountUpdated, connCountCCTV);
                Int32 countUpdated = (Int32)cmdCountUpdated.ExecuteScalar();
                ViewBag.UpdatedCount = countUpdated.ToString();
                ViewBag.UpdatedCountInt = countUpdated;
                connCountCCTV.Close();
            }


            modelData.clsNonUpdatedName = model2;
            modelData.clsUpdatedName = modelUpdated;
            #endregion

            #region READ DB INTO MODEL VIEW

            //String connectionString = "Server=tcp:festive.database.windows.net,1433;Initial Catalog=FestiveDB;Persist Security Info=False;User ID=admin_festive;Password=P@55w0rd2018;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            //String sqlReader = "SELECT * FROM NONUPDATED_CCTV";
            //SqlConnection conn = new SqlConnection(connectionString);
            //conn.Open();
            //SqlCommand cmdReader = new SqlCommand(sqlReader, conn);



            //var model2 = new List<clsNonUpdatedCCTV>();
            //using (conn = new SqlConnection(connectionString))
            //{

            //    SqlDataReader rdr = cmdReader.ExecuteReader();
            //    while (rdr.Read())
            //    {
            //        var cctvLog = new clsNonUpdatedCCTV();
            //        cctvLog.clsNonUpdatedName = rdr["NAME"].ToString();
            //        cctvLog.clsNonUpdatedDate = rdr["DATE"].ToString();
            //        cctvLog.clsNonUpdatedIpAdd = rdr["IP_ADD"].ToString();

            //        model2.Add(cctvLog);

            //    }
            //}
            #endregion


            return View(modelData);
        }

        public ActionResult SessionClosed()
        {
            return View();
        }
    }
}