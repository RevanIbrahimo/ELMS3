﻿using ELMS.Class.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Class.DataAccess
{
    class OrderDAL
    {
        public static DataSet SelectCustomerByID(int? ID)
        {
            string sql = null;
            if (ID == null)
                sql = $@"SELECT CU.ID,
                               CU.FULL_NAME FULL_NAME,                               
                               B.NAME BRANCH_NAME,
                               C.NAME COUNTRY_NAME,
                               CU.BIRTH_PLACE,
                               CU.REGISTERED_ADDRESS,                            
                               CU.BIRTHDAY,
                               SE.NAME SEX_NAME,                               
                               CU.ADDRESS,                               
                               CU.CLOSED_DATE,
                               CU.NOTE,
                               CU.INSERT_DATE,
                               CI.IMAGE,
                               CU.USED_USER_ID
                          FROM ELMS_USER.ORDER_TAB CU,
                               ELMS_USER.SEX SE,
                               ELMS_USER.COUNTRY C,
                               ELMS_USER.ORDER_TAB_IMAGE CI,
                               ELMS_USER.BRANCH B
                          WHERE CU.COUNTRY_ID = C.ID
                               AND CU.SEX_ID = SE.ID
                               AND CU.ID = CI.ORDER_TAB_ID
                               AND CU.BRANCH_ID = B.ID 
                               ORDER BY CU.ID";
            else
                sql = $@"SELECT CU.ID,
                               CU.FULL_NAME,                               
                               B.NAME BRANCH_NAME,
                               C.NAME COUNTRY_NAME,
                               CU.BIRTH_PLACE,
                               CU.REGISTERED_ADDRESS,                            
                               CU.BIRTHDAY,
                               SE.NAME SEX_NAME,                               
                               CU.ADDRESS,                               
                               CU.CLOSED_DATE,
                               CU.NOTE,
                               CU.INSERT_DATE,
                               CI.IMAGE,
                               CU.USED_USER_ID
                          FROM ELMS_USER.ORDER_TAB CU,
                               ELMS_USER.SEX SE,
                               ELMS_USER.COUNTRY C,
                               ELMS_USER.ORDER_TAB_IMAGE CI,
                               ELMS_USER.BRANCH B
                          WHERE     CU.COUNTRY_ID = C.ID
                               AND CU.SEX_ID = SE.ID
                               AND CU.ID = CI.ORDER_TAB_ID
                               AND CU.BRANCH_ID = B.ID 
                               WHERE CU.ID = {ID}";

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
                GlobalProcedures.LogWrite("Müştərinin məlumatları açılmadı.", sql, GlobalVariables.V_UserName, "CustomerDAL", "SelectCustomerByID", exx);
                return null;
            }
        }

        public static DataTable SelectViewData(int? ID)
        {
            string s = $@"SELECT CU.ID,
                               CU.FULL_NAME,                               
                               B.NAME BRANCH_NAME,
                               C.NAME COUNTRY_NAME,
                               CU.BIRTH_PLACE,
                               CU.REGISTERED_ADDRESS,                            
                               CU.BIRTHDAY,
                               SE.NAME SEX_NAME,                               
                               CU.ADDRESS,                               
                               CU.CLOSED_DATE,
                               CU.NOTE,
                               CU.INSERT_DATE,
                               CI.IMAGE,
                               CU.USED_USER_ID
                          FROM ELMS_USER.ORDER_TAB CU,
                               ELMS_USER.SEX SE,
                               ELMS_USER.COUNTRY C,
                               ELMS_USER.ORDER_TAB_IMAGE CI,
                               ELMS_USER.BRANCH B
                          WHERE     CU.COUNTRY_ID = C.ID
                               AND CU.SEX_ID = SE.ID
                               AND CU.ID = CI.ORDER_TAB_ID
                               AND CU.BRANCH_ID = B.ID {(ID.HasValue ? $@" AND CU.ID = {ID}" : null)}
                        ORDER BY CU.ID";

            try
            {
                using (OracleDataAdapter da = new OracleDataAdapter(s, GlobalFunctions.GetConnectionString()))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception exx)
            {
                GlobalProcedures.LogWrite("Musterinin məlumatları açılmadı.", s, GlobalVariables.V_UserName, "CustomerDAL", "SelectViewData", exx);
                return null;
            }
        }

        public static Int32 InsertCustomer(OracleTransaction tran, Order customer)
        {
            Int32 id = 0;
            OracleCommand command = tran.Connection.CreateCommand();
            command.CommandText = $@"INSERT INTO ELMS_USER.ORDER_TAB(FULL_NAME,
                                                                    BRANCH_ID,
                                                                    COUNTRY_ID,
                                                                    SEX_ID,
                                                                    BIRTHDAY,                                                                     
                                                                    REGISTERED_ADDRESS,
                                                                    BIRTH_PLACE,
                                                                    ADDRESS,
                                                                    NOTE)
                                                    VALUES(:inFULL_NAME,
                                                           :inBRANCH_ID,
                                                           :inCOUNTRY_ID,
                                                           :inSEXID,
                                                           :inBIRTHDAY,
                                                           :inREGISTERED_ADDRESS,                                                           
                                                           :inBIRTH_PLACE,                                                           
                                                           :inADDRESS,                                                        
                                                           :inNOTE) RETURNING ID INTO :outID";
            command.Parameters.Add(new OracleParameter("inFULL_NAME", customer.FULL_NAME));
            command.Parameters.Add(new OracleParameter("inBRANCH_ID", customer.BRANCH_ID));
            command.Parameters.Add(new OracleParameter("inCOUNTRY_ID", customer.COUNTRY_ID));
            command.Parameters.Add(new OracleParameter("inSEXID", customer.SEX_ID));
            command.Parameters.Add(new OracleParameter("inBIRTHDAY", customer.BIRTHDAY));
            command.Parameters.Add(new OracleParameter("inREGISTERED_ADDRESS", customer.REGISTERED_ADDRESS));
            command.Parameters.Add(new OracleParameter("inBIRTH_PLACE", customer.BIRTH_PLACE));
            command.Parameters.Add(new OracleParameter("inADDRESS", customer.ADDRESS));
            command.Parameters.Add(new OracleParameter("inNOTE", customer.NOTE));
            command.Parameters.Add(new OracleParameter("outID", OracleDbType.Int32, ParameterDirection.Output));

            if (tran != null)
                command.Transaction = tran;

            command.ExecuteNonQuery();
            id = Convert.ToInt32(command.Parameters["outID"].Value.ToString());

            command.Dispose();

            return id;
        }

        public static void UpdateCustomer(OracleTransaction tran, Order customer)
        {
            OracleCommand command = tran.Connection.CreateCommand();
            command.CommandText = $@"UPDATE ELMS_USER.ORDER_TAB SET FULL_NAME = :inFULL_NAME,
                                                                        BRANCH_ID = :inBRANCH_ID,
                                                                        COUNTRY_ID = :inCOUNTRY_ID,
                                                                        SEX_ID = :inSEXID,
                                                                        BIRTHDAY = :inBIRTHDAY,
                                                                        REGISTERED_ADDRESS = :inREGISTERED_ADDRESS,                                                                   
                                                                        BIRTH_PLACE = :inBIRTH_PLACE,
                                                                        ADDRESS = :inADDRESS,
                                                                        NOTE = :inNOTE,
                                                                        USED_USER_ID = :inUSEDUSERID,
                                                                        UPDATE_USER = :inUPDATEUSER,
                                                                        UPDATE_DATE = SYSDATE
                                                            WHERE ID = :inID";
            command.Parameters.Add(new OracleParameter("inFULL_NAME", customer.FULL_NAME));
            command.Parameters.Add(new OracleParameter("inBRANCH_ID", customer.BRANCH_ID));
            command.Parameters.Add(new OracleParameter("inCOUNTRY_ID", customer.COUNTRY_ID));
            command.Parameters.Add(new OracleParameter("inSEXID", customer.SEX_ID));
            command.Parameters.Add(new OracleParameter("inBIRTHDAY", customer.BIRTHDAY));
            command.Parameters.Add(new OracleParameter("inREGISTERED_ADDRESS", customer.REGISTERED_ADDRESS));
            command.Parameters.Add(new OracleParameter("inBIRTH_PLACE", customer.BIRTH_PLACE));
            command.Parameters.Add(new OracleParameter("inADDRESS", customer.ADDRESS));
            command.Parameters.Add(new OracleParameter("inNOTE", customer.NOTE));
            command.Parameters.Add(new OracleParameter("inUSEDUSERID", customer.USED_USER_ID));
            command.Parameters.Add(new OracleParameter("inUPDATEUSER", GlobalVariables.V_UserID));
            command.Parameters.Add(new OracleParameter("inID", customer.ID));

            if (tran != null)
                command.Transaction = tran;

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public static void DeleteCustomer(int doctorID)
        {
            GlobalProcedures.ExecuteProcedureWithParametr("ELMS_USER_TEMP.PROC_DELETE_ORDER_TAB_CARDS", "P_ORDER_TAB_ID", doctorID, "Müştəri bazadan silinmədi.");
        }

        public static void DeleteWorkPlaceTemp(int workID)
        {
            GlobalProcedures.ExecuteProcedureWithParametr("ELMS_USER_TEMP.PROC_DELETE_WORKPLACE_TEMP", "P_ORDER_TAB_ID", workID, "Müştəri bazadan silinmədi.");
        }
    }
}
