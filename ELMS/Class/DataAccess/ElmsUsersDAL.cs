using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Class.DataAccess
{
    public class ElmsUsersDAL
    {
        public static DataSet SelectUser(int? userID)
        {
            string sql = null;
            if (userID == null)
                sql = $@"SELECT U.ID,
                                 U.SURNAME,
                                 U.NAME,
                                 U.PATRONYMIC,
                                 U.SURNAME || ' ' || U.NAME || ' ' || U.PATRONYMIC FULL_NAME,
                                 U.NIKNAME,
                                 U.PASSWORD,
                                 U.STATUS_ID,
                                 S.NAME SEX_NAME,
                                 SESSION_ID,
                                 U.GROUP_ID,
                                 BIRTHDAY,
                                 NOTE,
                                 USED_USER_ID
                            FROM ELMS_USER.SYSTEM_USER U, ELMS_USER.SEX S
                           WHERE U.SEX_ID = S.ID
                        ORDER BY SURNAME";
            else
                sql = $@"SELECT U.ID,
                                 U.SURNAME,
                                 U.NAME,
                                 U.PATRONYMIC,
                                 U.SURNAME || ' ' || U.NAME || ' ' || U.PATRONYMIC FULL_NAME,
                                 U.NIKNAME,
                                 U.PASSWORD,
                                 U.STATUS_ID,
                                 S.NAME SEX_NAME,
                                 SESSION_ID,
                                 U.GROUP_ID,
                                 BIRTHDAY,
                                 NOTE,
                                 USED_USER_ID
                            FROM ELMS_USER.ELMS_USERS U, ELMS_USER.SEX S
                           WHERE U.SEX_ID = S.ID
                                 AND U.ID = {userID}";

            try
            {
                using (OracleDataAdapter adapter = new OracleDataAdapter(sql, GlobalFunctions.GetConnectionString()))
                {
                    DataSet dsAdapter = new DataSet();
                    adapter.Fill(dsAdapter);
                    return dsAdapter;
                }
            }
            catch (Exception exx)
            {
                return null;
                GlobalProcedures.LogWrite("İstifadəçinin məlumatları açılmadı.", sql, GlobalVariables.V_UserName, "LrsUsersDAL", "SelectUser", exx);
            }
        }
    }
}
