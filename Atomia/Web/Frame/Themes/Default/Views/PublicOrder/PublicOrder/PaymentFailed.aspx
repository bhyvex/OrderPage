<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("Title")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
       <h2><%= Html.Resource("Info1")%></h2>
       <p>
           <%= Html.ResourceNotEncoded("Info2")%>
       </p>
       <p>
           <a href="<%= Url.Action("Select", "PublicOrder", new { area = "PublicOrder" })%>"><%= Html.Resource("TryAgain")%></a>
       </p>
</asp:Content>
