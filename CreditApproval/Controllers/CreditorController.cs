using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using CreditApproval.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CreditApproval.Controllers
{
    [Route("CreditApproval/[controller]")]
    [ApiController]
    public class CreditorController : ControllerBase
    {
        public IAmazonSQS sqsClient { get; set; }

        public CreditorController(IAmazonSQS amazonSQS)
        {
            this.sqsClient = amazonSQS;
        }

        // GET: CreditApproval/Creditor
        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                string msg;
                List<Creditor> lstloandetail = new List<Creditor>();
                Creditor loandetail = new Creditor();
                lstloandetail = loandetail.Get_All_Loan();

                this.Response.ContentType = "text/json";
                this.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                if (lstloandetail.Count <= 0)
                {
                    msg = "No loan details found.";
                    return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
                else
                {
                    return new JsonResult(lstloandetail, new JsonSerializerSettings { Formatting = Formatting.Indented });
                }
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                return new JsonResult(ex.Message);
            }
        }

        // PUT: CreditApproval/Creditor/5
        [HttpPut("{id}")]
        public JsonResult Put(int id, [FromBody] Creditor value)
        {
            try
            {
                string msg = "";
                Creditor loandetail = new Creditor();
                bool result = loandetail.Update_LoanInfo(value, id);

                if (result == true)
                {
                    msg = "Loan details updated successfully.";
                }
                else
                {
                    msg = "Loan details not updated.";
                }

                this.Response.ContentType = "text/json";
                this.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return new JsonResult(msg, new JsonSerializerSettings { Formatting = Formatting.Indented });
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = 400;
                return new JsonResult(ex.Message);
            }
        }

    }
}

