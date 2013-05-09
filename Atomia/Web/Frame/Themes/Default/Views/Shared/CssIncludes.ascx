<%@ Import Namespace="System.IO"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Atomia.Web.Base.Minifiers.UI.HTMLExtensions" %>

<% 
// Load css`s (screen, style, vtip)
var cssPath = Server.MapPath(string.Format("~/Themes/{0}/Content/css", Session["Theme"]));

if (Directory.Exists(cssPath))
{
   var files = Directory.GetFiles(cssPath, "*.css");
   
   if (files.Any(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("style")))
   {
       if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
       {
           Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("style")))));
       }
       else
       {
		   %>
			  <link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file=>Path.GetFileNameWithoutExtension(file).ToLower().Equals("style")))))%>"  rel="stylesheet" type="text/css"  media="screen, projection"/>
		   <%
        }            
   } 
   
   if (files.Any(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("vtip")))
   {
       if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
       {
           Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("vtip")))));
       }
       else
       {
		   %>
			  <link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file=>Path.GetFileNameWithoutExtension(file).ToLower().Equals("vtip")))))%>"  rel="stylesheet" type="text/css"  media="screen, projection"/>
		   <%
        }            
   } 

   //patch from http://blogs.microsoft.co.il/blogs/egoldin/archive/2009/07/29/detecting-ie8-on-server-side.aspx

   string browserType = Request.Browser.Type;

   if (Request.UserAgent.ToUpper().Contains("TRIDENT"))

       browserType = browserType.Replace("IE7", "IE8");

   // end of patch

   if (files.Any(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("ie")) && Request.Browser.Browser.ToUpper() == "IE" && browserType.StartsWith("IE7"))
   {
       if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
       {
           Html.CSS().Add(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file => Path.GetFileNameWithoutExtension(file).ToLower().Equals("ie")))));
       }
       else
       {
		   %>
			<link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/css/{1}", Session["Theme"], Path.GetFileName(files.First(file=>Path.GetFileNameWithoutExtension(file).ToLower().Equals("ie")))))%>"  rel="stylesheet" type="text/css" media="screen, projection"/>
		   <% 
       }
   }
}

// Load plugin specific css
var pluginCssPath = Server.MapPath(string.Format("~/Themes/{0}/Content/pluginCss", Session["Theme"]));

if (Directory.Exists(pluginCssPath) && ViewData["cssFilesList"] != null)
{
   foreach (Atomia.Web.Base.Configs.CssFile file in (List<Atomia.Web.Base.Configs.CssFile>)ViewData["cssFilesList"])
   {
       if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
       {
           Html.CSS().Add(string.Format("~/Themes/{0}/Content/pluginCss/{1}", Session["Theme"], Path.GetFileName(file.Name)));
       }
       else
       {
        %>
            <link href="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/pluginCss/{1}", Session["Theme"], Path.GetFileName(file.Name)))%>"  rel="stylesheet" type="text/css" />
        <% 
       }
   }
} 
%>
<%
if (Application["cSSMinifier"] != null && Boolean.Parse(Application["cSSMinifier"].ToString()))
{
%>
<%= Html.CSS().HTML %>
<%
}
%>
