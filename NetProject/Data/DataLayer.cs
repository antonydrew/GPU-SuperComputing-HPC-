using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace Data
{
    public class SQLConnection
    {
        public SqlConnection conn;

        public SQLConnection()
        {
            conn = new SqlConnection();

            conn.ConnectionString = "Server=sql.cs.luc.edu;uid=adrew;pwd=p27614;" +
                "Initial Catalog=adrew";
            conn.Open();
        }
        public DataSet ExecuteSelect(string sql, DataTable dt)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            da.Fill(ds);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            da.Update(ds.Tables[0]);
            return ds;
        }


        public int ExecuteAction(string sql)
        {
            SqlCommand command = new SqlCommand(sql, conn);
            int result = command.ExecuteNonQuery();

            return result;

        }
    }
}
