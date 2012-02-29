<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Atomia.Web.Base.Helpers.General" %>

<% 
    if (ViewData["partialsList"] != null)
    {
        foreach (var allowedPartial in (List<PartialItems>) ViewData["partialsList"])
        {
            if (String.Compare(allowedPartial.Container, "partials") == 0)
            {
                RouteValueDictionary routeValueDict = new RouteValueDictionary { { "area", allowedPartial.AreaName } };
                Html.RenderAction(allowedPartial.Action, allowedPartial.Controller, routeValueDict);
            }
        }
    }
%>