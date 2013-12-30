using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace ADManager.Models
{
    public class UserPrincipalExHelper
    {
        private static readonly string sDomain = ConfigurationManager.AppSettings["domain"];
        private static readonly string sDefaultOU = ConfigurationManager.AppSettings["default_ou"];
        private static readonly string sDefaultRootOU = ConfigurationManager.AppSettings["default_root_ou"];
        private static readonly string sServiceUser = ConfigurationManager.AppSettings["domain_user"];
        private static readonly string sServicePassword = ConfigurationManager.AppSettings["domain_user_password"];

        public static readonly string[] RegionGroups = ConfigurationManager.AppSettings["region_groups"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        public static readonly string[] DivisionGroups = ConfigurationManager.AppSettings["division_groups"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        public static UserPrincipalEx GetUser(string account)
        {
            UserPrincipalEx user = null;
            PrincipalContext ctx = GetPrincipalContext();
            user = UserPrincipalEx.FindByIdentity(ctx, IdentityType.SamAccountName, account);
            return user;
        }

        public static IList<UserPrincipalEx> GetUsers()
        {
            IList<UserPrincipalEx> list = new List<UserPrincipalEx>();
            PrincipalContext ctx = GetPrincipalContext();
            UserPrincipalEx ex = new UserPrincipalEx(ctx);

            PrincipalSearcher search = new PrincipalSearcher(ex);
            foreach (UserPrincipalEx e in search.FindAll())
            {
                list.Add(e);
            }
            return list;
        }

        public static bool UserInGroup(string user, string group)
        {
            PrincipalContext ctx = GetPrincipalContext();
            GroupPrincipal g = GroupPrincipal.FindByIdentity(ctx, group);
            UserPrincipal u = UserPrincipal.FindByIdentity(ctx, user);
            return ((u != null) && (g != null) && u.IsMemberOf(g));
        }

        public static UserPrincipalEx GetManager(string account)
        {
            UserPrincipalEx manager = null;
            UserPrincipalEx user = GetUser(account);
            PrincipalContext ctx = GetPrincipalContext();
            manager = UserPrincipalEx.FindByIdentity(ctx, IdentityType.DistinguishedName, user.Manager);

            return manager;
        }

        public static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, sDomain, sDefaultOU, ContextOptions.SimpleBind,
               sServiceUser, sServicePassword);
            return ctx;
        }

        public static bool IsUserExisiting(string account)
        {
            if (GetUser(account) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static UserPrincipalEx CreateUser(string account, string password)
        {
            if (!IsUserExisiting(account))
            {
                PrincipalContext ctx = GetPrincipalContext();
                    UserPrincipalEx user = new UserPrincipalEx
                       (ctx, account, password, true);
                return user;
            }
            else
            {
                return GetUser(account);
            }
        }

        public static GroupPrincipal GetGroup(string group)
        {
            PrincipalContext ctx = GetPrincipalContext();
            return GroupPrincipal.FindByIdentity(ctx, group);
        }
    }
}