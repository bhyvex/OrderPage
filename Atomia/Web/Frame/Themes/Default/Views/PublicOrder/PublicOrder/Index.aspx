<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage`1[[Atomia.Web.Plugin.PublicOrder.Models.IndexForm, Atomia.Web.Plugin.PublicOrder]]" %>
<%@ Import Namespace="Atomia.Web.Plugin.PublicOrder.Helpers" %>
<%@ Import Namespace="Atomia.Web.Plugin.PublicOrder.Models"%>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("PageTitle")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <% var area = Url.RequestContext.RouteData.DataTokens["area"].ToString(); %>  
    <div class="settingsbox">
        <h3><%= Html.Resource("Title")%></h3>
        <div class="settingsboxinner">
            <%= Html.NotificationDialog("", "", ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui/{1}", Session["Theme"], "icn_close_button_s.png")), ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png")))%>
            <% Html.EnableClientValidation(); %>
            <% Html.BeginForm("Index", "PublicOrder", new { area }, FormMethod.Post, new { @id = "submit_form", @class="autocomplete_off" }); %>
                <% Html.EditorForModel(); %>
                <p>
                    <%= Html.ResourceNotEncoded("Info")%>
                </p>
                <div class="formrow">
                    <h5>
                        <label class="required">
                            <span>*</span><%= Html.Resource("Alternative")%>:
                        </label>
                    </h5>
                    <div class="col2row" id="OrderOptionsContainer">
                        <%
                            if (((List<OrderOptions>)ViewData["OrderOptions"]).Exists(oo => oo.Value == OrderOptions.New.Value))
                            {%>
                               <%=Html.RadioButton("Selected", "first", new {@id = "first"})%><label for="first"><%=Html.Resource("First")%></label><br  />
                          <%}

                            if (((List<OrderOptions>)ViewData["OrderOptions"]).Exists(oo => oo.Value == OrderOptions.Own.Value))
                            {%>
                               <%= Html.RadioButton("Selected", "second", new { @id = "second" })%><label for="second"><%= Html.Resource("Second")%></label><br  />
                          <%}

                            if (((List<OrderOptions>)ViewData["OrderOptions"]).Exists(oo => oo.Value == OrderOptions.Sub.Value))
                            {%>
                                <%= Html.RadioButton("Selected", "subdomain", new { @id = "subdomain" })%><label for="subdomain"><%= Html.Resource("Third")%></label><br  />
                          <%}%>
                    </div>
                   <br class="clear" />
                </div>
                
                <div class="formrow" id="protected1" style="display: block;">
                    <h5>
                        <label class="required">
                            <span>*</span><%= Html.Resource("DomainName")%>:
                        </label>
                    </h5>
                    <div class="col2row">
                        <%= Html.TextArea("Domains", new { cols = "40", rows = "5", style = "width: 290px; height: 75px;" })%>
                        <%= Html.ValidationMessage("Domains")%>
                    </div>
                    <br class="clear" />
                </div>
                
                <div class="formrow" id="protected2" style="display: block;">
                    <h5>
                        <label class="required">
                            <span>*</span><%= Html.Resource("DomainName")%>:
                        </label>
                    </h5>
                    <div class="col2row">
                        <%= Html.TextBox("Domain")%>
                        <%= Html.ValidationMessage("Domain")%>
                    </div>
                    <br class="clear" />
                </div>

                <div class="formrow" id="protected3" style="display: block;">
                    <h5>
                        <label class="required">
                            <span>*</span><%= Html.Resource("DomainName")%>:
                        </label>
                    </h5>
                    <div class="col2row">
                        <%= Html.TextBox("SubDomain")%> <em class="quiet">.<%= (string)ViewData["SubdomainValue"] %></em> <br />
                        <%= Html.ValidationMessage("SubDomain")%>
                    </div>
                    <br class="clear" />
                </div>

                <p class="actions"><a class="button green" id="orderbutton" href="javascript:void(0);"><%= Html.Resource("Continue")%></a></p>

            <% Html.EndForm(); %>            
        </div>
    </div>
    <script type="text/javascript">
        var IndexFormDomainsRequiredMessage = <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>;
        var ErrorEmptyFieldMessage = <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>;
        
        var notificationParams = {};
        notificationParams.wasAnError = "<%= ViewData["WasAnError"] %>";
        notificationParams.NotificationText = "<%= Html.ResourceNotEncoded("NotificationText") %>";
        notificationParams.ValidationErrorsErrorNumDomains = "<%= Html.ResourceNotEncoded("ValidationErrors, ErrorNumDomains") %>";
        notificationParams.NotificationTextInvalidDomain = "<%= Html.ResourceNotEncoded("NotificationTextInvalidDomain") %>";
        notificationParams.title = "<%= Html.Resource("NotificationHeader") %>";
        var subdomain = '<%= ViewData["SubdomainValue"] ?? "" %>';

        $(document).ready(function() {
            AddValidationRules();
            AddValidationMethods();

            var sidebarContentSubmenu = $.trim($('#sidebar .submenu').html());
            if (sidebarContentSubmenu == '') {
                $('#sidebar .submenu').remove();
            }
            var sidebarContent = $.trim($('#sidebar').html());
            if (sidebarContent == '') {
                $('#sidebar').remove();
            }
   
            // select first order option (which ever it is)
            if ($('#OrderOptionsContainer > input').length > 0) {
                $('#OrderOptionsContainer > input').first().attr('checked', 'checked');
            }
            
            $('#notification').notification({
              showTimeout: 1000,
              hideTimeout: 100000
            });
            
            setNotificationMessage(notificationParams);
            
            var validator = $("#submit_form").validate();

            $("#submit_form").validate().settings.onfocusout = function(element) {
                if ($(element).attr('id') != 'IndexForm_Domain') {
                    $(element).valid();
                }
            }
         
            setIndexFormDomainsRules(IndexFormDomainsRequiredMessage);
            setIndexFormDomainRules(ErrorEmptyFieldMessage);

            if ($('div[id^=protected]').length > 0) {
                $('div[id^=protected]').each(function(index) {
                    $(this).hide();
                    if (index == 0) {
                        $(this).show();
                    }
                });
            }

            var protectedContainers = ["protected1","protected2","protected3"];
            var inputFieldContainers = ["Domain","Domains"];

            bindSelectClick("first", protectedContainers, inputFieldContainers, 0);
            bindSelectClick("second", protectedContainers, inputFieldContainers, 1);
            bindSelectClick("subdomain", protectedContainers, inputFieldContainers, 2);

            bindOrderbuttonClick();
        });
        
        function AddValidationMethods(){
            
            jQuery.validator.addMethod(
                "ValidateNumOfDomains", function(value, element, params) {
                    return ValidateNumOfDomains(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateGroupOfDomains", function(value, element, params) {
                    return ValidateGroupOfDomains(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateDomainsLength", function(value, element, params) {
                    return ValidateDomainsLength(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateOwnDomainsBasedOnTLD", function(value, element, params) {
                    var returnData = false;
                    if (typeof value !== 'undefined' && value.length > 0) {
                        var values = { domainName: value };
                        returnData = ValidateValueServerSide(values, element, params.Url, 'POST');
                    }
                    else {
                        returnData = true;
                    }

                    return returnData;
                }
            );

            jQuery.validator.addMethod(
                "ValidateOwnDomainBasedOnTLD", function(value, element, params) {
                    var returnData = false;
                    if ($('#' + params.ParentContainer).is(':checked') && (typeof value !== 'undefined') && (value.length > 0)) {
                        var values = { domainName: value };
                        returnData = ValidateValueServerSide(values, element, params.Url, 'POST');
                    }
                    else {
                        returnData = true;
                    }

                    return returnData;
                }
            );

            jQuery.validator.addMethod(
                "ValidateOwnDomainExistanceInSystem", function(value, element, params) {
                    var returnData = false;
                    if ($('#' + params.ParentContainer).is(':checked') && (typeof value !== 'undefined') && (value.length > 0)) {
                        var values = { domainName: value };
                        returnData = !ValidateValueServerSide(values, element, params.Url, 'POST');
                    }
                    else {
                        returnData = true;
                    }

                    return returnData; 
                }
            );
            
            jQuery.validator.addMethod(
                "ValidateSubDomainExistanceInSystem", function(value, element, params) {
                    var returnData = false;
                    if ($('#' + params.ParentContainer).is(':checked') && (typeof value !== 'undefined') && (value.length > 0)) {
                        var values = { domainName: value + "." + subdomain };
                        returnData = !ValidateValueServerSide(values, element, params.Url, 'POST');
                    }
                    else {
                        returnData = true;
                    }

                    return returnData;
                }
            );

            jQuery.validator.addMethod(
                "ValidateSubDomain", function(value, element, params) {
                    var returnData = false;
                    if ($('#' + params.ParentContainer).is(':checked') && (typeof value !== 'undefined') && (value.length > 0)) {
                        var values = { domainName: value + "." + subdomain };
                        returnData = ValidateValueServerSide(values, element, params.Url, 'POST');
                    }
                    else {
                        returnData = true;
                    }

                    return returnData;
                }
            );

            jQuery.validator.addMethod(
                "ValidateSubDomainIsSingleLevel", function(value, element, params) {
                    if (value.indexOf('.') != -1) {
                        return false;
                    }

                    return true;
                }
            );
        }

        function AddValidationRules() {
            
            $('#Domains').rules("add", {
                ValidateNumOfDomains: {
                    NumberOfDomainsAllowed: "<%= ViewData["NumberOfDomainsAllowed"].ToString() %>"
                },
                messages: {
                    ValidateNumOfDomains: <%= Html.ResourceJavascript("ValidationErrors, ErrorNumDomains") %>
                }
            });

            $('#Domains').rules("add", {
                ValidateGroupOfDomains: {
                    RegDomainFront: "<%= ViewData["RegDomainFront"].ToString() %>", 
                    RegDomain: "<%= ViewData["RegDomain"].ToString() %>"
                },
                messages: {
                    ValidateGroupOfDomains: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidDomains") %>
                }
            });

            $('#Domains').rules("add", {
                ValidateDomainsLength: {
                    AllowedDomainLength: "<%= ViewData["AllowedDomainLength"] %>"
                },
                messages: {
                    ValidateDomainsLength: <%= Html.ResourceJavascript("ValidationErrors, ErrorStringLength") %>
                }
            });

            $('#Domains').rules("add", {
                ValidateOwnDomainsBasedOnTLD: {
                    Url: '<%= Url.Action("ValidateDomains", new { controller = "PublicOrder" }) %>',
                    ParentContainer: 'first'
                },
                messages: {
                    ValidateOwnDomainsBasedOnTLD: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidDomains") %>
                }
            });

            $('#Domain').rules("add", {
                ValidateOwnDomainBasedOnTLD: {
                    Url: '<%= Url.Action("ValidateDomain", new { controller = "PublicOrder" }) %>',
                    ParentContainer: 'second'
                },
                messages: {
                    ValidateOwnDomainBasedOnTLD: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidDomains") %>
                }
            });

            $('#Domain').rules("add", {
                ValidateOwnDomainExistanceInSystem: {
                    Url: '<%= Url.Action("DomainExistsInTheSystem", new { controller = "PublicOrder" }) %>',
                    ParentContainer: 'second'
                },
                messages: {
                    ValidateOwnDomainExistanceInSystem: <%= Html.ResourceJavascript("ValidationErrors, DomainExists") %>
                }
            });

            $('#SubDomain').rules("add", {
                ValidateSubDomainIsSingleLevel: {
                    ParentContainer: 'subdomain'
                },
                messages: {
                    ValidateSubDomainIsSingleLevel: <%= Html.ResourceJavascript("ValidationErrors, ErrorMultiLevelSubdomain") %>
                }
            });

            $('#SubDomain').rules("add", {
                ValidateSubDomain: {
                    Url: '<%= Url.Action("ValidateDomain", new { controller = "PublicOrder" }) %>',
                    ParentContainer: 'subdomain'
                },
                messages: {
                    ValidateSubDomain: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidSubdomain") %>
                }
            });

            $('#SubDomain').rules("add", {
                ValidateSubDomainExistanceInSystem: {
                    Url: '<%= Url.Action("DomainExistsInTheSystem", new { controller = "PublicOrder" }) %>',
                    ParentContainer: 'subdomain'
                },
                messages: {
                    ValidateSubDomainExistanceInSystem: <%= Html.ResourceJavascript("ValidationErrors, DomainExists") %>
                }
            });
        }
    </script>
</asp:Content>
