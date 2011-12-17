using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace NBrowserID
{
    /// <summary>
    /// Class that wraps the BrowserID authentication mechanism.
    /// </summary>
    public class BrowserIDAuthentication
    {
        /// <summary>
        /// Gets or sets the identity authority verification URL.
        /// </summary>
        public Uri IdentityAuthorityVerificationUrl { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserIDAuthentication"/> class.
        /// By default it uses <c>https://browserid.org/verify</c> as <see cref="IdentityAuthorityVerificationUrl"/>
        /// </summary>
        public BrowserIDAuthentication()
            : this("https://browserid.org/verify")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserIDAuthentication"/> class.
        /// </summary>
        /// <param name="verificationUrl">The identity authority verification URL.</param>
        public BrowserIDAuthentication(string verificationUrl)
        {
            this.IdentityAuthorityVerificationUrl = new Uri(verificationUrl);
        }

        /// <summary>
        /// Veriy the assertion supplied by the browser.
        /// </summary>
        /// <param name="assertion">The assertion data which needs to be verified.</param>
        /// <returns>Instance of <see cref="VerificationResult"/></returns>
        public VerificationResult Verify(string assertion)
        {
            JObject verificationResult = this.GetVerificationResult(assertion);
            string email = "";
            bool verified = false;

            if (verificationResult != null && verificationResult["status"].ToString() == "okay")
            {
                email = verificationResult["email"].ToString();
                verified = true;
            }

            return new VerificationResult {Email = email, IsVerified = verified };
        }

        /// <summary>
        /// Gets the verification result from Identity Authority.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns></returns>
        private JObject GetVerificationResult(string assertion)
        {
            // Build he request
            var req = (HttpWebRequest)WebRequest.Create(IdentityAuthorityVerificationUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            if (HttpContext.Current == null)
            {
                throw new NullReferenceException("HttContext is null. Please make sure that, your application is running in a web scenario");
            }

            // Build the data for posting
            var payload = string.Format("assertion={0}&audience={1}", assertion, HttpContext.Current.Request.Headers["Host"]);
            byte[] payloadInBytes = Encoding.UTF8.GetBytes(payload);
            req.ContentLength = payloadInBytes.Length;

            var dataStream = req.GetRequestStream();
            dataStream.Write(payloadInBytes, 0, payloadInBytes.Length);
            dataStream.Close();

            JObject result = null;

            var res = req.GetResponse();
            dataStream = res.GetResponseStream();
            if (dataStream != null)
            {
                var responseData = new StreamReader(dataStream).ReadToEnd();
                res.Close();
                dataStream.Close();
                result = JObject.Parse(responseData);
            }else
            {
                res.Close();
            }

            return result;
        }
    }
}
