<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage`1[[Atomia.Web.Plugin.PublicOrder.Models.IndexForm, Atomia.Web.Plugin.PublicOrder]]" %>
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
			        <div class="col2row">
                         <%= Html.RadioButton("Selected", "first", new { @id = "first", @checked = "checked" })%><label for="first"><%= Html.Resource("First")%></label><br  />
                         <%= Html.RadioButton("Selected", "second", new { @id = "second" })%><label for="second"><%= Html.Resource("Second")%></label><br  />
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
		        <p class="actions"><a class="b_b_create" id="orderbutton" href="javascript:void(0);"><%= Html.Resource("Continue")%></a></p>

            <% Html.EndForm(); %>            
		</div>
	</div>
    <script type="text/javascript">
		var notificationParams = {};
		notificationParams.wasAnError = "<%= ViewData["WasAnError"] %>";
		notificationParams.NotificationText = "<%= Html.ResourceNotEncoded("NotificationText") %>";
		notificationParams.ValidationErrorsErrorNumDomains = "<%= Html.ResourceNotEncoded("ValidationErrors, ErrorNumDomains") %>";
		notificationParams.NotificationTextInvalidDomain = "<%= Html.ResourceNotEncoded("NotificationTextInvalidDomain") %>";
		notificationParams.title = "<%= Html.Resource("NotificationHeader") %>";

//        var validationParams = {};
//        validationParams.validateDomainUrl = '<%= Url.Action("ValidateDomain", new { controller = "PublicOrder" }) %>';
//        validationParams.domainExistsInTheSystemUrl = '<%= Url.Action("DomainExistsInTheSystem", new { controller = "PublicOrder" }) %>';

        $(document).ready(function() {
//            var action = "<%=Url.Action("LoadProductsIntoSession", new { controller = "PublicOrder", area = "PublicOrder" })%>";
//            $.postJSON(action, function() { });            
			//Remove sidebar

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
   
            
            $('#notification').notification({
              showTimeout: 1000,
              hideTimeout: 100000
            });
            
            setNotificationMessage(notificationParams);
            
            var validator = $("#submit_form").validate({
                onfocusout: function(element) {
                    if ($(element).attr('id') != 'IndexForm_Domain') {
                        $(element).valid();
                    }
                }
            });
            $("#protected1").show();
            $("#protected2").hide();
			
			bindFirstClick();
			bindSecondClick();
            bindOrderbuttonClick();
            
            $("#first").click();
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
                "ValidateOwnDomainBasedOnTLD", function(value, element, params) {
                    return ValidateOwnDomainBasedOnTLD(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateOwnDomainExistanceInSystem", function(value, element, params) {
                    return ValidateOwnDomainExistanceInSystem(value, element, params); 
                }
            );
            
            
        }

        function AddValidationRules() {
            
            $('#Domains').rules("add", {
                ValidateNumOfDomains: {
                    NumberOfDomainsAllowed: "<%= ViewData["NumberOfDomainsAllowed"].ToString() %>"
                },
                messages: {
                    ValidateNumOfDomains: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorNumDomains") %>'
                }
            });

            $('#Domains').rules("add", {
                ValidateGroupOfDomains: {
                    RegDomainFront: "<%= ViewData["RegDomainFront"].ToString() %>", 
                    RegDomain: "<%= ViewData["RegDomain"].ToString() %>"
                },
                messages: {
                    ValidateGroupOfDomains: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidDomains") %>'
                }
            });

            $('#Domains').rules("add", {
                ValidateDomainsLength: {
                    AllowedDomainLength: "<%= ViewData["AllowedDomainLength"] %>"
                },
                messages: {
                    ValidateDomainsLength: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorStringLength") %>'
                }
            });

            $('#Domain').rules("add", {
                ValidateOwnDomainBasedOnTLD: {
                    Url: '<%= Url.Action("ValidateDomain", new { controller = "PublicOrder" }) %>'
                },
                messages: {
                    ValidateOwnDomainBasedOnTLD: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidDomains") %>'
                }
            });

            $('#Domain').rules("add", {
                ValidateOwnDomainExistanceInSystem: {
                    Url: '<%= Url.Action("DomainExistsInTheSystem", new { controller = "PublicOrder" }) %>'
                },
                messages: {
                    ValidateOwnDomainExistanceInSystem: '<%= Html.ResourceNotEncoded("ValidationErrors, DomainExists") %>'
                }
            });

        }

    </script>
    <%--<%= Html.ClientSideValidation<IndexForm>("IndexForm")
        --.AddRule("Domains", new CustomRule("ValidateNumOfDomains", new { NumberOfDomainsAllowed = ViewData["NumberOfDomainsAllowed"].ToString() }, Html.ResourceNotEncoded("ValidationErrors, ErrorNumDomains")))
        --.AddRule("Domains", new CustomRule("ValidateGroupOfDomains", new { RegDomainFront = ViewData["RegDomainFront"].ToString(), RegDomain = ViewData["RegDomain"].ToString()}, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidDomains")))
        --.AddRule("Domains", new CustomRule("ValidateDomainsLength", new { AllowedDomainLength = ViewData["AllowedDomainLength"] }, Html.ResourceNotEncoded("ValidationErrors, ErrorStringLength")))
        --.AddRule("Domain", new CustomRule("ValidateOwnDomainBasedOnTLD", new { Url = Url.Action("ValidateDomain", new { controller = "PublicOrder" }) }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidDomains")))
        --.AddRule("Domain", new CustomRule("ValidateOwnDomainExistanceInSystem", new { Url = Url.Action("DomainExistsInTheSystem", new { controller = "PublicOrder" }) }, Html.ResourceNotEncoded("ValidationErrors, DomainExists")))%>
        --%>    
     <script type="text/javascript">
         var IndexFormDomainsRequiredMessage = '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>';
         var ErrorEmptyFieldMessage = '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>';
         $(document).ready(function() {
             setIndexFormDomainsRules(IndexFormDomainsRequiredMessage);
             setIndexFormDomainRules(ErrorEmptyFieldMessage);
         });
    </script>
</asp:Content>