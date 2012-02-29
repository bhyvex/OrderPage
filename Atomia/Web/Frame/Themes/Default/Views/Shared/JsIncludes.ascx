<%@ Import Namespace="System.IO"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Atomia.Web.Base.Minifiers.UI.HTMLExtensions" %>

<!-- Add javascript hardcoded -->
<% 
if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
{
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-1.3.2.min.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-ui-1.7.2.custom.min.js", Session["Theme"]));
	Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery.validate.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/json2.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/MicrosoftMvcJQueryValidation.js", Session["Theme"]));
}
else
{
%>
    <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery-1.3.2.min.js", Session["Theme"])) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery-ui-1.7.2.custom.min.js", Session["Theme"])) %>" type="text/javascript"></script>
	<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery.validate.js", Session["Theme"])) %>" type="text/javascript"></script>
	<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/json2.js", Session["Theme"])) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/MicrosoftMvcJQueryValidation.js", Session["Theme"])) %>" type="text/javascript"></script>
<% 
}
%>

<!-- Add all javascripts -->
<% 

var path = Server.MapPath(string.Format("~/Themes/{0}/Scripts/PluginScripts", Session["Theme"]));

var defaultFilePath = String.Empty;

if (Directory.Exists(path) && ViewData["jsFilesList"] != null)            
{
   foreach (Atomia.Web.Base.Configs.JavscriptFile file in (List<Atomia.Web.Base.Configs.JavscriptFile>)ViewData["jsFilesList"]) 
   {
       var filePath = ResolveUrl(string.Format("~/Themes/{0}/Scripts/PluginScripts/{1}", Session["Theme"],
                                          String.IsNullOrEmpty(file.Path)
                                              ? Path.GetFileName(file.Name)
                                              : file.Path + "/" + Path.GetFileName(file.Name)));

       bool fileExists = File.Exists(Server.MapPath(filePath));

       if (!fileExists && Session["Theme"].ToString().ToLower() != "default")
       {
           defaultFilePath = ResolveUrl(string.Format("~/Themes/Default/Scripts/PluginScripts/{0}",
                                          String.IsNullOrEmpty(file.Path)
                                              ? Path.GetFileName(file.Name)
                                              : file.Path + "/" + Path.GetFileName(file.Name)));
           
           fileExists = File.Exists(Server.MapPath(defaultFilePath));
       }

       if (fileExists)
       {
           if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
           {
               Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/PluginScripts/{1}", (String.IsNullOrEmpty(defaultFilePath) ? Session["Theme"] : "Default"),
                                                String.IsNullOrEmpty(file.Path)
                                                    ? Path.GetFileName(file.Name)
                                                    : file.Path + "/" + Path.GetFileName(file.Name)));
           }
           else
           {
           %>
               <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/PluginScripts/{1}", (String.IsNullOrEmpty(defaultFilePath) ? Session["Theme"] : "Default"),
                                                  String.IsNullOrEmpty(file.Path)
                                                      ? Path.GetFileName(file.Name)
                                                      : file.Path + "/" + Path.GetFileName(file.Name)))%>" type="text/javascript"></script>
           <%
           }
       }

       defaultFilePath = String.Empty;
   }
} %>

<!-- Add AtomiaBase and print minified js files -->
<% 
if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
{
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/AtomiaBase.js", Session["Theme"]));
%>
    <%= Html.Scripts().HTML%>
<%
} else {
%>
    <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/AtomiaBase.js", Session["Theme"]))%>" type="text/javascript"></script>
<%
   }
%>
