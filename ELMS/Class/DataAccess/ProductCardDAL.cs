using ELMS.Class.Tables;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ELMS.Class.Enum;

namespace ELMS.Class.DataAccess
{
    class ProductCardDAL
    {

        public static DataSet SelectProductCardByID(int? typeID)
        {
            string sql = null;
            if (typeID == null)
                sql = $@"SELECT CC.ID,
                                   P.NAME PRODUCT_NAME,
                                   CC.TOTAL,
                                   CC.PRICE,
                                   CC.PRODUCT_COUNT,
                                   CC.IMEI,
                                   CC.USED_USER_ID
                              FROM ELMS_USER_TEMP.PRODUCT_CARDS_TEMP CC,
                                   ELMS_USER.PRODUCT P
                             WHERE CC.PRODUCT_ID = P.ID
                                   AND CC.IS_CHANGE <> {(int)ChangeTypeEnum.Delete}
                        ORDER BY CC.ID";
            else
                sql = $@"SELECT CC.ID,
                                    P.NAME PRODUCT_NAME,
                                   CC.TOTAL,
                                   CC.PRICE,
                                   CC.PRODUCT_COUNT,
                                   CC.IMEI,
                                   CC.USED_USER_ID
                              FROM ELMS_USER_TEMP.PRODUCT_CARDS_TEMP CC,
                                   ELMS_USER.PRODUCT P
                             WHERE CC.PRODUCT_ID = P.ID
                                   AND CC.IS_CHANGE <> {(int)ChangeTypeEnum.Delete}
                              AND CC.CUSTOMER_ID = {typeID}";

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
                GlobalProcedures.LogWrite("Sənəd növü açılmadı.", sql, GlobalVariables.V_UserName, "ProductCardDAL", "SelectProductCardByID", exx);
                return null;
            }
        }

