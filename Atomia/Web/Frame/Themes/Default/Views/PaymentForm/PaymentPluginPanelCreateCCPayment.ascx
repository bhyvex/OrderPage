<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="Atomia.Web.Plugin.ServiceReferences.AtomiaBillingApi" %>
<%@ Import Namespace="System.Linq" %>

<%
    int profilesCount = 0;
    var suffix = (null != ViewData["suffix"] && !String.IsNullOrEmpty(ViewData["suffix"].ToString())) ? ViewData["suffix"].ToString() : String.Empty;
    bool showPaymentProfileOptions = (bool)ViewData["ShowPaymentProfileOptions"];
    bool showPaymentProfileTerms = (bool)ViewData["ShowPaymentProfileTerms"];
    string paymentProfileTermsUrl = ViewData["PaymentProfileTermsUrl"].ToString();
    string pluginName = ViewData["GuiPluginName"].ToString();
    PaymentProfile[] payExProfiles = new PaymentProfile[] { };
    List<SelectListItem> selectListItems = new List<SelectListItem>();
    
    if (showPaymentProfileOptions)
    {
        if (ViewData["paymentPluginMode"] == "PAY")
        {
            PaymentProfile[] paymentProfiles = (PaymentProfile[])ViewData["paymentProfiles"];
            payExProfiles = paymentProfiles.Where(p => p.GuiPluginName == pluginName).ToArray();
            profilesCount = payExProfiles.Count();

            if (profilesCount > 0)
            {
                foreach (var profile in payExProfiles)
                {
                    string caption = string.Empty;
                    if (profile.Attributes.Any(p => p.Name == "CardType"))
                    {
                        caption += profile.Attributes.First(p => p.Name == "CardType").Value;
                    }

                    if (profile.Attributes.Any(p => p.Name == "Last4Digits"))
                    {
                        caption += string.IsNullOrEmpty(caption) ? string.Empty : " - " + profile.Attributes.First(p => p.Name == "Last4Digits").Value;
                    }

                    selectListItems.Add(new SelectListItem { Text = caption, Value = profile.Id.ToString() });
                }
            }

            selectListItems.Insert(0, new SelectListItem { Selected = true, Text = Html.Resource("SelectCard"), Value = string.Empty });
        }
    }
