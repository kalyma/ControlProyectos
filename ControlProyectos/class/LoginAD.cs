using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ControlProyectos.Class { 
    public class LoginAD
    {
        private string sDomain = System.Configuration.ConfigurationManager.AppSettings["sDomain"];
        private string sDefaultOU = System.Configuration.ConfigurationManager.AppSettings["sDefaultOU"];
        private string DomainLists = System.Configuration.ConfigurationManager.AppSettings["sDomainLists"];
        private string sServiceUser = @System.Configuration.ConfigurationManager.AppSettings["sServiceUser"];
        private string sServicePassword = System.Configuration.ConfigurationManager.AppSettings["sServicePassword"];

        private string sDomainAD = System.Configuration.ConfigurationManager.AppSettings["sDomainAD"];


        public bool ValidateCredentials(string sUserName, string sPassword)
        {
            try
            {
                PrincipalContext oPrincipalContext = GetPrincipalContext();
                return oPrincipalContext.ValidateCredentials(sUserName, sPassword);
            }
            catch (DirectoryServicesCOMException ex)
            {
                return false;
                //throw;
            }
        }

        public PrincipalContext GetPrincipalContext()
        {
            //try
            //{
                sDomain = "red.ecopetrol.com.co";
                sDefaultOU = "DC=red,DC=ecopetrol,DC=com,DC=co";
                PrincipalContext oPrincipalContext = new PrincipalContext(ContextType.Domain, sDomain, sDefaultOU, ContextOptions.SimpleBind);
                return oPrincipalContext;
            //}
            //catch (DirectoryServicesCOMException ex)
            //{
            //    return oPrincipalContext;
            //    //throw;
            //}
        }

        public bool validarLogin(string id, string pass)
        {
            bool authentic = false;

            string adPath = this.sDomainAD;

            //var validar = autenticado(adPath,adPath,adPath);
            //ArrayList gruposDe = new ArrayList();
            //ArrayList proUsuarios = new ArrayList();

            try
            {
                // DirectoryEntry entry = new DirectoryEntry("LDAP://red.ecopetrol.com.co/Contratistas", id, pass);
                //object nativeObject = entry.NativeObject;

                DirectoryEntry entry2 = new DirectoryEntry("LDAP://red.ecopetrol.com.co/OU=Contratistas,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co", id, pass);

                object nativeObject = entry2.NativeObject;
                //LDAP://red.ecopetrol.com.co/Cuentas de Usuario
                //LDAP://red.ecopetrol.com.co/OU=Cuentas de Usuario,OU=Cuentas Deshabilitadas,DC=red,DC=ecopetrol,DC=com,DC=co
                //LDAP://red.ecopetrol.com.co/OU=Funcionarios,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co
                //LDAP://red.ecopetrol.com.co/OU=Contratistas,OU=Colombia,DC=red,DC=ecopetrol,DC=com,DC=co

                // if(entry || entry2)
                authentic = true;

                //return true;
            }
            catch (DirectoryServicesCOMException ex)
            {
                authentic = false;
                //throw;
            }
            return authentic;

        }

    }


}