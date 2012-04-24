<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("PageTitle")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settingsbox">
       <h3><%= Html.Resource("PageTitle")%></h3>
       <div class="settingsboxinner">
	        <%= Html.ResourceNotEncoded(ViewData["TOSResource"].ToString())%>
	        <br class="clear" />
	    </div>
	</div>
</asp:Content>
