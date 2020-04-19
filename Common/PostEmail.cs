using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
  public class PostEmail
    {
        public static async Task EmailPost(string email,string html)
        {
            using (MailMessage mailMessage = new MailMessage())
            using (SmtpClient smtpClient = new SmtpClient("smtp.qq.com"))
            {
                //在数据库中记录邮箱 可以多个根据;分割

                mailMessage.To.Add(email);
                mailMessage.Body = "邮件正文";
                mailMessage.From = new MailAddress("554476199@qq.com");
                mailMessage.Subject = html;
                smtpClient.Credentials = new System.Net.NetworkCredential("554476199@qq.com", "nnvgfjseacqzbfab");//如果启用了“客户端授权码”，要用授权码代替密码
              await  smtpClient.SendMailAsync(mailMessage);
            }
        } 
    }
}
