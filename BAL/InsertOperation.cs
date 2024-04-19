using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessEntityLayer;
using System.Data;
using System.Data.SqlClient;
using DAL;
using System.Data.Common;
using System.Xml.Linq;

namespace BAL
{
    public class InsertOperation
    {
        public UserMst user = new UserMst();
        public dbconnection db = new dbconnection();
        public DriverMst driver = new DriverMst();
        public ApplicationMst appdb = new ApplicationMst();

        public DataTable login(UserMst user)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select * from UserMst where Email='" + user.Email + "'and Password='" + user.Password + "'";
            return db.ExeReader(cmd);

        }
       
        public int Userinsert(UserMst user)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Insert into UserMst (FullName,UserName,Email,Phone_no,Password,isAdmin) " +
                "Values('" + user.FullName + "','" + user.UserName + "','" + user.Email + "','" + user.Phone_no + "','" + user.Password + "','" + user.IsAdmin + "')";
            return db.ExeNonQuery(cmd);
        }

        public int UpdateDriver(int driverId, DriverMst driver, ApplicationMst appdb)
        {
            int result = -1;
            using (var transaction = db.getcon().BeginTransaction())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db.getcon();
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update DriverMst set FName=@fname,MName =@mname, LName= @lname, Email=@email, DOB=@dob,Gender= @gender, Phone_no= @phone" +
                        ",Address=@Adress,City=@city,Province=@province,Country=@Country,PostalCode=@postalcode,LicenseType=@lic,Document=@document,Document_image=@docimg"+
                        " where Driver_id = @driverid";
                    cmd.Parameters.AddWithValue("@fname", driver.FName);
                    cmd.Parameters.AddWithValue("@mname", driver.MName);
                    cmd.Parameters.AddWithValue("@lname", driver.LName);
                    cmd.Parameters.AddWithValue("@email", driver.Email);
                    cmd.Parameters.AddWithValue("@dob", driver.DOB);
                    cmd.Parameters.AddWithValue("@gender", driver.Gender);
                    cmd.Parameters.AddWithValue("@phone", driver.Phone_no);
                    cmd.Parameters.AddWithValue("@Adress", driver.Address);
                    cmd.Parameters.AddWithValue("@city", driver.City);
                    cmd.Parameters.AddWithValue("@province", driver.Province);
                    cmd.Parameters.AddWithValue("@Country", driver.Country);
                    cmd.Parameters.AddWithValue("@postalcode", driver.PostalCode);
                    cmd.Parameters.AddWithValue("@lic", driver.LicenseType);
                    cmd.Parameters.AddWithValue("@document", driver.Document);
                    cmd.Parameters.AddWithValue("@docimg", driver.Document_image);
                    cmd.Parameters.AddWithValue("@driverid", driverId);
                     result = cmd.ExecuteNonQuery();


                    using (SqlCommand cmd2 = new SqlCommand())
                    {
                        cmd2.Connection = db.getcon();
                        cmd2.Transaction = transaction;
                        cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "INSERT INTO ApplicationMst (LicenseNumber, Driver_Id, Creation_date, Expiry_date, Status, IsRenew,UserID) " +
                           "VALUES (@LicenseNumber, @Driver_Id, @Creation_date, @Expiry_date, @Status,@IsRenew,@UserID)";
                        cmd2.Parameters.AddWithValue("@LicenseNumber", appdb.LicenseNumber);
                        cmd2.Parameters.AddWithValue("@Driver_Id", driverId);
                        cmd2.Parameters.AddWithValue("@Creation_date", appdb.Creation_date);
                        cmd2.Parameters.AddWithValue("@Expiry_date", appdb.Expiry_date);
                        cmd2.Parameters.AddWithValue("@Status", appdb.Status);
                        cmd2.Parameters.AddWithValue("@IsRenew", true);
                        cmd2.Parameters.AddWithValue("@UserID", appdb.Userid);

                        result += cmd2.ExecuteNonQuery();

                    }


                    transaction.Commit();
                    return result; // Return the number of rows affected
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back due to exception: " + ex.Message);
                    return -1; // Return -1 to indicate failure
                }
            }
        }
        public int InsertDriverAndApplication(DriverMst driver, ApplicationMst appdb)
        {
            int result = -1;



            SqlTransaction transaction = db.getcon().BeginTransaction();

            try
            {
                // Insert driver
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = db.getcon();
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO DriverMst (Fname, Mname, Lname, Email, Dob, Gender, Phone_no, Address, City, Province, Country, PostalCode, LicenseType,Document,Document_image,UserID) " +
                           "VALUES (@Fname, @Mname, @Lname, @Email, @Dob, @Gender, @Phone_no, @Address, @City, @Province, @Country, @Postal_code, @License_type,@Document,@docimg,@UserID)";
                    cmd.Parameters.AddWithValue("@Fname", driver.FName);
                    cmd.Parameters.AddWithValue("@Mname", driver.MName);
                    cmd.Parameters.AddWithValue("@Lname", driver.LName);
                    cmd.Parameters.AddWithValue("@Email", driver.Email);
                    cmd.Parameters.AddWithValue("@Dob", driver.DOB);
                    cmd.Parameters.AddWithValue("@Gender", driver.Gender);
                    cmd.Parameters.AddWithValue("@Phone_no", driver.Phone_no);
                    cmd.Parameters.AddWithValue("@Address", driver.Address);
                    cmd.Parameters.AddWithValue("@City", driver.City);
                    cmd.Parameters.AddWithValue("@Province", driver.Province);
                    cmd.Parameters.AddWithValue("@Country", driver.Country);
                    cmd.Parameters.AddWithValue("@Postal_code", driver.PostalCode);
                    cmd.Parameters.AddWithValue("@License_type", driver.LicenseType);
                    cmd.Parameters.AddWithValue("@Document", driver.Document);
                    cmd.Parameters.AddWithValue("@docimg", driver.Document_image);
                    cmd.Parameters.AddWithValue("@UserID", driver.UserID);
                    result = cmd.ExecuteNonQuery();


                    int driverId = 0;
                    using (SqlCommand getIdCommand = new SqlCommand("SELECT Max(Driver_id) From DriverMst", db.getcon(), transaction))
                    {
                        object resultObj = getIdCommand.ExecuteScalar();
                        if (resultObj != null && resultObj != DBNull.Value)
                        {
                            if (int.TryParse(resultObj.ToString(), out int tempDriverId))
                            {
                                driverId = tempDriverId;
                            }
                            else
                            {
                                Console.WriteLine("Failed to convert SCOPE_IDENTITY() to an integer.");
                            }
                        }
                        else
                        {

                            Console.WriteLine("SCOPE_IDENTITY() returned null or DBNull value.");
                        }
                    }

                    using (SqlCommand cmd2 = new SqlCommand())
                    {
                        cmd2.Connection = db.getcon();
                            cmd2.Transaction = transaction;
                            cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "INSERT INTO ApplicationMst (LicenseNumber, Driver_Id, Creation_date, Expiry_date, Status, IsRenew,UserID) " +
                           "VALUES (@LicenseNumber, @Driver_Id, @Creation_date, @Expiry_date, @Status,@IsRenew,@UserID)";
                        cmd2.Parameters.AddWithValue("@LicenseNumber", appdb.LicenseNumber);
                        cmd2.Parameters.AddWithValue("@Driver_Id", driverId);
                        cmd2.Parameters.AddWithValue("@Creation_date", appdb.Creation_date);
                        cmd2.Parameters.AddWithValue("@Expiry_date", appdb.Expiry_date);
                        cmd2.Parameters.AddWithValue("@Status", appdb.Status);
                        cmd2.Parameters.AddWithValue("@IsRenew",false);
                        cmd2.Parameters.AddWithValue("@UserID", appdb.Userid);

                        result += cmd2.ExecuteNonQuery();

                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Handle the exception
                Console.WriteLine("Transaction rolled back due to exception: " + ex.Message);
            }


            return result;
        }

        //public List<object> Getdriver(string lic)
        //{
        //    List<object> result = new List<object>();

        //    using (var context = new ProjectLicenseEntities())
        //    {
        //        var query = from driver in context.DriverMsts
        //                    join licenes in context.LicenseMsts
        //                    on driver.Driver_id equals licenes.Driver_Id
        //                    where licenes.License_number == lic
        //                    select new
        //                    {
        //                        driver.FName,
        //                        driver.MName,
        //                        driver.LName,
        //                        driver.Email,
        //                        driver.DOB,
        //                        driver.Gender,
        //                        driver.Phone_no,
        //                        driver.Address,
        //                        driver.City,
        //                        driver.Province,
        //                        driver.Country,
        //                        driver.PostalCode,
        //                        driver.LicenseType,
        //                        driver.Document,
        //                        licenes.License_number,
        //                    };
        //                result = query.ToList<object>();
        //    }
        //    return result;
        //}
        public DataTable Getdriver(string lic)
        {
            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = db.getcon();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT dmst.Driver_id,FName, MName, LName, Email, DOB, Gender, Phone_no, " +
                    "Address, City, Province, Country, PostalCode, LicenseType, Document, License_number " +
                    "FROM DriverMst dmst " +
                    "JOIN LicenseMst lmst ON lmst.Driver_Id = dmst.Driver_id " +
                    "WHERE License_number = @lic";
                cmd.Parameters.AddWithValue("@lic", lic);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }

        public List<object> Searchadmin(string searchTerm)
        {
            List<object> allData = new List<object>();

            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where appdb.IsRenew != null && driver.FName.Contains(searchTerm) ||
                                         driver.LName.Contains(searchTerm) ||
                                         app.LicenseNumber.Contains(searchTerm) 
                                    group new { driver, app } by driver.Driver_id into grouped
                                    select new
                                    {
                                        DriverId = grouped.Key,
                                        FName = grouped.FirstOrDefault().driver.FName,
                                        LName = grouped.FirstOrDefault().driver.LName,
                                        Email = grouped.FirstOrDefault().driver.Email,
                                        Gender = grouped.FirstOrDefault().driver.Gender,
                                        Phone_no = grouped.FirstOrDefault().driver.Phone_no,
                                        Address = grouped.FirstOrDefault().driver.Address,
                                        PostalCode = grouped.FirstOrDefault().driver.PostalCode,
                                        LicenseType = grouped.FirstOrDefault().driver.LicenseType,
                                        LicenseNumber = grouped.FirstOrDefault().app.LicenseNumber,
                                        Status = grouped.FirstOrDefault().app.Status,
                                    };

                allData = combinedQuery.ToList<object>();
            }
            return allData;
        }

       
        public int Licesnetable(int driverId, string licenseNumber, DateTime creationDate, DateTime expiryDate)
        {
            using (var transaction = db.getcon().BeginTransaction())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db.getcon();
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO LicenseMst (License_number, Driver_Id, Creation_Date, Expiry_Date) " +
                        "VALUES (@LicenseNumber, @DriverId, @CreationDate, @ExpiryDate)";
                    cmd.Parameters.AddWithValue("@LicenseNumber", licenseNumber);
                    cmd.Parameters.AddWithValue("@DriverId", driverId);
                    cmd.Parameters.AddWithValue("@CreationDate", creationDate);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    int result = cmd.ExecuteNonQuery();

                    

                    transaction.Commit();
                    return result; // Return the number of rows affected
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back due to exception: " + ex.Message);
                    return -1; // Return -1 to indicate failure
                }
            }
        }
        public int UpdateLicesnetable(int Applciationid, string licenseNumber, DateTime creationDate, DateTime expiryDate)
        {
            using (var transaction = db.getcon().BeginTransaction())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db.getcon();
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Update LicenseMst set  Creation_Date = @CreationDate, Expiry_Date= @ExpiryDate " +
                        "ApplicationId = @Appid where LicenseNumber = @LicenseNumber";
                    cmd.Parameters.AddWithValue("@LicenseNumber", licenseNumber);
                    cmd.Parameters.AddWithValue("@Appid", Applciationid);
                    cmd.Parameters.AddWithValue("@CreationDate", creationDate);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    int result = cmd.ExecuteNonQuery();



                    transaction.Commit();
                    return result; // Return the number of rows affected
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back due to exception: " + ex.Message);
                    return -1; // Return -1 to indicate failure
                }
            }
        }

        public void UpdateStatus(int driverId, bool isAccepted)
        {
            
            using (var context = new ProjectLicenseEntities())
            {
                var driver = context.DriverMsts.FirstOrDefault(d => d.Driver_id == driverId);
                if (driver != null)
                {
                    var application = context.ApplicationMsts.FirstOrDefault(app => app.Driver_Id == driverId);
                    if (application != null)
                    {
                        if (isAccepted == true)
                        {
                            application.Status = "Accepted";
                            application.IsRenew = null;
                            context.SaveChanges();
                        }
                        else
                        {
                            application.Status = "Rejected";
                            application.IsRenew = null;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        // Handle case where application with specified driver ID is not found
                        Console.WriteLine("Application not found for the specified driver.");
                    }
                }
                else
                {
                    // Handle case where driver with specified ID is not found
                    Console.WriteLine("Driver not found.");
                }
            }
        }
        //public List<object> GetAllData(int userId)
        //{
        //    List<object> allData = new List<object>();

        //    using (var context = new ProjectLicenseEntities()) // Replace YourDbContext with your actual context class
        //    {
        //        var combinedQuery = from driver in context.DriverMsts
        //                            join app in context.ApplicationMsts
        //                            on driver.Driver_id equals app.Driver_Id
        //                            where driver.UserID == userId
        //                            select new
        //                            {
        //                                driver.Driver_id,
        //                                driver.FName,
        //                                driver.LName,
        //                                driver.Email,
        //                                driver.Gender,
        //                                driver.Phone_no,
        //                                driver.Address,
        //                                driver.PostalCode,
        //                                driver.LicenseType,
        //                                app.LicenseNumber,
        //                                app.Status,
        //                            };

        //        allData = combinedQuery.ToList<object>();
        //    }

        //    return allData;
        //}
        public List<object> GetFilterdata(int userId,bool isappc)
        {
            List<object> allData = new List<object>();
            bool isAccepted = false;
            if (isappc == true)
            {
                 isAccepted = true;
            }
            else 
            { 
                isAccepted = false;
            }
          
            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where driver.UserID == userId && app.IsRenew == isAccepted
                                    group new { driver, app } by driver.Driver_id into grouped
                                    select new
                                    {
                                        DriverId = grouped.Key,
                                        FName = grouped.FirstOrDefault().driver.FName,
                                        LName = grouped.FirstOrDefault().driver.LName,
                                        Email = grouped.FirstOrDefault().driver.Email,
                                        Gender = grouped.FirstOrDefault().driver.Gender,
                                        Phone_no = grouped.FirstOrDefault().driver.Phone_no,
                                        Address = grouped.FirstOrDefault().driver.Address,
                                        PostalCode = grouped.FirstOrDefault().driver.PostalCode,
                                        LicenseType = grouped.FirstOrDefault().driver.LicenseType,
                                        LicenseNumber = grouped.FirstOrDefault().app.LicenseNumber,
                                        Status = grouped.FirstOrDefault().app.Status,
                                    };

                allData = combinedQuery.ToList<object>();
            }

            return allData;
        }
        public List<object> GetFilterdataadmin(bool isappc)
        {
            List<object> allData = new List<object>();
            bool isAccepted = false;
            if (isappc == true)
            {
                isAccepted = true;
            }
            else
            {
                isAccepted = false;
            }

            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where app.IsRenew == isAccepted
                                    group new { driver, app } by driver.Driver_id into grouped
                                    select new
                                    {
                                        DriverId = grouped.Key,
                                        FName = grouped.FirstOrDefault().driver.FName,
                                        LName = grouped.FirstOrDefault().driver.LName,
                                        Email = grouped.FirstOrDefault().driver.Email,
                                        Gender = grouped.FirstOrDefault().driver.Gender,
                                        Phone_no = grouped.FirstOrDefault().driver.Phone_no,
                                        Address = grouped.FirstOrDefault().driver.Address,
                                        PostalCode = grouped.FirstOrDefault().driver.PostalCode,
                                        LicenseType = grouped.FirstOrDefault().driver.LicenseType,
                                        LicenseNumber = grouped.FirstOrDefault().app.LicenseNumber,
                                        Status = grouped.FirstOrDefault().app.Status,
                                    };

                allData = combinedQuery.ToList<object>();
            }

            return allData;
        }
        public List<object> GetAllData(int userId)
        {
            List<object> allData = new List<object>();
            
            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where driver.UserID == userId 
                                    group new { driver, app } by driver.Driver_id into grouped
                                    select new
                                    {
                                        DriverId = grouped.Key,
                                        FName = grouped.FirstOrDefault().driver.FName,
                                        LName = grouped.FirstOrDefault().driver.LName,
                                        Email = grouped.FirstOrDefault().driver.Email,
                                        Gender = grouped.FirstOrDefault().driver.Gender,
                                        Phone_no = grouped.FirstOrDefault().driver.Phone_no,
                                        Address = grouped.FirstOrDefault().driver.Address,
                                        PostalCode = grouped.FirstOrDefault().driver.PostalCode,
                                        LicenseType = grouped.FirstOrDefault().driver.LicenseType,
                                        LicenseNumber = grouped.FirstOrDefault().app.LicenseNumber,
                                        Status = grouped.FirstOrDefault().app.Status,
                                    };

                allData = combinedQuery.ToList<object>();
            }

            return allData;
        }




        public List<object> GetList(string status)
        {
            List<object> allData = new List<object>();

            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where appdb.Status == status
                                    group new { driver, app } by driver.Driver_id into grouped
                                    select new
                                    {
                                        DriverId = grouped.Key,
                                        FName = grouped.FirstOrDefault().driver.FName,
                                        LName = grouped.FirstOrDefault().driver.LName,
                                        Email = grouped.FirstOrDefault().driver.Email,
                                        Gender = grouped.FirstOrDefault().driver.Gender,
                                        Phone_no = grouped.FirstOrDefault().driver.Phone_no,
                                        Address = grouped.FirstOrDefault().driver.Address,
                                        PostalCode = grouped.FirstOrDefault().driver.PostalCode,
                                        LicenseType = grouped.FirstOrDefault().driver.LicenseType,
                                        LicenseNumber = grouped.FirstOrDefault().app.LicenseNumber,
                                        Status = grouped.FirstOrDefault().app.Status,
                                    };

                allData = combinedQuery.ToList<object>();
            }

            return allData;
        }
        public List<object> GetLicenseList()
        {
            List<object> allData = new List<object>();

            using (var context = new ProjectLicenseEntities()) // Replace YourDbContext with your actual context class
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join lic in context.LicenseMsts
                                    on driver.Driver_id equals lic.Driver_Id
                                   
                                    select new
                                    {
                                        driver.Driver_id,
                                        driver.FName,
                                        driver.LName,
                                        driver.LicenseType,

                                        lic.License_number,
                                        lic.Creation_Date,
                                        lic.Expiry_date
                                    };

                allData = combinedQuery.ToList<object>();
            }

            return allData;
        }
        public List<Dictionary<string, object>> GetListByDriverId(int drivid)
        {
            List<Dictionary<string, object>> allData = new List<Dictionary<string, object>>();

            using (var context = new ProjectLicenseEntities())
            {
                var combinedQuery = from driver in context.DriverMsts
                                    join app in context.ApplicationMsts
                                    on driver.Driver_id equals app.Driver_Id
                                    where driver.Driver_id == drivid
                                    select new
                                    {
                                        Driver = driver,
                                        Application = app
                                    };

                var materializedQuery = combinedQuery.ToList(); // Materialize the query to a list

                foreach (var result in materializedQuery)
                {
                    var rowData = new Dictionary<string, object>();
                    rowData["DriverId"] = result.Driver.Driver_id;
                    rowData["FirstName"] = result.Driver.FName;
                    rowData["MiddleName"] = result.Driver.MName;
                    rowData["LastName"] = result.Driver.LName;
                    rowData["Email"] = result.Driver.Email;
                    rowData["DOB"] = result.Driver.DOB;
                    rowData["Gender"] = result.Driver.Gender;
                    rowData["Phone"] = result.Driver.Phone_no;
                    rowData["Address"] = result.Driver.Address;
                    rowData["City"] = result.Driver.City;
                    rowData["Province"] = result.Driver.Province;
                    rowData["Country"] = result.Driver.Country;
                    rowData["PostalCode"] = result.Driver.PostalCode;
                    rowData["LicenseType"] = result.Driver.LicenseType;
                    rowData["Document"] = result.Driver.Document;
                    rowData["Document_image"] = result.Driver.Document_image;
                    rowData["LicenseNumber"] = result.Application.LicenseNumber;
                    rowData["CreationDate"] = result.Application.Creation_date;
                    rowData["ExpiryDate"] = result.Application.Expiry_date;

                    allData.Add(rowData);
                }
            }

            return allData;
        }



    }
}
