using Newtonsoft.Json;
using Plantilla_de_Correo.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace ControlProyectos.Class
{

    public class ADMethodsAccountManagement
{
    #region Variables

    private string sDomain = System.Configuration.ConfigurationManager.AppSettings["sDomain"];
    private string sDefaultOU = System.Configuration.ConfigurationManager.AppSettings["sDefaultOU"];
    private string DomainLists = System.Configuration.ConfigurationManager.AppSettings["sDomainLists"];
    private string sServiceUser = @System.Configuration.ConfigurationManager.AppSettings["sServiceUser"];
    private string sServicePassword = System.Configuration.ConfigurationManager.AppSettings["sServicePassword"];

    #endregion
    #region Validate Methods

    /// <summary>
    /// Validates the username and password of a given user
    /// </summary>
    /// <param name="sUserName">The username to validate</param>
    /// <param name="sPassword">The password of the username to validate</param>
    /// <returns>Returns True of user is valid</returns>
    public bool ValidateCredentials(string sUserName, string sPassword)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext();
        return oPrincipalContext.ValidateCredentials(sUserName, sPassword);

    }

    /// <summary>
    /// Checks if the User Account is Expired
    /// </summary>
    /// <param name="sUserName">The username to check</param>
    /// <returns>Returns true if Expired</returns>
    public bool IsUserExpired(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        if (oUserPrincipal.AccountExpirationDate != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Checks if user exsists on AD
    /// </summary>
    /// <param name="sUserName">The username to check</param>
    /// <returns>Returns true if username Exists</returns>
    public bool IsUserExisiting(string sUserName)
    {
        if (GetUser(sUserName) == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public string get_AccountExpirationDate(string samAccountName)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(samAccountName);


            if (oUserPrincipal != null)
            {
                if (oUserPrincipal.AccountExpirationDate != null)
                {
                    return (oUserPrincipal.AccountExpirationDate.Value).AddHours(-5).ToString("yyyy-MM-dd hh:mm:ss");
                }
                else
                {
                    CUtil.enviarCorreo("Error al tomar evidencia fecha de expiración de la cuenta <b>" + samAccountName + "</b>", "rgonzalezc@indracompany.com", "automatizacionBoton@ecopetrol.com.co", "Error Automatización con registro " + samAccountName, "");
                    return "";
                }
            }
            else
            {
                CUtil.enviarCorreo("Error al tomar evidencia fecha de expiración de la cuenta <b>" + samAccountName + "</b>", "rgonzalezc@indracompany.com", "automatizacionBoton@ecopetrol.com.co", "Error Automatización con registro " + samAccountName, "");
                return "";
            }
        }
        catch (Exception ex)
        {

            CUtil.enviarCorreo("Error al tomar evidencia fecha de expiración de la cuenta <b>" + samAccountName + " </ b ><br /><br /> Mensaje: " + ex.Message + "<br /><br /> StackTrace: " + ex.StackTrace, "rgonzalezc@indracompany.com", "automatizacionBoton@ecopetrol.com.co", "Error Automatización con registro " + samAccountName, "");
            CUtil.LogError("Error al tomar fecha de des-vinculación");
            CUtil.LogError("Registro " + samAccountName);
            CUtil.LogError(ex.Message);
            CUtil.LogError(ex.Source);
            CUtil.LogError(ex.StackTrace);
            CUtil.LogError("Target " + ex.TargetSite);
            return "";
        }
    }

    /// <summary>
    /// Mostrar informaci&oacute;n de usuarios
    /// </summary>
    /// <param name="samAccountName">Registro del usuario</param>
    /// <returns>Retorna string con todos los datos del usuario</returns>
    public string infoUser(string samAccountName)
    {
        UserPrincipal oUserPrincipal = GetUser(samAccountName);
        DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
        string info = "";
        string br = "<br />";
        info = "NOMBRE: " + oUserPrincipal.GivenName + br;
        info += "FECHA DE VIGENCIA: " + oUserPrincipal.AccountExpirationDate + br;
        info += "DistinguishedName: " + oUserPrincipal.DistinguishedName + br;
        info += "Description: " + oUserPrincipal.Description + br;
        info += "DisplayName: " + oUserPrincipal.DisplayName + br;
        info += "EmailAddress: " + oUserPrincipal.EmailAddress + br;
        info += "GivenName: " + oUserPrincipal.GivenName + br;
        info += "HomeDirectory: " + oUserPrincipal.HomeDirectory + br;
        info += "HomeDrive: " + oUserPrincipal.HomeDrive + br;
        info += "LastBadPasswordAttempt: " + oUserPrincipal.LastBadPasswordAttempt + br;
        info += "LastLogon: " + oUserPrincipal.LastLogon + br;
        info += "LastPasswordSet: " + oUserPrincipal.LastPasswordSet + br;
        info += "UserPrincipalName: " + oUserPrincipal.UserPrincipalName + br;
        info += "VoiceTelephoneNumber: " + oUserPrincipal.VoiceTelephoneNumber + br;
        info += "Company: " + ent.Properties["wWWHomePage"].Value + br;
        info += br + br + br + "propiedades" + br + br;

        //ent.Properties["wWWHomePage"].Value = "Deshabilitado por expiracion en 2015-06-02";
        foreach (System.DirectoryServices.PropertyValueCollection obj in ent.Properties)
        {
            info += obj.PropertyName + " : " + obj.Value + "<br />";
        }

        /*     //ent.CommitChanges();




        DirectoryEntry domainRoot = new DirectoryEntry("LDAP://red.ecopetrol.com.co/OU=Cuentas de Usuario,OU=Cuentas Deshabilitadas,DC=red,DC=ecopetrol,DC=com,DC=co");
        domainRoot.Username = "Ecopetrol\\_DesbloqueoUsuarios";
        domainRoot.Password = "Ecopetrol2015";

        // set up directory searcher based on default naming context entry
        DirectorySearcher ouSearcher = new DirectorySearcher(domainRoot);

        ouSearcher.SearchScope = SearchScope.OneLevel;

        ouSearcher.PropertiesToLoad.Add("ou");
        ouSearcher.Filter = "(objectCategory=organizationalUnit)";
        info += "<BR /><BR /><BR /><BR />";

        foreach (SearchResult deResult in ouSearcher.FindAll())
        {
            string ouName = deResult.Properties["ou"][0].ToString();

            DirectoryEntry domainRoot2 = new DirectoryEntry(deResult.Path);
            domainRoot2.Username = "Ecopetrol\\_DesbloqueoUsuarios";
            domainRoot2.Password = "Ecopetrol2015";
            DirectorySearcher ouSearcher2 = new DirectorySearcher(domainRoot2);
            ouSearcher2.SearchScope = SearchScope.OneLevel;
            ouSearcher2.PropertiesToLoad.Add("ou");
            ouSearcher2.Filter = "(objectCategory=organizationalUnit)";
            foreach (SearchResult deResult2 in ouSearcher2.FindAll())
            {

                    info += "<b>" + deResult2.Path + "</b><br />";

            }
        }


        // do search and iterate over results
        /*foreach (SearchResult deResult in ouSearcher.FindAll())
        {
            string ouName = deResult.Properties["ou"][0].ToString();
            info += "<b>"+ deResult.Path + "</b><br />";
            DirectoryEntry domainRoot2 = new DirectoryEntry(deResult.Path);
            domainRoot2.Username = "Ecopetrol\\_DesbloqueoUsuarios";
            domainRoot2.Password = "Ecopetrol2015";
            DirectorySearcher ouSearcher2 = new DirectorySearcher(domainRoot2);
            ouSearcher2.SearchScope = SearchScope.OneLevel;
            ouSearcher2.PropertiesToLoad.Add("ou");
            ouSearcher2.Filter = "(objectCategory=organizationalUnit)";
            foreach (SearchResult deResult2 in ouSearcher2.FindAll())
            {
                info +=  deResult2.Path + "<br />";
                 DirectoryEntry domainRoot3 = new DirectoryEntry(deResult2.Path);
            domainRoot3.Username = "Ecopetrol\\_DesbloqueoUsuarios";
            domainRoot3.Password = "Ecopetrol2015";
            DirectorySearcher ouSearcher3 = new DirectorySearcher(domainRoot3);
            ouSearcher3.SearchScope = SearchScope.OneLevel;
            ouSearcher3.PropertiesToLoad.Add("ou");
            ouSearcher3.Filter = "(objectCategory=organizationalUnit)";
                foreach (SearchResult deResult3 in ouSearcher3.FindAll())
                {
                    info +=  deResult3.Path + "<br />";

                    DirectoryEntry domainRoot4 = new DirectoryEntry(deResult3.Path);
                    domainRoot4.Username = "Ecopetrol\\_DesbloqueoUsuarios";
                    domainRoot4.Password = "Ecopetrol2015";
                    DirectorySearcher ouSearcher4 = new DirectorySearcher(domainRoot4);
                    ouSearcher4.SearchScope = SearchScope.OneLevel;
                    ouSearcher4.PropertiesToLoad.Add("ou");
                    ouSearcher4.Filter = "(objectCategory=organizationalUnit)";
                    foreach (SearchResult deResult4 in ouSearcher4.FindAll())
                    {
                        info += deResult4.Path + "<br />";


                    }
                }
            }



        }
       */
        return info;
    }


    public string Buscarcorreo(string samAccountName)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(samAccountName);
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
            string info = "";

            info = oUserPrincipal.Name;


            return info;
        }
        catch
        {

            return "Registro no encontrado";
        }
    }




    /// <summary>
    ///  Obtiene una lista de las unidades organizacionales de Funcionarios y Contratistas
    /// </summary>
    /// <returns>Retorna un ListItemCollection de las unidades Organizacionales</returns>

    public DropDownList GetListOU(DropDownList ddl)
    {
        ListItemCollection listItemCollectionOU = new ListItemCollection();
        ddl.Items.Add(new ListItem("Seleccionar OU", "-1"));
        DirectoryEntry domainRoot = new DirectoryEntry("LDAP://red.ecopetrol.com.co/OU=Funcionarios,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co");
        domainRoot.Username = sServiceUser;
        domainRoot.Password = sServicePassword;
        DirectorySearcher ouSearcher = new DirectorySearcher(domainRoot);
        ouSearcher.SearchScope = SearchScope.OneLevel;
        ouSearcher.PropertiesToLoad.Add("ou");
        ouSearcher.Filter = "(objectCategory=organizationalUnit)";


        foreach (SearchResult deResult in ouSearcher.FindAll())
        {
            string ouName = deResult.Properties["ou"][0].ToString();

            DirectoryEntry domainRoot2 = new DirectoryEntry(deResult.Path);
            domainRoot2.Username = sServiceUser;
            domainRoot2.Password = sServicePassword;
            DirectorySearcher ouSearcher2 = new DirectorySearcher(domainRoot2);
            ouSearcher2.SearchScope = SearchScope.OneLevel;
            ouSearcher2.PropertiesToLoad.Add("ou");
            ouSearcher2.Filter = "(objectCategory=organizationalUnit)";
            foreach (SearchResult deResult2 in ouSearcher2.FindAll())
            {
                if (deResult2.Properties["ou"][0].ToString() == "Usuarios")
                {

                    ddl.Items.Add(new ListItem("Funcionarios " + deResult.Properties["ou"][0].ToString(), deResult2.Path));
                }
            }
        }


        domainRoot = new DirectoryEntry("LDAP://red.ecopetrol.com.co/OU=Contratistas,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co");
        domainRoot.Username = sServiceUser;
        domainRoot.Password = sServicePassword;
        ouSearcher = new DirectorySearcher(domainRoot);
        ouSearcher.SearchScope = SearchScope.OneLevel;
        ouSearcher.PropertiesToLoad.Add("ou");
        ouSearcher.Filter = "(objectCategory=organizationalUnit)";


        foreach (SearchResult deResult in ouSearcher.FindAll())
        {
            string ouName = deResult.Properties["ou"][0].ToString();

            DirectoryEntry domainRoot2 = new DirectoryEntry(deResult.Path);
            domainRoot2.Username = sServiceUser;
            domainRoot2.Password = sServicePassword;
            DirectorySearcher ouSearcher2 = new DirectorySearcher(domainRoot2);
            ouSearcher2.SearchScope = SearchScope.OneLevel;
            ouSearcher2.PropertiesToLoad.Add("ou");
            ouSearcher2.Filter = "(objectCategory=organizationalUnit)";
            foreach (SearchResult deResult2 in ouSearcher2.FindAll())
            {
                if (deResult2.Properties["ou"][0].ToString() == "Usuarios")
                {
                    ddl.Items.Add(new ListItem("Contratistas " + deResult.Properties["ou"][0].ToString(), deResult2.Path));
                }
            }
        }

        return ddl;
    }

    /// <summary>
    /// Checks if user accoung is locked
    /// </summary>
    /// <param name="sUserName">The username to check</param>
    /// <returns>Retruns true of Account is locked</returns>
    public bool IsAccountLocked(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        return oUserPrincipal.IsAccountLockedOut();
    }
    #endregion

    #region Search Methods

    /// <summary>
    /// Gets a certain user on Active Directory
    /// </summary>
    /// <param name="sUserName">The username to get</param>
    /// <returns>Returns the UserPrincipal Object</returns>
    public UserPrincipal GetUser(string sUserName)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext();

        UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);

        return oUserPrincipal;
    }


    public UserPrincipal GetUser(string sUserName, string Domain)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContextByDomain(Domain);

        UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);

        return oUserPrincipal;
    }



    public string GetUserByCedula(string sCedula)
    {

        string userName = "";
        PrincipalContext oPrincipalContext = GetPrincipalContext();

        DirectoryEntry domainRoot = new DirectoryEntry("LDAP://red.ecopetrol.com.co/OU=Funcionarios,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co");

        domainRoot.Username = sServiceUser;
        domainRoot.Password = sServicePassword;

        // set up directory searcher based on default naming context entry
        DirectorySearcher ouSearcher = new DirectorySearcher(domainRoot);

        //ouSearcher.SearchScope = SearchScope.OneLevel;

        //ouSearcher.PropertiesToLoad.Add("postalCode");
        ouSearcher.Filter = "(EmailAddress ='" + sCedula + "')";

        foreach (SearchResult deResult in ouSearcher.FindAll())
        {


            return deResult.Properties["SamAccountName"][0].ToString();

        }
        ouSearcher.Dispose();
        return null;
    }


    /// <summary>
    /// Gets a certain group on Active Directory
    /// </summary>
    /// <param name="sGroupName">The group to get</param>
    /// <returns>Returns the GroupPrincipal Object</returns>
    public GroupPrincipal GetGroup(string sGroupName)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext();

        GroupPrincipal oGroupPrincipal = GroupPrincipal.FindByIdentity(oPrincipalContext, sGroupName);
        return oGroupPrincipal;
    }

    #endregion

    #region User Account Methods

    /// <summary>
    /// Sets the user password
    /// </summary>
    /// <param name="sUserName">The username to set</param>
    /// <param name="sNewPassword">The new password to use</param>
    /// <param name="sMessage">Any output messages</param>
    public void SetUserPassword(string sUserName, string sNewPassword, out string sMessage)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.SetPassword(sNewPassword);
            sMessage = "";
        }
        catch (Exception ex)
        {
            sMessage = ex.Message;
        }

    }


    /// <summary>
    /// Change the user password
    /// </summary>
    /// <param name="sUserName">The username to set</param>
    /// <param name="sNewPassword">The new password to use</param>        
    public void ChangeUserPassword(string sUserName, string sNewPassword)
    {
        try
        {
            string[] domains = DomainLists.Split(',');
            foreach (string domain in domains)
            {
                try
                {
                    UserPrincipal oUserPrincipal = GetUser(sUserName, domain);
                    oUserPrincipal.SetPassword(sNewPassword);
                    oUserPrincipal.ExpirePasswordNow();
                }
                catch (Exception ex)
                {

                    CUtil.LogError("Error cambiarPassWord " + sUserName);
                    CUtil.LogError("Controlador de Dominio" + domain);
                    CUtil.LogError(ex.Message);
                    CUtil.LogError(ex.StackTrace);
                    CUtil.LogError(ex.Source);
                }
            }

        }
        catch (Exception ex)
        {

        }

    }

    public string updateFecha(string sUserName, string newDate, string hora, string SDid)
    {
        try
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            // 
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");

            //string fecha = oUserPrincipal.AccountExpirationDate.Value.ToString();
            DateTime dt = new DateTime();

            dt = Convert.ToDateTime(newDate);
            if (hora == null)
            {
                dt = dt.AddDays(1);
                dt = dt.AddHours(4);
                dt = dt.AddMinutes(59);
                dt = dt.AddSeconds(59);
            }
            else
            {
                int h = int.Parse(hora);

                dt = dt.AddHours(h + 5);
            }



            string[] domains = DomainLists.Split(',');
            foreach (string domain in domains)
            {
                try
                {
                    UserPrincipal oUserPrincipal_ = GetUser(sUserName, domain);

                    oUserPrincipal_.AccountExpirationDate = dt;
                    oUserPrincipal_.Save();
                }
                catch (Exception ex)
                {
                    CUtil.LogError("Error Desbloqueo de usuario " + sUserName);
                    CUtil.LogError("Controlador de Dominio " + domain);
                    CUtil.LogError(ex.Message);
                    CUtil.LogError(ex.StackTrace);
                    CUtil.LogError(ex.Source);
                }
            }




            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);

            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);

            ent.Properties["extensionAttribute15"].Value += ";Boton autogestión No." + SDid;

            ent.CommitChanges();
            return sUserName + " Nueva Fecha " + newDate;
        }
        catch (Exception e)
        {
            //  return sUserName + " " + e.Message.ToString();
        }
        return "";
    }


    public DatosUser datosUser(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        string nombre = "";
        string correo = "";
        string cedula = "";
        string fecha = "";
        if (oUserPrincipal != null)
        {
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);


            try
            {
                nombre = oUserPrincipal.DisplayName;
            }
            catch { }

            try
            {
                correo = oUserPrincipal.EmailAddress;
            }
            catch { }

            try
            {
                cedula = ent.Properties["postOfficeBox"].Value.ToString();
            }
            catch { }

            try
            {
                fecha = oUserPrincipal.AccountExpirationDate.Value.AddDays(-1).ToString("dd/MM/yyyy");
            }
            catch { }
        }

        string json = "{\"nombre\": \"" + nombre + "\", \"correo\": \"" + correo + "\", \"cedula\": \"" + cedula + "\", \"fechaVigencia\": \"" + fecha + "\"}";




            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        DatosUser user = (DatosUser)json_serializer.Deserialize<DatosUser>(json);
        return user;
    }

    public string GetName(string sUserName)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext();
        UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
        try
        {
            if (oUserPrincipal != null) return oUserPrincipal.DisplayName.ToString();
        }
        catch { return ""; }

        return "";
    }

    public string updateFecha(string sUserName, string newDate, string docid, string ceco, string cargo, string dependencia, string tipo, string extension, string ubicacion, string empresa, string autorizado)
    {
        try
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");

            string fecha = oUserPrincipal.AccountExpirationDate.Value.ToString();
            DateTime dt = new DateTime();

            dt = Convert.ToDateTime(newDate);
            oUserPrincipal.AccountExpirationDate = dt.AddDays(2);
            oUserPrincipal.Save();


            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
            ent.Properties["wWWHomePage"].Value = " ";
            ent.Properties["postOfficeBox"].Value = docid;
            ent.Properties["postalCode"].Value = ceco;
            ent.Properties["title"].Value = cargo;
            ent.Properties["department"].Value = dependencia;
            ent.Properties["extensionAttribute5"].Value = tipo;
            ent.Properties["telephoneNumber"].Value = extension;
            ent.Properties["physicalDeliveryOfficeName"].Value = ubicacion;
            ent.Properties["company"].Value = empresa;
            //falta el autorizador

            UserPrincipal userManager = UserPrincipal.FindByIdentity(oPrincipalContext, autorizado);
            DirectoryEntry userManager_DE = (userManager.GetUnderlyingObject() as DirectoryEntry);
            ent.Properties["manager"].Value = userManager_DE.Properties["distinguishedName"].Value;

            ent.CommitChanges();
            return sUserName + " Nueva Fecha " + newDate;
        }
        catch (Exception e)
        {
            return sUserName + " " + e.Message.ToString();
        }
    }

    public void evidenciaUpdate(string samAccountName, string sd)
    {
        string ruta_evidencia_DA = System.Configuration.ConfigurationManager.AppSettings["Ruta_evidencia"] + "\\" + sd + "_" + samAccountName + ".txt";
        try
        {

            UserPrincipal oUserPrincipal = GetUser(samAccountName);
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);

            if (ent == null)
            {
                System.IO.StreamWriter sw = new StreamWriter(ruta_evidencia_DA, true);
                sw.WriteLine("Usuario no encontrado en directorio activo");
                sw.Close();
            }
            else
            {
                string ruta = System.Configuration.ConfigurationManager.AppSettings["Ruta_local"];
                string ruta_evidencia = HttpContext.Current.Server.MapPath("\\Evidencia") + "\\";
                CUtil.ExecuteCommandAsAdmin(ruta + "\\repad3.bat \"" + oUserPrincipal.DistinguishedName + "\" " + ruta_evidencia + sd + "_" + samAccountName + ".txt " + samAccountName);
                Thread.Sleep(10000);
            }
        }
        catch
        {
            System.IO.StreamWriter sw = new StreamWriter(ruta_evidencia_DA, true);
            sw.WriteLine("Error al tomar evidencia del directorio activo");
            sw.Close();
        }


        string Archivo = System.Configuration.ConfigurationManager.AppSettings["Ruta_evidencia"] + "\\plantillas_evidencias.html";
        string Archivo_evidencia = System.Configuration.ConfigurationManager.AppSettings["Ruta_evidencia"] + "\\" + sd + "_" + samAccountName + ".html";
        if (System.IO.File.Exists(Archivo))
        {
            try
            {
                System.IO.File.Copy(Archivo, Archivo_evidencia);


                System.IO.TextReader sw = new StreamReader(ruta_evidencia_DA, false);
                string texto_evidencia = sw.ReadToEnd();
                sw.Close();
                string texto_plantilla = System.IO.File.ReadAllText(Archivo_evidencia);
                CUtil.LogEvent("Ruta Evidencia DA " + ruta_evidencia_DA);
                CUtil.LogEvent("Evidencia " + texto_evidencia);
                texto_plantilla = texto_plantilla.Replace("#evidencia_da#", texto_evidencia);
                System.IO.File.WriteAllText(Archivo_evidencia, texto_plantilla);

                System.IO.File.Delete(ruta_evidencia_DA);
            }
            catch (Exception ex) { CUtil.LogError(ex.Message); }
        }
    }

    public string reactivarUser(string sUserName, string fecha, string ou)
    {
        try
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
            string fecha_ant = oUserPrincipal.AccountExpirationDate.Value.ToString();

            DirectoryEntry newOU = new DirectoryEntry(ou);
            newOU.Username = sServiceUser;
            newOU.Password = sServicePassword;

            //mover usuarios de OU
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
            ent.MoveTo(newOU);
            ent.Properties["wWWHomePage"].Value = " ";
            ent.CommitChanges();
            oUserPrincipal.Enabled = true;
            DateTime dt = new DateTime();
            dt = Convert.ToDateTime(fecha);
            oUserPrincipal.AccountExpirationDate = dt.AddDays(2);
            try
            {
                oUserPrincipal.UnlockAccount();
            }
            catch { }
            this.AddUserToGroup(oUserPrincipal.SamAccountName, System.Configuration.ConfigurationManager.AppSettings["grupo_reactivaciones"]);
            oUserPrincipal.Save();
            return sUserName + " Activado";
        }
        catch (Exception e)
        {
            return sUserName + " " + e.Message.ToString();
        }
    }

    public string reactivarUser(string sUserName, string fecha, string ou, string docid, string ceco, string cargo, string dependencia, string tipo, string extension, string ubicacion, string empresa, string autorizado)
    {
        try
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
            string fecha_ant = oUserPrincipal.AccountExpirationDate.Value.ToString();

            DirectoryEntry newOU = new DirectoryEntry(ou);
            newOU.Username = sServiceUser;
            newOU.Password = sServicePassword;

            //mover usuarios de OU
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
            ent.MoveTo(newOU);
            ent.Properties["wWWHomePage"].Value = " ";
            ent.Properties["postOfficeBox"].Value = docid;
            ent.Properties["postalCode"].Value = ceco;
            ent.Properties["title"].Value = cargo;
            ent.Properties["department"].Value = dependencia;
            ent.Properties["extensionAttribute5"].Value = tipo;
            ent.Properties["telephoneNumber"].Value = extension;
            ent.Properties["physicalDeliveryOfficeName"].Value = ubicacion;
            ent.Properties["company"].Value = empresa;
            //falta el autorizador

            UserPrincipal userManager = UserPrincipal.FindByIdentity(oPrincipalContext, autorizado);
            DirectoryEntry userManager_DE = (userManager.GetUnderlyingObject() as DirectoryEntry);
            ent.Properties["manager"].Value = userManager_DE.Properties["distinguishedName"].Value;

            ent.CommitChanges();
            oUserPrincipal.Enabled = true;
            DateTime dt = new DateTime();
            dt = Convert.ToDateTime(fecha);
            oUserPrincipal.AccountExpirationDate = dt.AddDays(2);
            try
            {
                oUserPrincipal.UnlockAccount();
            }
            catch { }
                //this.AddUserToGroup(oUserPrincipal.SamAccountName, ConfigurationSettings.AppSettings["grupo_reactivaciones"]);
                this.AddUserToGroup(oUserPrincipal.SamAccountName, System.Configuration.ConfigurationManager.AppSettings["grupo_reactivaciones"]);
                
            oUserPrincipal.Save();
            return sUserName + " Activado";
        }
        catch (Exception e)
        {
            return sUserName + " " + e.Message.ToString();
        }
    }

    public string deleteUser(string sUserName)
    {
        try
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            DateTime dt = DateTime.Now;
            string path = System.Configuration.ConfigurationManager.AppSettings["directoryEntry_Funcionarios"];
            if (sUserName.Trim().Substring(0, 1).ToUpper() == "C")
            {
                path = System.Configuration.ConfigurationManager.AppSettings["directoryEntry_Contratistas"];
            }
            DirectoryEntry newOU = new DirectoryEntry(path);
            newOU.Username = sServiceUser;
            newOU.Password = sServicePassword;

            //mover usuarios de OU
            DirectoryEntry ent = (oUserPrincipal.GetUnderlyingObject() as DirectoryEntry);
            ent.MoveTo(newOU);
            ent.Properties["wWWHomePage"].Value = "Deshabilitado por solicitud en " + DateTime.Now;
            ent.CommitChanges();
            oUserPrincipal.AccountExpirationDate = dt.AddDays(-1);
            oUserPrincipal.Enabled = false;

            try
            {
                this.RemoveUserFromGroup(oUserPrincipal.SamAccountName, System.Configuration.ConfigurationManager.AppSettings["grupo_reactivaciones"]);
            }
            catch { }
            oUserPrincipal.Save();

            return sUserName + " Desactivado";
        }
        catch (Exception e)
        {
            return sUserName + " " + e.Message.ToString();
        }
    }

    /// <summary>
    /// Enables a disabled user account
    /// </summary>
    /// <param name="sUserName">The username to enable</param>
    public void EnableUserAccount(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        oUserPrincipal.Enabled = true;
        oUserPrincipal.Save();
    }

    /// <summary>
    /// Force disbaling of a user account
    /// </summary>
    /// <param name="sUserName">The username to disable</param>
    public void DisableUserAccount(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        oUserPrincipal.Enabled = false;
        oUserPrincipal.Save();
    }

    /// <summary>
    /// Force expire password of a user
    /// </summary>
    /// <param name="sUserName">The username to expire the password</param>
    public void ExpireUserPassword(string sUserName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        oUserPrincipal.ExpirePasswordNow();
        oUserPrincipal.Save();

    }

    /// <summary>
    /// Unlocks a locked user account
    /// </summary>
    /// <param name="sUserName">The username to unlock</param>
    public void UnlockUserAccount(string sUserName)
    {
        string[] domains = DomainLists.Split(',');
        foreach (string domain in domains)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName, domain);
                oUserPrincipal.UnlockAccount();
                oUserPrincipal.Save();
            }
            catch (Exception ex)
            {
                CUtil.LogError("Error Desbloqueo de usuario " + sUserName);
                CUtil.LogError("Controlador de Dominio " + domain);
                CUtil.LogError(ex.Message);
                CUtil.LogError(ex.StackTrace);
                CUtil.LogError(ex.Source);
            }
        }
    }

    /// <summary>
    /// Creates a new user on Active Directory
    /// </summary>
    /// <param name="sOU">The OU location you want to save your user</param>
    /// <param name="sUserName">The username of the new user</param>
    /// <param name="sPassword">The password of the new user</param>
    /// <param name="sGivenName">The given name of the new user</param>
    /// <param name="sSurname">The surname of the new user</param>
    /// <returns>returns the UserPrincipal object</returns>
    public UserPrincipal CreateNewUser(string sOU, string sUserName, string sPassword, string sGivenName, string sSurname)
    {
        if (!IsUserExisiting(sUserName))
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext(sOU);

            UserPrincipal oUserPrincipal = new UserPrincipal(oPrincipalContext, sUserName, sPassword, true /*Enabled or not*/);

            //User Log on Name
            oUserPrincipal.UserPrincipalName = sUserName;
            oUserPrincipal.GivenName = sGivenName;
            oUserPrincipal.Surname = sSurname;
            oUserPrincipal.Save();

            return oUserPrincipal;
        }
        else
        {
            return GetUser(sUserName);
        }
    }

    /// <summary>
    /// Deletes a user in Active Directory
    /// </summary>
    /// <param name="sUserName">The username you want to delete</param>
    /// <returns>Returns true if successfully deleted</returns>
    public bool DeleteUser(string sUserName)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);

            oUserPrincipal.Delete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Group Methods

    /// <summary>
    /// Creates a new group in Active Directory
    /// </summary>
    /// <param name="sOU">The OU location you want to save your new Group</param>
    /// <param name="sGroupName">The name of the new group</param>
    /// <param name="sDescription">The description of the new group</param>
    /// <param name="oGroupScope">The scope of the new group</param>
    /// <param name="bSecurityGroup">True is you want this group to be a security group, false if you want this as a distribution group</param>
    /// <returns>Retruns the GroupPrincipal object</returns>
    public GroupPrincipal CreateNewGroup(string sOU, string sGroupName, string sDescription, GroupScope oGroupScope, bool bSecurityGroup)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext(sOU);

        GroupPrincipal oGroupPrincipal = new GroupPrincipal(oPrincipalContext, sGroupName);
        oGroupPrincipal.Description = sDescription;
        oGroupPrincipal.GroupScope = oGroupScope;
        oGroupPrincipal.IsSecurityGroup = bSecurityGroup;
        oGroupPrincipal.Save();

        return oGroupPrincipal;
    }

    /// <summary>
    /// Adds the user for a given group
    /// </summary>
    /// <param name="sUserName">The user you want to add to a group</param>
    /// <param name="sGroupName">The group you want the user to be added in</param>
    /// <returns>Returns true if successful</returns>
    public bool AddUserToGroup(string sUserName, string sGroupName)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (!IsUserGroupMember(sUserName, sGroupName))
                {
                    oGroupPrincipal.Members.Add(oUserPrincipal);
                    oGroupPrincipal.Save();
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes user from a given group
    /// </summary>
    /// <param name="sUserName">The user you want to remove from a group</param>
    /// <param name="sGroupName">The group you want the user to be removed from</param>
    /// <returns>Returns true if successful</returns>
    public bool RemoveUserFromGroup(string sUserName, string sGroupName)
    {
        try
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (IsUserGroupMember(sUserName, sGroupName))
                {
                    oGroupPrincipal.Members.Remove(oUserPrincipal);
                    oGroupPrincipal.Save();
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if user is a member of a given group
    /// </summary>
    /// <param name="sUserName">The user you want to validate</param>
    /// <param name="sGroupName">The group you want to check the membership of the user</param>
    /// <returns>Returns true if user is a group member</returns>
    public bool IsUserGroupMember(string sUserName, string sGroupName)
    {
        UserPrincipal oUserPrincipal = GetUser(sUserName);
        GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);

        if (oUserPrincipal != null && oGroupPrincipal != null)
        {
            bool resultado = oGroupPrincipal.Members.Contains(oUserPrincipal);
            //if (resultado) CUtil.LogEvent(sUserName + " pertenece al grupo " + sGroupName);
            return resultado;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Checks if user is a member of a given group
    /// </summary>
    /// <param name="sUserName">The user you want to validate</param>
    /// <param name="sGroupName">The group you want to check the membership of the user</param>
    /// <returns>Returns true if user is a group member</returns>
    public bool IsUserGroupMemberByJWT(string jwt, string sGroupName)
    {

        Char[] separator = { '.' };
        string[] parts = jwt.Split(separator);
        string payload = parts[1];

        DatosUser datosUser = null;
        string json_payload = CUtil.Base64Decode_Iso_8859_1(payload);
        datosUser = JsonConvert.DeserializeObject<DatosUser>(json_payload);

        UserPrincipal oUserPrincipal = GetUser(datosUser.registro);
        GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);

        if (oUserPrincipal != null && oGroupPrincipal != null)
        {
            bool resultado = oGroupPrincipal.Members.Contains(oUserPrincipal);
            //if (resultado) CUtil.LogEvent(sUserName + " pertenece al grupo " + sGroupName);
            return resultado;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Gets a list of the users group memberships
    /// </summary>
    /// <param name="sUserName">The user you want to get the group memberships</param>
    /// <returns>Returns an arraylist of group memberships</returns>
    public ArrayList GetUserGroups(string sUserName)
    {
        ArrayList myItems = new ArrayList();
        UserPrincipal oUserPrincipal = GetUser(sUserName);

        PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetGroups();

        foreach (Principal oResult in oPrincipalSearchResult)
        {
            myItems.Add(oResult.Name);
        }
        return myItems;
    }

    /// <summary>
    /// Gets a list of the users authorization groups
    /// </summary>
    /// <param name="sUserName">The user you want to get authorization groups</param>
    /// <returns>Returns an arraylist of group authorization memberships</returns>
    public ArrayList GetUserAuthorizationGroups(string sUserName)
    {
        ArrayList myItems = new ArrayList();
        UserPrincipal oUserPrincipal = GetUser(sUserName);

        PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetAuthorizationGroups();

        foreach (Principal oResult in oPrincipalSearchResult)
        {
            myItems.Add(oResult.Name);
        }
        return myItems;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the base principal context
    /// </summary>
    /// <returns>Retruns the PrincipalContext object</returns>
    public PrincipalContext GetPrincipalContext()
    {
        PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Domain, sDomain, sDefaultOU, ContextOptions.SimpleBind, sServiceUser, sServicePassword);
        return oPrincipalContext;
    }


    /// <summary>
    /// Gets the base principal context
    /// </summary>
    /// <returns>Retruns the PrincipalContext object</returns>
    public PrincipalContext GetPrincipalContextByDomain(string domain)
    {
        PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Domain, domain, sDefaultOU, ContextOptions.SimpleBind, sServiceUser, sServicePassword);
        return oPrincipalContext;
    }


    /// <summary>
    /// Gets the principal context on specified OU
    /// </summary>
    /// <param name="sOU">The OU you want your Principal Context to run on</param>
    /// <returns>Retruns the PrincipalContext object</returns>
    public PrincipalContext GetPrincipalContext(string sOU)
    {
        PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Domain, sDomain, sOU, ContextOptions.SimpleBind, sServiceUser, sServicePassword);
        return oPrincipalContext;
    }

    #endregion
    public string listarUsuarios(string fecha_ini, string fecha_fin)
    {
        PrincipalContext oPrincipalContext = GetPrincipalContext();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
        string html = "<table>";
        html += "<tr>";
        html += "<td>Cuenta</td>";
        html += "<td>Nombre</td>";
        html += "<td>Responsable</td>";
        html += "<td>Empresa</td>";
        html += "<td>Registro Último aprobador</td>";
        html += "<td>Nombre Último aprobador</td>";
        html += "<td>SD</td>";
        html += "<td>Fecha Aprobaci&oacute;n</td>";
        html += "<td>Vencimiento</td>";
        html += "</tr>";

        DateTime dt_ini = new DateTime();
        dt_ini = Convert.ToDateTime(fecha_ini);
        DateTime dt_fin = new DateTime();
        dt_fin = Convert.ToDateTime(fecha_fin);
        dt_fin = dt_fin.AddDays(1);




        PrincipalSearchResult<UserPrincipal> oPrincipalSearchResult = UserPrincipal.FindByExpirationTime(oPrincipalContext, dt_ini, MatchType.GreaterThanOrEquals);
        List<UserPrincipal> usuarios = oPrincipalSearchResult.Where(x => x.AccountExpirationDate <= dt_fin /*&& x.SamAccountName.Substring(0,1).ToUpper() == "C" */).OrderBy(x => x.AccountExpirationDate).ToList();


        foreach (UserPrincipal oResult in usuarios)
        {
            if (oResult.AccountExpirationDate != null && oResult.AccountExpirationDate <= dt_fin)
            {

                SqlConnection sqlConnection1 = new SqlConnection(ConfigurationManager.ConnectionStrings["Conn_ServiceManager"].ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "SELECT INCIDENT_ID, TI_IMAC_APROADMIN, TI_IMAC_APROADMINNAME, OPEN_TIME FROM INCIDENTSM1 ";
                cmd.CommandText += "WHERE TI_IMAC_SUBCATEGORIA LIKE 'Gesti&oacute;n de Accesos (Red, Correo, Internet, Oficina Virtual y Lync)' ";
                cmd.CommandText += "AND CONTACT_NAME LIKE '" + oResult.SamAccountName + "' ";
                cmd.CommandText += "AND TI_IMAC_APROADMIN IS NOT NULL ";
                cmd.CommandText += "ORDER BY OPEN_TIME DESC";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();






                DirectoryEntry ent = (oResult.GetUnderlyingObject() as DirectoryEntry);
                html += "<tr>";
                html += "<td>" + oResult.SamAccountName + "</td>";
                html += "<td>" + oResult.DisplayName + "</td>";
                string responsable = "";
                if (ent.Properties["Manager"].Value != null)
                {
                    responsable = ent.Properties["Manager"].Value.ToString();
                    string[] split = responsable.Split(new Char[] { ',' });
                    responsable = split[0].Substring(3);

                }
                html += "<td>" + responsable + "</td>";
                html += "<td>" + ent.Properties["company"].Value + "</td>";
                if (reader.Read())
                {
                    html += "<td>" + reader["TI_IMAC_APROADMIN"] + "</td>";
                    try
                    {
                        UserPrincipal aprobador = this.GetUser(reader["TI_IMAC_APROADMIN"].ToString());
                        html += "<td>" + aprobador.DisplayName + "</td>";
                    }
                    catch
                    {
                        html += "<td>" + reader["TI_IMAC_APROADMINNAME"] + "</td>";
                    }
                    html += "<td>" + reader["INCIDENT_ID"] + "</td>";
                    html += "<td>" + reader["OPEN_TIME"] + "</td>";
                }
                else
                {
                    html += "<td> </td>";
                    html += "<td> </td>";
                    html += "<td> </td>";
                    html += "<td> </td>";
                }
                html += "<td>" + oResult.AccountExpirationDate.Value.ToString().Substring(0, 10) + "</td>";
                html += "</tr>";
                sqlConnection1.Close();
            }

        }



        html += "</table>";
        return html;
    }


}

}