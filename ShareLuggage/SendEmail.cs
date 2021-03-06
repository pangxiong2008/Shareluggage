﻿using System;
using System.Configuration;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using WebCWBasicClass.Log;

namespace ShareLuggage.Email
{
    public interface IEmailCentre
    {
        Email_raw _EmailRaw{ get; set; }
        EmailConfig _EmailConfig { get; set; }
        EmailResult _EmailResult { get; set; }
        string  Email_create(string Type);


    }
    public abstract class EmailCentre : IEmailCentre
    {
      public   abstract Email_raw _EmailRaw { set; get; }
      public abstract EmailConfig _EmailConfig { get; set; }
      public abstract EmailResult _EmailResult { get; set; }
      public abstract string  Email_create(string Type);

    }

    class EmailShareLuggage : EmailCentre
    {
       public  override Email_raw _EmailRaw { set; get; }
        public override  EmailConfig _EmailConfig { get; set; }
        public override  EmailResult _EmailResult { get; set; }
        public override  string  Email_create(string Type)
        {
            string result;
            try
            {
                Email _email = EmailSimpleFactory.createEmail(Type);
                _email._EmailConfig = _EmailConfig;
                _email._EmailRaw = _EmailRaw;

                _email.Email_write();
                _EmailResult = _email._EmailResult;
                result = "Success";
                TransitLog.LogTransit("senderName=" + _EmailRaw.senderAddress + ";senderAddress=" + _EmailRaw.senderAddress + ";"
                                       + "receiverAddress=" + _EmailRaw.receiverAddress + ";body=" + _EmailRaw.body + ";"
                                       + "ResultCode="+ _EmailResult.ResultCode+"; ResultText="+_EmailResult.ResultText+"; ResultBody="+ _EmailResult.ResultBody, "EmailShareLuggage");

            }
            catch (Exception Ex)
            {
                _EmailResult.ResultCode = "437";
                _EmailResult.ResultText = "Failed";
                _EmailResult.ResultBody = "SystemError";
                result = "Failed";
                ErrorLog.LogError("senderName=" + _EmailRaw.senderAddress + ";senderAddress=" + _EmailRaw.senderAddress + ";"
                                       + "receiverAddress=" + _EmailRaw.receiverAddress + ";body=" + _EmailRaw.body + ";"
                                       + "ResultCode=" + _EmailResult.ResultCode + "; ResultText=" + _EmailResult.ResultText + "; ResultBody=" + _EmailResult.ResultBody+"；Exception="+Ex.ToString(), "EmailShareLuggage");

            }
            return result;
        }


    }
    public abstract class Email
    {
        public abstract Email_raw _EmailRaw { set; get; }
        public abstract EmailConfig _EmailConfig { get; set; }
        public abstract EmailResult _EmailResult { get; set; }
        public abstract void Email_write();

    }
    public class EmailSimpleFactory
    {
        public static Email createEmail(string Type)
        {
            Email _email = null;
            if (Type.Equals("Gmail_Connect"))
            {
                _email = new Email_goolge();
            }
            else if (Type.Equals("Gmail_SetPassword"))
            {
                _email = new Email_goolge(Type);

            }
            return _email;
        }

    }
    //public class PXGpaymentCharge
    //{
    //    public void InboundData_PXG_ADC_Charged()
    //    {
    //        ChargedPayment Purchase_create = ChargeSimpleFactory.CreatePurchase(AppType);

    //        response = Purchase_create.run(ANI, SSID, Amount, Gateway, SubProfile);
    //    }

    //}


    //public abstract class ChargedPayment
    //{
    //    public abstract string[] run(String ID, string Type);
    //    public abstract string[] run(string ANI, string SSID, string Amount, string Gateway, string SubProfile);
    //}
    //public class Charge_Paybyphone : ChargedPayment
    //{

    //}


    //public class ChargeSimpleFactory
    //{
    //    public static ChargedPayment CreatePurchase(string Type)
    //    {
    //        ChargedPayment charged_Payment = null;
    //        if (Type.Equals("PayByPhone"))
    //        {
    //            charged_Payment = new Charge_Paybyphone();
    //        }

    //        return charged_Payment;
    //    }
    //}

    sealed class Email_goolge : Email
    {
        // abstract public void Create_draft();
        public override Email_raw _EmailRaw { set; get; }
        public override EmailConfig _EmailConfig { get; set; }
        public override EmailResult _EmailResult { get; set; }
        private string type;
        public Email_goolge()
        {
            type = "simple";
        }
        public Email_goolge(string Type)
        {
            type = Type;
        }

        public override void Email_write()
        {
            _EmailResult=Singleton_sendEmail.Single_gmail.run(_EmailRaw, _EmailConfig);
        }



    }


    
   public  class Email_raw
    {
        // clientSecret = "dfDfdOJeobb1x0VNrTDHsEGO";
        public string senderName { get; set; }
        public string senderAddress { get; set; }
        public string receiverName { get; set; }
        public string receiverAddress { get; set; }
        public string subject { set; get; }
        public string body { set; get; }
    }


    public sealed class Singleton_sendEmail
    {
        private static Singleton_sendEmail single_gmail;

        private static object Lock = new object();
        private Singleton_sendEmail()
        { }
        public static Singleton_sendEmail Single_gmail
        {
            get
            {
                if (single_gmail == null)
                {
                    lock (Lock)
                    {
                        if (single_gmail == null)
                        {
                            single_gmail = new Singleton_sendEmail();
                        }
                    }

                }
                return single_gmail;
            }

        }
        public  EmailResult run(Email_raw _Email_raw, EmailConfig _EmailConfig)
        {
            Send_Gmail _Send_Gmail = new Send_Gmail();
            EmailResult _EmailResult = new EmailResult();
            _EmailResult = _Send_Gmail.send(_Email_raw, _EmailConfig);
            return _EmailResult;

        }

    }


