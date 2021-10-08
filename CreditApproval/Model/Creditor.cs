using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreditApproval.Model;
using System.Data;

namespace CreditApproval.Model
{
    public class Creditor
    {
        public static string CallbackURL;

        public int CreditorAssigned_ID { get; set; }
        public string Applicant_fname { get; set; }
        public string Applicant_mname { get; set; }
        public string Applicant_lname { get; set; }
        public string Business_Name { get; set; }
        public decimal LoanApplication_Amount { get; set; }
        public DateTime LoanApplication_Date { get; set; }
        public string LoanApplication_Description { get; set; }
        public int LoanApplication_Status { get; set; }
        public string LoanApplication_BankerComment { get; set; }
        public int External_ID { get; set; }
        public string CreditorCallBackURL { get; set; }


        public List<Creditor> Get_All_Loan()
        {
            List<Creditor> loanlist = new List<Creditor>();
            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlDataReader loanReader = dbHelper.ExecuteReader("Get_All_Loans_For_Creditor", DBHelper.QueryType.StotedProcedure, null);

                while (loanReader.Read())
                {
                    Creditor loanObj = new Creditor();
                    loanObj.CreditorAssigned_ID = int.Parse(loanReader["CreditorAssigned_ID"].ToString());
                    loanObj.Applicant_fname = loanReader["Applicant_FName"].ToString();
                    loanObj.Applicant_mname = loanReader["Applicant_MName"].ToString();
                    loanObj.Applicant_lname = loanReader["Applicant_LName"].ToString();
                    loanObj.Business_Name = loanReader["Business_Name"].ToString();
                    loanObj.LoanApplication_Amount = Convert.ToDecimal(loanReader["LoanAmount"].ToString());
                    loanObj.LoanApplication_Description = loanReader["LoanDescription"].ToString();
                    loanObj.LoanApplication_Status = int.Parse(loanReader["LoanStatus"].ToString());
                    loanObj.LoanApplication_Date = Convert.ToDateTime(loanReader["LoanApplication_Date"].ToString());
                    loanObj.LoanApplication_BankerComment = loanReader["LoanBanker_Comment"].ToString();
                    loanObj.External_ID = int.Parse(loanReader["External_ID"].ToString());
                    loanObj.CreditorCallBackURL = CallbackURL;

                    loanlist.Add(loanObj);
                }

                return loanlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public bool Update_LoanInfo(Creditor CreditorPara, int id)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[3];
                loan_para[0] = new MySqlParameter("CreditorAssigned_ID", id);
                loan_para[1] = new MySqlParameter("LoanApplication_Status", CreditorPara.LoanApplication_Status);
                loan_para[2] = new MySqlParameter("LoanApplication_BankerComment", CreditorPara.LoanApplication_BankerComment);

                int r = dbHelper.Execute("Update_Loan_From_Creditor", DBHelper.QueryType.StotedProcedure, loan_para);

                if (r == 1)
                {
                    Result = true;
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public bool CheckIsClosed(int CreditorAssigned_ID)
        {
            DBHelper dbHelper = new DBHelper();
            bool Result = false;
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] loan_para = new MySqlParameter[1];
                loan_para[0] = new MySqlParameter("CreditorAssigned_ID", CreditorAssigned_ID);

                DataSet dsloan = dbHelper.ExecuteDS("Get_LoansDetails_By_ID", DBHelper.QueryType.StotedProcedure, loan_para);

                if (dsloan.Tables[0].Rows.Count > 0)
                {
                    if (int.Parse(dsloan.Tables[0].Rows[0]["LoanStatus"].ToString()) == 8)// Loan Status 8 i.e. Closed by External Service
                    {
                        Result = true;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }

        public void LogMessage(string msg)
        {
            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                MySqlParameter[] app_para = new MySqlParameter[1];
                app_para[0] = new MySqlParameter("LogMsg", msg);

                int r = dbHelper.Execute("Add_LogMsg", DBHelper.QueryType.StotedProcedure, app_para);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbHelper.DisConnect();
                dbHelper = null;
            }
        }
    }
}
