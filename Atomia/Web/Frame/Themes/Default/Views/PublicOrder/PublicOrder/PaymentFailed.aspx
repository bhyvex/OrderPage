<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("Title")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settingsbox">
        <h3><%= Html.Resource("Info1")%></h3>
        <div class="settingsboxinner">
            <p><%= Html.ResourceNotEncoded("Info2")%></p>
            <p><a href="<%= Application["OrderApplicationRawURL"]%>"><%= Html.Resource("TryAgain")%></a></p>
            <br class="clear" />
       </div>
    </div>
</asp:Content>