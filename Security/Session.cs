using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace Cfe.Security
{
    public class Session
    {
        /// <summary>
        /// Crea los mecanismos necesarios para asegurar que esta sesión no será suplantada.
        /// 
        /// Llámese luego de que se ha creado la sesión en el servidor, para asegurarla en
        /// futuros usos de la sesión.
        /// </summary>
        public void Secure(int id)
        {
            
            System.Diagnostics.Debug.Print("Creando token para la sesión segura");

            HttpContext.Current.Session["Id"] = id;
            HttpContext.Current.Session["token"] = this.Encrypt(this.GetSeed()) as string;
        }

        public void Remove()
        {
            // Obtengo la configuración predetermina de la sesión.
            SessionStateSection sessionStateSection = (System.Web.Configuration.SessionStateSection) ConfigurationManager.GetSection("system.web/sessionState");

            // Eliminar los datos de seguridad de la sesión.
            System.Web.HttpContext.Current.Session["Id"] = null;
            System.Web.HttpContext.Current.Session["Entries"] = null;

            // Eliminar la cookie para evitar que se asigne el mismo identificador.
            HttpContext.Current.Response.Cookies[sessionStateSection.CookieName].Expires = DateTime.Now.AddYears(-1);
        }

        /// <summary>
        /// Abre de manera segura la sesión, para evitar que haya sido establecida sin seguridad.
        /// 
        /// Si la sessión no fue correctamente abierta, se resetarán los datos y devolverá false.
        /// </summary>
        /// <returns>Verdadero si la sesión es segura.</returns>
        public bool Open()
        {
            if (!this.IsValid())
            {
                System.Diagnostics.Debug.Print("El token de la sesión no es válido, procedo a desactivar al usuario.");

                HttpContext.Current.Session["Id"] = 0;
                HttpContext.Current.Session["Entries"] = null;

                HttpContext.Current.Session.Clear();

                return false;
            }

            return true;
        }

        private bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        private string Decrypt(string cipherString)
        {
            if (string.IsNullOrEmpty(cipherString))
            {
                return string.Empty;
            }

            if (!this.IsBase64String(cipherString))
            {
                return string.Empty;
            }

            byte[] keyArray;
            byte[] toEncryptArray;

            try
            {
                //get the byte code of the string
                toEncryptArray = Convert.FromBase64String(cipherString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print("Error de conversión en el token de la sesión: {0}", ex.Message);
                return string.Empty;
            }
            

            //if hashing was not implemented get the byte code of the key
            keyArray = UTF8Encoding.UTF8.GetBytes(this.GetCustomSecurityKey());

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;

            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);

            //Release resources held by TripleDes Encryptor                
            tdes.Clear();

            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (string.IsNullOrEmpty(toEncrypt))
            {
                return string.Empty;
            }

            keyArray = UTF8Encoding.UTF8.GetBytes(this.GetCustomSecurityKey());

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;

            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);

            //Release resources held by TripleDes Encryptor
            tdes.Clear();

            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string GetIp()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? string.Empty;

            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                int le = ipRange.Length - 1;
                string trueIP = ipRange[le];
            }
            else
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;
            }

            System.Diagnostics.Debug.Print("Fijando IP en: {0}", ip);

            return ip;
        }

        private string GetSeed()
        {
            System.Diagnostics.Debug.Print("Estableciendo parte del token usnado a {0}", HttpContext.Current.Request.UserAgent);
            return this.GetIp() + HttpContext.Current.Request.UserAgent + HttpContext.Current.Session["Id"];
        }

        private bool IsValid()
        {
            System.Diagnostics.Debug.Print("Determinando si el token es válido");
            return this.Decrypt((HttpContext.Current.Session["token"] ?? string.Empty).ToString()) == this.GetSeed();
        }

        private string GetCustomSecurityKey()
        {
            System.Diagnostics.Debug.Print("Obteniendo semilla de encriptación...");
            return System.Configuration.ConfigurationManager.AppSettings["SecurityKey"] ?? "q\"w9!CjI(,<1~3x{";
        }

    }
}