        public static DataTable SelectViewData(int? ID)
        {
            string s = $@"SELECT CC.ID,
                                    P.NAME PRODUCT_NAME,
                                   CC.TOTAL,
                                   CC.PRICE,
                                   CC.PRODUCT_COUNT,
                                   CC.IMEI,
                                   CC.USED_USER_ID
                              FROM ELMS_USER_TEMP.PRODUCT_CARDS_TEMP CC,
                                   ELMS_USER.PRODUCT P
                             WHERE CC.PRODUCT_ID = P.ID
                                   AND CC.IS_CHANGE <> {(int)ChangeTypeEnum.Delete} {(ID.HasValue ? $@" AND CC.ORDER_TAB_ID = {ID}" : null)}
                        ORDER BY CC.ID";

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
                GlobalProcedures.LogWrite("Musterinin məlumatları açılmadı.", s, GlobalVariables.V_UserName, "ProductCardDAL", "SelectViewData", exx);
                return null;
            }
        }

        public static void InsertProductCard(ProductCard customer)
        {
            string commandSql = null;
            using (OracleConnection connection = new OracleConnection())
            {
                OracleTransaction transaction = null;
                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    {
                        connection.ConnectionString = GlobalFunctions.GetConnectionString();
                        connection.Open();
                    }

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        transaction = connection.BeginTransaction();
                        command.Transaction = transaction;
                        command.CommandText = $@"INSERT INTO ELMS_USER_TEMP.CUSTOMER_CARDS_TEMP(CARD_NUMBER,
                                                                                                DOCUMENT_GROUP_ID,
                                                                                                DOCUMENT_TYPE_ID,
                                                                                                ISSUE_DATE,
                                                                                                RELIABLE_DATE,
                                                                                                PINCODE,
                                                                                                CARD_ISSUING_ID,
                                                                                                CUSTOMER_ID)
                                                    VALUES(:inCARDNUMBER,
                                                           :inDOCUMENTGROUPID,
                                                           :inDOCUMENTTYPEID,
                                                           :inISSUEDATE,
                                                           :inRELIABLEDATE,
                                                           :inPINCODE,
                                                           :inCARDISSUINGID,
                                                           :inCUSTOMER_ID)";
                        command.Parameters.Add(new OracleParameter("inCARDNUMBER", customer.ID));
                        command.Parameters.Add(new OracleParameter("inDOCUMENTGROUPID", customer.PRODUCT_ID));
                        command.Parameters.Add(new OracleParameter("inDOCUMENTTYPEID", customer.PRODUCT_COUNT));
                        command.Parameters.Add(new OracleParameter("inISSUEDATE", customer.TOTAL));
                        command.Parameters.Add(new OracleParameter("inRELIABLEDATE", customer.PRODUCT_NAME));
                        command.Parameters.Add(new OracleParameter("inPINCODE", customer.PRICE));
                        command.Parameters.Add(new OracleParameter("inCARDISSUINGID", customer.IMEI));
                        command.Parameters.Add(new OracleParameter("inCUSTOMER_ID", customer.ORDER_TAB_ID));
                        commandSql = command.CommandText;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        command.Connection.Close();
                    }
                }
                catch (Exception exx)
                {
                    transaction.Rollback();
                    GlobalProcedures.LogWrite("Sənəd bazaya daxil edilmədi.", commandSql, GlobalVariables.V_UserName, "ProductCardDAL", "InsertProductCard", exx);
                }
                finally
                {
                    transaction.Dispose();
                    connection.Dispose();
                }
            }
        }

        public static void UpdateProductCard(ProductCard customer)
        {
            string commandSql = null;
            using (OracleConnection connection = new OracleConnection())
            {
                OracleTransaction transaction = null;
                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    {
                        connection.ConnectionString = GlobalFunctions.GetConnectionString();
                        connection.Open();
                    }

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        transaction = connection.BeginTransaction();
                        command.Transaction = transaction;
                        command.CommandText = $@"UPDATE ELMS_USER_TEMP.CUSTOMER_CARDS_TEMP SET CARD_NUMBER = :inCARDNUMBER,
                                                                                                DOCUMENT_GROUP_ID = :inDOCUMENTGROUPID,
                                                                                                DOCUMENT_TYPE_ID = :inDOCUMENTTYPEID,
                                                                                                ISSUE_DATE = :inISSUEDATE,
                                                                                                RELIABLE_DATE = :inRELIABLEDATE,                                                                   
                                                                                                PINCODE = :inPINCODE,
                                                                                                CARD_ISSUING_ID = :inCARDISSUINGID,
                                                                                                USED_USER_ID = :inUSEDUSERID,
                                                                                                IS_CHANGE = :inISCHANGE
                                                            WHERE CUSTOMER_ID = :inCUSTOMER_ID AND ID = :inID";
                        command.Parameters.Add(new OracleParameter("inCARDNUMBER", customer.PRODUCT_NAME));
                        command.Parameters.Add(new OracleParameter("inDOCUMENTGROUPID", customer.PRODUCT_NAME));
                        command.Parameters.Add(new OracleParameter("inDOCUMENTTYPEID", customer.PRODUCT_NAME));
                        command.Parameters.Add(new OracleParameter("inISSUEDATE", customer.PRODUCT_NAME));
                        command.Parameters.Add(new OracleParameter("inRELIABLEDATE", customer.PRICE));
                        command.Parameters.Add(new OracleParameter("inPINCODE", customer.PRODUCT_COUNT));
                        command.Parameters.Add(new OracleParameter("inCARDISSUINGID", customer.TOTAL));
                        command.Parameters.Add(new OracleParameter("inUSEDUSERID", GlobalVariables.V_UserID));
                        command.Parameters.Add(new OracleParameter("inISCHANGE", customer.IS_CHANGE));
                        command.Parameters.Add(new OracleParameter("inCUSTOMER_ID", customer.ORDER_TAB_ID));
                        command.Parameters.Add(new OracleParameter("inID", customer.ID));
                        commandSql = command.CommandText;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        command.Connection.Close();
                    }
                }
                catch (Exception exx)
                {
                    transaction.Rollback();
                    GlobalProcedures.LogWrite("Sənəd bazada dəyişdirilmədi.", commandSql, GlobalVariables.V_UserName, "ProductCardDAL", "UpdateProductCard", exx);
                }
                finally
                {
                    transaction.Dispose();
                    connection.Dispose();
                }
            }
        }

        public static void DeleteProductCard(int phoneID, int ownerID)
        {
            string commandSql = null;
            using (OracleConnection connection = new OracleConnection())
            {
                OracleTransaction transaction = null;
                try
                {
                    if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    {
                        connection.ConnectionString = GlobalFunctions.GetConnectionString();
                        connection.Open();
                    }

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        transaction = connection.BeginTransaction();
                        command.Transaction = transaction;
                        command.CommandText = $@"UPDATE ELMS_USER_TEMP.CUSTOMER_CARDS_TEMP SET IS_CHANGE = {(int)ChangeTypeEnum.Delete}
                                                        WHERE  CUSTOMER_ID = :inCUSTOMERID
                                                          AND ID = :inID";
                        command.Parameters.Add(new OracleParameter("inCUSTOMERID", ownerID));
                        command.Parameters.Add(new OracleParameter("inID", phoneID));
                        commandSql = command.CommandText;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        command.Connection.Close();
                    }
                }
                catch (Exception exx)
                {
                    transaction.Rollback();
                    GlobalProcedures.LogWrite("Sənəd temp cədvəldən silinmədi.", commandSql, GlobalVariables.V_UserName, "ProductCardDAL", "DeleteProductCard", exx);
                }
                finally
                {
                    transaction.Dispose();
                    connection.Dispose();
                }
            }
        }
    }
}
