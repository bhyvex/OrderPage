<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<CustomField>>" %>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="Atomia.Web.Plugin.Validation.Models" %>

<div id="CustomFields">  
<%
    int i = 0;
    foreach (CustomField customField in Model)
{
  %>
  <div class="formrow">
                        <h5>
                          <% if (customField.Required)
{	
  %>
                          <label class="required" for="CustomFields_<%=i %>__Value"><span>*</span> <%=LocalizationHelpers.GlobalResource("CustomFields," + customField.Name) %>:</label>
  <%
} 
        else
{  %>
                          <label for="CustomFields_<%=i %>__Value"><%=LocalizationHelpers.GlobalResource("CustomFields," + customField.Name)%>:</label>
  <%
}%>
                        </h5>
                        <div class="col2row">
                            <%= Html.Hidden("CustomFields[" + i + "].Key", customField.Name)%>
                            <%= Html.TextBox("CustomFields[" + i + "].Value", customField.Value)%>
                            <span class="field-validation-valid" id = "CustomFields_<%=i %>__Value_validationMessage"></span>
                        </div>
                        <br class="clear" />
                    </div>
  <%
    i++;
}
%>                                                   

</div>
           
<script type="text/javascript">
    var existingArticleNumbersForFields = [];
<%
    i = 0;
    foreach (CustomField customField in Model)
    {
%>
    if (Object.keys($("input[name='CustomFields[<%= i %>].Value'").rules()).length > 0) {
        $("input[name='CustomFields[<%= i %>].Value'").rules("remove");
    }
    $("input[name='CustomFields[<%= i %>].Value'").rules( "add", {
        CustomFields_<%=customField.Name %>: true
    });

    <%
        i++;
    }

    
%> 
<%
    if (ViewData["existingArticleNumbers"] != null)
    {
%>
     existingArticleNumbersForFields = <%= ViewData["existingArticleNumbers"] as string %>;
<%  
    }
%>

</script>
