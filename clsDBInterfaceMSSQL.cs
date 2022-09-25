using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace Project1
{
    public class clsDBInterfaceMSSQL
    {
        private string _masterTable;
        private string _primaryField;
        private string _secondField;
        private string _strConn;
        private string _strErr;
        private string _strExec;
        private string strConnStr;
        private string strAppPath;
        private SqlConnection _connObj = new SqlConnection();

        public SqlConnection objSqlConn
        {
            get
            {
                return _connObj;
            }
            set
            {
                _connObj = new SqlConnection();
                _connObj = value;
                if (_connObj.State != ConnectionState.Open)
                {
                    _connObj.Open();
                }
            }
        }
        public string ConnStr()
        {
            string MyConn;

            if (strConnStr == "" || strConnStr == null)
            {
                MyConn = getConnStrXML();
                strConnStr = MyConn;
            }
            else
            {
                MyConn = strConnStr;
            }

            return MyConn;
        }
        public string getConnStrXML()
        {
            return ConfigurationSettings.AppSettings["ConnectionString"];
        }
        public string strErr
        {
            get
            {
                return _strErr;
            }
            set
            {
                _strErr = value;
            }
        }

        public string strExec
        {
            get
            {
                return _strExec;
            }
            set
            {
                _strExec = value;
            }
        }


        public string masterTable
        {
            get
            {
                return _masterTable;
            }
            set
            {
                _masterTable = value;
            }
        }

        public string primaryField
        {
            get
            {
                return _primaryField;
            }
            set
            {
                _primaryField = value;
            }
        }

        public string secondPrimaryField
        {
            get
            {
                return _secondField;
            }
            set
            {
                _secondField = value;
            }
        }

        public string strConn
        {
            get
            {
                return _strConn;
            }
            set
            {
                _strConn = value;
            }
        }

        public clsDBInterfaceMSSQL()
        {
            _masterTable = "";
            _connObj = new SqlConnection();
        }


        public string getConcatCmd(string strFirstField, string strSecondField)
        {
            if (ConfigurationManager.AppSettings["ConnectionType"].ToString().ToUpper() == "MSSQL")
            {
                return strFirstField + " + " + strSecondField;
            }
            else if (ConfigurationManager.AppSettings["ConnectionType"].ToString().ToUpper() == "MYSQL")
            {
                return "CONCAT(" + strFirstField + " , " + strSecondField + ")";
            }
            else
            {
                return "";
            }
        }

        public string getISNullCmd()
        {
            if (ConfigurationManager.AppSettings["ConnectionType"].ToString().ToUpper() == "MSSQL")
            {
                return "ISNULL";
            }
            else if (ConfigurationManager.AppSettings["ConnectionType"].ToString().ToUpper() == "MYSQL")
            {
                return "IFNULL";
            }
            else
            {
                return "";
            }
        }
        public void InsertRecord(clsBoundField[] objBoundField)
        {
            string strSQL;
            SqlCommand sqlCmnd;

            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
                _connObj.Open();
            }

            //string sqltype = ConfigurationSettings.AppSettings["sqltype"].ToString();

            strSQL = "INSERT INTO " + _masterTable + " (";

            if (objBoundField[0].inputType != "auto")
            {
                if (objBoundField[0].inputValue != "")
                {
                    strSQL = strSQL + _primaryField + ",";
                }
            }

            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                if (objBoundField[intI].inputType != "auto")
                {
                    strSQL = strSQL + objBoundField[intI].boundField + ",";
                }
            }

            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + ") VALUES (";
            if (objBoundField[0].inputValue != "")
            {
                if (objBoundField[0].inputType != "auto")
                {
                    if (objBoundField[0].inputType == "number")
                    {
                        strSQL = strSQL + objBoundField[0].inputValue + ",";
                    }
                    else
                    {
                        strSQL = strSQL + ADOQuoteField(objBoundField[0].inputValue) + ",";
                    }
                }
            }
            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                if (objBoundField[intI].inputType != "auto")
                {
                    if (objBoundField[intI].inputValue != "")
                    {

                        if (objBoundField[intI].inputType == "number")
                        {
                            strSQL = strSQL + objBoundField[intI].inputValue + ",";
                        }
                        else if (objBoundField[intI].inputType == "sysdate")
                        {
                            if (objBoundField[intI].inputValue.ToLower() == "getdate()")
                            {
                                strSQL = strSQL + objBoundField[intI].inputValue + ",";
                            }
                            else
                            {
                                strSQL = strSQL + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                            }
                        }
                        else
                        {
                            strSQL = strSQL + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                        }
                    }
                    else
                    {
                        strSQL = strSQL + "null,";
                    }
                }
            }
            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + ")";
            sqlCmnd = new SqlCommand(strSQL, _connObj);            
            try
            {
                sqlCmnd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
            }
        }

		  public int InsertRecordwithReturn(clsBoundField[] objBoundField)
		  {
			  string strSQL;
			  SqlCommand sqlCmnd;

			  int intI;
			  int intRecNo;
			  intRecNo = objBoundField.GetLength(0);
			  if (_connObj.State != ConnectionState.Open)
			  {
				  _connObj = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
				  _connObj.Open();
			  }
			  strSQL = "INSERT INTO " + _masterTable + " (";
			  if (objBoundField[0].inputValue != "")
			  {
				  strSQL = strSQL + _primaryField + ",";
			  }
			  for (intI = 1; intI <= intRecNo - 1; intI++)
			  {
				  strSQL = strSQL + objBoundField[intI].boundField + ",";
			  }
			  strSQL = strSQL.Remove(strSQL.Length - 1, 1);
			  strSQL = strSQL + ") VALUES (";
			  if (objBoundField[0].inputValue != "")
			  {
				  strSQL = strSQL + ADOQuoteField(objBoundField[0].inputValue) + ",";
			  }
			  for (intI = 1; intI <= intRecNo - 1; intI++)
			  {
				  if (objBoundField[intI].inputValue != "")
				  {
					  if (objBoundField[intI].inputType == "number")
					  {
						  strSQL = strSQL + objBoundField[intI].inputValue + ",";
					  }
					  else if (objBoundField[intI].inputType == "sysdate")
					  {
						  if (objBoundField[intI].inputValue.ToLower() == "getdate()")
						  {
							  strSQL = strSQL + objBoundField[intI].inputValue + ",";
						  }
						  else
						  {
							  strSQL = strSQL + ADOQuoteField(objBoundField[intI].inputValue) + ",";
						  }
					  }
					  else
					  {
						  strSQL = strSQL + ADOQuoteField(objBoundField[intI].inputValue) + ",";
					  }
				  }
				  else
				  {
					  strSQL = strSQL + "null,";
				  }
			  }
			  strSQL = strSQL.Remove(strSQL.Length - 1, 1);
			  strSQL = strSQL + "); SELECT " + _primaryField + " FROM " + _masterTable + " WHERE   " + _primaryField + " = @@identity ";
			  sqlCmnd = new SqlCommand(strSQL, _connObj);

			  int iID;
			  iID = 0;
			  try
			  {
				  iID = System.Convert.ToInt32(sqlCmnd.ExecuteScalar());
				  return iID;
			  }
			  catch (System.Exception ex)
			  {
				  _strErr = ex.Message;
				  return iID;
			  }
			  finally
			  {
				  sqlCmnd.Dispose();
			  }
		  }

        public void InsertRecordOra(clsBoundField[] objBoundField)
        {
            string strSQL;
            SqlCommand sqlCmnd;

            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
                _connObj.Open();
            }

            string sqltype = ConfigurationSettings.AppSettings["sqltype"].ToString();

            strSQL = "INSERT INTO " + _masterTable + " (";
            if (objBoundField[0].inputValue != "")
            {
                strSQL = strSQL + _primaryField + ",";
            }
            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                strSQL = strSQL + objBoundField[intI].boundField + ",";
            }
            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + ") VALUES (";
            if (objBoundField[0].inputValue != "")
            {
                if (objBoundField[0].inputType == "number")
                {
                    strSQL = strSQL + objBoundField[0].inputValue + ",";
                }
                else
                {
                    strSQL = strSQL + ADOQuoteField(objBoundField[0].inputValue) + ",";
                }
            }
            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                if (objBoundField[intI].inputValue != "")
                {
                    if (objBoundField[intI].inputType == "number")
                    {
                        strSQL = strSQL + objBoundField[intI].inputValue + ",";
                    }
                    else if (objBoundField[intI].inputType == "sysdate")
                    {
                        if (objBoundField[intI].inputValue.ToLower() == "getdate()")
                        {
                            objBoundField[intI].inputValue = "sysdate";
                            strSQL = strSQL + objBoundField[intI].inputValue + ",";
                        }
                        else
                        {
                            strSQL = strSQL + "to_timestamp('" + objBoundField[intI].inputValue + "', 'MM/DD/YYYY HH24:MI:SS')" + ",";
                        }
                    }
                    else
                    {
                        strSQL = strSQL + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                    }
                }
                else
                {
                    strSQL = strSQL + "null,";
                }
            }
            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + ")";
            sqlCmnd = new SqlCommand(strSQL, _connObj);
            try
            {
                sqlCmnd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
            }
        }

        public void ExecSQL(string pstrSQL)
        {
            _strErr = "";
            _strExec = "";
            SqlCommand sqlCmnd;
            int intRecNo;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            sqlCmnd = new SqlCommand(pstrSQL, _connObj);
            try
            {
                intRecNo = sqlCmnd.ExecuteNonQuery();
                _strExec = intRecNo.ToString ();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
            }
        }

        public void ExecSQL2(string pstrSQL)
        {
            _strErr = "";
            _strExec = "";
            SqlCommand sqlCmnd;
            int intRecNo;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            sqlCmnd = new SqlCommand(pstrSQL, _connObj);
            try
            {
                intRecNo = sqlCmnd.ExecuteNonQuery();
                _strExec = intRecNo.ToString();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("unique"))
                    _strErr = "1";
                else if (ex.Message.Contains("FOREIGN"))
                    _strErr = "2";
                else
                    _strErr = "3";

            }
            finally
            {
                sqlCmnd.Dispose();
            }
        }

        public void ExecSQLWithReturn(string pstrSQL)
        {
            _strErr = "";
            _strExec = "";
            SqlCommand sqlCmnd;
            int intRecNo;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            sqlCmnd = new SqlCommand(pstrSQL, _connObj);
            try
            {
                sqlCmnd.ExecuteNonQuery(); 
                sqlCmnd.CommandText = "SELECT @@IDENTITY";
                intRecNo = (Int32) sqlCmnd.ExecuteScalar();
                _strExec = intRecNo.ToString();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
            }
        }
         
        public string getNewCode(string pstrPrefixed)
        {
            _strErr = "";
            string strSQL;
            string strNewCode;
            strSQL = "SELECT max(" + _primaryField + ") as pkmax FROM " + _masterTable;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataView dvData;
            DataTable dtData;

            int intPrefixedLength;
            string strTmp;
            Int64 intTmp;
            daData.Fill(dsData, _masterTable);
            dvData = dsData.Tables[0].DefaultView;
            dtData = dsData.Tables[0];

            if (dtData.Rows.Count > 0)
            {
                if (!(dtData.Rows[0].IsNull(0)))
                {
                    //strNewCode = dtData.Rows[0].Table.Columns[0].ToString();
                    strNewCode = dtData.Rows[0]["pkmax"].ToString();
                    intPrefixedLength = pstrPrefixed.Length;
                    strNewCode = strNewCode.Substring(intPrefixedLength);
                    intTmp = System.Convert.ToInt64(strNewCode) + 1;
                    strTmp = intTmp.ToString();
                    //strNewCode = pstrPrefixed + getZeroString(strTmp.Length) + strTmp;
                    strNewCode = pstrPrefixed + Left("000000", 7 - strTmp.Length) + strTmp;
                }
                else
                {
                    strNewCode = pstrPrefixed + "0000001";
                }
            }
            else
            {
                strNewCode = pstrPrefixed + "0000001";
            }
            daData = null;
            dsData = null;
            dvData = null;
            return strNewCode;
        }

        public string getNewCode()
        {
            string strSQL;
            string strNewCode;
            strSQL = "SELECT max( " + _primaryField + " ) as pkmax FROM " + _masterTable;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataView dvData;
            DataTable dtData;
            string strTmp;
            Int64 intTmp;

            daData.Fill(dsData, _masterTable);
            dvData = dsData.Tables[0].DefaultView;
            dtData = dsData.Tables[0];

            if (dtData.Rows.Count > 0)
            {
                if (!(dtData.Rows[0].IsNull(0)))
                {
                    strNewCode = dtData.Rows[0]["pkmax"].ToString();
                    intTmp = System.Convert.ToInt64(strNewCode) + 1;
                    strTmp = intTmp.ToString();
                    strNewCode = Left("000000", 7 - strTmp.Length) + strTmp;
                }
                else
                {
                    strNewCode = "0000001";
                }
            }
            else
            {
                strNewCode = "0000001";
            }
            daData = null;
            dsData = null;
            dvData = null;
            return strNewCode;
        }
        public string getNewCode(string pstrPK, string pstrTableName, int iFieldLength)
        {
            string strSQL;
            string strNewCode;
            strSQL = "SELECT max(" + pstrPK + ") as pkmax FROM " + pstrTableName + " " +
                " WHERE len(" + pstrPK + ") = " + iFieldLength + "";
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }

            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataView dvData;
            DataTable dtData;
            string strTmp;
            string strParseZero;
            int intTmp;

            strParseZero = getZeroString(iFieldLength);
            daData.Fill(dsData, pstrTableName);
            dvData = dsData.Tables[0].DefaultView;
            dtData = dsData.Tables[0];

            if (dtData.Rows.Count > 0)
            {
                if (!(dtData.Rows[0].IsNull(0)))
                {
                    strNewCode = dtData.Rows[0]["pkmax"].ToString();
                    intTmp = System.Convert.ToInt32(strNewCode) + 1;
                    strTmp = intTmp.ToString();
                    strNewCode = Left(strParseZero, iFieldLength - strTmp.Length) + strTmp;
                }
                else
                {
                    strNewCode = Left(strParseZero, iFieldLength - 1) + "1";
                }
            }
            else
            {
                strNewCode = Left(strParseZero, iFieldLength - 1) + "1";
            }
            daData = null;
            dsData = null;
            dvData = null;
            return strNewCode;
        }


        public string getNewCodeWhere(string pstrPK, string pstrTableName, string pstrWhere,int iFieldLength)
        {
            string strSQL;
            string strNewCode;
            strSQL = "SELECT max(" + pstrPK + ") as pkmax FROM " + pstrTableName + " " +
                " WHERE len(" + pstrPK + ") = " + iFieldLength + " and " + pstrWhere;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }

            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataView dvData;
            DataTable dtData;
            string strTmp;
            string strParseZero;
            int intTmp;

            strParseZero = getZeroString(iFieldLength);
            daData.Fill(dsData, pstrTableName);
            dvData = dsData.Tables[0].DefaultView;
            dtData = dsData.Tables[0];

            if (dtData.Rows.Count > 0)
            {
                if (!(dtData.Rows[0].IsNull(0)))
                {
                    strNewCode = dtData.Rows[0]["pkmax"].ToString();
                    intTmp = System.Convert.ToInt32(strNewCode) + 1;
                    strTmp = intTmp.ToString();
                    strNewCode = Left(strParseZero, iFieldLength - strTmp.Length) + strTmp;
                }
                else
                {
                    strNewCode = Left(strParseZero, iFieldLength - 1) + "1";
                }
            }
            else
            {
                strNewCode = Left(strParseZero, iFieldLength - 1) + "1";
            }
            daData = null;
            dsData = null;
            dvData = null;
            return strNewCode;
        }

		  public string getNewCodeWLength(int iLength)
		  {
			  string strSQL;
			  string strNewCode;

              if (ConfigurationSettings.AppSettings["customer"] == "CUSCAPI" && iLength == 5)
              { strSQL = "SELECT max(" + _primaryField + ") as pkmax FROM " + _masterTable + " WHERE " + _primaryField + " <> '99999' AND len(" + _primaryField + ")=5"; }
              else
              {
                  strSQL = "SELECT max(" + _primaryField + ") as pkmax FROM " + _masterTable + " ";
              }
			  if (_connObj.State != ConnectionState.Open)
			  {
				  _connObj = new SqlConnection(_strConn);
				  _connObj.Open();
			  }
			  SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
			  DataSet dsData = new DataSet();
			  DataView dvData;
			  DataTable dtData;
			  string strTmp;
			  int intTmp;

			  daData.Fill(dsData, _masterTable);
			  dvData = dsData.Tables[0].DefaultView;
			  dtData = dsData.Tables[0];
			  string strPrefix = getZeroString(iLength - 1);
			  if (dtData.Rows.Count > 0)
			  {
				  if (!(dtData.Rows[0].IsNull(0)))
				  {
					  strNewCode = dtData.Rows[0]["pkmax"].ToString();
					  intTmp = System.Convert.ToInt32(strNewCode) + 1;
					  strTmp = intTmp.ToString();
					  strNewCode = Left(strPrefix, iLength - strTmp.Length) + strTmp;
				  }
				  else
				  {
					  strNewCode = strPrefix + "1";
				  }
			  }
			  else
			  {
				  strNewCode = strPrefix + "1";
			  }
			  daData = null;
			  dsData = null;
			  dvData = null;
			  return strNewCode;
		  }
       
        private string getZeroString(int intNoofZero)
        {
            int intI;
            string strTmp;
            strTmp = "";
            for (intI = 1; intI <= intNoofZero; intI++)
            {
                strTmp = strTmp + "0";
            }
            return strTmp;
        }

        public void DeleteRecord(string strPrimaryFieldValue, string strSecondFieldValue)
        {
            _strErr = "";
            string strSQL;

            SqlCommand sqlCmnd;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            strSQL = "DELETE FROM " + _masterTable;
            strSQL = strSQL + " WHERE " + _primaryField + "=" + ADOQuoteField(strPrimaryFieldValue);
            if (strSecondFieldValue.Trim() != "")
            {
                strSQL = strSQL + " AND " + _secondField + "=" + ADOQuoteField(strSecondFieldValue);
            }
            sqlCmnd = new SqlCommand(strSQL, _connObj);
            try
            {
                sqlCmnd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
                sqlCmnd = null;
            }
        }

        public void UpdateRecord(clsBoundField[] objBoundField, string strSegID)
        {
            _strErr = "";
            string strSQL;
            SqlCommand sqlCmnd;
            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            strSQL = "UPDATE " + _masterTable + " SET ";
            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                if (objBoundField[intI].inputValue != "")
                {
                    if (objBoundField[intI].inputType == "number")
                    {
                        strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
                    }
                    else if (objBoundField[intI].inputType == "sysdate")
                    {
                        if (objBoundField[intI].inputValue.ToLower() == "getdate()")
                        {
                            strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
                        }
                        else
                        {
                            strSQL = strSQL + objBoundField[intI].boundField + "=" + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                        }
                    }
                    else if (objBoundField[intI].inputType == "oradate")
                    {
                        strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
                    }
                    else
                    {
                        strSQL = strSQL + objBoundField[intI].boundField + "=" + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                    }
                }
                else
                {
                    strSQL = strSQL + objBoundField[intI].boundField + "=null,";
                }
            }
            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + " WHERE " + _primaryField + "=" + ADOQuoteField(objBoundField[0].inputValue);

            if (strSegID.Trim() != "")
            {
                strSQL = strSQL + " AND " + _secondField + "=" + ADOQuoteField(strSegID);
            }

            sqlCmnd = new SqlCommand(strSQL, _connObj);
            try
            {
                sqlCmnd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
                sqlCmnd = null;
            }
        }


        //public void UpdateRecordWithSegID(clsBoundField[] objBoundField,string pstrSegID)
        //{
        //    _strErr = "";
        //    string strSQL;
        //    SqlCommand sqlCmnd;
        //    int intI;
        //    int intRecNo;
        //    intRecNo = objBoundField.GetLength(0);
        //    if (_connObj.State != ConnectionState.Open)
        //    {
        //        _connObj = new SqlConnection(_strConn);
        //        _connObj.Open();
        //    }
        //    strSQL = "UPDATE " + _masterTable + " SET ";
        //    for (intI = 1; intI <= intRecNo - 1; intI++)
        //    {
        //        if (objBoundField[intI].inputValue != "")
        //        {
        //            if (objBoundField[intI].inputType == "number")
        //            {
        //                strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
        //            }
        //            else if (objBoundField[intI].inputType == "sysdate")
        //            {
        //                if (objBoundField[intI].inputValue.ToLower() == "getdate()")
        //                {
        //                    strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
        //                }
        //                else
        //                {
        //                    strSQL = strSQL + objBoundField[intI].boundField + "=" + mag.ADOQuoteField(objBoundField[intI].inputValue) + ",";
        //                }
        //            }
        //            else if (objBoundField[intI].inputType == "oradate")
        //            {
        //                strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
        //            }
        //            else
        //            {
        //                strSQL = strSQL + objBoundField[intI].boundField + "=" + mag.ADOQuoteField(objBoundField[intI].inputValue) + ",";
        //            }
        //        }
        //        else
        //        {
        //            strSQL = strSQL + objBoundField[intI].boundField + "=null,";
        //        }
        //    }
        //    strSQL = strSQL.Remove(strSQL.Length - 1, 1);
        //    strSQL = strSQL + " WHERE " + _primaryField + "=" + mag.ADOQuoteField(objBoundField[0].inputValue) + " and seg_id ='" + pstrSegID + "'";
        //    sqlCmnd = new SqlCommand(strSQL, _connObj);
        //    try
        //    {
        //        sqlCmnd.ExecuteNonQuery();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        _strErr = ex.Message;
        //    }
        //    finally
        //    {
        //        sqlCmnd.Dispose();
        //        sqlCmnd = null;
        //    }
        //}
        public void UpdateRecordOra(clsBoundField[] objBoundField)
        {
            _strErr = "";
            string strSQL;
            SqlCommand sqlCmnd;
            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            strSQL = "UPDATE " + _masterTable + " SET ";
            for (intI = 1; intI <= intRecNo - 1; intI++)
            {
                if (objBoundField[intI].inputValue != "")
                {
                    if (objBoundField[intI].inputType == "number")
                    {
                        strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
                    }
                    else if (objBoundField[intI].inputType == "sysdate")
                    {
                        if (objBoundField[intI].inputValue.ToLower() == "getdate()")
                        {
                            objBoundField[intI].inputValue = "sysdate";
                            strSQL = strSQL + objBoundField[intI].boundField + "=" + objBoundField[intI].inputValue + ",";
                        }
                        else
                        {
                            strSQL = strSQL + objBoundField[intI].boundField + "=" + "to_timestamp('" + objBoundField[intI].inputValue + "', 'MM/DD/YYYY HH24:MI:SS'),";
                        }
                    }
                    else
                    {
                        strSQL = strSQL + objBoundField[intI].boundField + "=" + ADOQuoteField(objBoundField[intI].inputValue) + ",";
                    }
                }
                else
                {
                    strSQL = strSQL + objBoundField[intI].boundField + "=null,";
                }
            }
            strSQL = strSQL.Remove(strSQL.Length - 1, 1);
            strSQL = strSQL + " WHERE " + _primaryField + "=" + ADOQuoteField(objBoundField[0].inputValue);
            sqlCmnd = new SqlCommand(strSQL, _connObj);
            try
            {
                sqlCmnd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
            }
            finally
            {
                sqlCmnd.Dispose();
                sqlCmnd = null;
            }
        }

        public static bool IsInteger(string theValue)
        {
            try
            {
                System.Convert.ToInt32(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsInteger


        public static bool IsDate(string theValue)
        {
            try
            {
                System.Convert.ToDateTime(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        } //IsInteger

        public String getSearchWhereClause(clsBoundField[] objBoundField, string strOrderByField, string strOrderDirection, ref string pstrReturn)
        {
            string strSQL;
            string strWhere;
            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            strWhere = "";
            for (intI = 0; intI <= intRecNo - 1; intI++)
            { 
                if (objBoundField[intI].inputValue != "0000000")
                {
                    if (objBoundField[intI].inputValue.Trim() != "")
                    {
                        if (objBoundField[intI].inputType == "number")
                        {
                            if (IsInteger(objBoundField[intI].inputValue))
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + " = " + objBoundField[intI].inputValue + " AND ";
                            }
                        }
                        else if (objBoundField[intI].inputType == "date")
                        {
                            if (IsDate(objBoundField[intI].inputValue))
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + " = " + ADOQuoteField(objBoundField[intI].inputValue) + " AND ";
                            }
                        }

                        else if (objBoundField[intI].inputType == "join")
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + "= " + objBoundField[intI].inputValue.ToLower() + " AND ";
                            }
                        }
                        else if (objBoundField[intI].inputType == "stringexact")
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + " " + objBoundField[intI].boundField + " = " + ADOQuoteField(objBoundField[intI].inputValue.ToLower()) + " AND ";
                            }
                        }
                        else
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + " " + objBoundField[intI].boundField + " LIKE " + ADOQuoteField("%" + objBoundField[intI].inputValue.ToLower() + "%") + " AND ";
                            }
                        }
                    }
                }
            }

            if (strWhere.Trim() != "")
            {
                strWhere = strWhere.Remove(strWhere.Length - 4, 4);
                strSQL = "SELECT * FROM " + _masterTable + " WHERE " + strWhere + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            else
            {
                strSQL = "SELECT * FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            pstrReturn = strWhere;

            return strWhere;
        }

        public DataTable getSearchResult(clsBoundField[] objBoundField, string strOrderByField, string strOrderDirection, ref string pstrReturn)
        {
            string strSQL;
            string strWhere;
            int intI;
            int intRecNo;
            intRecNo = objBoundField.GetLength(0);
            strWhere = "";
            for (intI = 0; intI <= intRecNo - 1; intI++)
            {
                //if (objBoundField[intI].inputValue != "0000000" & objBoundField[intI].inputValue != "0")
                if (objBoundField[intI].inputValue != "0000000")
                {
                    if (objBoundField[intI].inputValue.Trim() != "")
                    {
                        if (objBoundField[intI].inputType == "number")
                        {
                            if (IsInteger(objBoundField[intI].inputValue))
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + " = " + objBoundField[intI].inputValue + " AND ";
                            }
                        }
                        else if (objBoundField[intI].inputType == "date")
                        {
                            if (IsDate(objBoundField[intI].inputValue))
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + " = " + ADOQuoteField(objBoundField[intI].inputValue) + " AND ";
                            }
                        }
                     
                        else if (objBoundField[intI].inputType == "join")
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + objBoundField[intI].boundField + "= " + objBoundField[intI].inputValue.ToLower() +  " AND ";
                            }
                        }
                        else if (objBoundField[intI].inputType == "stringexact")
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + " " + objBoundField[intI].boundField + " = " + ADOQuoteField( objBoundField[intI].inputValue.ToLower() ) + " AND ";
                            }
                        }
                        else 
                        {
                            if (objBoundField[intI].inputValue != "")
                            {
                                strWhere = strWhere + " " + objBoundField[intI].boundField + " LIKE " + ADOQuoteField("%" + objBoundField[intI].inputValue.ToLower() + "%") + " AND ";
                            }
                        }
                    }
                }
            }

            if (strWhere.Trim() != "")
            {
                strWhere = strWhere.Remove(strWhere.Length - 4, 4);
                strSQL = "SELECT * FROM " + _masterTable + " WHERE " + strWhere + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            else
            {
                strSQL = "SELECT * FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            pstrReturn = strWhere;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, ConfigurationSettings.AppSettings["ConnectionString"]);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, _masterTable);
            dtData = dsData.Tables[0];
            return dtData;
        }

        public DataTable getSearchResultAll(string strOrderByField, string strOrderDirection)
        {
            _strErr = "";
            string strSQL;
            strSQL = "SELECT * FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }

            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, _masterTable);
            dtData = dsData.Tables[0];
            daData.Dispose();
            return dtData;
        }

        public DataTable getSearchResultFilter(string strFilterField, string strFilterValue, string strOrderByField, string strOrderDirection)
        {
            string strSQL;
            if (strFilterField != "" & strFilterValue != "")
            {
                strSQL = "SELECT * FROM " + _masterTable + " WHERE " + strFilterField + "=" + ADOQuoteField(strFilterValue) + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            else
            {
                strSQL = "SELECT * FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, _masterTable);
            dtData = dsData.Tables[0];
            return dtData;
        }

        public DataTable getSearchResultWithFieldFilter(string strFields,string strWhere, string strOrderByField, string strOrderDirection)
        { 
            string strSQL;
            if (strWhere != "")
            {
                strSQL = "SELECT " + strFields + " FROM " + _masterTable + " WHERE " + strWhere + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            else
            {
                strSQL = "SELECT " + strFields + " FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
            }
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, _masterTable);
            dtData = dsData.Tables[0];
            daData = null;
            dsData = null;
            return dtData;
        }


        public DataTable getSearchResultWhere(string strWhere, string strOrderByField, string strOrderDirection)
        {
            string strSQL; 
            DataSet dsData = new DataSet();
            DataTable dtData=new DataTable();
            try
            {
                if (strWhere != "")
                {
                    strSQL = "SELECT * FROM " + _masterTable + " WHERE " + strWhere + " ORDER BY " + strOrderByField + " " + strOrderDirection;
                }
                else
                {
                    strSQL = "SELECT * FROM " + _masterTable + " ORDER BY " + strOrderByField + " " + strOrderDirection;
                }
                if (_connObj.State != ConnectionState.Open)
                {
                    _connObj = new SqlConnection(_strConn);
                    _connObj.Open();
                }
                SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj); 
                daData.Fill(dsData, _masterTable);
                dtData = dsData.Tables[0];
                daData = null;
                dsData = null;
                return dtData;
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
                return dtData;
            }
        }

        public DataTable getResults(string pstrSQL)
        {
            _strErr = "";
            DataTable dtData = new DataTable();
            DataSet dsData = new DataSet();
            try
            {
                if (_connObj.State != ConnectionState.Open)
                {
                    _connObj = new SqlConnection(_strConn);
                    _connObj.Open();
                }
                SqlDataAdapter daData = new SqlDataAdapter(pstrSQL, _connObj);
                daData.Fill(dsData, "fromtable");
                dtData = dsData.Tables[0];
                daData = null;
                dsData = null;
                return dtData;
            }
            catch (System.Exception ex)
            {
                _strErr = ex.Message;
                return dtData;
            }
        }

        public DataTable getRecord(string strPrimaryFieldValue)
        {
            string strSQL;
            //string strWhere;
            //int intI;
            strSQL = "SELECT * FROM " + _masterTable + " WHERE " + _primaryField + "=" + ADOQuoteField(strPrimaryFieldValue);
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, _masterTable);
            dtData = dsData.Tables[0];
            return dtData;
        }
        public string getDefaultPage(string strRoleID, string strSegID)
        {
            clsDBInterfaceMSSQL pobjDBInterfaceMSSQL = new clsDBInterfaceMSSQL();
            pobjDBInterfaceMSSQL.strConn = ConnStr();
            DataTable dtRecord;
            dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='DEFAULT_PAGE_" + strRoleID.Trim() + "'");
            if (dtRecord.Rows.Count > 0)
            {
                return dtRecord.Rows[0][0].ToString();
            }
            else
            {
                dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='DEFAULT_PAGE_" + strSegID.Trim() + "'");
                if (dtRecord.Rows.Count > 0)
                {
                    return dtRecord.Rows[0][0].ToString();
                }
                else
                {
                    dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='DEFAULT_PAGE'");
                    if (dtRecord.Rows.Count > 0)
                    {
                        return dtRecord.Rows[0][0].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
        public string getDefaultSegID()
        {
            clsDBInterfaceMSSQL pobjDBInterfaceMSSQL = new clsDBInterfaceMSSQL();
            pobjDBInterfaceMSSQL.strConn = ConnStr();
            DataTable dtRecord;
            dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='DEFAULT_SEG_ID'");
            if (dtRecord.Rows.Count > 0)
            {
                return dtRecord.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
        public bool isExist(string pstrFieldName, string pstrTableName, string pstrWhere)
        {
            string strSQL;
            //string strWhere;
            //int intI;
            strSQL = "SELECT " + pstrFieldName + " FROM " + pstrTableName + " WHERE " + pstrWhere;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, pstrTableName);
            dtData = dsData.Tables[0];
            if (dtData.Rows.Count > 0)
            {
                _connObj.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getFieldValue(string pstrFieldName, string pstrTableName, string pstrWhere)
        {
            if (ifnull(_strConn, "") == "")
            {
                _strConn = ConnStr();
            }
            string strSQL;
            strSQL = "SELECT " + pstrFieldName + " FROM " + pstrTableName + " WHERE " + pstrWhere;
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsData = new DataSet();
            DataTable dtData;
            daData.Fill(dsData, pstrTableName);
            dtData = dsData.Tables[0];
            if (dtData.Rows.Count > 0)
            {
                if (dtData.Rows[0].IsNull(0))
                {
                    return "";
                }
                else
                {
                    return dtData.Rows[0][0].ToString();
                }
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// If Null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strrtn"></param>
        /// <returns></returns>
        public string ifnull(object obj, string strrtn)
        {
            if (obj == null)
            {
                return strrtn;
            }
            else
            {
                return obj.ToString();
            }
        }
        public int Asc(char ch)
        {
            return (int)System.Text.Encoding.ASCII.GetBytes(ch.ToString())[0];
        }
        public char Chr(int i)
        {
            return System.Convert.ToChar(i);
        }
        public string Left(string MyString, int length)
        {
            string tmpstr = MyString.Substring(0, length);
            return tmpstr;
        }
        public string Right(string MyString, int length)
        {
            string tmpstr = MyString.Substring(MyString.Length - length, length);
            return tmpstr;
        }
        public Boolean IsNumeric(string MyString)
        {
            Double dblTmp;
            try
            {
                dblTmp = Double.Parse(MyString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Boolean IsInt(string MyString)
        {
            int dblTmp;
            try
            {
                dblTmp = int.Parse(MyString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string Mid(string s, int start, int length)
        {
            string s1 = "";
            int j = 0;
            int i = 0;
            for (i = start; i < start + length; i++)
            {
                char ch = s[i];
                s1 = s1 + ch.ToString();
                j++;
            }
            return s1;
        }
        public DateTime GetDateWithTimeddMMyyyyHHmmss(string pstrDate_ddMMyyyy_HHmmss)
        {
            string[] strStart;
            string[] ArrDate;
            string[] ArrTime;

            strStart = pstrDate_ddMMyyyy_HHmmss.Split(new Char[] { ' ' });

            if (pstrDate_ddMMyyyy_HHmmss.Trim() == "")
            {
                return System.DateTime.Now;
            }

            ArrDate = strStart[0].Split(new Char[] { '/' });
            ArrTime = strStart[1].Split(new Char[] { ':' });


            return new DateTime(Convert.ToInt16(ArrDate[2]), Convert.ToInt16(ArrDate[1]), Convert.ToInt16(ArrDate[0]), Convert.ToInt16(ArrTime[0]), Convert.ToInt16(ArrTime[1]), Convert.ToInt16(ArrTime[2]));
        }
        public string RedirectURLString(string CMD)
        {
            try
            {
                if (CMD.Trim() != "")
                {
                    string url = ConfigurationSettings.AppSettings["OutboundServer"];
                    url = url + ConfigurationSettings.AppSettings["OutboundPara1"] + "=" + CMD;
                    System.Uri URL_Object = new System.Uri(url);
                    System.Net.WebRequest URL_WebRequest;
                    System.Net.WebResponse URL_WebResponse;
                    try
                    {

                        URL_WebRequest = System.Net.WebRequest.Create(URL_Object);
                        URL_WebRequest.Timeout = 100000;
                        URL_WebResponse = URL_WebRequest.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    URL_WebResponse = null;
                    URL_WebRequest = null;
                    URL_Object = null;
                    return "";
                }
                return "";
            }
            catch (Exception ex1)
            {
                return ex1.Message;
            }
        }
        public string NCodePass(string pstrKey, string pstrPassword)
        {
            try
            {
                pstrKey = pstrKey.ToUpper();
                string strTmp = "";
                int intI;
                int intTmp;
                for (intI = 0; intI <= pstrPassword.Length - 1; intI++)
                {
                    if (intI % 2 == 0)
                    {
                        intTmp = Asc(pstrKey.ToCharArray()[1]) + Asc(pstrPassword.ToCharArray()[intI]) + 3;
                    }
                    else
                    {
                        intTmp = Asc(pstrKey.ToCharArray()[1]) + Asc(pstrPassword.ToCharArray()[intI]) - 5;
                    }


                    if (intTmp > 126 && intTmp <= 160)
                    {
                        intTmp = (intTmp - 126) + 32;
                    }
                    else if (intTmp > 160)
                    {
                        intTmp = (intTmp - 161) + 32;
                    }

                    strTmp = strTmp + Chr(intTmp);
                }
                return strTmp.Trim();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string nullDB2String(DataRow pObj, string pColName)
        {
            try
            {
                if (pObj.IsNull(pColName))
                {
                    return "";
                }
                else
                {
                    return pObj[pColName].ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string nullDB2Boolean(DataRow pObj, string pColName)
        {
            if (pObj.IsNull(pColName))
            {
                return "False";
            }
            else
            {
                return "True";
            }
        }
        public double nullDB2Double(DataRow pObj, string pColName)
        {
            try
            {
                if (pObj.IsNull(pColName))
                {
                    return 0;
                }
                else
                {
                    return System.Convert.ToDouble(pObj[pColName]);
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }
        public void LoadLookUp(string pstrTableName, string pstrValueField, string pstrTextField, string pstrWhereField, string pstrWhereValue, string pstrOrderBy, bool pblnAddAll, DropDownList objDrpList)
        {
            string strSQL;
            if (pstrWhereField == "" | pstrWhereValue == "")
            {
                strSQL = "SELECT " + pstrValueField + " , " + pstrTextField + " From " + pstrTableName + " ORDER BY " + pstrOrderBy;
            }
            else
            {
                strSQL = "SELECT " + pstrValueField + " , " + pstrTextField + " From " + pstrTableName + " WHERE " + pstrWhereField + " = " + ADOQuoteField(pstrWhereValue) + " ORDER BY " + pstrOrderBy;
            }
            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }
            SqlDataAdapter daLookUp = new SqlDataAdapter(strSQL, _connObj);
            DataSet dsLookUp = new DataSet();
            DataRow drNewRow;
            DataView dvLookUp;
            daLookUp.Fill(dsLookUp, pstrTableName);
            if (pblnAddAll == true)
            {
                drNewRow = dsLookUp.Tables[0].NewRow();
                drNewRow[pstrTextField] = "0-Please Select";
                drNewRow[pstrValueField] = "0000000";
                dsLookUp.Tables[pstrTableName].Rows.Add(drNewRow);
            }
            dvLookUp = dsLookUp.Tables[pstrTableName].DefaultView;
            dvLookUp.Sort = pstrOrderBy;
            objDrpList.DataSource = dvLookUp;
            objDrpList.DataTextField = pstrTextField;
            objDrpList.DataValueField = pstrValueField;
            objDrpList.DataBind();
        }
        internal void LoadLookUpWhere(string pstrTableName, string pstrValueField, string pstrTextField, string pstrWhereStr, string pstrOrderBy, bool pblnAddAll, DropDownList objDrpList)
        {
            string strSQL;
            try
            {
                strSQL = "SELECT " + pstrValueField + " , " + pstrTextField + " From " + pstrTableName + " WHERE " + pstrWhereStr + " ORDER BY " + pstrOrderBy;
                SqlDataAdapter daLookUp = new SqlDataAdapter(strSQL, ConnStr());
                DataSet dsLookUp = new DataSet();
                DataView dvLookUp;
                DataRow drNewRow;
                daLookUp.Fill(dsLookUp, pstrTableName);
                if (pblnAddAll == true)
                {
                    drNewRow = dsLookUp.Tables[0].NewRow();
                    drNewRow[pstrValueField] = "0000000";
                    drNewRow[pstrTextField] = "0-Please Select";

                    dsLookUp.Tables[pstrTableName].Rows.Add(drNewRow);
                }
                dvLookUp = dsLookUp.Tables[pstrTableName].DefaultView;
                dvLookUp.Sort = pstrOrderBy;
                objDrpList.DataSource = dvLookUp;
                objDrpList.DataTextField = pstrTextField;
                objDrpList.DataValueField = pstrValueField;
                objDrpList.DataBind();
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message.ToString());
            }
        }
        internal void LoadLookUpWhere2(string pstrTableName, string pstrValueField, string pstrTextField, string pstrTextFieldSelection, string pstrWhereStr, string pstrOrderBy, bool pblnAddAll, DropDownList objDrpList)
        {
            string strSQL;
            try
            {
                strSQL = "SELECT " + pstrValueField + " , " + pstrTextFieldSelection + " AS " + pstrTextField + " From " + pstrTableName + " WHERE " + pstrWhereStr + " ORDER BY " + pstrOrderBy;
                SqlDataAdapter daLookUp = new SqlDataAdapter(strSQL, ConnStr());
                DataSet dsLookUp = new DataSet();
                DataView dvLookUp;
                DataRow drNewRow;
                daLookUp.Fill(dsLookUp, pstrTableName);
                if (pblnAddAll == true)
                {
                    drNewRow = dsLookUp.Tables[0].NewRow();
                    drNewRow[pstrValueField] = "0000000";
                    drNewRow[pstrTextField] = "0-Please Select";

                    dsLookUp.Tables[pstrTableName].Rows.Add(drNewRow);
                }
                dvLookUp = dsLookUp.Tables[pstrTableName].DefaultView;
                dvLookUp.Sort = pstrOrderBy;
                objDrpList.DataSource = dvLookUp;
                objDrpList.DataTextField = pstrTextField;
                objDrpList.DataValueField = pstrValueField;
                objDrpList.DataBind();
            }
            catch (Exception ex)
            {
                WriteToLogFile(ex.Message.ToString());
            }
        }

        public string ADOQuoteField(string pstrData)
        {
            string strX;
            int intI;
            bool blnQuote;
            strX = pstrData;
            blnQuote = true;

            if (blnQuote)
            {
                intI = pstrData.Length;
                while (intI > 0)
                {
                    intI = strX.IndexOf("'", intI);
                    //intI = InStr(intI, strX, "'");
                    if (intI > 0)
                    {
                        if (intI == 1)
                        {
                            strX = "'" + strX;
                        }
                        else if (intI == strX.Length)
                        {
                            strX = strX + "'";
                        }
                        else
                        {
                            strX = strX.Substring(1, intI) + "'" + strX.Substring(intI + 1);
                            //strX = Mid(strX, 1, intI) + "'" + Mid(strX, intI + 1);
                        }
                        intI = intI + 2;
                    }
                }
                strX = "'" + strX + "'";
            }

            return strX;
        } 


       
        public string getRows(string pstrSQL)
        {
            DataTable dtTemp;

            if (_connObj.State != ConnectionState.Open)
            {
                _connObj = new SqlConnection(_strConn);
                _connObj.Open();
            }

            SqlDataAdapter daData = new SqlDataAdapter(pstrSQL, _connObj);

            DataSet dsData = new DataSet();
            daData.Fill(dsData);
            dtTemp = dsData.Tables[0];

            if (dtTemp.Rows.Count > 0)
            {
                _connObj.Close();
                return dtTemp.Rows.Count.ToString();
            }
            else
            {
                _connObj.Close();
                return "0";
            }
        }

		  public bool CheckDependency(string sTable, string sField, string sValue)
		  {
			  // Check relationship dependency. for use before delete.
			  string strSQL;
			  strSQL = "SELECT  " + sField + "  FROM " + sTable + " WHERE " + sField + " = " + ADOQuoteField(sValue) + " ";
			  if (_connObj.State != ConnectionState.Open)
			  {
				  _connObj = new SqlConnection(_strConn);
				  _connObj.Open();
			  }
			  SqlDataAdapter daData = new SqlDataAdapter(strSQL, _connObj);
			  DataSet dsData = new DataSet();
			  DataTable dtData;
			  daData.Fill(dsData, "dependency");
			  dtData = dsData.Tables[0];
			  if (dtData.Rows.Count > 0)
			  {
				  _connObj.Close();
				  return true; // still in use
			  }
			  else
			  {
				  return false; // not in use
			  }
		  }
          public void WriteToLogFile(string pstrMesssage)
          {
              try
              {
                  StreamWriter sw;
                  sw = new StreamWriter(ConfigurationSettings.AppSettings["ErrorLogFile"].ToString() + System.DateTime.Now.ToString("yyyyMMddHH_") + "myLogs.log", true);
                  sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " : " + pstrMesssage);
                  sw.Close();
              }
              catch (Exception ex)
              {
              }
          }
          public void CreateRoleXML(string sRoleID, string sApplicationPath)
          {
              SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
              DataSet objDS = new DataSet();

              SqlDataAdapter daSuppliers = new SqlDataAdapter("SELECT " +
                  "DISTINCT C.LEVEL1_ID, C.LEVEL1_DESC, C.LEVEL1_ICON_PATH " +
                  "FROM SCREEN_ACCESS A, LEVEL2_TREEVIEW B, LEVEL1_TREEVIEW C " +
                  "WHERE A.LEVEL2_ID = B.LEVEL2_ID " +
                  "AND B.LEVEL1_ID = C.LEVEL1_ID " +
                  "AND A.ROLEID = '" + sRoleID + "' ORDER BY C.LEVEL1_ID", myConnection);

              SqlDataAdapter daProducts = new SqlDataAdapter("SELECT " +
                  "B.LEVEL2_ID,B.LEVEL2_DESC, B.LEVEL1_ID, B.LEVEL2_PATH,'X' VIEWCOUNT, LEVEL2_ICON_PATH " +
                  "FROM SCREEN_ACCESS A, LEVEL2_TREEVIEW B, LEVEL1_TREEVIEW C " +
                  "WHERE A.LEVEL2_ID = B.LEVEL2_ID " +
                  "AND B.LEVEL1_ID = C.LEVEL1_ID " +
                  "AND A.ROLEID = '" + sRoleID + "' ORDER BY B.LEVEL1_ID,B.LEVEL2_ID", myConnection);

              daSuppliers.Fill(objDS, "dtParent");
              daProducts.Fill(objDS, "dtChild");

              myConnection.Close();

              objDS.Relations.Add("SuppToProd",
                  objDS.Tables["dtParent"].Columns["Level1_ID"],
                  objDS.Tables["dtChild"].Columns["Level1_ID"]);
          }
          public string getRedirectPage(string strPageName, string strSegID)
          {
              clsDBInterfaceMSSQL pobjDBInterfaceMSSQL = new clsDBInterfaceMSSQL();
              pobjDBInterfaceMSSQL.strConn = ConnStr();
              DataTable dtRecord;
              dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='" + strPageName.Trim() + "_" + strSegID.Trim() + "'");
              if (dtRecord.Rows.Count > 0)
              {
                  return dtRecord.Rows[0][0].ToString();
              }
              else
              {
                  dtRecord = pobjDBInterfaceMSSQL.getResults("SELECT CONFIG_VALUE FROM INT_SETUP_DBCONFIG WHERE CONFIG_ID='" + strPageName + "'");
                  if (dtRecord.Rows.Count > 0)
                  {
                      return dtRecord.Rows[0][0].ToString();
                  }
                  else
                  {
                      return "";
                  }
              }
          }
          public string GetMMddyyyyStringDate(string pstrDate_ddMMyyyy)
          {
              string strStart;
              string strDate;
              strStart = pstrDate_ddMMyyyy.Trim();
              if (strStart.Length >= 8)
              {
                  if (strStart[1].ToString() == "/")
                  {
                      strStart = "0" + strStart;
                  }
                  if (strStart[4].ToString() == "/")
                  {
                      strStart = Left(strStart, 3) + "0" + Right(strStart, 6);
                  }
                  strDate = strStart[3].ToString() + strStart[4].ToString() + "/" + strStart[0].ToString() + strStart[1].ToString() + "/" + Right(strStart, 4);
                  return strDate;
              }
              else
              {
                  return "";
              }
          }
          public string GetMMddyyyyStringDateWithTime(string pstrDate_ddMMyyyy_HHmmss)
          {
              string[] strStart;
              string strDate;
              string strTime;
              string strTmpDate;
              strStart = pstrDate_ddMMyyyy_HHmmss.Split(new Char[] { ' ' });
              if (pstrDate_ddMMyyyy_HHmmss.Trim() == "")
              {
                  return "";
              }
              strDate = strStart[0];
              strTime = strStart[1];
              if (strDate.Length >= 8)
              {
                  if (strDate[1].ToString() == "/")
                  {
                      strDate = "0" + strDate;
                  }
                  if (strDate[4].ToString() == "/")
                  {
                      strDate = Left(strDate, 3) + "0" + Right(strDate, 6);
                  }
                  strTmpDate = strDate[3].ToString() + strDate[4].ToString() + "/" + strDate[0].ToString() + strDate[1].ToString() + "/" + Right(strDate, 4);

                  return strTmpDate + " " + strTime;
              }
              else
              {
                  return "";
              }
          }

          public void WriteToLogFile2(string pstrMesssage, string pstrFilename)
          {
              try
              {
                  StreamWriter sw;
                  sw = new StreamWriter(pstrFilename, true);
                  sw.Write(pstrMesssage);
                  sw.Close();
              }
              catch (Exception ex)
              {

              }
          }
          public void setAppPath(string pstrAppPath)
          {
              strAppPath = pstrAppPath;
          }
    }
}
