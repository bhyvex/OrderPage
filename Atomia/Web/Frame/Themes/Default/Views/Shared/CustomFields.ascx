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
                        </div>
                        <br class="clear" />
                    </div>
  <%
    i++;
}
%>                                                   

</div>
           
<script type="text/javascript">
<%
    i = 0;
    foreach (CustomField customField in Model)
{
  %>
    $("input[name='CustomFields[<%= i %>].Value'").rules( "add", {
        CustomFields_<%=customField.Name %>: true,
        messages: {
            CustomFields_<%=customField.Name %>: "Invalid data"
        }
    });

  <%
    i++;
}
%> 

</script>

   
