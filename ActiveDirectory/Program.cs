using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Domain, "dis", "DC=dis,DC=dgi,DC=gob,DC=ni", "ggonzalez", "Gedgonz792*6351");
                UserPrincipal insUserPrincipal = new UserPrincipal(insPrincipalContext);
                SearchUsers(insUserPrincipal);
                Console.ReadLine();
            }
            catch (AuthenticationException ae)
            {

                throw;
            }
            //ExisteUsuario("ggonzalez");

            //GetADUsers();
            //var Existe = CheckUserinAD("dis", "ksilva");
            
        }

        public static bool ExisteUsuario(string username)
        {
            // create LDAP connection object  
            string strDomain = "dis";
            string domainAndUsername = strDomain + @"\" + username;
            DirectoryEntry myLdapConnection = createDirectoryEntry(domainAndUsername, "Gedgonz792*");

            // create search object which operates on LDAP connection object  
            // and set search object to only find the user specified  

            DirectorySearcher search = new DirectorySearcher(myLdapConnection);
            //search.Filter = "(cn=" + username + ")";
            search.Filter = "name=" + username;
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("usergroup");
            search.PropertiesToLoad.Add("displayname");//first name
                                                       // SearchResult result;
                                                       // create results objects from search object  

            SearchResult result = search.FindOne();

            if (result != null)
            {
                // user exists, cycle through LDAP fields (cn, telephonenumber etc.)  

                ResultPropertyCollection fields = result.Properties;

                foreach (String ldapField in fields.PropertyNames)
                {
                    // cycle through objects in each field e.g. group membership  
                    // (for many fields there will only be one object such as name)  

                    foreach (Object myCollection in fields[ldapField])
                        Console.WriteLine(String.Format("{0,-20} : {1}",
                                      ldapField, myCollection.ToString()));
                }
            }
            return true;
        }
        public static DirectoryEntry createDirectoryEntry(string domainAndUsername, string pwd)
        {
            var strPath = "LDAP://10.16.210.54/DC=dis,DC=dgi,DC=gob,DC=ni";
            // create and return new LDAP connection with desired settings  
            DirectoryEntry entry = new DirectoryEntry(strPath, domainAndUsername, pwd);
            //DirectoryEntry ldapConnection = new DirectoryEntry("10.16.210.54");
            //entry.Path = "LDAP://10.16.210.54/CN=Users;DC=dis,DC=dgi,DC=gob,DC=ni";
            //ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return entry;
        }

        public static List<Users> GetADUsers()
        {
            try
            {
                List<Users> lstADUsers = new List<Users>();
                string DomainPath = "LDAP://10.16.210.54/DC=dis,DC=dgi,DC=gob,DC=ni";
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = "(&(objectClass=user))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");//first name
                SearchResult result;
                SearchResultCollection resultCol = search.FindAll();
                if (resultCol != null)
                {

                    
                    
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("displayname") && (String)result.Properties["displayname"][0]=="ggonzalez")
                        {
                            Users objSurveyUsers = new Users();
                            objSurveyUsers.Email = (String)result.Properties["mail"][0] +
                              "^" + (String)result.Properties["displayname"][0];
                            objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                            objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];
                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                }
                return lstADUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void SearchUsers(UserPrincipal parUserPrincipal)
        {
            //lbUsers.Items.Clear();
            PrincipalSearcher insPrincipalSearcher = new PrincipalSearcher();
            insPrincipalSearcher.QueryFilter = parUserPrincipal;
            var results = insPrincipalSearcher.FindAll();
            foreach (Principal p in results)
            {
                //lbUsers.Items.Add(p);
            }
        }
        public static bool CheckUserinAD(string domain, string username)
        {
            using (var domainContext = new PrincipalContext(ContextType.Domain, domain,"ggonzalez","Gedgonz792*"))
            {
                using (var user = new UserPrincipal(domainContext))
                {
                    user.SamAccountName = username;

                    using (var pS = new PrincipalSearcher())
                    {
                        pS.QueryFilter = user;

                        using (PrincipalSearchResult<Principal> results = pS.FindAll())
                        {
                            if (results != null && results.Count() > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


    }

    public class Users
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool isMapped { get; set; }
    }
}