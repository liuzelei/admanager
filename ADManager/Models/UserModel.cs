using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ADManager.Models
{
    public class UserModel
    {
        [Display(Name = "账号")]
        [Required]
        public string Account { get; set; }

        [Display(Name = "电子邮箱")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "姓")]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "名")]
        [Required]
        public string GivenName { get; set; }

        [Display(Name = "英文名")]
        public string DisplayName { get; set; }

        [Display(Name = "地域")]
        [Required]
        public string Region { get; set; }

        [Display(Name = "部门")]
        [Required]
        public IList<string> Divisions { get; set; }

        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "固定电话")]
        public string Telephone { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }

        [Display(Name = "直接领导")]
        public string Manager { get; set; }

        [Display(Name = "直接领导")]
        public string ManagerDisplay { get; set; }

        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "确认密码")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "已删除")]
        public bool Deleted { get; set; }

        public NameValueCollection ValidateCreate()
        {
            NameValueCollection list = new NameValueCollection();
            if (UserPrincipalExHelper.IsUserExisiting(this.Account))
            {
                list.Add("Account", "账户已经存在.");
            }
            if (this.Password != this.PasswordConfirm)
            {
                list.Add("Password", "两次密码输入不一致.");
            }

            return list;
        }

        public NameValueCollection ValidateUpdate()
        {
            NameValueCollection list = new NameValueCollection();
            if (!String.IsNullOrEmpty(this.Password) && (this.Password != this.PasswordConfirm))
            {
                list.Add("Password", "两次密码输入不一致.");
            }
            return list;
        }

        public static UserPrincipalEx GetUserPrincipal(UserModel user)
        {
            UserPrincipalEx ex = UserPrincipalExHelper.CreateUser(user.Account, user.Password);
            ex.Account = user.Account;
            ex.DisplayName = user.DisplayName;
            ex.Email = user.Email;
            ex.Surname = user.Surname;
            ex.Telephone = user.Telephone;
            ex.GivenName = user.GivenName;
            ex.Manager = user.Manager;
            ex.Mobile = user.Mobile;
            ex.Title = user.Title;
            ex.Manager = user.Manager;
            return ex;
        }

        public static UserModel GetUser(UserPrincipalEx principalEx)
        {
            UserModel u = new UserModel();

            if (principalEx != null)
            {
                u.Account = principalEx.Account;
                u.DisplayName = principalEx.DisplayName;
                u.Email = principalEx.Email;
                u.Surname = principalEx.Surname;
                u.Telephone = principalEx.Telephone;
                u.GivenName = principalEx.GivenName;
                u.Manager = principalEx.Manager;
                u.Mobile = principalEx.Mobile;
                u.Title = principalEx.Title;
                u.Manager = principalEx.Manager;

                u.Divisions = new List<string>();
                foreach (string g in UserPrincipalExHelper.DivisionGroups)
                {
                    if (UserPrincipalExHelper.UserInGroup(u.Account, g))
                    {
                        u.Divisions.Add(g);
                    }
                }

                foreach (string g in UserPrincipalExHelper.RegionGroups)
                {
                    if (UserPrincipalExHelper.UserInGroup(u.Account, g))
                    {
                        u.Region = g;
                        break;
                    }
                }
                if (principalEx.Manager != null)
                {
                    u.ManagerDisplay = UserPrincipalExHelper.GetManager(u.Account).DisplayName;
                }

                u.Deleted = principalEx.Enabled.HasValue ? !principalEx.Enabled.Value : false;
            }

            return u;
        }
    }
}