%>
<script type="text/javascript">
    var addNonEmptyRule;
    var setSuffices;
    var suffices;
    var suffixCount;
    var setExpiresYearValidation;

    bindDropDownListValidation = function (currentSuffix) {
        var suffix = currentSuffix; 
        $("#expires_month" + suffix).unbind().bind('change', function () {
            var ccExpire = '';
            if (($("#expires_month" + suffix).val() != '') && ($("#expires_year" + suffix).val() != '')) {
                ccExpire = $("#expires_month" + suffix).val() + $("#expires_year" + suffix).val();
            }
            $("#validate_expire" + suffix).val(ccExpire);
            $("#submit_form" + suffix).validate().element("#validate_expire" + suffix);
        });
        $("#expires_year" + suffix).unbind().bind('change', function () {
            var ccExpire = '';
            if (($("#expires_month" + suffix).val() != '') && ($("#expires_year" + suffix).val() != '')) {
                ccExpire = $("#expires_month" + suffix).val() + $("#expires_year" + suffix).val();
            }
            $("#validate_expire" + suffix).val(ccExpire);
            $("#submit_form" + suffix).validate().element("#validate_expire" + suffix);
        });
    <% if (profilesCount > 0) { %>
        $("#profile" + suffix).unbind().bind('change', function () {
            var profile = $("#profile" + suffix).val();
            $("#validate_profile" + suffix).val(profile);
            $("#submit_form" + suffix).validate().element("#validate_profile" + suffix);
        });
    <% } %>
    }

    bindAutopayCheckbox = function (currentSuffix) {
        var suffix = currentSuffix;
        $("#cc_saveccinfo" + suffix).unbind().bind("change", function () {
            if (!($("#cc_saveccinfo" + suffix).attr("checked"))) {
                $("#cc_autopay" + suffix).attr("checked",false);
            }
        });
        $("#cc_autopay" + suffix).unbind().bind("change", function () {
            if (!($("#cc_saveccinfo" + suffix).attr("checked")) && $("#cc_autopay" + suffix).attr("checked")) {
                $("#cc_saveccinfo" + suffix).attr("checked", true);
            }
        });
    }

    bindCardSelector = function (currentSuffix) {
        var suffix = currentSuffix;
        $('#cc_old_or_new input[type=radio]').change(function () {
            if ($("#cc_existing").is(':checked')) {
                $('#cc-box').hide('blind', 500);
                $('#cc_list').show();
            } else {
                $('#cc-box').show('blind', 500);
                $('#cc_list').hide();
            }
        });
    }

    $(document).ready(function () {

        if (typeof (setValidation) != 'function') {
            setValidation = function () {
                if (typeof (setSuffices) != 'function') {
                    setSuffices = function () {
                        suffixCount = 0;
                        suffices = {};
                        $("div [id^='suffixContainer']").each(function (i) {
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

                if (typeof (addNonEmptyRule) != 'function') {
                    addNonEmptyRule = function (selector, pluginSuffix) {
                        $(selector).rules("add", {
                            required: function (element) {
                                return $("#cc-box" + pluginSuffix).is(':visible');
                            },
                            messages: {
                                required: <%= Html.ResourceJavascript("ErrorEmptyField") %>
                            }
                        });
                    }
                }

                if (typeof (setSelectCardValidation) != 'function') {
                    setSelectCardValidation = function (selector, current_suffix) {
                        $(selector).rules("add", {
                            required: function (element) {
                                return !$("#cc-box" + current_suffix).is(':visible');
                            },
                            messages: {
                                required: <%= Html.ResourceJavascript("ErrorEmptyProfile") %>
                            }
                        });
                    };
                }

                if (0 < suffixCount) {
                    for (var counter = 0; counter < suffixCount; counter++) {
                        var currentSuffix = suffices[counter];
                        addNonEmptyRule("#cc_number" + currentSuffix, currentSuffix);
                        addNonEmptyRule("#ccv" + currentSuffix, currentSuffix);
                        addNonEmptyRule("#cc_name" + currentSuffix, currentSuffix);
                        addNonEmptyRule("#validate_expire" + currentSuffix, currentSuffix);
                        
                        <% if (profilesCount > 0) { %>
                        if ($("#cc-box"+currentSuffix).isPrototypeOf(':visible')) {
                            setSelectCardValidation("#validate_profile" + currentSuffix, currentSuffix);
                        }
                        <% } %>
                        bindDropDownListValidation(currentSuffix);
                        bindAutopayCheckbox(currentSuffix);
                        bindCardSelector(currentSuffix);
                    }
                }

            };
        }
        setValidation();
    });
</script>
<%
    if (showPaymentProfileOptions && ViewData["paymentPluginMode"] == "PAY")
    {
%>        
    	<div id="cc-options<%= suffix %>" class="formrow"> 
			<h5><label class="required" for="cc_profile<%=suffix%>"><span>*</span> <%=Html.Resource("CreditCardToUse")%></label></h5> 
			<div id="cc_old_or_new" class="col2row"> 
				<input id="cc_new<%= suffix %>" type="radio" name="UseSavedCCInfo<%= suffix %>" value="no"/> <label for="cc_new<%= suffix %>"><%= Html.Resource("New") %></label><br />
				<input id="cc_existing<%= suffix %>" type="radio" name="UseSavedCCInfo<%= suffix %>" 
				<% if (profilesCount > 0) { Response.Write("checked=\"checked\""); }%> value="yes" /> 
				<label for="UseSavedCCInfo<%=suffix%>"><%=Html.Resource("Existing")%></label> 
                
				<%= Html.DropDownList("profile" + suffix, new SelectList(selectListItems, "Value", "Text", selectListItems[0].Value), new { @class = "auto-width"}) %>
				<input type="hidden" id="validate_profile<%= suffix %>" name="validate_profile<%=suffix %>" value=""/>			
				<% if (Html.ValidationMessage("validate_profile" + suffix) != null)
				   { %>
						<p class="errorinfo"><%= Html.ValidationMessage("validate_profile" + suffix)%></p>
				<% } %>
			</div> 
			<br class="clear" /> 
		</div> 
<%
    }
%>

<div id="cc-box<%=suffix %>" <% if (profilesCount > 0) { Response.Write("style=\"display:none\""); }%> > 
	<div class="formrow"> 
		<h5><label class="required" for="cc_number<%=suffix %>"><span>*</span> <%= Html.Resource("CardNumber") %></label></h5> 
		<div class="col2row"> 
			<label> 
				<input id="cc_number<%=suffix %>" type="text" name="cc_number<%=suffix %>" /> 
			</label> 
			<% if (Html.ValidationMessage("cc_number" + suffix) != null)
		       { %>
		            <p class="errorinfo"><%= Html.ValidationMessage("cc_number" + suffix)%></p>
		    <% } %>		    
		</div> 
		<br class="clear" /> 
	</div> 

	<div class="formrow"> 
		<h5><label class="required" for="ccv<%=suffix %>"><span>*</span> <%= Html.Resource("Ccv") %></label></h5> 
		<div class="col2row"> 
			<label> 
				<input id="ccv<%=suffix %>" type="text" name="ccv<%=suffix %>" /> 
			</label> 
			<% if (Html.ValidationMessage("ccv" + suffix) != null)
		       { %>
		            <p class="errorinfo"><%= Html.ValidationMessage("ccv" + suffix)%></p>
		    <% } %>
		</div> 
		<br class="clear" /> 
	</div> 
	
	<div class="formrow"> 
		<h5><label class="required"><span>*</span> <%= Html.Resource("Expires") %></label></h5> 
		<div class="col2row"> 
			<select class="auto-width" id="expires_month<%=suffix %>" name="expires_month<%=suffix %>"> 
				<option selected="selected" disabled="disabled"></option> 
				<option value="1">01</option> 
				<option value="2">02</option> 
				<option value="3">03</option> 
				<option value="4">04</option> 
				<option value="5">05</option> 
				<option value="6">06</option> 
				<option value="7">07</option> 
				<option value="8">08</option> 
				<option value="9">09</option> 
				<option value="10">10</option> 
				<option value="11">11</option> 
				<option value="12">12</option> 
			</select> / 
			<select class="auto-width" id="expires_year<%=suffix %>" name="expires_year<%=suffix %>"> 
				<option selected="selected" disabled="disabled"></option> 
				<% 
		        var currentDate = DateTime.Now;		    
		        for (var i = 0; i < 15; i++)
                {
                %>
                <option value="<%= currentDate.Year %>"><%= currentDate.Year %></option>
				<%
                    currentDate = currentDate.AddYears(1);
                } 
		        %>
			</select> 
			<input type="hidden" id="validate_expire<%=suffix %>" name="validate_expire<%=suffix %>" value=""/>			
            <% if (Html.ValidationMessage("validate_expire" + suffix) != null)
		       { %>
		            <p class="errorinfo"><%= Html.ValidationMessage("validate_expire" + suffix)%></p>
		    <% } %>
		</div> 
		<br class="clear" /> 
	</div> 
	<div class="formrow"> 
		<h5><label class="required" for="cc_name<%=suffix%>"><span>*</span> <%=Html.Resource("CardOwner")%></label></h5> 
		<div class="col2row"> 
			<label> 
				<input id="cc_name<%=suffix%>" type="text" name="cc_name<%=suffix%>" /> 
			</label> 
			<%
                if (Html.ValidationMessage("cc_name" + suffix) != null)
                {
            %>
		            <p class="errorinfo"><%=Html.ValidationMessage("cc_name" + suffix)%></p>
		    <%
                }
		    %>
		</div> 
		<br class="clear" /> 
	</div> 
<%
    if (showPaymentProfileOptions && (ViewData["paymentPluginMode"] == "CREATE" || ViewData["paymentPluginMode"] == "PAY"))
    {
%>	
	<div class="formrow"> 
		<h5><label class="required" for="cc_profile<%=suffix%>"><%=Html.Resource("PaymentProfileOptions")%></label></h5> 
		<div class="col2row"> 
            <input id="cc_saveccinfo<%=suffix%>" type="checkbox" name="cc_saveccinfo<%=suffix%>" /> 
			<label for="cc_saveccinfo<%=suffix%>"><%=Html.Resource("SaveCCInfo")%></label><br />

            <input id="cc_autopay<%=suffix%>" type="checkbox" name="cc_autopay<%=suffix%>" /> 
			<label for="cc_autopay<%=suffix%>"><%=Html.Resource("Autopay")%></label><br />
<%
    if (showPaymentProfileTerms)
    {
%>
        <p><a href="<%= paymentProfileTermsUrl %>" target="_blank"><%= Html.Resource("TermsAndConditions") %></a></p>
<%
    }
%>
		</div> 
		<br class="clear" /> 
	</div> 
<%            
    }
%>
</div>
