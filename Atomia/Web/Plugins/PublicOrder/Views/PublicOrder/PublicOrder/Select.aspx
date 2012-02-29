<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage`1[[Atomia.Web.Plugin.PublicOrder.Models.SubmitForm, Atomia.Web.Plugin.PublicOrder]]" %>
<%@ Assembly Name="Atomia.Web.Plugin.DomainSearch" %>
<%@ Assembly Name="Atomia.Web.Plugin.Cart" %>
<%@ Import Namespace="Atomia.Web.Plugin.Cart.Models" %>
<%@ Import Namespace="Atomia.Web.Plugin.PublicOrder.Models"%>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Html.Resource("PageTitle")%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <% var area = Url.RequestContext.RouteData.DataTokens["area"].ToString();%>
    <script type="text/javascript">
        $(document).ready(function() {
            $("#submit_form").validate();
            //Remove sidebar
            var sidebarContentSubmenu = $.trim($('#sidebar .submenu').html());
            if (sidebarContentSubmenu == '') {
                $('#sidebar .submenu').remove();
            }
            var sidebarContent = $.trim($('#sidebar').html());
            if (sidebarContent == '') {
                $('#sidebar').remove();
            }
        });
    </script>    
    <%= Html.ValidationMessage("GeneralError")%>
    <div class="settingsbox">
        <h3><%= Html.Resource("Title")%></h3>
        <div class="settingsboxinner">
            <%= Html.NotificationDialog("", "", ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui/{1}", Session["Theme"], "icn_close_button_s.png")), ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png")))%>
            
	        <p>
		        <%= Html.ResourceNotEncoded("Info")%>
	        </p>
            <h2>
                <%= Html.Resource("DomainName")%>   
            </h2>
	        <%
            if (!(bool)ViewData["firstOption"])
            {
                Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml domain = (Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml)Session["singleDomain"];
                string domainName = domain.ProductName;
                string productId = domain.ProductID;
	        %>
	            <div class="formrow">
		            <h4><%= Html.Resource("Alternative")%>:</h4>
		            <div class="col2row">
			            <p>
				            <%= Html.Resource("AltInfo")%>
			            </p>

		            </div>
		            <br class="clear" />
	            </div>
	            <div class="formrow">
		            <label class="required">
		                <span>*</span><%= Html.Resource("DomainName")%>:
		            </label>
		            <div class="col2row">
			            <p id="singleDomain" rel="<%= Html.Encode(productId)%>">
				            <%= Html.Encode(domainName)%>
			            </p>

		            </div>
		            <br class="clear" />
	            </div>
            <%
            }
            else
            {
            %>
                <div id="DomainSearchContainer">
                </div>
            <%
            }
            %>
            <% Html.EnableClientValidation(); %>
            <% Html.BeginForm("Select", "PublicOrder", new { area }, FormMethod.Post, new { @id = "submit_form", autocomplete = "off" }); %>
		        <% Html.EditorForModel(); %>
                <h2>
		            <%= Html.Resource("Package")%>
		        </h2>
			
                <div class="formrow" style="margin-top: 8px;">
                    <h5>
                        <label class="required"><span>*</span> <%= Html.Resource("AvalablePackages")%>:</label>
                    </h5>
                    <div class="col2row">
                    <%if (ViewData["radioList"] != null)
                    {
                        List<RadioRow> list = (List<RadioRow>)ViewData["radioList"];
                        for (int i = 0; i < list.Count; i++)
                        {
                    %>
                        <p>
                            <%
                            if(i == 0)
                            {
                            %>
  				                <%= Html.RadioButton("RadioProducts", list[i].productId, new { @id = "radioProducts" + i, @checked = "checked" })%> <label for="radioProducts<%= i%>"><strong><%= Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].productNameDesc)) %></strong></label><br />
  				            <% 
                            }
                            else
                            {
                            %>
                                 <%= Html.RadioButton("RadioProducts", list[i].productId, new { @id = "radioProducts" + i })%> <label for="radioProducts<%= i%>"><strong><%= Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].productNameDesc)) %></strong></label><br />                   
                            <%
                            }      
                            %>
                            <input type="hidden" value="<%= Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].productNameDesc)) %>" />
                        </p>
                        <p class="package-description">
                            <%= Html.Resource("Description")%>:<br class="clear" />
                            <%= Html.ResourceNotEncoded(string.Format("{0}Common, {1}", Session["Theme"], list[i].info)) %>
                        </p>
                    <%
                        }
                    }
                    %>
                    </div>
                    <br class="clear" />
                </div>
			    <div id="MainDomainWrapperOuter">  
		            <h2 id="MainDomainHeader" style="display:none;">
		                <%= Html.Resource("MainDomainTitle")%>
		            </h2>
    			
		            <div class="formrow" id="MainDomainWrapper" style="display:none;">
			            <h5>
			              <label class="required" for="main_domain"><span>*</span><%= Html.Resource("MainDomainTitle")%>:</label>
			            </h5>
			            <div class="col2row">
				            <select name="MainDomainSelect" id="MainDomainSelect" style="width: 175px;"></select>
				            <%= Html.ValidationMessage("MainDomainSelect")%>
			            </div>
			           <br class="clear" />
		            </div>
		        </div>
		        <div id="invoiceDivWrapper">
			        <h2><%= Html.Resource("Invoice")%></h2>
			        <div class="formrow" id="invoiceDiv">
					    <div id="CartContainer">
					        <table class="invoicespec list" id="product_list"></table>
					    </div>
		            </div>                    
                    <p id="vatValidationInfo" style="font-style:italic"></p>
	            </div>
                <%= Html.Hidden("ArrayOfProducts")%>
                <%= Html.ValidationMessage("ArrayOfProducts")%>
                
	            <h2><%= Html.Resource("ContactInformation")%></h2>
	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_name"><span>*</span><%= Html.Resource("FirstName")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("ContactName")%>
			            <%= Html.ValidationMessage("ContactName")%>
		            </div>
		           <br class="clear" />
	            </div>
			    
	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_name"><span>*</span><%= Html.Resource("LastName")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("ContactLastName")%>
			            <%= Html.ValidationMessage("ContactLastName")%>
		            </div>
		           <br class="clear" />
	            </div>
			    
	            <div class="formrow">
		            <h5>
		              <label for="contact_company"><%= Html.Resource("Company")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Company")%>
			            <%= Html.ValidationMessage("Company")%>
		            </div>
		            <br class="clear" />
	            </div>
    
                <% 
                    if ((bool)ViewData["ShowPersonalNumber"])
                    {
                %>
	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_numb"><span>*</span><%=Html.Resource("PersonalNum")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%=Html.TextBox("OrgNumber")%> <span class="f_example"><%=Html.Resource("xxxx")%></span>			            
                        <%=Html.ValidationMessage("OrgNumber")%>
		            </div>
		            <br class="clear" />
	            </div>
                <%
                    }
                %>
                
	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_address"><span>*</span><%= Html.Resource("Address")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Address")%>
			            <%= Html.ValidationMessage("Address")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label for="contact_address2"><%= Html.Resource("Address2")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Address2")%>
			            <%= Html.ValidationMessage("Address2")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_postnummer"><span>*</span><%= Html.Resource("PostNum")%>:</label>
		            </h5>
		            <div class="col2row">
                        <%= Html.TextBox("PostNumber")%>
                        <%= Html.ValidationMessage("PostNumber")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_city"><span>*</span><%= Html.Resource("City")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("City")%>
			            <%= Html.ValidationMessage("City")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_phone"><span>*</span><%= Html.Resource("Telephone")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Telephone")%>
			            <p id="infotipTelephone" class="infotip" style="display: none;">
		                    <%= Html.Resource("PhoneInfo")%>
		                </p>
			            <%= Html.ValidationMessage("Telephone")%>
		            </div>
		            <br class="clear" />
	            </div>
	            
	            <div class="formrow">
		            <h5>
		              <label for="contact_mobile"><%= Html.Resource("Mobile")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Mobile")%>
			            <p id="infotipMobile" class="infotip" style="display: none;">
		                    <%= Html.Resource("PhoneInfo")%>
		                </p>
			            <%= Html.ValidationMessage("Mobile")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label for="contact_fax"><%= Html.Resource("Fax")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Fax")%>
			            <p id="infotipFax" class="infotip" style="display: none;">
		                    <%= Html.Resource("PhoneInfo")%>
		                </p>
			            <%= Html.ValidationMessage("Fax")%>
		            </div>
		            <br class="clear" />
	            </div>

	            <div class="formrow">
		            <h5>
		              <label class="required" for="contact_email"><span>*</span><%= Html.Resource("Email")%>:</label>
		            </h5>
		            <div class="col2row">
			            <%= Html.TextBox("Email")%> <img id="email_check_loading" src="<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif")) %>" height="24" width="24" title="<%= Html.Resource("Loading")%>" alt="<%= Html.Resource("Loading")%>" style="display:none;" />
			            <p class="errorinfo" id="email_error" style="display:none"><%= Html.Resource("CustomerAlreadyExists")%></p>
			            <p class="errorinfo" id="email_domain_error" style="display:none"><%= Html.Resource("ErrorInvalidEmail")%></p>
			            <%= Html.ValidationMessage("Email")%>
		            </div>
		            <br class="clear" />
	            </div>
                
                <div class="formrow">
                    <h5>
                        <label><%= Html.Resource("BillingAddress")%><br class="clear" /></label>
                    </h5>
                    <div class="col2row">
                        <label for="secondAddressFalse">
                            <%= Html.RadioButton("SecondAddress", false, new { @id = "secondAddressFalse", @checked = "checked" })%> <%= Html.Resource("SecondFalse")%>
                        </label>
                        <br class="clear" />
                        <label for="secondAddressTrue">
                            <%= Html.RadioButton("SecondAddress", true, new { @id = "secondAddressTrue" })%> <%= Html.Resource("SecondTrue")%>
                        </label>
                        <br class="clear" />
                        <%= Html.ValidationMessage("SecondAddress")%>
                    </div>
                </div>
                <%= Html.ValidationMessage("SecondAddress")%>
		        <br class="clear" />

	            <div id="secondAddress" <% if(!Model.SecondAddress) { %> style="display: none;" <% } %>>
		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_name"><span>*</span><%= Html.Resource("FirstName")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceContactName")%>
				            <%= Html.ValidationMessage("InvoiceContactName")%>
			            </div>
			            <br class="clear" />
		            </div>
				    
		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_lastname"><span>*</span><%= Html.Resource("LastName")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceContactLastName")%>
				            <%= Html.ValidationMessage("InvoiceContactLastName")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label for="invoice_company"><%= Html.Resource("Company")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceCompany")%>
				            <%= Html.ValidationMessage("InvoiceCompany")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
			              <label class="required" for="invoice_address"><span>*</span><%= Html.Resource("Address")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceAddress")%>
				            <%= Html.ValidationMessage("InvoiceAddress")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label for="invoice_address2"><%= Html.Resource("Address2")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceAddress2")%>
				            <%= Html.ValidationMessage("InvoiceAddress2")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_postnummer"><span>*</span><%= Html.Resource("PostNum")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoicePostNumber")%>
				            <%= Html.ValidationMessage("InvoicePostNumber")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_city"><span>*</span><%= Html.Resource("City")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceCity")%>
				            <%= Html.ValidationMessage("InvoiceCity")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_phone"><span>*</span><%= Html.Resource("Telephone")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceTelephone")%>
				            <p id="infotipInvoiceTelephone" class="infotip" style="display: none;">
			                    <%= Html.Resource("PhoneInfo")%>
			                </p>
				            <%= Html.ValidationMessage("InvoiceTelephone")%>
			            </div>
			            <br class="clear" />
		            </div>
		            
		            <div class="formrow">
			            <h5>
				            <label for="invoice_mobile"><%= Html.Resource("Mobile")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceMobile")%>
				            <p id="infotipInvoiceMobile" class="infotip" style="display: none;">
			                    <%= Html.Resource("PhoneInfo")%>
			                </p>
				            <%= Html.ValidationMessage("InvoiceMobile")%>
			            </div>
			            <br class="clear" />
		            </div>

		            <div class="formrow">
			            <h5>
				            <label class="required" for="invoice_email"><span>*</span><%= Html.Resource("Email")%>:</label>
			            </h5>
			            <div class="col2row">
				            <%= Html.TextBox("InvoiceEmail")%>
				            <%= Html.ValidationMessage("InvoiceEmail")%>
			            </div>
			            <br class="clear" />
		            </div>
                </div>

                <%  var paymentEnabled = (bool)ViewData["PaymentEnabled"];
                    var orderByEmailEnabled = (bool)ViewData["OrderByEmailEnabled"];
                    var orderByPostEnabled = (bool)ViewData["OrderByPostEnabled"];
                    var onlyOneOption = false;
                    if (paymentEnabled == false && orderByEmailEnabled == false || paymentEnabled == false && orderByPostEnabled == false || orderByEmailEnabled == false && orderByPostEnabled == false)
                    {
                        onlyOneOption = true;
                    }
                %>
                <div id="PaymentDiv">
                    <div <%= onlyOneOption ? "style='display: none;'" : String.Empty %>>
		                <h2><%= Html.Resource("Payment")%></h2>
		                <h4><%= Html.Resource("PaymentMethod")%></h4>
		                <div class="formrow">
		                    <%if (orderByEmailEnabled)
                            {
                            %>
			                    <label for="PaymentMethodEmail">
		                            <%= Html.RadioButton("RadioPaymentMethod", "email", Model.RadioPaymentMethod == "email", new Dictionary<string, object> { { "id", "PaymentMethodEmail" } })%> <%= Html.Resource("E_invoice")%>
			                    </label>
			                    <br class="clear" />
			                    <div class="smalltext"><%= Html.Resource("PaymentSmall1")%></div>
			                    <br class="clear" />
			                <%
                            }
                            if (orderByPostEnabled)
                            { %>
			                    <label for="PaymentMethodPost">
				                    <%= Html.RadioButton("RadioPaymentMethod", "post", Model.RadioPaymentMethod == "post", new Dictionary<string, object> { { "id", "PaymentMethodPost" } })%> <%= Html.Resource("Post_invoice")%>
			                    </label>
			                    <br class="clear" />
			                    <div class="smalltext"><%= Html.Resource("PaymentSmall2")%></div>
			                    <br class="clear" />
			                <%
                            }
                            if (paymentEnabled)
                            { %>
		                        <label for="PaymentMethodCard">
			                        <%= Html.RadioButton("RadioPaymentMethod", "card", Model.RadioPaymentMethod == "card", new Dictionary<string, object> { { "id", "PaymentMethodCard" } })%> <%= Html.Resource("Credit_card")%>
		                        </label>
		                        <br class="clear" />
		                        <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
			                <% 
                            } %>
		                </div>
	                </div>
	            </div>
	            <%if (paymentEnabled)
                { %>
                <div id="cc_paymentDiv" style="display: none;">
			        <h2><%= Html.Resource("CreditCardPayment")%></h2>
			        <% Html.RenderAction("Index", "PaymentForm", new { area = "PaymentForm", listOfPlugins = new[]{ new Atomia.Billing.Core.Common.PaymentPlugins.GuiPaymentPluginData("CCPayment", "Credit card payment") } }); %>
		        </div>
		        <% 
                } %>
            
		        <h2><%= Html.Resource("ClickOrder")%></h2>
		        <h4><%= Html.Resource("Billing")%></h4>
		        <div id="BillingText" class="formrow">
		            <%if(orderByEmailEnabled)
                    { %>
                        <%= Html.Resource("OnEmailBilling")%>
                    <%
                    }else if(orderByPostEnabled)
                    { %>
			            <%= Html.Resource("OnPostBilling")%>
			        <%
                    }else if(paymentEnabled)
                    {
                     %>
			            <%= Html.Resource("OnCCBilling")%>
			        <% 
                    } %>
		        </div>
		        <h4><%= Html.Resource("Activation")%></h4>
		        <div id="ActivationText" class="formrow">
		            <%if(orderByEmailEnabled)
                    { %>
                        <%= Html.Resource("OnEmailActivation")%>
                    <%
                    }else if(orderByPostEnabled)
                    { %>
			            <%= Html.Resource("OnPostActivation")%>
			        <%
                    }else if(paymentEnabled)
                    {
                     %>
			            <%= Html.Resource("OnCCActivation")%>
			        <% 
                    } %>
		        </div>
                <div id="termsDiv">
                    <h2><%= Html.Resource("TermsDiv")%></h2>
                    <div id="termsList">
                    </div>
                    <br class="clear" />
                    <!-- Add this field for Terms of service, validation function and trigger are located in cart partial -->
                    <%= Html.ValidationMessage("errorTerm")%>
                    <%= Html.Hidden("errorTerm", "asdasd")%>
                    <br class="clear" />
    	        </div>
	            <h2>
		            <%= Html.Resource("TotalAmount")%>: <span id="totalPrice">0,0  <span class="currency"><%= (string)this.Session["OrderCurrencyResource"] ?? Html.Resource(String.Format("{0}Common, Currency", Session["Theme"]))%></span></span>
	            </h2>
		        <br class="clear" />
	            <p class="actions"><a href="javascript:void(0);" class="b_b_create" id="orderbutton" style="display: inline;"><%= Html.Resource("Order")%></a></p>
		        <%= Html.Hidden("TelephoneProcessed")%>
		        <%= Html.Hidden("InvoiceTelephoneProcessed")%>
		        <%= Html.Hidden("FaxProcessed")%>
				<%= Html.Hidden("InvoiceFaxProcessed")%>
		        <%= Html.Hidden("MobileProcessed")%>
                <%= Html.Hidden("InvoiceMobileProcessed")%>
		        <%= Html.Hidden("CountryCode", (string)ViewData["defaultCountry"])%>
				<%= Html.Hidden("InvoiceCountryCode", (string)ViewData["defaultCountry"])%>
		        <%= Html.Hidden("SearchDomains", Model.SearchDomains)%>
		        <%= Html.Hidden("OwnDomain", ViewData["OwnDomain"])%>
		        <%= Html.Hidden("FirstOption", (bool)ViewData["firstOption"])%>
		        <%= Html.Hidden("VATNumber", "")%>
		        <%= Html.Hidden("InvoiceFax", "")%>
	        	<%= Html.Hidden("OrderCustomData", "")%>
		        <%= Html.Hidden("formater", "")%>
                <%= Html.Hidden("VATValidationMessage", "")%>
	        <% Html.EndForm(); %>
	    </div>
        <%= Html.Hidden("dontShowTaxesForThisResellerHidden", Session["dontShowTaxesForThisResellerHidden"])%>
    </div>
	<script type="text/javascript">

        var showPersonalNumber = '<%= ViewData["ShowPersonalNumber"]%>';            

	    var initializeParams = {};
        initializeParams.ResourcePersonalNumber = '<%= Html.ResourceNotEncoded("PersonalNumber")%>';
        initializeParams.ResourceCompanyNumber = '<%= Html.ResourceNotEncoded("CompanyNumber")%>';
        var initializedObj = initializeAdditionalThemeMethods(initializeParams);
	    
	    var notificationParams = {};
		notificationParams.wasAnError = '<%= ViewData["WasAnError"] %>';
		notificationParams.NotificationText = '<%= Html.ResourceNotEncoded("NotificationText") %>';
		notificationParams.NotificationTextPayment = '<%= Html.ResourceNotEncoded("NotificationTextPayment") %>';
		notificationParams.title = '<%= Html.Resource("NotificationHeader") %>';
	    
        var validateVATNumberParams = {};
        validateVATNumberParams.ValidateVATNumberUrl = '<%= Url.Action("ValidateVatNumber", new { controller = "PublicOrder", area = "PublicOrder" }) %>';        
        validateVATNumberParams.ValidationResultFalseMessage = '<%= Html.Resource("VATValidationResultFalseMessage") %>';
        validateVATNumberParams.ValidationResultNotValidated = '<%= Html.Resource("VATValidationResultNotValidated") %>';

		var OrderByPostSelected = false;
		var canSubmit = true;
		var decimalDigits = 2;

		var decimalParserParams = {};
		decimalParserParams.DecimalSeparator = '<%= ViewData["decimalSeparator"] %>';
		decimalParserParams.GroupSeparator = '<%= ViewData["groupSeparator"] %>';
		decimalParserParams.Locale = 'se';
		initializeDecimalParser(decimalParserParams);

		$(document).ready(function() {
		    
            var validator = $("#submit_form").validate();

            AddValidationRules();
            AddValidationMethods();            
            bindSecondAddressCheckBoxClick();

		    $('#notification').notification({
		        showTimeout: 1000,
		        hideTimeout: 100000
		    });

            <% if (ViewData["qs_CampaignCode"] != null && ViewData["qs_CampaignCode"].ToString() != String.Empty)
			  {	%>
//				$('#OrderCustomData').val('{"CustomAttributesArray":[{"nameField":"CampaignCode","valueField":"<%= ViewData["qs_CampaignCode"]%>"}]}');
				$('#OrderCustomData').val('[{"Name":"CampaignCode","Value":"<%= ViewData["qs_CampaignCode"]%>"}]');
            <%} %>
            
            // Call methods from theme js files that are not in the Default theme js files, empty in Default, params are not used in Default
            var params = {};
            params.PersonalDataButtonAction = '<%= Url.Action("GetAddressInfo", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
            params.ResourcePersonalNum = '<%= Html.ResourceNotEncoded("PersonalNum") %>';
            params.ResourceCompany = '<%= Html.ResourceNotEncoded("Company")%>';
            params.ResourceOrgNum = '<%= Html.ResourceNotEncoded("OrgNum")%>';
            params.ResourceOrganisation = '<%= Html.ResourceNotEncoded("Organisation")%>';
            params.ResourceRemove = '<%= Html.ResourceNotEncoded("Remove")%>';
            params.ResourceViewInfo = '<%= Html.ResourceNotEncoded("ViewInfo")%>';
            params.ImgHideInfo = '<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/{1}", Session["Theme"], "butt_hideinfo.gif"))%>';
            params.ImgShowInfo = '<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/{1}", Session["Theme"], "butt_showinfo.gif"))%>';
            params.DefaultCountryCode = '<%= (string)ViewData["defaultCountry"]%>';
            params.InitializedObj = initializedObj;
		    initializeAdditionalThemeMethodsDocReady(params);

		    setNotificationMessage(notificationParams);

		    setOrderByPost('<%= Model.RadioPaymentMethod%>');

		    initializeVtip('<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"]))%>');

		    companyKeyUpBind();
//            orgNumberKeyUpBind();

            params = {};
            params.CheckEmailAction = '<%= Url.Action("CheckEmail", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
            params.CheckEmailDomainAction = '<%= Url.Action("CheckEmailDomain", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
		    emailBlurBind(params);
		    invoiceEmailBlurBind();
		    emailKeyUpBind();

		    secondAddressRadioBind();

		    params = {};
		    params.BillingTextMail = '<%= Html.ResourceNotEncoded("OnEmailBilling")%>';
		    params.ActivationTextMail = '<%= Html.ResourceNotEncoded("OnEmailActivation")%>';
		    paymentMethodEmailBind(params);

		    params.BillingTextPost = '<%= Html.ResourceNotEncoded("OnPostBilling")%>';
		    params.ActivationTextPost = '<%= Html.ResourceNotEncoded("OnPostActivation")%>';
		    paymentMethodPostBind(params);

		    params.BillingTextCC = '<%= Html.ResourceNotEncoded("OnCCBilling")%>';
		    params.ActivationTextCC = '<%= Html.ResourceNotEncoded("OnCCActivation")%>';
		    paymentMethodCarBind(params);

		    fillPaymentMethod('<%= Model.RadioPaymentMethod%>');

		    submitOnceUnbind();

		    $('#submit_form input,select').keydown(function(e) {
		        if (e.keyCode == 13) {
		            var submitParams = {};
		            submitParams.IsFirstOption = '<%= (bool)ViewData["firstOption"]%>';
		            submitParams.DefaultCountryCode = '<%= (string)ViewData["defaultCountry"]%>';
		            onSubmit(submitParams);
		            return false;
		        }
		    });

		    $('#orderbutton').bind('click', function() {
                var lbl = document.getElementById('vatValidationInfo');
                $("#VATValidationMessage").val($("#vatValidationInfo").text());
                
                var submitParams = {};
		        submitParams.IsFirstOption = '<%= (bool)ViewData["firstOption"]%>';
		        submitParams.DefaultCountryCode = '<%= (string)ViewData["defaultCountry"]%>';
		        onSubmit(submitParams);
		    });

		});

        function AddValidationMethods() {
        
            if (showPersonalNumber != 'False')
            {
                jQuery.validator.addMethod(
                    "ValidateOrgNumberEx", function(value, element, params) {
                        return ValidateOrgNumberEx(value, element, params); 
                    }
                );

                jQuery.validator.addMethod(
                    "ValidateOrgNumberCheckSum", function(value, element, params) {
                        return ValidateOrgNumberCheckSum(value, element, params); 
                    }
                );

                jQuery.validator.addMethod(
                    "ValidateVATNumberOnExistence", function(value, element, params) {
                        return ValidateVATNumberOnExistence(value, element, params); 
                    }
                );
            }

            jQuery.validator.addMethod(
                "ValidateVATNumberEx", function(value, element, params) {
                    return ValidateVATNumberEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidatePostNumberEx", function(value, element, params) {
                    return ValidatePostNumberEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoicePostNumberEx", function(value, element, params) {
                    return !$('#secondAddressTrue').attr('checked') || ValidateInvoicePostNumberEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateTelephoneEx", function(value, element, params) {
                    return ValidateTelephoneEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceTelephoneEx", function(value, element, params) {
                    return !$('#secondAddressTrue').attr('checked') || ValidateInvoiceTelephoneEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateMobileEx", function(value, element, params) {
                    return this.optional(element) || ValidateMobileEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceMobileEx", function(value, element, params) {
                    return this.optional(element) || !$('#secondAddressTrue').attr('checked') || ValidateInvoiceMobileEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateFax", function(value, element, params) {
                    return this.optional(element) || ValidateFax(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceFax", function(value, element, params) {
                    return this.optional(element) || !$('#secondAddressTrue').attr('checked') || ValidateInvoiceFax(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateTerm", function(value, element, params) {
                    return ValidateTerm(value, element, params); 
                }
            );
        }

        function AddValidationRules() {

            if (showPersonalNumber != 'False')
            {
                $('#OrgNumber').rules("add", {
                    ValidateOrgNumberEx: true,
                    messages: {
                        ValidateOrgNumberEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidOrgNumber") %>'
                    }
                });

                $('#OrgNumber').rules("add", {
                    ValidateOrgNumberCheckSum: true,
                    messages: {
                        ValidateOrgNumberCheckSum: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorOrgNumberCheckSum") %>'
                    }
                });

                $('#OrgNumber').rules("add", {
                    ValidateVATNumberOnExistence: true,
                    messages: {
                        ValidateVATNumberOnExistence: '<%= Html.ResourceNotEncoded("VATValidationResultFalseMessage") %>'
                    }
                });
            }
            $('#VATNumber').rules("add", {
                ValidateVATNumberEx: { 
                    EUCountries: "<%= ViewData["EUCountries"] %>" 
                },
                messages: {
                    ValidateVATNumberEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidVATNumber") %>'
                }
            });

             $('#PostNumber').rules("add", {
                ValidatePostNumberEx: { 
                    DefaultCountryCode: "<%= ViewData["defaultCountry"] %>"
                },
                messages: {
                    ValidatePostNumberEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidPostNumber") %>'
                }
            });

            $('#InvoicePostNumber').rules("add", {
                ValidateInvoicePostNumberEx: { 
                    DefaultCountryCode: "<%= ViewData["defaultCountry"] %>"
                },
                messages: {
                    ValidateInvoicePostNumberEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidPostNumber") %>'
                }
            });

            $('#Telephone').rules("add", {
                ValidateTelephoneEx: true,
                messages: {
                    ValidateTelephoneEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });

            $('#InvoiceTelephone').rules("add", {
                ValidateInvoiceTelephoneEx: true,
                messages: {
                    ValidateInvoiceTelephoneEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });
        
            $('#Mobile').rules("add", {
                ValidateMobileEx: true,
                messages: {
                    ValidateMobileEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });

            $('#InvoiceMobile').rules("add", {
                ValidateInvoiceMobileEx: true,
                messages: {
                    ValidateInvoiceMobileEx: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });

            $('#Fax').rules("add", {
                ValidateFax: true,
                messages: {
                    ValidateFax: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });

            $('#InvoiceFax').rules("add", {
                ValidateInvoiceFax: true,
                messages: {
                    ValidateInvoiceFax: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat") %>'
                }
            });

            $('#errorTerm').rules("add", {
                ValidateTerm: true,
                messages: {
                    ValidateTerm: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorTermNotChecked") %>'
                }
            });
            
            AddRequiredValidationRulesForInvoiceFields();

        }

        function AddRequiredValidationRulesForInvoiceFields() { 

            $("#InvoiceContactName").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });

  
            $("#InvoiceContactLastName").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });
  
            $("#InvoiceAddress").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });

            $("#InvoicePostNumber").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });
            
            $("#InvoiceCity").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });

            $("#InvoiceTelephone").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });

            $("#InvoiceEmail").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').attr('checked');
                },
                messages: {
                    required: '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>'
                }
            });
            

        }

    </script>
    <%--<%= Html.ClientSideValidation<SubmitForm>("SubmitForm")
        --.AddRule("OrgNumber", new CustomRule("ValidateOrgNumberEx", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidOrgNumber")))
        --.AddRule("OrgNumber", new CustomRule("ValidateOrgNumberCheckSum", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorOrgNumberCheckSum")))
        --.AddRule("OrgNumber", new CustomRule("ValidateVATNumberOnExistence", new { }, Html.ResourceNotEncoded("VATValidationResultFalseMessage")))
        --.AddRule("PostNumber", new CustomRule("ValidatePostNumberEx", new { DefaultCountryCode = ViewData["defaultCountry"] }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidPostNumber")))
        --.AddRule("InvoicePostNumber", new CustomRule("ValidateInvoicePostNumberEx", new { DefaultCountryCode = ViewData["defaultCountry"] }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidPostNumber")))
        --.AddRule("Telephone", new CustomRule("ValidateTelephoneEx", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("InvoiceTelephone", new CustomRule("ValidateInvoiceTelephoneEx", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("Mobile", new CustomRule("ValidateMobileEx", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("InvoiceMobile", new CustomRule("ValidateInvoiceMobileEx", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("Fax", new CustomRule("ValidateFax", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("InvoiceFax", new CustomRule("ValidateInvoiceFax", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidFormat")))
        --.AddRule("errorTerm", new CustomRule("ValidateTerm", new { }, Html.ResourceNotEncoded("ValidationErrors, ErrorTermNotChecked")))%>--%>
            
    <script type="text/javascript">
        var emptyFieldMessage = '<%= Html.ResourceNotEncoded("ValidationErrors, ErrorEmptyField") %>';
        $(document).ready(function() {
            var params = {};
            params.EmptyFieldMessage = emptyFieldMessage;
            params.DefaultCountryCode = '<%= ViewData["defaultCountry"]%>';
            setSubmitFormAdditionalThemeRules(params);
        });
    </script>
</asp:Content>
