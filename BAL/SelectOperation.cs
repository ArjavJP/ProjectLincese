using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessEntityLayer;
using System.Data;
using System.Data.SqlClient;
using DAL;

namespace BAL
{
    public class SelectOperation
    {
        public dbconnection db = new dbconnection();

        public List<object> GetAllData()
        {
        }
    }
}
