<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[Atomia.Web.Frame.Models.LocalePickerFormData, Atomia.Web.Frame]]" %>
<%@ Import Namespace="Atomia.Web.Frame.Models" %>
<%  if (!String.IsNullOrEmpty(Model.ReturnUrl) && Model.Countries != null && Model.Countries.Length > 1)
    {
%>
    <div id="localePickerDiv">
        <%= Html.Resource("Country") %>
        <ul>
            <% foreach (LocaleModel country in Model.Countries)
               {
            %>
                    <li class="<%= country.IsDefault ? "active" : string.Empty %>">
                        <a href="javascript:void(0);" rel="<%= country.IsDefault ? "Default" : country.Code %>">
                            <img class="inline-icon-24" src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/" + country.Image, Session["Theme"])) %>" width="24" height="24" alt="<%= Html.Resource(country.Code) %>" />
                        </a>    
                        <a style="text-decoration: none; color: #331F00;" href="javascript:void(0);" rel="<%= country.IsDefault ? "Default" : country.Code %>"><%= Html.Resource(country.Code) %></a>
                    </li>
            <% } %>
        </ul>
    </div>
    <% Html.BeginForm("LocalePicker", "Home", new { area = "root" }, FormMethod.Post, new { @id = "locale_form", autocomplete = "off" }); %>
        <%= Html.Hidden("localePickerFormData.ReturnUrl", Model.ReturnUrl)%>
        <%= Html.Hidden("localePickerFormData.SelectedCountry")%>
    <% Html.EndForm();%>
    <script type="text/javascript">
        $(document).ready(function() {
            $('#localePickerDiv a').bind('click', function() {
                var locale = $(this).attr('rel');
                if (locale != 'Default') {
                    $('#localePickerFormData_SelectedCountry').val(locale);
                    $("#locale_form").submit();
                }
            });
        });
    </script>
<%
    }
%>