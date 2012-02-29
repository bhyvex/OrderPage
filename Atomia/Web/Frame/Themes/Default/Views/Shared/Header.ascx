<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%--<div id="header" class="span-24">
    <h1><%= Html.Resource("Common, AtomiaHostingControlPanel") %></h1>
    <p id="logininfo">
        <% Html.RenderPartial("LogOnUserControl"); %>
    </p> 
</div>--%>
<div id="header_settings" class="span-24">
    <% 
        var sessionAccountID = String.Empty;
        
        if (Session["currentSessionAccount"] != null)
        {
            sessionAccountID = Session["currentSessionAccount"].ToString();
        }
        
        var routeDataDictionary = this.Url.RequestContext.RouteData.Values;
        Html.RenderAction("Render", "Menu", new { routeDictionary = routeDataDictionary, menuid = (sessionAccountID != String.Empty && sessionAccountID != null) ? "TopLevelMenu" : "TopLevelMenuUnauthenticated" });
    %>
</div>