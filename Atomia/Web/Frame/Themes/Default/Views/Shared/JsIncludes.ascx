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
        foreach (string globalScript in new string[] {
            "jquery-1.7.2.min.js", "jquery.validate.js", "MicrosoftMvcJQueryValidation.js", "jquery-ui-1.8.20.custom.min.js", "json2.js", "AtomiaValidation.js", "custom-global.js"
        }) {
            string filePath = pathScriptsExist ? Server.MapPath(string.Format("~/Themes/{0}/Scripts/{1}", Session["Theme"], globalScript)) : null;
		    if (pathScriptsExist && File.Exists(filePath)) {
			    Html.Scripts().Add(string.Format("~/Themes/{0}/Scripts/{1}", Session["Theme"], globalScript));
            } else {
                Html.Scripts().Add(string.Format("~/Themes/Default/Scripts/{0}", globalScript));
            }
		}		
    } 
    else 
    { 
        foreach (string globalScript in new string[] { 
            "jquery-1.7.2.min.js", "jquery.validate.js", "MicrosoftMvcJQueryValidation.js", "jquery-ui-1.8.20.custom.min.js", "json2.js", "AtomiaValidation.js", "custom-global.js"
        }) {
            string filePath = pathScriptsExist ? Server.MapPath(string.Format("~/Themes/{0}/Scripts/{1}", Session["Theme"], globalScript)) : null;
            if (pathScriptsExist && File.Exists(filePath)) {
            %>
                <script src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Scripts/{1}", Session["Theme"], globalScript)) %>" type="text/javascript"></script>
            <%
            } else {
            %>
                <script src="<%= ResolveClientUrl(string.Format("~/Themes/Default/Scripts/{0}", globalScript)) %>" type="text/javascript"></script>
            <%
		    } 
        }
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
    bool atomiaBaseExists = pathScriptsExist && File.Exists(Server.MapPath(string.Format("~/Themes/{0}/Scripts/AtomiaBase.js", Session["Theme"])));
    if(!atomiaBaseExists){
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
<script type="text/javascript">
    AtomiaValidation.init('AtomiaRequired', 'AtomiaRegularExpression', 'AtomiaStringLength', 'AtomiaRange');
</script>