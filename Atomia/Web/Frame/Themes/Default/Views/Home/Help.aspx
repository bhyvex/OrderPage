<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= Html.Resource("Title")%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div class="settingsbox">
        <h3><%= Html.Resource("Title")%></h3>
        <div class="settingsboxinner">
            <p><%= Html.Resource("IntroText")%></p>
            <br class="clear" />
        </div>
    </div>

</asp:Content>