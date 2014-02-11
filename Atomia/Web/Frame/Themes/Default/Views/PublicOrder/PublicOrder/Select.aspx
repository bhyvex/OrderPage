<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage`1[[Atomia.Web.Plugin.PublicOrder.Models.SubmitForm, Atomia.Web.Plugin.PublicOrder]]" %>
<%@ Assembly Name="Atomia.Web.Plugin.DomainSearch" %>
<%@ Assembly Name="Atomia.Web.Plugin.Cart" %>
<%@ Import Namespace="Atomia.Web.Plugin.Cart.Models" %>
<%@ Import Namespace="Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService" %>
<%@ Import Namespace="Atomia.Web.Plugin.PublicOrder.Models"%>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Atomia.Billing.Core.Common.PaymentPlugins" %>

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

            <% Html.EnableClientValidation(); %>
            <% Html.BeginForm("Select", "PublicOrder", new { area }, FormMethod.Post, new { @id = "submit_form", autocomplete = "off" }); %>
            <% Html.EditorForModel(); %>

            <!-- DOMAINS SELECT CONTAINER-->
            <div id="step_0" >
                <h2>
                    <%= Html.Resource("DomainName")%>
                </h2>
                <%
                if (!(bool)ViewData["firstOption"])
                {
                    Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml domain = (Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml)Session["singleDomain"];

                    if (domain != null)
                    {
                        string domainName = domain.ProductName;
                        string productId = domain.ProductID;
                    %>
                    <%
                        if ((bool) ViewData["AddingSubdomain"])
                        {%>
                            <div class="formrow">
                                <h4><%=Html.Resource("SubdomainAlternative")%>:</h4>
                                <div class="col2row">
                                    <p>
                                        <%=Html.Resource("SubdomainAltInfo")%>
                                    </p>

                                </div>
                                <br class="clear" />
                            </div>
                    <%
                        }
                        else
                        {%>
                           <div class="formrow">
                                <h4><%=Html.Resource("Alternative")%>:</h4>
                                <div class="col2row">
                                    <p>
                                        <%=Html.Resource("AltInfo")%>
                                    </p>

                                </div>
                                <br class="clear" />
                            </div>
                        <%
                        }%>

                        <div class="formrow">
                            <label class="required">
                                <span>*</span><%=Html.Resource("DomainName")%>:
                            </label>
                            <div class="col2row">
                                <p id="singleDomain" rel="<%=Html.Encode(productId)%>">
                                    <%=Html.Encode(domainName)%>
                                </p>

                            </div>
                            <br class="clear" />
                        </div>
                    <%}%>
                <%
                }
                else
                {
                %>
                    <div id="DomainSearchContainer"></div>
                <%
                }
                %>

                <br class="clear" />
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
            </div>
            
            <!-- PACKAGE SELECT CONTAINER-->
            <div id="step_1" >
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
                        bool filteredExists = Session["PreselectedPackage"] == null && string.IsNullOrEmpty((string)Session["PreselectedPackage"])
                            ? list.Exists(p => p.productId == (string)Session["PreselectedPackage"])
                            : false;
                        for (int i = 0; i < list.Count; i++)
                        {
                    %>
                        <p>
                            <%
                            if (!filteredExists && i == 0)
                            {
                            %>
                                <%= Html.RadioButton("RadioProducts", list[i].productId, new { @id = "radioProducts" + i, @checked = "checked" })%> <label for="radioProducts<%= i%>"><strong><%= Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].productNameDesc)) %></strong></label><br />
                            <% 
                            }
                            else if ((string)Session["PreselectedPackage"] == list[i].productId)
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
                            <input type="hidden" name="RadioProductsName" value="<%= Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].productNameDesc)) %>" />
                            <input type="hidden" name="RadioProductsRenewalPeriod" value="<%= list[i].RenewalPeriodId %>" />
                            <input type="hidden" name="RadioProductsSetupFeeId" value="<%= list[i].SetupFee != null ? list[i].SetupFee.productID : string.Empty  %>" />
                            <input type="hidden" name="RadioProductsSetupFeeDescription" value="<%= list[i].SetupFee != null ? Html.Resource(string.Format("{0}Common, {1}", this.Session["Theme"], list[i].SetupFee.productDesc)) : string.Empty  %>" />
                            <input type="hidden" name="RadioProductsSetupFeeRenewalPeriodId" value="<%= list[i].SetupFee != null ? list[i].SetupFee.RenewalPeriodId : string.Empty  %>" />
                        </p>
                        <p class="package-description">
                            <%= Html.ResourceNotEncoded(string.Format("{0}Common, {1}", Session["Theme"], list[i].info)) %>
                        </p>
                    <%
                        }
                    }
                    %>
                    </div>
                    <br class="clear" />
                </div>
            </div>

            <%= Html.Hidden("ArrayOfProducts")%>
            <%= Html.ValidationMessage("ArrayOfProducts")%>
            
            <!-- INFO INPUT CONTAINER-->
            <div id="step_2" >
                <h2><%= Html.Resource("ContactInformation")%></h2>
				<div class="formrow">
                    <h5>
                      <label class="required" for="contact_name"><span>*</span><%= Html.Resource("CustomerType")%>:</label>
                    </h5>
                    <div class="col2row">
							<select name="RadioYouAre" id="YouAreCompany">
								<option value="company"><%= Html.Resource("Company-opt")%></option>
								<option value="private"><%= Html.Resource("Privateperson") %></option>
								<option value="organization"><%= Html.Resource("Org-opt")%></option>
							</select>
							<span id="RadioYouAre_validationMessage" class="field-validation-valid"></span>
					</div>
                   <br class="clear" />
                </div>
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
                      <label class="required" for="contact_country"><span>*</span><%= Html.Resource("Country")%>:</label>
                    </h5>
                    <div class="col2row">
                        <% var selectCountryList = new SelectList((List<SelectListItem>)ViewData["CountryList"], "Value", "Text", Model.CountryCode); %>
                        <%= Html.DropDownList("CountryCode", selectCountryList)%>
                        <%= Html.ValidationMessage("CountryCode")%>
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
                          <label class="required" for="invoice_country"><span>*</span><%= Html.Resource("Country")%>:</label>
                        </h5>
                        <div class="col2row">
                            <%  selectCountryList = new SelectList((List<SelectListItem>)ViewData["CountryList"], "Value", "Text", Model.InvoiceCountryCode); %>
                            <%= Html.DropDownList("InvoiceCountryCode", selectCountryList)%>
                            <%= Html.ValidationMessage("InvoiceCountryCode")%>
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
                <% if (this.Session["resellerAccountData"] != null && this.Session["showContactOptions"] != null && (bool)this.Session["showContactOptions"])
                {	
                %>
                <div id="ContactsDiv">
                    <h2><%= Html.Resource("ContactsSettings")%></h2>		                
                    <h5><%= Html.Resource("BillingContact")%></h5>
                    <div class="formrow">
                        <label for="BillingContactReseller">
                            <%= Html.RadioButton("RadioBillingContact", "reseller", Model.RadioBillingContact == "reseller", new Dictionary<string, object> { { "id", "BillingContactReseller" } })%> 
                            <%= ((AccountData)this.Session["resellerAccountData"]).Name%>
                        </label>
                        <br class="clear" />
                        <label for="BillingContactCustomer">
                            <%= Html.RadioButton("RadioBillingContact", "customer", Model.RadioBillingContact == "customer", new Dictionary<string, object> { { "id", "BillingContactCustomer" } })%> 
                            <span id="BillingContactCustomerText"><%= Html.Resource("CustomerRadio")%></span>
                        </label>
                    </div>
                    <h5><%= Html.Resource("TechnicalContact")%></h5>
                    <div class="formrow">
                        <label for="TechContactReseller">
                            <%= Html.RadioButton("RadioTechContact", "reseller", Model.RadioTechContact == "reseller", new Dictionary<string, object> { { "id", "TechContactReseller" } })%> 
                            <%= ((AccountData)this.Session["resellerAccountData"]).Name%>
                        </label>
                        <br class="clear" />
                        <label for="TechContactCustomer">
                            <%= Html.RadioButton("RadioTechContact", "customer", Model.RadioTechContact == "customer", new Dictionary<string, object> { { "id", "TechContactCustomer" } })%> 
                            <span id="TechContactCustomerText"><%= Html.Resource("CustomerRadio")%></span>
                        </label>
                    </div>
                </div>
                <% 
                } %>
				<div id="nodeclaration" style="display:none">
					<h4><%= Html.Resource("NoridDeclaration")%></h4>
					
					
					 <div class="formrow">
                        <h5>
                            <label class="required" for="invoice_city"><span>*</span><%= Html.Resource("SignedName")%>:</label>
                        </h5>
                        <div class="col2row">
                            <%= Html.TextBox("SignedName")%>
                            <%= Html.ValidationMessage("SignedName")%>
							<%= Html.TextBox("DomainSpeciffic")%>
                            
                        </div>
                        <br class="clear" />
                    </div>
					
					<div class="formrow">
							<h5><label class="required"><span>*</span> <%= Html.Resource("Confirmation")%>:</label></h5>
							<div class="col2row"><input type="checkbox" name="acceptdeclaration" id="Acceptdeclaration" /><label for="acceptdeclaration"><%= Html.Resource("NoridDeclarationInfo")%><br/><a href="#" class="noriddeclaration"><%= Html.Resource("NoridLink")%></a></label>
								<%= Html.ValidationMessage("DomainSpeciffic")%>
							</div>
							
							<br class="clear">
						</div>
				</div>
            </div>

            <!-- INVOICE SELECT CONTAINER-->
            <div id="invoiceDivWrapper">
                <h2><%= Html.Resource("Invoice")%></h2>
                <div class="formrow" id="invoiceDiv">
                    <div id="CartContainer" style="position: relative">
                        <table class="invoicespec list" id="product_list"></table>
                    </div>
                </div>                    
                <p id="vatValidationInfo" style="font-style:italic"></p>
            </div>
			
			<!-- PAYMENT SELECT CONTAINER-->
            <div id="step_3" >
                <%  var paymentEnabled = (bool)ViewData["PaymentEnabled"];
                    var orderByEmailEnabled = (bool)ViewData["OrderByEmailEnabled"];
                    var orderByPostEnabled = (bool)ViewData["OrderByPostEnabled"];
                    var payPalEnabled = (bool)ViewData["PayPalEnabled"];
                    var payexRedirectEnabled = (bool)ViewData["PayexRedirectEnabled"];
                    var worldPayRedirectEnabled = (bool)ViewData["WorldPayRedirectEnabled"];
                    var dibsFlexwinEnabled = (bool)ViewData["dibsFlexwinEnabled"];
                    var worldPayXmlRedirectEnabled = (bool)ViewData["WorldPayXmlRedirectEnabled"];
                    var adyenHppEnabled = (bool)ViewData["AdyenHppEnabled"];
                    var defaultPluginName = ViewData["DefaultPaymentPlugin"].ToString();
					
					int optionCounter = 0;
                    foreach (var item in new List<Boolean>() { paymentEnabled, orderByEmailEnabled, orderByPostEnabled, payPalEnabled, payexRedirectEnabled, worldPayRedirectEnabled, dibsFlexwinEnabled, worldPayXmlRedirectEnabled, adyenHppEnabled })
                    {
                        if (item)
                        {
                            optionCounter++;
                        }                   
                    }

                    var onlyOneOption = (optionCounter < 2);
                    
                %>
                <div id="PaymentDiv">
                    <div <%= onlyOneOption ? "style='display: none;'" : String.Empty %>>
                        <h2><%= Html.Resource("Payment")%></h2>
                        <h4><%= Html.Resource("PaymentMethod")%></h4>
                        <div class="formrow">
                            <%if (orderByEmailEnabled)
                            {
                            %>
                                <label for="InvoiceByEmail">
                                    <%= Html.RadioButton("RadioPaymentMethod", "InvoiceByEmail", defaultPluginName == "InvoiceByEmail", new Dictionary<string, object> { { "id", "InvoiceByEmail" } })%> <%= Html.Resource("E_invoice")%>
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall1")%></div>
                                <br class="clear" />
                            <%
                            }
                            if (orderByPostEnabled)
                            { %>
                                <label for="InvoiceByPost">
                                    <%= Html.RadioButton("RadioPaymentMethod", "InvoiceByPost", defaultPluginName == "InvoiceByPost", new Dictionary<string, object> { { "id", "InvoiceByPost" } })%> <%= Html.Resource("Post_invoice")%>
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall2")%></div>
                                <br class="clear" />
                            <%
                            }
                            
                            if (paymentEnabled)
                            { %>
                                <label for="CCPayment">
                                    <%= Html.RadioButton("RadioPaymentMethod", "CCPayment", defaultPluginName == "CCPayment", new Dictionary<string, object> { { "id", "CCPayment" } })%> <%= Html.Resource("CCPayment")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                                <div id="cc_paymentDiv" style="display: none;">
									<% Html.RenderAction("Index", "PaymentForm", new { area = "PaymentForm", listOfPlugins = new List<GuiPaymentPluginData> { new GuiPaymentPluginData("CCPayment", "Credit card payment")}.ToArray() }); %>
								</div>
                            <% 
                            }
                            
                            if (payexRedirectEnabled)
                            { %>
                                <label for="PayExRedirect">
                                    <%= Html.RadioButton("RadioPaymentMethod", "PayExRedirect", defaultPluginName == "PayExRedirect", new Dictionary<string, object> { { "id", "PayExRedirect" } })%> <%= Html.Resource("PayExRedirect")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                            <% 
                            }
                            
                            if (worldPayRedirectEnabled)
                            { %>
                                <label for="WorldPayRedirect">
                                    <%= Html.RadioButton("RadioPaymentMethod", "WorldPayRedirect", defaultPluginName == "WorldPayRedirect", new Dictionary<string, object> { { "id", "WorldPayRedirect" } })%> <%= Html.Resource("WorldPayRedirect")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                            <% 
                            }
                            
                            if (dibsFlexwinEnabled)
                            { %>
                                <label for="DibsFlexwin">
                                    <%= Html.RadioButton("RadioPaymentMethod", "DibsFlexwin", defaultPluginName == "DibsFlexwin", new Dictionary<string, object> { { "id", "DibsFlexwin" } })%> <%= Html.Resource("DibsFlexwin")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                            <% 
                            }
                            
                            if (worldPayXmlRedirectEnabled)
                            { %>
                                <label for="WorldPayXmlRedirect">
                                    <%= Html.RadioButton("RadioPaymentMethod", "WorldPayXmlRedirect", defaultPluginName == "WorldPayXmlRedirect", new Dictionary<string, object> { { "id", "WorldPayXmlRedirect" } })%> <%= Html.Resource("WorldPayXmlRedirect")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                            <% 
                            }
                            
                            if (adyenHppEnabled)
                            { %>
                                <label for="AdyenHpp">
                                    <%= Html.RadioButton("RadioPaymentMethod", "AdyenHpp", defaultPluginName == "AdyenHpp", new Dictionary<string, object> { { "id", "AdyenHpp" } })%> <%= Html.Resource("AdyenHpp")%> 
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall3")%></div>
                                <br class="clear" />
                            <% 
                            }
                            
                            if (payPalEnabled)
                            { %>
                                <label for="PayPal">
                                    <%= Html.RadioButton("RadioPaymentMethod", "PayPal", defaultPluginName == "PayPal", new Dictionary<string, object> { { "id", "PayPal" } })%> <%= Html.Resource("Pay_pal")%>
                                </label>
                                <br class="clear" />
                                <div class="smalltext"><%= Html.Resource("PaymentSmall4")%></div>
                            <% 
                            }
							%>
		                </div>
	                </div>
	            </div>
	            
            
                <h2><%= Html.Resource("ClickOrder")%></h2>
                <h4><%= Html.Resource("Billing")%></h4>
                <div id="BillingText" class="formrow">
                    <%if(orderByEmailEnabled)
                    { %>
                        <p><%= Html.Resource("OnEmailBilling")%></p>
                        <p class="paymentNeededNotification notice" style="display:none;"><%= Html.Resource("PaymentNeededNotification") %></p>
                    <%
                    }else if(orderByPostEnabled)
                    { %>
                        <p><%= Html.Resource("OnPostBilling")%></p>
                        <p class="paymentNeededNotification notice" style="display:none;"><%= Html.Resource("PaymentNeededNotification") %></p>
                    <%
                    }else if(paymentEnabled || payexRedirectEnabled || worldPayRedirectEnabled || dibsFlexwinEnabled || worldPayXmlRedirectEnabled || adyenHppEnabled)
                    {
                     %>
                        <%= Html.Resource("OnCCBilling")%>
                    <% 
                    }else if(payPalEnabled)
                    {
                     %>
                        <%= Html.Resource("OnPayPalBilling")%>
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
                    }else if(paymentEnabled || payexRedirectEnabled || worldPayRedirectEnabled || dibsFlexwinEnabled || worldPayXmlRedirectEnabled || adyenHppEnabled)
                    {
                     %>
                        <%= Html.Resource("OnCCActivation")%>
                    <% 
                    }
                    else if(payPalEnabled)
                    {
                     %>
                        <%= Html.Resource("OnCCActivation")%>
                    <% 
                    }					%>
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
                <%= Html.Hidden("TelephoneProcessed")%>
                <%= Html.Hidden("InvoiceTelephoneProcessed")%>
                <%= Html.Hidden("FaxProcessed")%>
                <%= Html.Hidden("InvoiceFaxProcessed")%>
                <%= Html.Hidden("MobileProcessed")%>
                <%= Html.Hidden("InvoiceMobileProcessed")%>
                <%= Html.Hidden("SearchDomains", Model.SearchDomains)%>
                <%= Html.Hidden("OwnDomain", ViewData["OwnDomain"])%>
                <%= Html.Hidden("FirstOption", (bool)ViewData["firstOption"])%>
                <%= Html.Hidden("VATNumber", "")%>
                <%= Html.Hidden("InvoiceFax", "")%>
                <%= Html.Hidden("OrderCustomData", "")%>
                <%= Html.Hidden("formater", "")%>
                <%= Html.Hidden("VATValidationMessage", "")%>

                <div id="BillingTextEmailContainer" style="display:none;">
                    <p><%= Html.ResourceNotEncoded("OnEmailBilling")%></p>
                    <p class="paymentNeededNotification notice" style="display:none;"><%= Html.Resource("PaymentNeededNotification") %></p>
                </div>
                <div id="BillingTextPostContainer" style="display:none;">
                    <p><%= Html.ResourceNotEncoded("OnPostBilling")%></p>
                    <p class="paymentNeededNotification notice" style="display:none;"><%= Html.Resource("PaymentNeededNotification") %></p>
                </div>
                <div id="BillingTextCCContainer" style="display:none;"><%= Html.ResourceNotEncoded("OnCCBilling")%></div>
                <div id="BillingTextPayPalContainer" style="display:none;"><%= Html.ResourceNotEncoded("OnPayPalBilling")%></div>
            </div>


            <p class="actions">
                <a href="#top" class="button large green" id="orderbutton" ><%= Html.Resource("Order")%></a>
            </p>
            <% Html.EndForm(); %>
        </div>
        <%= Html.Hidden("dontShowTaxesForThisResellerHidden", Session["dontShowTaxesForThisResellerHidden"])%>
    </div>
    <script type="text/javascript">

        var initializeParams = {};
        initializeParams.ResourcePersonalNumber = <%= Html.ResourceJavascript("PersonalNumber") %>;
        initializeParams.ResourceCompanyNumber = <%= Html.ResourceJavascript("CompanyNumber") %>;
        var initializedObj = initializeAdditionalThemeMethods(initializeParams);
        
        var notificationParams = {};
        notificationParams.wasAnError = '<%= ViewData["WasAnError"] %>';
        notificationParams.NotificationText = <%= Html.ResourceJavascript("NotificationText") %>;
        notificationParams.NotificationTextPayment = <%= Html.ResourceJavascript("NotificationTextPayment") %>;
        notificationParams.title = <%= Html.ResourceJavascript("NotificationHeader") %>;
        
        var validateVATNumberParams = {};
        validateVATNumberParams.ValidateVATNumberUrl = '<%= Url.Action("ValidateVatNumber", new { controller = "PublicOrder", area = "PublicOrder" }) %>';        
        validateVATNumberParams.ValidationResultFalseMessage = <%= Html.ResourceJavascript("VATValidationResultFalseMessage") %>;
        validateVATNumberParams.ValidationResultNotValidated = <%= Html.ResourceJavascript("VATValidationResultNotValidated") %>;

        var OrderByPostSelected = false;
        var canSubmit = true;
        var decimalDigits = 2;

        var skipFirstStep = <%= !(bool)ViewData["firstOption"] && ((Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml)Session["singleDomain"] == null) ? "true" : "false" %>;
        var skippedStepsCount = 0;
        if (skipFirstStep) {
            skippedStepsCount++;
        }

        var decimalParserParams = {};
        decimalParserParams.DecimalSeparator = '<%= ViewData["decimalSeparator"] %>';
        decimalParserParams.GroupSeparator = '<%= ViewData["groupSeparator"] %>';
        decimalParserParams.Locale = 'se';
        initializeDecimalParser(decimalParserParams);

        var emptyFieldMessage = <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>;

        $(document).ready(function() {
            var formValidator = $("#submit_form").validate();
            
            AddValidationRules();
            AddValidationMethods();            
            bindSecondAddressCheckBoxClick();
            addBillingCustomerDataBlur(<%= Html.ResourceJavascript("CustomerRadio") %>);
            addTechCustomerDataBlur(<%= Html.ResourceJavascript("CustomerRadio") %>);

            $('#notification').notification({
                showTimeout: 1000,
                hideTimeout: 100000
            });

            <% if (ViewData["qs_CampaignCode"] != null && ViewData["qs_CampaignCode"].ToString() != String.Empty)
              {	%>
                $('#OrderCustomData').val('[{"Name":"CampaignCode","Value":"<%= ViewData["qs_CampaignCode"]%>"}]');
            <%} %>
            
            // Call methods from theme js files that are not in the Default theme js files, empty in Default, params are not used in Default
            var params = {};
            params.PersonalDataButtonAction = '<%= Url.Action("GetAddressInfo", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
            params.ResourcePersonalNum = <%= Html.ResourceJavascript("PersonalNum") %>;
            params.ResourceCompany = <%= Html.ResourceJavascript("Company") %>;
            params.ResourceOrgNum = <%= Html.ResourceJavascript("OrgNum") %>;
            params.ResourceSignedName = <%= Html.ResourceJavascript("SignedName") %>;
            params.ResourceMustBeCompany = <%= Html.ResourceJavascript("ValidationErrors, ErrorMustBeCompany") %>;
            params.ResourceMustBeFromNorway = <%= Html.ResourceJavascript("ValidationErrors, ErrorMustBeFromNorway") %>;
            params.ResourceOrganisation = <%= Html.ResourceJavascript("Organisation") %>;
            params.ResourceRemove = <%= Html.ResourceJavascript("Remove") %>;
            params.ResourceViewInfo = <%= Html.ResourceJavascript("ViewInfo") %>;
			params.ResourceValidationRequired = <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>;
			params.ResourceValidationDeclaration = <%= Html.ResourceJavascript("ValidationErrors, ErrorDeclarationNotChecked") %>;
			params.ResourceValidationDeclarationFill = <%= Html.ResourceJavascript("ValidationErrors, ErrorDeclarationFill") %>;
            params.ImgHideInfo = '<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/{1}", Session["Theme"], "butt_hideinfo.gif"))%>';
            params.ImgShowInfo = '<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/{1}", Session["Theme"], "butt_showinfo.gif"))%>';
            params.DefaultCountryCode = '<%= (string)ViewData["defaultCountry"]%>';
            params.InitializedObj = initializedObj;
            params.EUCountries = "<%= ViewData["EUCountries"] %>";
            initializeAdditionalThemeMethodsDocReady(params);

            setNotificationMessage(notificationParams);

            setOrderByPost('<%= Model.RadioPaymentMethod%>');

            initializeVtip('<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"]))%>');

            companyKeyUpBind();
            countryChangeBind(params);

            params = {};
            params.CheckEmailAction = '<%= Url.Action("CheckEmail", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
            params.CheckEmailDomainAction = '<%= Url.Action("CheckEmailDomain", new { controller = "PublicOrder", area = "PublicOrder" }) %>';
            emailBlurBind(params);
            invoiceEmailBlurBind();
            emailKeyUpBind();

            secondAddressRadioBind();

            params = {};
            params.ActivationTextMail = <%= Html.ResourceJavascript("OnEmailActivation") %>;
            paymentMethodEmailBind(params);

            params.ActivationTextPost = <%= Html.ResourceJavascript("OnPostActivation") %>;
            paymentMethodPostBind(params);

            params.ActivationTextCC = <%= Html.ResourceJavascript("OnCCActivation") %>;
            paymentMethodCardBind(params, $("#CCPayment"));
            paymentMethodCardBind(params, $("#PayExRedirect"));
            paymentMethodCardBind(params, $("#WorldPayRedirect"));
            paymentMethodCardBind(params, $("#DibsFlexwin"));
            paymentMethodCardBind(params, $("#WorldPayXmlRedirect"));
            paymentMethodCardBind(params, $("#AdyenHpp"));
            
            paymentMethodPayPalBind(params);

            $('#<%= defaultPluginName %>').click();

            submitOnceUnbind();

            params = {};
            params.EmptyFieldMessage = emptyFieldMessage;
            params.DefaultCountryCode = '<%= ViewData["defaultCountry"]%>';
            setSubmitFormAdditionalThemeRules(params);

            // order page wizzard methods
            var submitParams = {};
            submitParams.IsFirstOption = '<%= (bool)ViewData["firstOption"]%>';
            submitParams.DefaultCountryCode = '<%= (string)ViewData["defaultCountry"]%>';
            OrderPageWizzard.Validator = formValidator;
            OrderPageWizzard.skipFirstNSteps = skippedStepsCount;
            OrderPageWizzard.InitializeButtons(submitParams);
        });

        function AddValidationMethods() {
        
            if ($('#OrgNumber').length > 0)
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
                "ValidateTelephoneEx", function(value, element, params) {
                    return ValidateTelephoneEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateMobileEx", function(value, element, params) {
                    return this.optional(element) || ValidateMobileEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateFax", function(value, element, params) {
                    return this.optional(element) || ValidateFax(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateTerm", function(value, element, params) {
                    return ValidateTerm(value, element, params); 
                }
            );

            // invoice methods
            jQuery.validator.addMethod(
                "InvoiceContactNameEx", function(value, element, params) {
                    return !$('#secondAddressTrue').is(':checked'); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoicePostNumberEx", function(value, element, params) {
                    return !$('#secondAddressTrue').is(':checked') || ValidatePostNumberEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceTelephoneEx", function(value, element, params) {
                    return !$('#secondAddressTrue').is(':checked') || ValidateInvoiceTelephoneEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceMobileEx", function(value, element, params) {
                    return this.optional(element) || !$('#secondAddressTrue').is(':checked') || ValidateInvoiceMobileEx(value, element, params); 
                }
            );

            jQuery.validator.addMethod(
                "ValidateInvoiceFax", function(value, element, params) {
                    return this.optional(element) || !$('#secondAddressTrue').is(':checked') || ValidateInvoiceFax(value, element, params); 
                }
            );

        }

        function AddValidationRules() {

            AddOrgNumberValidationRules();

            AddVATNumberValidationRules();

             $('#PostNumber').rules("add", {
                ValidatePostNumberEx: { 
                    DefaultCountryCode: "<%= ViewData["defaultCountry"] %>"
                },
                messages: {
                    ValidatePostNumberEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidPostNumber") %>
                }
            });

            $('#InvoicePostNumber').rules("add", {
                ValidateInvoicePostNumberEx: { 
                    DefaultCountryCode: "<%= ViewData["defaultCountry"] %>"
                },
                messages: {
                    ValidateInvoicePostNumberEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidPostNumber") %>
                }
            });

            $('#Telephone').rules("add", {
                ValidateTelephoneEx: true,
                messages: {
                    ValidateTelephoneEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });

            $('#InvoiceTelephone').rules("add", {
                ValidateInvoiceTelephoneEx: true,
                messages: {
                    ValidateInvoiceTelephoneEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });
        
            $('#Mobile').rules("add", {
                ValidateMobileEx: true,
                messages: {
                    ValidateMobileEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });

            $('#InvoiceMobile').rules("add", {
                ValidateInvoiceMobileEx: true,
                messages: {
                    ValidateInvoiceMobileEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });

            $('#Fax').rules("add", {
                ValidateFax: true,
                messages: {
                    ValidateFax: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });

            $('#InvoiceFax').rules("add", {
                ValidateInvoiceFax: true,
                messages: {
                    ValidateInvoiceFax: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidFormat") %>
                }
            });

            $('#errorTerm').rules("add", {
                ValidateTerm: true,
                messages: {
                    ValidateTerm: <%= Html.ResourceJavascript("ValidationErrors, ErrorTermNotChecked") %>
                }
            });
            
            AddRequiredValidationRulesForInvoiceFields();
        }

        function AddOrgNumberValidationRules(){

            if ($('#OrgNumber').length > 0)            
            {
                $('#OrgNumber').rules("add", {
                    ValidateOrgNumberEx: true,
                    messages: {
                        ValidateOrgNumberEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidOrgNumber") %>
                    }
                });

                $('#OrgNumber').rules("add", {
                    ValidateOrgNumberCheckSum: true,
                    messages: {
                        ValidateOrgNumberCheckSum: <%= Html.ResourceJavascript("ValidationErrors, ErrorOrgNumberCheckSum") %>
                    }
                });

                $('#OrgNumber').rules("add", {
                    ValidateVATNumberOnExistence: true,
                    messages: {
                        ValidateVATNumberOnExistence: <%= Html.ResourceJavascript("VATValidationResultFalseMessage") %>
                    }
                });
            }

        }

        function AddVATNumberValidationRules(){
            $('#VATNumber').rules("add", {
                ValidateVATNumberEx: { 
                    EUCountries: "<%= ViewData["EUCountries"] %>" 
                },
                messages: {
                    ValidateVATNumberEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorInvalidVATNumber") %>
                }
            });
        }

        function AddRequiredValidationRulesForInvoiceFields() { 

            $("#InvoiceContactName").rules("add", {
                InvoiceContactNameEx: {},
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    InvoiceContactNameEx: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>,
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });

  
            $("#InvoiceContactLastName").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });
  
            $("#InvoiceAddress").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });

            $("#InvoicePostNumber").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });
            
            $("#InvoiceCity").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });

            $("#InvoiceTelephone").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });

            $("#InvoiceEmail").rules("add", {
                required: function(element) {
                    return $('#secondAddressTrue').is(':checked');
                },
                messages: {
                    required: <%= Html.ResourceJavascript("ValidationErrors, ErrorEmptyField") %>
                }
            });
            

        }

    </script>
</asp:Content>
