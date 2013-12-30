using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace ADManager.Models
{
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class UserPrincipalEx : UserPrincipal
    {
        public UserPrincipalEx(PrincipalContext context) : base(context) { }

        public UserPrincipalEx(PrincipalContext context, string samAccountName, string password, bool enabled) :
            base(context, samAccountName, password, enabled) { }

        public string Account 
        {
            get { return base.SamAccountName; }
            set { base.SamAccountName = value; }
        }

        public string Email 
        {
            get { return base.EmailAddress; }
            set { base.EmailAddress = value; }
        }

        public new string Surname
        {
            get { return base.Surname; }
            set { base.Surname = value; }
        }

        public new string GivenName 
        {
            get { return base.GivenName; }
            set { base.GivenName = value; }
        }
  
        public new string DisplayName
        {
            get { return base.DisplayName; }
            set { base.DisplayName = value; }
        }

        [DirectoryProperty("mobile")]
        public string Mobile
        {
            get
            {
                if (ExtensionGet("mobile").Length != 1)
                    return null;

                return (string)ExtensionGet("mobile")[0];

            }
            set { this.ExtensionSet("mobile", value); }
        }

        [DirectoryProperty("telephoneNumber")]
        public string Telephone
        {
            get
            {
                if (ExtensionGet("telephoneNumber").Length != 1)
                    return null;

                return (string)ExtensionGet("telephoneNumber")[0];

            }
            set { this.ExtensionSet("telephoneNumber", value); }
        }

        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if (ExtensionGet("title").Length != 1)
                    return null;

                return (string)ExtensionGet("title")[0];

            }
            set { this.ExtensionSet("title", value); }
        }

        [DirectoryProperty("manager")]
        public string Manager
        {
            get
            {
                if (ExtensionGet("manager").Length != 1)
                    return null;

                return (string)ExtensionGet("manager")[0];

            }
            set { this.ExtensionSet("manager", value); }
        }

        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context,  typeof(UserPrincipalEx), identityType, identityValue);
        }
    }
}