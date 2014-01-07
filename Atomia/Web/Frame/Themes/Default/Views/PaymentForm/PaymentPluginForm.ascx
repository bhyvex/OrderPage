<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Atomia.Web.Plugin.PaymentForm.Helpers" %>

<%
    var pluginsToLoad = ViewData["paymentPluginsToLoad"] as List<Atomia.Billing.Core.Common.PaymentPlugins.GuiPaymentPluginData>;
    var mode = ViewData["paymentPluginsMode"] as string;
    var suffix = (null != ViewData["suffix"] && !String.IsNullOrEmpty(ViewData["suffix"].ToString())) ? ViewData["suffix"].ToString() : String.Empty;
    var defaultPluginName = ViewData["DefaultPaymentPlugin"].ToString();
%>

<script type="text/javascript">

    var setSuffices;
    var suffices;
    var suffixCount;
    
    var bindFunctions;
    var validator;

    var showPaymentDiv;
    var setPaymentSelectValidate;

    $(document).ready(function() {
	
		<%
            for(int i = 0; i < pluginsToLoad.Count; i++) 
			{
		%>
			$("input[value='<%=pluginsToLoad[i].Name %>']").click(function() {
				showPaymentDiv('<%=pluginsToLoad[i].Name %>','<%=suffix %>');
			});
		<%
            }
		%>
	
	
        if (typeof (bindFunctions) != 'function') {
            bindFunctions = function() {
                
                if (typeof (setSuffices) != 'function') {
                    setSuffices = function() {
                        suffixCount = 0;
                        suffices = {};
                        $("div [id^='suffixContainer']").each(function(i) {
                            var currentID = $(this).attr("id");
                            var content = $(this).html();
                            if (('' !== content && -1 < currentID.indexOf(content)) || '' === content) {
                                suffices[i] = content;
                                suffixCount++;
                            }
                        });
                    };
                }
                setSuffices();
				
                if (typeof (showPaymentDiv) != 'function') {
                    showPaymentDiv = function(pluginName, plugin_suffix) {
                        $("[name='paymentPluginCommonName" + plugin_suffix + "']").hide();
						$("#paymentPlugin" + pluginName + plugin_suffix).show();
                        $("#payment_select_validate" + plugin_suffix).parents("form:first").validate().element("#payment_select_validate" + plugin_suffix);
                    };
					
                }
				
                if (typeof (setPaymentSelectValidate) != 'function') {
                    setPaymentSelectValidate = function(suffix) {
                        $("#payment_select_validate" + suffix).rules("add", {
                            required: function(element) {
                                if (undefined === $("input[name='pluginSelector" + suffix + "']:checked").val() && $("#payment-select" + suffix).length > 0) {
                                    return true;
                                }

                                return false;
                            },
                            messages: {
                                required: <%= Html.ResourceJavascript("ErrorSelectPaymentMethod") %>
                            }
                        });
                    };
                };

                if (0 < suffixCount) {
                    for (var counter = 0; counter < suffixCount; counter++) {
                        validator = $("#submit_form" + suffices[counter]).validate();
                        setPaymentSelectValidate(suffices[counter]);
                    }
                }
            };
            bindFunctions();
        }
    });
</script>

<% 
    if (mode == "CREATE" || mode == "PAY")
    {
        if (pluginsToLoad.Count > 1)
        {
%>
            <div class="formrow" id="paymentPluginList<%=suffix %>"> 
                <h5><label class="required" for="payment-select<%=suffix %>"><span>*</span> <%= Html.Resource("PaymentMethod") %></label></h5> 
                <div id="payment-select<%=suffix %>" class="col2row"> 
<%
            for(int i = 0; i < pluginsToLoad.Count; i++) 
			{
%>
				<label><input type="radio" <%= pluginsToLoad[i].Name == defaultPluginName ? "checked='checked'" : ""%> name="pluginSelector<%=suffix %>" value="<%=pluginsToLoad[i].Name %>"/><%= Html.Resource(pluginsToLoad[i].Name) %></label><br />
<%
            }
%>
                </div> 
	            <br class="clear" /> 
            </div> 
<%
        }
        else if(pluginsToLoad.Count > 0)
        {
%>
            <input type="hidden" name="pluginSelector<%=suffix %>" value="<%= pluginsToLoad[0].Name%>" />
<%
        }
%>
        <input type="hidden" id="payment_select_validate<%=suffix %>" name="payment_select_validate<%=suffix %>" value=""/>
<%
        foreach (var plugin in pluginsToLoad)
        {
            string visibility = (plugin.Name == defaultPluginName || pluginsToLoad.Count == 1) ? string.Empty : "display:none";
            bool showPaymentProfileOptions = false;
            bool showPaymentProfileTerms = false;
            Boolean.TryParse((string)LocalConfigurationHelper.FetchPluginSetting(plugin.Name, "ShowPaymentProfileOptions"), out showPaymentProfileOptions);
            Boolean.TryParse((string)LocalConfigurationHelper.FetchPluginSetting(plugin.Name, "ShowPaymentProfileTerms"), out showPaymentProfileTerms);
            ViewData["ShowPaymentProfileOptions"] = showPaymentProfileOptions;
            ViewData["ShowPaymentProfileTerms"] = showPaymentProfileTerms;
            ViewData["PaymentProfileTermsUrl"] = (string)LocalConfigurationHelper.FetchPluginSetting(plugin.Name, "PaymentProfileTermsURL");
            ViewData["paymentPluginMode"] = plugin.Mode;
            ViewData["GuiPluginName"] = plugin.Name;
%>
            <div name="paymentPluginCommonName<%=suffix %>" id="paymentPlugin<%=plugin.Name %><%=suffix %>" style="<%= visibility %>">
            <%
                Html.RenderPartial("PaymentPluginPanelCreate" + plugin.Name, ViewData);
            %>
            </div>
<%
        }
    }
%>
<div id="suffixContainer<%=suffix %>" style="display:none"><%=suffix %></div>
