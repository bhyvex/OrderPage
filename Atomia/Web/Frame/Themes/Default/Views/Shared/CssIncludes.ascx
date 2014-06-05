<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Atomia.Web.Base.Minifiers.UI.HTMLExtensions" %>
<% 
       var pluginCssPath = Server.MapPath(string.Format("~/Themes/{0}/Content/pluginCss", Session["Theme"]));
       bool themeHasPluginCss = Directory.Exists(pluginCssPath);

       if (ViewData["cssFilesList"] != null)
       {
           foreach (Atomia.Web.Base.Configs.CssFile file in (List<Atomia.Web.Base.Configs.CssFile>)ViewData["cssFilesList"])
           {
               string filePath = null;
               if (themeHasPluginCss && File.Exists(Server.MapPath(ResolveUrl(string.Format("~/Themes/{0}/Content/pluginCss/{1}", Session["Theme"], Path.GetFileName(file.Name))))))
               {
                   filePath = string.Format("~/Themes/{0}/Content/pluginCss/{1}", Session["Theme"], Path.GetFileName(file.Name));

               } else {
                   filePath = string.Format("~/Themes/Default/Content/pluginCss/{0}", Path.GetFileName(file.Name));
               }
               
				if (File.Exists(Server.MapPath(filePath)))
				{
                   if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
                   {
                       Html.CSS().Add(filePath);
                   }
                   else
                   {
%>
<link href="<%= ResolveClientUrl(filePath) %>" rel="stylesheet" type="text/css" />
<% 
                    }
				}
           }
       }

	   Atomia.Web.Plugin.HCP.Provisioning.Helpers.AccountData accountData = null;
	   if (this.Url.RequestContext.HttpContext.User.Identity.IsAuthenticated)
	   {
		   HttpSessionStateBase sessionBase = new HttpSessionStateWrapper(this.Session);
		   accountData = new Atomia.Web.Plugin.HCP.Provisioning.Helpers.AccountData(sessionBase, this.Url.RequestContext.RouteData, this.Url.RequestContext.HttpContext.User.Identity.Name, true);
	   }

        if (accountData != null && accountData.LoggedUserDetails != null && !String.IsNullOrEmpty(accountData.LoggedUserDetails.CssUrl))
        {
%>
<link href="<%= accountData.LoggedUserDetails.CssUrl %>" rel="stylesheet" type="text/css"
    media="screen, projection" />
<% 
        }
        else
        {

            var cssPath = Server.MapPath(string.Format("~/Themes/{0}/Content/css/{0}", Session["Theme"]));
            bool themeHasCssDir = Directory.Exists(cssPath);

            string styleTheme = "Default";
            if (themeHasCssDir && File.Exists(Server.MapPath(string.Format("~/Themes/{0}/Content/css/{0}/style.css", Session["Theme"])))) {
                styleTheme = (string) Session["Theme"];
            }
            
            if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
            {
                Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{0}/style.css", styleTheme));
            }
            else
            {
%>
<link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{0}/style.css", styleTheme)) %>"
    rel="stylesheet" type="text/css" media="screen, projection" />
<% 
            }

            //patch from http://blogs.microsoft.co.il/blogs/egoldin/archive/2009/07/29/detecting-ie8-on-server-side.aspx

            string browserType = Request.Browser.Type;

            if (Request.UserAgent.ToUpper().Contains("TRIDENT"))
            {
                browserType = browserType.Replace("IE7", "IE8");
            }

            // end of patch
            
            string ie7Theme = "Default";
            if (themeHasCssDir && File.Exists(Server.MapPath(string.Format("~/Themes/{0}/Content/css/{0}/ie7.css", Session["Theme"]))))
            {
                ie7Theme = (string) Session["Theme"];
            }

            if (Request.Browser.Browser.ToUpper() == "IE" && browserType.StartsWith("IE7"))
            {
                if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
                {
                    Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{0}/ie7.css", ie7Theme));
                }
                else
                {
%>
<link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{0}/ie7.css", ie7Theme))%>"
    rel="stylesheet" type="text/css" media="screen, projection" />
<% 
                }
            }
            
            string ieTheme = "Default";
            if (themeHasCssDir && File.Exists(Server.MapPath(string.Format("~/Themes/{0}/Content/css/{0}/ie.css", Session["Theme"])))) {
                ieTheme = (string) Session["Theme"];
            }
            
            if (Request.Browser.Browser.ToUpper() == "IE")
            {
                if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
                {
                    Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{0}/ie.css", ieTheme));
                }
                else
                {
%>
<link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{0}/ie.css", ieTheme))%>"
    rel="stylesheet" type="text/css" media="screen, projection" />
<% 
                }
            }
            
            if (themeHasCssDir && File.Exists(Server.MapPath(string.Format("~/Themes/{0}/Content/css/{0}/style-custom.css", Session["Theme"])))) {
                if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
                {
                    Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{0}/style-custom.css", Session["Theme"]));
                }
                else
                {
%>
<link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{0}/style-custom.css", Session["Theme"]))%>"
    rel="stylesheet" type="text/css" media="screen, projection" />
<% 
                }                
            }
            if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
            {
%>
<%= Html.CSS().HTML%>
<%
            }
        }
%>