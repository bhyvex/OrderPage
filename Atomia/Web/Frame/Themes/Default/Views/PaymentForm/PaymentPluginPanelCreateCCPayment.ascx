<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>

<script type="text/javascript">
    var addNonEmptyRule;
    var setSuffices;
    var suffices;
    var suffixCount;
    var setExpiresYearValidation;

	bindExpireDateCCValidation = function (currentSuffix) {
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
                                required: '<%= Html.ResourceNotEncoded("ErrorEmptyField") %>'
                            }
                        });
                    }
                }

                if (typeof (setExpiresYearValidation) != 'function') {
                    setExpiresYearValidation = function (selector, current_suffix) {
                        $(selector).rules("add", {
                            required: function (element) {
								return $("#cc-box" + current_suffix).is(':visible');
                            },
                            messages: {
                                required: '<%= Html.ResourceNotEncoded("ErrorEmptyField") %>'
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
						setExpiresYearValidation("#validate_expire" + currentSuffix, currentSuffix);
						bindExpireDateCCValidation(currentSuffix);
                    }
                }
            };
        }
        setValidation();
    });
</script>
<%
    var suffix = (null != ViewData["suffix"] && !String.IsNullOrEmpty(ViewData["suffix"].ToString())) ? ViewData["suffix"].ToString() : String.Empty;
%>
<div id="cc-box<%=suffix %>"> 
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
		<h5><label class="required" for="cc_name<%=suffix %>"><span>*</span> <%= Html.Resource("CardOwner") %></label></h5> 
		<div class="col2row"> 
			<label> 
				<input id="cc_name<%=suffix %>" type="text" name="cc_name<%=suffix %>" /> 
			</label> 
			<% if (Html.ValidationMessage("cc_name" + suffix) != null)
		       { %>
		            <p class="errorinfo"><%= Html.ValidationMessage("cc_name" + suffix)%></p>
		    <% } %>
		</div> 
		<br class="clear" /> 
	</div> 
</div> 