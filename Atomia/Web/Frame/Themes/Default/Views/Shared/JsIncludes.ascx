﻿<%@ Import Namespace="System.IO"%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Atomia.Web.Base.Minifiers.UI.HTMLExtensions" %>

<!-- Add javascript hardcoded -->
<% 
if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
{
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-1.7.2.min.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-ui-1.8.20.custom.min.js", Session["Theme"]));
	Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery.validate.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/json2.js", Session["Theme"]));
    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/MicrosoftMvcJQueryValidation.js", Session["Theme"]));
}
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Atomia.Web.Base.Minifiers.UI.HTMLExtensions" %>
<%  
	var path = Server.MapPath(string.Format("~/Themes/{0}/Scripts", Session["Theme"]));
	bool pathScriptsExist = true;
	if (!Directory.Exists(path)){
		pathScriptsExist = false;
	}
if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
    {
		if(pathScriptsExist){
			Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-1.7.2.min.js", Session["Theme"]));
			Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery.validate.js", Session["Theme"]));
			Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/MicrosoftMvcJQueryValidation.js", Session["Theme"]));
			Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/jquery-ui-1.8.20.custom.min.js", Session["Theme"])); 
			Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/json2.js", Session["Theme"])); 
		}else{
			Html.Scripts().Add("~/Themes/Default/Scripts/jquery-1.7.2.min.js");
			Html.Scripts().Add("~/Themes/Default/Scripts/jquery.validate.js");
			Html.Scripts().Add("~/Themes/Default/Scripts/MicrosoftMvcJQueryValidation.js");
			Html.Scripts().Add("~/Themes/Default/Scripts/jquery-ui-1.8.20.custom.min.js");
			Html.Scripts().Add("~/Themes/Default/Scripts/json2.js");
		}
		
    } 
    else 
    { 
		if(pathScriptsExist){
		%>
			<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery-1.7.2.min.js", Session["Theme"])) %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery.validate.js", Session["Theme"])) %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/MicrosoftMvcJQueryValidation.js", Session["Theme"])) %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/jquery-ui-1.8.20.custom.min.js", Session["Theme"]))%>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/json2.js", Session["Theme"]))%>" type="text/javascript"></script>
		<% } 
		else
		{
		%>
			<script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/jquery-1.7.2.min.js") %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/jquery.validate.js") %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/MicrosoftMvcJQueryValidation.js") %>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/jquery-ui-1.8.20.custom.min.js")%>" type="text/javascript"></script>
			<script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/json2.js")%>" type="text/javascript"></script>
		<% } 
	}
%>
    
<% 
	bool pathScriptPluginExist = pathScriptsExist;
    path = Server.MapPath(string.Format("~/Themes/{0}/Scripts/PluginScripts", Session["Theme"]));
	if (!Directory.Exists(path)){
		path = Server.MapPath("~/Themes/Default/Scripts/PluginScripts");
		pathScriptPluginExist = false;
	}
    string defaultFilePath = null;

    if (Directory.Exists(path) && ViewData["jsFilesList"] != null)            
    {
        foreach (Atomia.Web.Base.Configs.JavscriptFile file in (List<Atomia.Web.Base.Configs.JavscriptFile>)ViewData["jsFilesList"]) 
        {
            bool fileExists;
            if(pathScriptPluginExist){
				var filePath =
					ResolveUrl(string.Format("~/Themes/{0}/Scripts/PluginScripts/{1}", Session["Theme"],
												  String.IsNullOrEmpty(file.Path)
													  ? Path.GetFileName(file.Name)
													  : file.Path + "/" + Path.GetFileName(file.Name)));


				fileExists = File.Exists(Server.MapPath(filePath));
				
				if (!fileExists && Session["Theme"].ToString().ToLower() != "default")
				{
					defaultFilePath = ResolveUrl(string.Format("~/Themes/Default/Scripts/PluginScripts/{0}",
												  String.IsNullOrEmpty(file.Path)
													  ? Path.GetFileName(file.Name)
													  : file.Path + "/" + Path.GetFileName(file.Name)));
				   
					fileExists = File.Exists(Server.MapPath(defaultFilePath));
					   
				}
			}else{
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
                { %>
                    <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/PluginScripts/{1}", (String.IsNullOrEmpty(defaultFilePath) ? Session["Theme"] : "Default"),
                                                          String.IsNullOrEmpty(file.Path) ? Path.GetFileName(file.Name) : file.Path + "/" + Path.GetFileName(file.Name)))%>" type="text/javascript">
                    </script>
             <% }
            }

            defaultFilePath = null;
        }
    } 
%>
       
<%
    if(!pathScriptsExist){
	if (Application["javascriptMinifier"] != null && Boolean.Parse(Application["javascriptMinifier"].ToString()))
       {
            Html.Scripts().Add("~/Themes/Default/Scripts/AtomiaBase.js");  %>
            <%= Html.Scripts().HTML%>
    <% } 
    else 
        { %>
        <script src="<%= ResolveClientUrl("~/Themes/Default/Scripts/AtomiaBase.js")%>" type="text/javascript"></script>
     <% }
}else{	 
%>

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
   }
%>