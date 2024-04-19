using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class dbconnection
    {
        public SqlConnection cn = new SqlConnection("Data Source=ARJAV-TAB;Initial Catalog=ProjectLicense;Integrated Security=True;Encrypt=False");
        public SqlConnection getcon()
        {
            if(cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            return cn;
        }

        public int ExeNonQuery(SqlCommand cmd)
        {
            cmd.Connection = getcon();
            int rowsAddfected = -1;
            rowsAddfected = cmd.ExecuteNonQuery();
            cn.Close(); 
            return rowsAddfected;
        }

        public object ExeScalar(SqlCommand cmd)
        {
            cmd.Connection = getcon();
            object val = -1;
            val = cmd.ExecuteScalar();
            cn.Close();
            return val;
        }
        public DataTable ExeReader(SqlCommand cmd)
        {
            cmd.Connection = getcon();
            SqlDataReader dr;
            DataTable dt = new DataTable();

            dr = cmd.ExecuteReader();
            dt.Load(dr);
            cn.Close();
            return dt;
        }
    }
}
