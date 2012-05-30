<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Html.Resource("Title") %>
</asp:content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="settingsbox"> 
		<h3><%= Html.Resource("Title") %></h3>
		<div class="settingsboxinner">
			<p>
                <%= Html.Resource("Description") %>
            </p>
			<p class="data-row"> 
				<strong><%= Html.ResourceNotEncoded("Amount") %></strong> 
				<span><%= ViewData["PayAmount"]%>
                <span class="currency"><%= ViewData["Currency"]%></span>
                </span>				
			</p>
           
			<% Html.BeginForm("PayPalConfirm", "Payment", new { area = "Payment" }, FormMethod.Post, new { @id = "submit_form", autocomplete = "off" }); %>
				<%= Html.Hidden("token", ViewData["ReferenceNumber"])%>
				<%= Html.Hidden("PayerID", ViewData["PayerId"]) %>
				<p class="actions" style="display: block;">
					<%= Html.Button(Html.Resource("Order"), "javascript:void(0);", "button large green", "submitLink")%>
					<%= Html.Button(Html.Resource("Cancel"), ViewData["CancelUrl"].ToString(), "button large")%>
				</p>
			<% Html.EndForm(); %>
           		 
		</div> 
	</div>     
	 <br class="clear" /> 

     <script type="text/javascript">

         $(document).ready(function () {

             $('#submitLink').click(function () {
                 $('#submit_form').submit();
             });

         });
    </script>
</asp:content>
