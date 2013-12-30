using ADManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADManager.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            IList<UserModel> users = new List<UserModel>();

            foreach (UserPrincipalEx p in UserPrincipalExHelper.GetUsers())
            {
                users.Add(UserModel.GetUser(p));
            }

            return View(users);
        }

        public ActionResult Details(string account)
        {
            UserModel u = UserModel.GetUser(UserPrincipalExHelper.GetUser(account));
            return View(u);
        }

        public ActionResult Create()
        {
            ViewBag.Managers = UserPrincipalExHelper.GetUsers().Select(c => new SelectListItem { Value = c.DistinguishedName, Text = c.DisplayName });
            ViewBag.DivisionGroups = UserPrincipalExHelper.DivisionGroups.Select(c => new SelectListItem { Value = c, Text = c });
            ViewBag.RegionGroups = UserPrincipalExHelper.RegionGroups.Select(c => new SelectListItem { Value = c, Text = c });
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserModel user)
        {
            ViewBag.Managers = UserPrincipalExHelper.GetUsers().Select(c => new SelectListItem { Value = c.DistinguishedName, Text = c.DisplayName });
            ViewBag.DivisionGroups = UserPrincipalExHelper.DivisionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(user.Account, c) });
            ViewBag.RegionGroups = UserPrincipalExHelper.RegionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(user.Account, c) });
            if (ModelState.IsValid)
            {
                NameValueCollection nvc = user.ValidateCreate();
                if (nvc.Count != 0)
                {
                    foreach (string key in nvc)
                    {
                        ModelState.AddModelError(key, nvc[key]);
                    }
                    return View();
                }
                else
                {
                    UserPrincipalEx ex = UserModel.GetUserPrincipal(user);
                    try
                    {
                        ex.Save();
                        foreach (string g in UserPrincipalExHelper.RegionGroups)
                        {
                            GroupPrincipal g_region_old = UserPrincipalExHelper.GetGroup(g);
                            if (g_region_old != null)
                            {
                                g_region_old.Members.Remove(ex);
                                g_region_old.Save();
                            }
                        }
                        GroupPrincipal g_region = UserPrincipalExHelper.GetGroup(user.Region);
                        if (g_region != null)
                        {
                            g_region.Members.Add(ex);
                            g_region.Save();
                        }
                        foreach (string g in UserPrincipalExHelper.DivisionGroups)
                        {
                            GroupPrincipal g_division = UserPrincipalExHelper.GetGroup(g);
                            if (g_division != null)
                            {
                                g_division.Members.Remove(ex);
                                g_division.Save();
                            }
                        }
                        foreach (string g in user.Divisions)
                        {
                            GroupPrincipal g_division = UserPrincipalExHelper.GetGroup(g);
                            if (g_division != null)
                            {
                                g_division.Members.Add(ex);
                                g_division.Save();
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("Account", e.Message);
                        return View();
                    }
                }
            }
            else
            {
                
                return View();
            }
        }

        public ActionResult Edit(string account)
        {
            UserModel u = UserModel.GetUser(UserPrincipalExHelper.GetUser(account));
            ViewBag.Managers = UserPrincipalExHelper.GetUsers().Where(c => c.Account != account).Select(c => new SelectListItem { Value = c.DistinguishedName, Text = c.DisplayName, Selected = (c.DisplayName == u.Manager) });
            ViewBag.DivisionGroups = UserPrincipalExHelper.DivisionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(u.Account, c) });
            ViewBag.RegionGroups = UserPrincipalExHelper.RegionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(u.Account, c) });
            return View(u);
        }

        [HttpPost]
        public ActionResult Edit(string account, UserModel user)
        {
            ViewBag.Managers = UserPrincipalExHelper.GetUsers().Where(c => c.Account != account).Select(c => new SelectListItem { Value = c.DistinguishedName, Text = c.DisplayName, Selected = (c.DisplayName == user.Manager) });
            ViewBag.DivisionGroups = UserPrincipalExHelper.DivisionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(user.Account, c) });
            ViewBag.RegionGroups = UserPrincipalExHelper.RegionGroups.Select(c => new SelectListItem { Value = c, Text = c, Selected = UserPrincipalExHelper.UserInGroup(user.Account, c) });
            if (ModelState.IsValid)
            {
                NameValueCollection nvc = user.ValidateUpdate();
                if (nvc.Count != 0)
                {
                    foreach (string key in nvc)
                    {
                        ModelState.AddModelError(key, nvc[key]);
                    }
                    return View();
                }
                else
                {
                    UserPrincipalEx ex = UserModel.GetUserPrincipal(user);
                    try
                    {
                        ex.Save();
                        foreach (string g in UserPrincipalExHelper.RegionGroups)
                        {
                            GroupPrincipal g_region_old = UserPrincipalExHelper.GetGroup(g);
                            if (g_region_old != null)
                            {
                                g_region_old.Members.Remove(ex);
                                g_region_old.Save();
                            }
                        }
                        GroupPrincipal g_region = UserPrincipalExHelper.GetGroup(user.Region);
                        if (g_region != null)
                        {
                            g_region.Members.Add(ex);
                            g_region.Save();
                        }
                        foreach (string g in UserPrincipalExHelper.DivisionGroups)
                        {
                            GroupPrincipal g_division = UserPrincipalExHelper.GetGroup(g);
                            if (g_division != null)
                            {
                                g_division.Members.Remove(ex);
                                g_division.Save();
                            }
                        }
                        foreach (string g in user.Divisions)
                        {
                            GroupPrincipal g_division = UserPrincipalExHelper.GetGroup(g);
                            if (g_division != null)
                            {
                                g_division.Members.Add(ex);
                                g_division.Save();
                            }
                        }
                        if (!String.IsNullOrEmpty(user.Password))
                        {
                            ex.SetPassword(user.Password);
                            ex.Save();
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("Account", e.Message);
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Delete(string account)
        {
            UserPrincipalEx user = UserPrincipalExHelper.GetUser(account);
            user.Enabled = false;
            user.Save();

            return RedirectToAction("Index");
        }
    }
}
