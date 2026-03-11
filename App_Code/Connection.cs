using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.Common;
using System.Diagnostics;

namespace DBConnection
{
    public class Connection
    {
        protected string sCon = ConfigurationManager.ConnectionStrings["FruitUnited"].ConnectionString;

        public DataTable FillDatatable(string sql)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.CommandTimeout = 100;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    conn.Close();
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable FillDatatable(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection conn = new SqlConnection(sCon))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.CommandTimeout = 100;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    conn.Close();
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public int ExecuteSQLQuery(string SQL)
        {
            try
            {
                using (SqlConnection OConn = new SqlConnection(sCon))
                {
                    OConn.Open();
                    SqlCommand command = new SqlCommand(SQL, OConn);
                    command.CommandTimeout = 180;
                    int result = command.ExecuteNonQuery();

                    OConn.Close();

                    return result;
                }
            }
            catch (SqlException exp)
            {
                //Snapshot update conflict
                if (exp.Number == 3960)
                    throw new Exception("There was another user/process updating same record. Please try again.");

                throw new Exception(SQL);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("SQL : {0] : {1}", exp.Message, exp.StackTrace));
            }
        }

        public object ExecuteSQLQueryWithOneReturn(string SQL)
        {
            try
            {
                using (SqlConnection OConn = new SqlConnection(sCon))
                {
                    OConn.Open();


                    SqlCommand command = new SqlCommand(SQL, OConn);
                    command.CommandTimeout = 180;
                    object result = command.ExecuteScalar();

                    OConn.Close();

                    return result;
                }
            }
            catch (SqlException exp)
            {
                //Snapshot update conflict
                if (exp.Number == 3960)
                    throw new Exception("There was another user/process updating same record. Please try again.");

                throw new Exception(SQL);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("SQL : {0] : {1}", exp.Message, exp.StackTrace));
            }
        }

        public DataSet FillDataSet(string sSQL)
        {
            sSQL.Trim();
            try
            {

                DataSet ds = new DataSet();
                using (SqlConnection OConn = new SqlConnection(sCon))
                {
                    OConn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sSQL, OConn);
                    adapter.SelectCommand.CommandTimeout = 180;
                    adapter.Fill(ds);
                    OConn.Close();
                }
                return ds;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message, "FillDataSet");
                throw new Exception(exp.Message + " : " + sSQL);
            }
            finally
            {

            }
        }

    }
}