    //public class SingeCharge
    //{
    //    private static SingeCharge SendSms;

    //    private static readonly object Locker = new object();
    //    private SingeCharge()
    //    { }
    //    public static SingeCharge send()
    //    {
    //        lock (Locker)
    //        {
    //            if (SendSms == null)
    //            {
    //                SendSms = new SingeCharge();
    //            }
    //        }
    //        return SendSms;
    //    }
        

    //}

   public  class EmailConfig
    {
        public string clientId { get; set; }
        //public string clientId = "739330450434-e4cq0bonlglucdnmodofbjg09qj26u36.apps.googleusercontent.com";
        public string clientSecret { get; set; }
        public string ApplicationName { get; set; }
        public UserCredential credential { get; set; }
        public GmailService service { get; set; }
    }

   public  class EmailResult
    {
        public string ResultCode { get; set; }
        public string ResultText { get; set; }
        public string ResultBody { get; set; }

    }



    class Send_Gmail
    {
        public  EmailResult send(Email_raw _Email_raw, EmailConfig _EmailConfig)
        {
            // EmailConfig _EmailConfig = new EmailConfig();
            // _EmailConfig.clientId = "739330450434-e4cq0bonlglucdnmodofbjg09qj26u36.apps.googleusercontent.com";
            //  var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            // _EmailConfig.clientSecret = "dfDfdOJeobb1x0VNrTDHsEGO";
            //var senderName = ConfigurationManager.AppSettings["EmailSenderName"];
            //var senderAddress = ConfigurationManager.AppSettings["EmailSenderAddress"];
            //var receiverName = ConfigurationManager.AppSettings["EmailReceiverName"];
            //var receiverAddress = ConfigurationManager.AppSettings["EmailReceiverAddress"];
            EmailResult _emailResult = new EmailResult();
            try
            {
                _EmailConfig.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = _EmailConfig.clientId,
                        ClientSecret = _EmailConfig.clientSecret,
                    },
                    new[] { GmailService.Scope.GmailCompose },
                    "user",
                    CancellationToken.None).Result;

                _EmailConfig.service = new GmailService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = _EmailConfig.credential,
                    ApplicationName = _EmailConfig.ApplicationName,
                });

                // only the raw parameter of the message resource needs set, see: http://stackoverflow.com/questions/24460422/how-to-send-a-message-successfully-using-the-new-gmail-rest-api
                // from that there is a working example in the RFC 2822nspecification: "From: John Doe <jdoe@machine.example>\nTo: Mary Smith <mary@example.net>\nSubject: Saying Hello\nDate: Fri, 21 Nov 1997 09:55:06 -0600\nMessage-ID: <1234@local.machine.example>\n\nThis is a message just to say hello.\nSo, \"Hello\".";
                // use that as a working base
                // the values Date and Message-ID have no bearing on the final email (or didn't to me) so have keep them as placeholders and haven't tried to replace them

                // const string subject = "Email Subject";

                // there are some issues around the body encoding/decoding
                // this message decoded will have a '5' at the end
                // a full stop at the end will make an invalid raw parameter
                // but this was good enough for my purposes...
                //const string body = "Hello this is an email wriien by a very simple console application";
                //senderName = "51ShareLuggage";
                //senderAddress = "51ShareLuggage@gmail.com";
                //receiverName = "pangxiong";
                //receiverAddress = "wuxiaonong@gmail.com";
                // format the message
                var text = string.Format("From: {0} <{1}>\nTo: {2} <{3}>\nSubject: {4}\nDate: Fri, 21 Nov 1997 09:55:06 -0600\nMessage-ID: <1234@local.machine.example>\n\n{5}",
                    _Email_raw.senderName,
                    _Email_raw.senderAddress,
                    _Email_raw.receiverName,
                    _Email_raw.receiverAddress,
                    _Email_raw.subject,
                    _Email_raw.body);
                // must be base64 encoded but also web safe and also initially UTF8
                // http://stackoverflow.com/questions/24460422/how-to-send-a-message-successfully-using-the-new-gmail-rest-api
                // http://stackoverflow.com/questions/13017703/java-and-net-base64-conversion-confusion
                var encodedText = System.Web.HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(text));

                Console.WriteLine("Raw email:\n{0}", encodedText);

                Message message = new Message { Raw = encodedText };

                Message request = _EmailConfig.service.Users.Messages.Send(message, "me").Execute();

                _emailResult.ResultCode="200";
                //Console.WriteLine(
                //    string.IsNullOrEmpty(request.Id) ? "Issue sending, returned id: {0}" : "Email looks good, id populated: {0}",
                //    request.Id);
                //Console.WriteLine(
                _emailResult.ResultText = string.IsNullOrEmpty(request.Id) ? "Failed" : "Success";
                _emailResult.ResultCode=string.Format( string.IsNullOrEmpty(request.Id) ? "Issue sending, returned id: {0}" : "Email looks good, id populated: {0}",request.Id);

            }
            catch (Exception Ex)
            {
                _emailResult.ResultCode = "437";
                
                _emailResult.ResultText = "Failed";
                _emailResult.ResultCode = "SystemError";


            }
            return _emailResult;

        }

    }
}
