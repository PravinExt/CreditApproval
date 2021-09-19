using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using CreditApproval.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditApproval.Controllers
{
    [Route("CreditApproval/[controller]")]
    [ApiController]
    public class CreditorController : ControllerBase
    {
        IAmazonSQS sqsClient { get; set; }

        public CreditorController(IAmazonSQS amazonSQS)
        {
            this.sqsClient = amazonSQS;
        }

        // GET: api/Creditor
        [HttpGet]
        public List<Creditor> Get()
        {
            List<Creditor> lstloandetail = new List<Creditor>();
            Creditor loandetail = new Creditor();
            lstloandetail = loandetail.Get_All_Loan();
            return lstloandetail;
        }

        //// GET: api/Creditor/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Creditor
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT: api/Creditor/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Creditor value)
        {
            string msg = "";
            Creditor loandetail = new Creditor();
            bool result = loandetail.Update_LoanInfo(value, id);

            string qUrl = "https://sqs.ap-south-1.amazonaws.com/052987743965/CreditDecision";
            string messageBody = "This is a test message executed";
            SendMessageResponse responseSendMsg = await sqsClient.SendMessageAsync(qUrl, messageBody);

            if (result == true)
            {
                msg = "Loan data updated successfully for Loan " + id.ToString();
            }
            else
            {
                msg = "Loan data not updated.";
            }

            //return msg;
        }

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
    }
}

