using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using CreditApproval.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CreditApproval.Controllers
{
    [Route("CreditApproval/[controller]")]
    [ApiController]
    public class CreditorController : ControllerBase
    {
        public IAmazonSQS sqsClient { get; set; }

        private IConfiguration _configuration;

        public CreditorController(IAmazonSQS amazonSQS, IConfiguration Configuration)
        {
            this.sqsClient = amazonSQS;
            _configuration = Configuration;
        }

        // GET: CreditApproval/Creditor
        [HttpGet]
        public JsonResult Get()
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string msg;
            List<Creditor> lstloandetail = new List<Creditor>();
            Creditor loandetail = new Creditor();

            try
            {
                lstloandetail = loandetail.Get_All_Loan();

                if (lstloandetail.Count <= 0)
                {
                    msg = "No loan details found.";
                    loandetail.LogMessage("Get Loan Data ----" + " " + msg.ToString());
                    return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
                else
                {
                    var content = JsonConvert.SerializeObject(lstloandetail);
                    loandetail.LogMessage("Get Loan Data ----" + " " + content.ToString());
                    return new JsonResult(lstloandetail, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                loandetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }
        }

        // PUT: CreditApproval/Creditor/5
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody] Creditor value)
        {
            this.Response.ContentType = "application/json";
            this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string msg = ""; int Loan_Status = 0; bool result = false;
            Creditor loandetail = new Creditor();
            try
            {
                bool IsClosed = false;
                //Check the if the loan application has closed.
                IsClosed = loandetail.CheckIsClosed(id);

                if (IsClosed == false)
                {
                    Loan_Status = value.LoanApplication_Status;
                    //Update Loan Status
                    result = loandetail.Update_LoanInfo(value, id);

                    if (Loan_Status == 6) //Loan_Status 6 i.e. Sent To  Dcision Engine
                    {
                        value.CreditorCallBackURL = _configuration.GetSection("CallBackURL").Value.ToString() + "/" + id.ToString();
                        string qUrl = _configuration.GetSection("QueueURL").Value.ToString();
                        var JsonMessage = JsonConvert.SerializeObject(value);

                        var res = sqsClient.SendMessageAsync(qUrl, JsonMessage);
                    }
                }


                if (result == true)
                {
                    if (Loan_Status == 6)
                    {
                        msg = "Loan application sent to the decision engine.";
                        var content = JsonConvert.SerializeObject(value);
                        loandetail.LogMessage(msg + " " + content.ToString());
                    }
                    if (Loan_Status == 7)
                    {
                        msg = "Loan application approved by decision engine.";
                        var content = JsonConvert.SerializeObject(value);
                        loandetail.LogMessage(msg + " " + content.ToString());
                    }
                    if (Loan_Status == 8)
                    {
                        msg = "Loan application closed by external service.";
                        var content = JsonConvert.SerializeObject(value);
                        loandetail.LogMessage(msg + " " + content.ToString());
                    }
                    else
                    {
                        msg = "Loan data updated successfully.";
                        var content = JsonConvert.SerializeObject(value);
                        loandetail.LogMessage(msg + " " + content.ToString());
                    }
                }
                else if (IsClosed == true)
                {
                    msg = "This loan application is closed, You can not process this.";
                    var content = JsonConvert.SerializeObject(value);
                    loandetail.LogMessage(msg + " " + content.ToString());
                }
                else
                {
                    msg = "Loan application not updated.";
                    var content = JsonConvert.SerializeObject(value);
                    loandetail.LogMessage(msg + " " + content.ToString());
                }

                return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                loandetail.LogMessage(ex.Message.ToString() + " " + ex.Message.ToString());
                return new JsonResult(ex.Message);
            }
        }

        // Options: CreditApproval/ApiWithActions/5
        [HttpDelete("{id}")]
        public JsonResult Options(int id)
        {
            try
            {
                HttpResponseMessage res = new HttpResponseMessage();

                this.Response.ContentType = "application/json";
                this.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return new JsonResult(res, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                return new JsonResult(ex.Message);
            }

        }
    }
}

