<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("Title")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settingsbox">
        <h2><%= Html.Resource("Sorry")%></h2>
        <div class="settingsboxinner">
	        <p>
		        <%= Html.Resource("Message")%>
	        </p>
	    </div>
	</div>
</asp:Content>