using ApplicationSubmission.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditApproval.Model
{
    public class Creditor
    {
        public int LoanApplication_ID { get; set; }
        public int Applicant_ID { get; set; }
        public string Applicant_fname { get; set; }
        public string Applicant_mname { get; set; }
        public string Applicant_lname { get; set; }
        public int Business_ID { get; set; }
        public string Business_Name { get; set; }
        public decimal LoanApplication_Amount { get; set; }
        public DateTime LoanApplication_Date { get; set; }
        public string LoanApplication_Description { get; set; }
        public int LoanApplication_Status { get; set; }
        public string LoanApplication_BankerComment { get; set; }


        public List<Creditor> Get_All_Loan()
        {
            List<Creditor> loanlist = new List<Creditor>();
            DBHelper dbHelper = new DBHelper();
            try
            {
                dbHelper.Connect(dbHelper.GetConnStr());

                //MySqlParameter[] banker_para = new MySqlParameter[1];
                //banker_para[0] = new MySqlParameter("Business_ID", Business_ID);
                MySqlDataReader loanReader = dbHelper.ExecuteReader("Get_All_Loans_For_Creditor", DBHelper.QueryType.StotedProcedure, null);

                while (loanReader.Read())
                {
                    Creditor loaniobj = new Creditor();

                    loaniobj.LoanApplication_ID = int.Parse(loanReader["LoanApplication_ID"].ToString());
                    loaniobj.Applicant_ID = int.Parse(loanReader["Applicant_ID"].ToString());
                    loaniobj.Applicant_fname = loanReader["Applicant_FName"].ToString();
                    loaniobj.Applicant_mname = loanReader["Applicant_MName"].ToString();
                    loaniobj.Applicant_lname = loanReader["Applicant_LName"].ToString();
                    loaniobj.Business_ID = int.Parse(loanReader["Business_ID"].ToString());
                    loaniobj.Business_Name = loanReader["BusinessName"].ToString();
                    loaniobj.LoanApplication_Amount = Convert.ToDecimal(loanReader["LoanAmount"].ToString());
                    loaniobj.LoanApplication_Description = loanReader["LoanDescription"].ToString();
                    loaniobj.LoanApplication_Status = int.Parse(loanReader["LoanStatus"].ToString());
                    loaniobj.LoanApplication_Date = Convert.ToDateTime(loanReader["LoanApplication_Date"].ToString());
                    loaniobj.LoanApplication_BankerComment = loanReader["LoanBanker_Comment"].ToString();

                    loanlist.Add(loaniobj);
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
                loan_para[0] = new MySqlParameter("LoanApplication_ID", id);
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
    }
}
