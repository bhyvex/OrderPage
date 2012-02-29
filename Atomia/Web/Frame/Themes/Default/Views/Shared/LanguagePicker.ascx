<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[Atomia.Web.Frame.Models.LanguagePickerFormData, Atomia.Web.Frame]]" %>
<%@ Import Namespace="Atomia.Web.Frame.Models" %>
<%  if (!String.IsNullOrEmpty(Model.ReturnUrl) && Model.Languages != null && Model.Languages.Length > 1)
    {
%>
    <div id="languagePickerDiv">
        <%= Html.Resource("Language") %>
        <ul>
            <% foreach (LanguageModel language in Model.Languages)
               {
                   var imgName = language.Code.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries)[0];
            %>
                    <li class="<%= language.IsDefault ? "active" : string.Empty %>">
                        <a href="javascript:void(0);" rel="<%= language.IsDefault ? "Default" : language.Code %>">
                            <img class="inline-icon-24" src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/flg_" + imgName + ".png", Session["Theme"])) %>" width="24" height="24" alt="<%= Html.Resource(language.Name) %>" />
                        </a>    
                        <a style="text-decoration: none; color: #331F00;" href="javascript:void(0);" rel="<%= language.IsDefault ? "Default" : language.Code %>"><%= Html.Resource(language.Name) %></a>
                    </li>
            <% } %>
        </ul>
    </div>
    <% Html.BeginForm("LanguagePicker", "Home", new { area = "root" }, FormMethod.Post, new { @id = "languages_form", autocomplete = "off" }); %>
        <%= Html.Hidden("languagePickerFormData.ReturnUrl", Model.ReturnUrl)%>
        <%= Html.Hidden("languagePickerFormData.SelectedLanguage")%>
    <% Html.EndForm();%>
    <script type="text/javascript">
        $(document).ready(function() {
            $('#languagePickerDiv a').bind('click', function() {
                var language = $(this).attr('rel');
                if (language != 'Default') {
                    $('#languagePickerFormData_SelectedLanguage').val(language);
                    $("#languages_form").submit();
                }
            });
        });
    </script>
<%
    }
%>
        