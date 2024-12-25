using SV21T1020742.DataLayers;
using SV21T1020742.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string userName, string passWord)
        {
            if(userTypes == UserTypes.Employee) 
                return employeeAccountDB.Authorize(userName, passWord);
            else
                return customerAccountDB.Authorize(userName,passWord);
        }

        public static bool ChangePassword(UserTypes userTypes, string userName, string passWord)
        {
            if (userTypes == UserTypes.Employee)
                return employeeAccountDB.ChangePassword(userName, passWord);
            else
                return customerAccountDB.ChangePassword(userName, passWord);
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}
