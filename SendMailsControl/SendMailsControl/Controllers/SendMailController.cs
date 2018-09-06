using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using System.Reflection;
using System.Web.Http.Cors;
using System.Net.Mail;
using System.Configuration;
using SendMailsCommonsPackage.Models;
using SendMailsCommonsPackage.Classes;
using Newtonsoft.Json;
using DisplayControlFilter.Filters;

namespace SendMailsControl.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[DisplayFilter]
    /// <summary>
    /// Created: Thais Lima Dias 
    /// When: 06/10/2016
    /// For: API for Send Emails Automatically
    /// </summary> 

    /*
        ======> PLEASE DON'T FORGET TO SET THE WEB.CONFIG  
    */

    public class SendMailController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Created: Thais Lima Dias 
        /// When: 06/10/2016
        /// For: API for Send Emails Automatically for Alerts fired
        /// </summary>  
        /// <summary>
        /// Modifed: Fernando Souza
        /// When: 15/10/2017
        /// </summary>  
        public IHttpActionResult PostSendMail(Alert alert)
        {

            
            try
            {
                if (alert.groupMailsListId ==0)
                {
                    return BadRequest("Não Existe uma lista de e-mails associada a este alerta, verifique!");
                }
                //Loading the Group Mails List
                string UrlGroupMailsList = ConfigurationManager.AppSettings["UrlGroupMailsList"];
                UrlGroupMailsList = UrlGroupMailsList + "?id=" + alert.groupMailsListId;

                string groupMailList = string.Empty;

                if (alert.groupMailsListId >0)
                {
                    RestComunication restComunication = new RestComunication();
                    groupMailList = restComunication.RequestOtherAPI("Get", UrlGroupMailsList, "xpto", "application/json", null);
                }
                GroupMails groupMailsReturn  = JsonConvert.DeserializeObject<GroupMails>(groupMailList);

                //Loading the E-Mails List oh the users
                List<User> users = groupMailsReturn.users;
                Log.Debug("Recuperando e-mail dos usuários cadastrado no alerta");
                if (users.Count() > 0)
                {
                    foreach (User listUser in users)
                    {
                        string UrlAccessControlUser = ConfigurationManager.AppSettings["UrlAccessControlUser"];
                        UrlAccessControlUser = UrlAccessControlUser + "?userId=" + listUser.userIdAPIUser;

                        RestComunication restComunication = new RestComunication();
                        string email = restComunication.RequestOtherAPI("Get", UrlAccessControlUser, "xpto", "application/json", null);
                        string mailReturned = JsonConvert.DeserializeObject<string>(email);
                        Email a = new Email();
                        a.email = mailReturned;
                        groupMailsReturn.emails.Add(a);
                    }
                }


                //Loading parameters of the serv E-mails
                string subject = "Aviso Sistema de Monitoramento de Energia"; //alert.descAlert;                
                string body = alert.message;
                string FromMail = ConfigurationManager.AppSettings["FromMail"];
                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"]);
                //mail.From = new MailAddress(FromMail);
                //mail.Subject = subject;
                //mail.Body = body;
                //SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                //SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["CredentialsEmail"],
                //                                               ConfigurationManager.AppSettings["CredentialsUser"]);
                //SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

                //foreach (Email EmailsList in groupMailsReturn.emails)
                //{
                //    Log.Debug("Eviando e-mail para -- " + EmailsList.email);
                //    mail.To.Add(EmailsList.email);
                //}
                ////Send Email(s) to destiny
                //SmtpServer.Send(mail);

                foreach (Email email in groupMailsReturn.emails)
                {
                    Log.Debug("Eviando e-mail para -- " + email.email);
                    
                    using (MailMessage message = new MailMessage(FromMail,email.email,subject,body))
                    {
                        SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServerHost"]);
                        client.UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                        client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                        Log.Debug(client.Host);
                        Log.Debug(client.Port);
                        client.Send(message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + " - " + ex.ToString());
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            return Ok("Send Mails Success, verify your MailBox");
        }
    }
}
