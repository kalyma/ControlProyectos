using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using System.Diagnostics;

namespace Plantilla_de_Correo.Class
{
    public class CUtil
    {
        public static string leerArchivo(string archivo)
        {
            try
            {
                string path = ConfigurationManager.AppSettings.Get("path") + archivo;
                StreamReader sr = new StreamReader(path);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public static void enviarCorreo(string body, string to, string from, string subject, string cc, string url_adjunto)
        {
            try
            {
                CUtil.LogEvent("Enviar correo " + subject + "a " + to + " desde " + from);
                MailMessage email = new MailMessage();

                if (to.Contains(";"))
                {

                    string[] listacorreos = to.Split(';');
                    foreach (string correo in listacorreos)
                    {
                        if (correo.Contains('@'))
                        {
                            email.To.Add(new MailAddress(correo.Trim()));
                        }
                    }
                }
                else
                {
                    email.To.Add(new MailAddress(to));
                }

                if (cc != String.Empty)
                {
                    email.CC.Add(new MailAddress(cc));
                }
                email.From = new MailAddress(from);
                email.Subject = subject;
                email.Body = htmlentities(body);
                email.IsBodyHtml = true;
                email.Priority = MailPriority.Normal;
                email.Attachments.Add(new Attachment(url_adjunto));


                SmtpClient smtp = new SmtpClient();
                smtp.Host = "10.1.141.213";
                smtp.Port = 25;

                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Send(email);
                CUtil.LogEvent("correo enviado");
                email.Dispose();

            }
            catch (Exception ex)
            {
                CUtil.LogError("Error enviando correo");
                CUtil.LogError("Subject: " + subject);
                CUtil.LogError(ex.Message);
                CUtil.LogError(ex.Source);
                CUtil.LogError(ex.StackTrace);

            }


        }

        public static void enviarCorreo(string body, string to, string from, string subject, string cc)
        {
            try
            {
                CUtil.LogEvent("Enviar correo " + subject + "a " + to + " desde " + from);
                MailMessage email = new MailMessage();

                if (to.Contains(";"))
                {

                    string[] listacorreos = to.Split(';');
                    foreach (string correo in listacorreos)
                    {
                        if (correo.Contains('@'))
                        {
                            email.To.Add(new MailAddress(correo.Trim()));
                        }
                    }
                }
                else
                {
                    email.To.Add(new MailAddress(to));
                }

                if (cc != String.Empty)
                {
                    email.CC.Add(new MailAddress(cc));
                }
                email.From = new MailAddress(from);
                email.Subject = subject;
                email.Body = htmlentities(body);
                email.IsBodyHtml = true;
                email.Priority = MailPriority.Normal;


                SmtpClient smtp = new SmtpClient();
                smtp.Host = "10.1.141.213";
                smtp.Port = 25;

                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Send(email);
                CUtil.LogEvent("correo enviado");
                email.Dispose();

            }
            catch (Exception ex)
            {
                CUtil.LogError("Error enviando correo");
                CUtil.LogError("Subject: " + subject);
                CUtil.LogError(ex.Message);
                CUtil.LogError(ex.Source);
                CUtil.LogError(ex.StackTrace);

            }


        }


        public static string GetDireccionIp()
        {

            //// Recuperamos la IP de la máquina del cliente
            //// Primero comprobamos si se accede desde un proxy
            //string ipAddress1 = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //// Acceso desde una máquina particular
            //string ipAddress2 = request.ServerVariables["REMOTE_ADDR"];
            //string ipAddress = string.IsNullOrEmpty(ipAddress1) ? ipAddress2 : ipAddress1;
            //// Devolvemos la ip
            //return ipAddress;
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GeneratePassWord()
        {
            string password = "";
            string[] colores = { "Amarillo", "Rojo", "Azul", "Verde", "Naranja", "Rosado", "Cafe", "Negro", "Blanco", "Violeta", "Morado", "Gris", "Marron" };


            Random rnd = new Random();
            password = colores[rnd.Next(0, 13)];
            int lenght = password.Length;
            int min = 0, max = 0;
            switch (lenght)
            {

                case 4: min = 9999; max = 100000; break;
                case 5: min = 999; max = 10000; break;
                case 6: min = 99; max = 1000; break;
                case 7: min = 9; max = 100; break;
                case 8: min = 0; max = 10; break;
            }


            password = rnd.Next(10, 100) + password + rnd.Next(min, max) + "*";

            return password;
        }

        public static bool ValidatePassWord(string passWord)
        {
            string regexp = "\\s+|\\@+|\\¿+|\\?+|\\°+|\\¬+|\\| +|\\!+|\\#+|\\$+|\\% +|\\&+|\\+|\\= +|\\’+|\\¡+|\\++|\\*+|\\~+|\\[+|\\]+|\\{+|\\}+|\\^+|\\<+|\\>+|\\\"+ ";
            bool upper = passWord.Any(c => char.IsUpper(c));
            bool lower = passWord.Any(c => char.IsLower(c));
            bool digit = passWord.Any(c => char.IsDigit(c));
            bool specialChar = Regex.IsMatch(passWord, regexp);
            return (upper && lower && digit && specialChar && passWord.Count() >= 12) ? true : false;
        }

        public void LogRequest(string Log)
        {
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("/Log") + "\\Log.txt", true);
            sw.WriteLine(DateTime.Now.ToString() + " - " + Log);
            sw.Flush();
            sw.Close();

        }

        public static void LogEvent(string Log)
        {
            try
            {
                StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("/Log") + "\\Log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt", true);
                sw.WriteLine(DateTime.Now.ToString() + "\t" + Log + "\t" + GetDireccionIp());
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }

        public static void LogError(string Log)
        {
            try
            {
                StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("/Log") + "\\Error_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt", true);
                sw.WriteLine(DateTime.Now.ToString() + "\t" + Log + "\t" + GetDireccionIp());
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }

        public static string userName(string registro)
        {
            try
            {
                string sUserName = registro;
                sUserName = sUserName.Split('(')[1];
                sUserName = sUserName.Split(')')[0];
                sUserName = sUserName.Trim();
                return sUserName;
            }
            catch
            {
                //CUtil.LogError("Error " + ex.Message.ToString());
                //CUtil.LogError("Error " + ex.StackTrace.ToString());
                return registro;
            }
        }

        public static string fecha()
        {
            string fecha = "";
            fecha = CUtil.mes(DateTime.Now.Month) + DateTime.Now.ToString(" dd ") + "del" + DateTime.Now.ToString(" yyyy");
            return fecha;
        }

        public static string mes(int mes)
        {
            switch (mes)
            {
                case 1: return "Enero";
                case 2: return "Febrero";
                case 3: return "Marzo";
                case 4: return "Abril";
                case 5: return "Mayo";
                case 6: return "Junio";
                case 7: return "Julio";
                case 8: return "Agosto";
                case 9: return "Septiembre";
                case 10: return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre";
            }
            return "";
        }

        public static string htmlentities(string text)
        {
            text = text.Replace("á", "&aacute;");
            text = text.Replace("é", "&eacute;");
            text = text.Replace("í", "&iacute;");
            text = text.Replace("ó", "&oacute;");
            text = text.Replace("ú", "&uacute;");
            text = text.Replace("ñ", "&ntilde;");
            text = text.Replace("Á", "&Aacute;");
            text = text.Replace("É", "&Eacute;");
            text = text.Replace("Í", "&Iacute;");
            text = text.Replace("Ó", "&Oacute;");
            text = text.Replace("Ú", "&Uacute;");
            text = text.Replace("Ñ", "&Ntilde;");

            return text;
        }


        public static void ExecuteCommandAsAdmin(string command)
        {

            try
            {
                //ProcessStartInfo procStartInfo = new ProcessStartInfo(
                //                                        "cmd.exe",
                //                                        "/c " + command);
                //procStartInfo.UseShellExecute = true;
                //procStartInfo.CreateNoWindow = true;
                //procStartInfo.Verb = "runas";
                //procStartInfo.Arguments = "/env /user:" + "Administrator" + " " + command;

                /////command contains the command to be executed in cmd
                //System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //proc.StartInfo = procStartInfo;
                //proc.Start();


                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/user:" + "Administrator /C " + command;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Decode_Iso_8859_1(string base64EncodedData)
        {
            byte[] base64EncodedBytes;
            try
            {
                base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            }
            catch (Exception ex)
            {
                try
                {
                    base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData + "=");
                }
                catch
                {
                    base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData + "==");
                }
            }
            //return System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("iso-8859-1"), System.Text.Encoding.UTF8, base64EncodedBytes));
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }



    }
}