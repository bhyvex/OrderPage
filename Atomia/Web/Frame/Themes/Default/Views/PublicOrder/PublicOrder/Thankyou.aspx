<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("Title")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settingsbox">
       <h3><%= Html.Resource("Title")%></h3>
       <div class="settingsboxinner">
            <h3>
                <%= Html.Resource("Thanks")%>
            </h3>
	        <p>
	            <%= Html.ResourceNotEncoded("Info2")%>
	        </p>
	        <p>
	            <%= Html.ResourceNotEncoded("Info3")%>
	        </p>
	        <br class="clear" />
	    </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            var orderJson = JSON.parse('<%= ViewData["CreatedOrderAsJson"]%>');
            <%-- DomainRegContact is separated out from CustomData on order items. --%>
            var domainRegContactJson = JSON.parse('<%= ViewData["DomainRegContactAsJson"]%>');
        });
    </script>
</asp:Content>
