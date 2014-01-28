<%@ Assembly Name="Atomia.Web.Plugin.DomainSearch" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Atomia.Web.Plugin.DomainSearch.Models" %>
<%@ Import Namespace="System.Collections.Generic"%>
    
<%
if ((bool)Session["firstOption"])
{
    List<DomainDataFromXml> domains = (List<DomainDataFromXml>)ViewData["multiDomains"];
%>
    <div id="DSPartialDiv">
        
        <div class="formrow" id="domainsLoadingDiv" style="display:none;">
            <div style="padding: 20px 0px; text-align: center;">
                <img height="32" width="32" alt="DomainName" src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif"))%>"/>
            </div>
        </div>
        <div id="domainsDiv" class="formrow">
            <table class="invoicespec list" width="100%" border="0" cellpadding="0" cellspacing="0">
                <thead>
                    <tr>
                        <th><%=Html.Resource("DomainName")%></th>
                        <th style="width: 40px;"><%=Html.Resource("Status")%></th>
                        <th style="width: 100px;"><%=Html.Resource("PriceUp")%></th>
                        <th style="width: 7em;"><%=Html.Resource("Buy")%></th>
                    </tr>
                </thead>
                <tbody>
                    <%
    for (int i = 0; i < domains.Count; i++)
    {
        string domainName = domains[i].ProductName;
        string productId = domains[i].ProductID;
        var title = domainName;
        var arrTld = domainName.Split('.');
        if (domainName.Length > 20)
        {
            if (arrTld.Length > 1)
            {
                domainName = domainName.Substring(0, 20).Split('.')[0] + "..." + arrTld[arrTld.Length - 1];
            }
            else
            {
                domainName = domainName.Substring(0, 20).Split('.')[0] + "...";
            }
        }

        var shorten = "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + domainName + "</div>";
        string evenodd = i%2 == 0 ? "even" : "odd";
%>
                    <tr id="<%=Html.Encode(productId)%>" class="<%=Html.Encode(evenodd)%>">
	                    <%
        if (domains[i].ProductStatus == "loading")
        {
%>
                                <td class="loading"><%=shorten%></td>
                                <td>
                                    <img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif"))%>" height="24" width="24" title="<%=Html.Resource("Loading")%>" alt="<%=Html.Resource("Loading")%>" />
                                </td>
	                            <td class="right loading">
                                    <%=domains[i].ProductPrice%> <span class="currency"><%= (string)this.Session["OrderCurrencyResource"] ?? Html.Resource(String.Format("{0}Common, Currency", Session["Theme"]))%></span>
                                    <input type="hidden" name="renewalPeriod" value="<%=domains[i].RenewalPeriodId %>" />
                                </td>
                                <td class="right loading">&nbsp;</td>
	                    <%
        }
        else if (domains[i].ProductStatus == "taken")
        {
%>
                                <td class="loading"><%=shorten%></td>
                                <td>
                                    <img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png"))%>" height="24" width="24" title="<%=Html.Resource("Taken")%>" alt="<%=Html.Resource("Taken")%>" />
                                </td>
	                            <td class="right loading">
                                    <%=domains[i].ProductPrice%> <span class="currency"><%= (string)this.Session["OrderCurrencyResource"] ?? Html.Resource(String.Format("{0}Common, Currency", Session["Theme"]))%></span>
                                    <input type="hidden" name="renewalPeriod" value="<%=domains[i].RenewalPeriodId %>" />
                                </td>
                                <td class="right">&nbsp;</td>
	                    <%
        }
%>
                    </tr>
                 <%
    }
%>
                </tbody>
            </table>
        </div>
        <h5><%=Html.Resource("AdditionalDomain")%></h5>
        <div class="formrow">
            <h5><%=Html.Resource("AnotherDomain")%>:</h5>
            <div class="col2row">
                <%=Html.TextBox("SearchDomain")%>
                <div class="action-button">
			        <ul>
				        <li><a id="domaincontrol" href="javascript:void(0);"><%=Html.Resource("Search")%></a></li>
			        </ul>
		        </div>
                <p id="errorSearchDomain" class="errorinfo"></p>
            </div>
        </div>
    </div>
               
    <script type="text/javascript">
    $.postJSON = function(url, data, callback) {
        $.post(url, data, callback, "json");
    }; 
    
    var transactionId;
    var numOfDomains=0;
    var repeats=0;
    var FinishSearch = false;

    var checkResultsInProgress = false;
    var CheckResults = function (i) {
	    if(!checkResultsInProgress){
		    checkResultsInProgress = true;
		    if (transactionId) {
			    var action = "<%=Url.Action("GetStatus", new { controller = "PublicOrder", area = "PublicOrder" })%>";
			    $.postJSON(action, { sTransactionId: transactionId }, function (data, textStatus){
			        FinishSearch = data.FinishSearch;
				     if(data.TransactionId == transactionId && data.DomainStatuses.length > 0) {
					    var ids = new Array();//products in cart
    					
					    for(var i= 0; i<cartArray.length; i++)
                        {
                            ids[i] = cartArray[i].id;
                        }
    					
					    for(var i =0; i< data.DomainStatuses.length; i++) {
						    var currentStatus = data.DomainStatuses[i];
    						
						    var stringToShorten = currentStatus.DomainName;
                            var arrTld = currentStatus.DomainName.split('.');
                            var isTooLong = false;
                            if (stringToShorten.length > 40) {
                                isTooLong = true;
                                if(arrTld.length > 1)
                                {
                                    stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "..." + arrTld[arrTld.length - 1];
                                }
                                else
                                {
                                    stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "...";
                                }
                            }
                            var title = currentStatus.DomainName;
                            var shorten = "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + stringToShorten + "</div>";
                            
                            $('#domainsDiv table tbody tr').each(function() {
                              if ($(this).attr('id') == currentStatus.ProductId && currentStatus.DomainName == $(this).find('.vtip').attr('title')) 
                              {
                                  //generation of IDs for unified testing, atomia.ids.generator.js
                                    var splitedDomainName = currentStatus.DomainName.split('.');
                                    var tld = splitedDomainName[splitedDomainName.length - 1];
                                    GenerateDomainSearchID(tld);
                                    var dsBtnID = GenerateDSButtonID(tld);
                                    var dsDomainNameID = GenerateDSDomainNameID(tld);
                                    var dsStatusID = GenerateDSStatusID(tld);
                                    var dsPriceID = GenerateDSPriceID(tld);
                                    
                                  if(currentStatus.Status == 'available')
                                  {
                                    var existsInCart = false;
                                    for(var j = 0; j < ids.length; j++)
                                    {
                                        if(ids[j] == currentStatus.ProductId + "|" + currentStatus.DomainName)
                                        {
                                            existsInCart = true;
                                        }
                                    }
                                    
                                    if(existsInCart)
                                    {
                                        $(this).find('td').eq(3).html('<a id="' + dsBtnID + '" class="button small red" rel="remove" href="javascript:void(0);"><%=Html.ResourceNotEncoded("Del")%></a>');
                                        $(this).find('td').eq(0).html(shorten);
                                        $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                        $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                        $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_ok.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceNotEncoded("Available")%>" alt="<%=Html.ResourceNotEncoded("Available")%>" />');
                                        $(this).find('td').eq(1).attr({'id': dsStatusID});
                                        $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                        $(this).find('td').eq(2).attr({'id': dsPriceID});
                                        $(this).find('td').eq(2).removeClass('loading');
                                    }
                                    else
                                    {
                                        $(this).find('td').eq(3).html('<a id="' + dsBtnID + '" class="button small green" rel="add" href="javascript:void(0);"><%=Html.ResourceNotEncoded("Add")%></a>');
                                        $(this).find('td').eq(0).html(shorten);
                                        $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                        $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                        $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_ok.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("Available")%>" alt="<%=Html.ResourceJavascript("Available")%>" />');
                                        $(this).find('td').eq(1).attr({'id': dsStatusID});
                                        $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                        $(this).find('td').eq(2).attr({'id': dsPriceID});
                                        $(this).find('td').eq(2).removeClass('loading');
                                    }
                                    
                                    numOfDomains--;
                                  }
                                  else if (currentStatus.Status == 'taken')
                                  {
                                    $(this).find('td').eq(3).html('&nbsp;');
                                    $(this).find('td').eq(0).html(shorten);
                                    $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                    $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                    $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("Taken")%>" alt="<%=Html.ResourceJavascript("Taken")%>" />');
                                    $(this).find('td').eq(1).attr({'id': dsStatusID});
                                    $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                    $(this).find('td').eq(2).attr({'id': dsPriceID});
                                    $(this).find('td').eq(2).removeClass('loading');
                                    
                                    numOfDomains--;
                                  }
                                  else if (currentStatus.Status == 'special')
                                  {
                                    $(this).find('td').eq(3).html('&nbsp;');
                                    $(this).find('td').eq(0).html(shorten);
                                    $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                    $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                    $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_warning.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("NotAllowedDomain")%>" alt="<%=Html.ResourceJavascript("NotAllowedDomain")%>" />');
                                    $(this).find('td').eq(1).attr({'id': dsStatusID});
                                    $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                    $(this).find('td').eq(2).attr({'id': dsPriceID});
                                    $(this).find('td').eq(2).removeClass('loading');
                                    
                                    numOfDomains--;
                                  }
                                  else if (currentStatus.Status == 'warning')
                                  {
                                    $(this).find('td').eq(3).html('&nbsp;');
                                    $(this).find('td').eq(0).html(shorten);
                                    $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                    $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                    $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_warning.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("ErrorProcessing")%>" alt="<%=Html.ResourceJavascript("ErrorProcessing")%>" />');
                                    $(this).find('td').eq(1).attr({'id': dsStatusID});
                                    $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                    $(this).find('td').eq(2).attr({'id': dsPriceID});
                                    $(this).find('td').eq(2).removeClass('loading');
                                    
                                    numOfDomains--;
                                  }
                                  else if (currentStatus.Status == 'unavailable')
                                  {
                                    $(this).find('td').eq(3).html('&nbsp;');
                                    $(this).find('td').eq(0).html(shorten);
                                    $(this).find('td').eq(0).attr('class', 'js_labelShorten');
                                    $(this).find('td').eq(0).attr({'id': dsDomainNameID});
                                    $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("Unavailable")%>" alt="<%=Html.ResourceJavascript("Unavailable")%>" />');
                                    $(this).find('td').eq(1).attr({'id': dsStatusID});
                                    $(this).find('td').eq(2).html(currentStatus.Price + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span><input type="hidden" name="renewalPeriod" value="' + currentStatus.RenewalPeriodId +'" />');
                                    $(this).find('td').eq(2).attr({'id': dsPriceID});
                                    $(this).find('td').eq(2).removeClass('loading');
                                    
                                    numOfDomains--;
                                  }
                              }
                            });
                        }
                        
                        $("#domainsDiv table tbody tr td a").unbind().click(function() {
                            
                            if($(this).attr('rel') == "add")
                            {
                                $(this).attr({ rel: "remove" });
                                $(this).text("<%=Html.ResourceNotEncoded("Del")%>").removeClass("green").addClass("red");
                                    
                                var id = $(this).parent().parent().attr('id');    								
                                var domain = $(this).parent().parent().find('.vtip').attr('title');
                                var renewalPeriod = $(this).parent().parent().find('input[name="renewalPeriod"]').val();
                                    
                                $.fn.AtomiaShoppingCart.AddItem(id, domain, 1, true, renewalPeriod, false);
                                    
                                $('#MainDomainSelect').append('<option value="'+domain+'">'+domain+'</option>');
                                if($('#MainDomainSelect').children().length > 1) 
                                {
                                    $('#MainDomainHeader:hidden, #MainDomainWrapper:hidden').show('blind', 500);
                                } 
                                else 
                                {
                                    $('#MainDomainHeader:visible, #MainDomainWrapper:visible').hide('blind', 500);
                                }
                            } else {
                                $(this).attr({ rel: "add" });
                                $(this).text("<%=Html.ResourceNotEncoded("Add")%>").removeClass("red").addClass("green");
                                    
                                var id = $(this).parent().parent().attr('id');
                                    
                                var domain = $(this).parent().parent().find('.vtip').attr('title');
                                    
                                $.fn.AtomiaShoppingCart.RemoveItem(id, domain, 1, true);
                                    
                                $('#MainDomainSelect option[value="'+domain+'"]').remove();
                                    
                                if($('#MainDomainSelect').children().length > 1) 
                                {
                                    $('#MainDomainHeader:hidden, #MainDomainWrapper:hidden').show('blind', 500);
                                } 
                                else 
                                {
                                    $('#MainDomainHeader:visible, #MainDomainWrapper:visible').hide('blind', 500);
                                }


							    }
					     });
				     }
    				 
				     if(data.TransactionId == -1)
			         {
			            $('#domainsDiv table tbody tr').each(function() {
						    $(this).find('td').eq(0).attr('class', 'js_labelShorten');
						    $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_warning.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("ErrorProcessing")%>" alt="<%=Html.ResourceJavascript("ErrorProcessing")%>" />');
    						
						    numOfDomains--;
					    });
				     }
    				 
                     var img_url = "<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui/", Session["Theme"]))%>";
                     vtip(img_url);
                    
                    var timeout = <%= ((int) ViewData["domainSearchTimeout"]).ToString() %>;
				    checkResultsInProgress = false;
				    if (++repeats * 0.3 >= timeout || FinishSearch)
				    {
					    repeats = 0;
					    $(document).stopTime("documentTimer");
					    
			            $('#domainsDiv table tbody tr').each(function() {
			                if ($(this).find('td').eq(0).hasClass('loading')) 
			                {
						        $(this).find('td').eq(0).attr('class', 'js_labelShorten');
						        $(this).find('td').eq(1).html('<img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_warning.png"))%>" class="inline-icon" height="24" width="24" title="<%=Html.ResourceJavascript("ErrorProcessing")%>" alt="<%=Html.ResourceJavascript("ErrorProcessing")%>" />');
						    }						    
					    });
				    }
			    });
		    }
	    }
    }

    function startDomainSearching(aDomainsArray) {
        FinishSearch = false;
        checkResultsInProgress = false;
        repeats = 0;
        var res = -1;
        var markedDomainsCount = 0;
        ResetDomainSearchIDGenerator();
        
        var action = "<%=Url.Action("GetDomainsForCheck", new { controller = "DomainSearch", area = "DomainSearch" })%>";
        $.postJSON(action, { domainsArray: aDomainsArray }, function(preparedData) { 
            if(preparedData != 'undefined')
            {
                action = "<%=Url.Action("GetUnavailableDomains", new { controller = "PublicOrder", area = "PublicOrder" })%>";
                $.postJSON(action, { domains: preparedData }, function(markedDomainsString) { 
                    if(markedDomainsString != 'undefined')
                    {
                        action = "<%=Url.Action("MarkDomainsAsUnavailable", new { controller = "PublicOrder", area = "PublicOrder" })%>";
                        $.postJSON(action, { domains: markedDomainsString }, function(markedData) {
                            if(markedData != 'undefined')
                            {
                                markedDomainsCount = markedData.length;
                                var tag = "";
                                $('#domainsDiv table tbody').html('');
                                
							    var decodedDomains = new Array();
    							 
                                for(var i=0; i < markedData.length; i++)
                                {
                                     // add to domain list for encoding
								    decodedDomains[decodedDomains.length] = markedData[i].ProductName;
                                    
                                    var evenodd = "";
                                    if(i%2==0)
                                    {
                                        evenodd = "even";
                                    }
                                    else
                                    {
                                        evenodd = "odd";
                                    }
                                    
                                    tag += '<tr id="' + markedData[i].ProductID + '" class="' + evenodd + '">';
                                    
                                 
                                    var stringToShorten = markedData[i].ProductName;
                                    var arrTld = markedData[i].ProductName.split('.');
                                    var isTooLong = false;
                                    if (stringToShorten.length > 40) {
                                        isTooLong = true;
                                        if(arrTld.length > 1)
                                        {
                                            stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "..." + arrTld[arrTld.length - 1];
                                        }
                                        else
                                        {
                                            stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "...";
                                        }
                                    }
                                    var title = markedData[i].ProductName;
                                    var shorten = "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + stringToShorten + "</div>"
                                 
                                    tag += '    <td class="js_labelShorten">' + shorten + '</td>';
                                    tag += '    <td>';
                                    if(markedData[i].ProductStatus == 'taken')
                                    {
                                        tag += '        <img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_canceled.png"))%>" height="24" width="24" title="<%=Html.ResourceJavascript("Taken")%>" alt="<%=Html.ResourceJavascript("Taken")%>" />';
                                    }
                                    else if(markedData[i].ProductStatus == 'special')
                                    {
                                        tag += '        <img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "sts_warning.png"))%>" height="24" width="24" title="<%=Html.ResourceJavascript("NotAllowedDomain")%>" alt="<%=Html.ResourceJavascript("NotAllowedDomain")%>" />';
                                    }
                                    tag += '    </td>';
	                                tag += '    <td class="right">' + markedData[i].ProductPrice + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceJavascript(String.Format("{0}Common, Currency", Session["Theme"]))%></span></td>';
	                                tag += '    <td>&nbsp;</td>';
	                                tag += '</tr>';
                                }
                                $('#domainsDiv table tbody').append(tag);
                                
                                var img_url = "<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"]))%>";
                                vtip(img_url);
                                
							    var action = "<%=Url.Action("EncodeDomains", new { controller = "DomainSearch", area = "DomainSearch" })%>";
                                $.postJSON(action, { domains: decodedDomains }, function(encodedDomains) { 
                                     if(encodedDomains != 'undefined')
                                     {
                                        for(var i=0; i < encodedDomains.length; i++)
                                        {
                                            for(var j=0; j< preparedData.length; j++)
                                            {
                                                if(preparedData[j] == encodedDomains[i])
                                                {
                                                    preparedData.splice(j, 1);
                                                }
                                            }
                                        }
                                
                                        action = "<%=Url.Action("StartSearch", new { controller = "PublicOrder", area = "PublicOrder" })%>";
                                        $.postJSON(action, { domainsArray: preparedData }, function(data) { 
                                             if(data != 'undefined')
                                             {
                                                var tag = "";
                                                for(var i = 0; i<data.length; i++)
                                                {
                                                    var evenodd = "";
                                                    if((markedDomainsCount + i)%2==0)
                                                    {
                                                        evenodd = "even";
                                                    }
                                                    else
                                                    {
                                                        evenodd = "odd";
                                                    }
                                                    tag += '<tr id="' + data[i].ProductID + '" class="' + evenodd + '">';
                                                 
                                                    var stringToShorten = data[i].ProductName;
                                                    var arrTld = data[i].ProductName.split('.');
                                                    var isTooLong = false;
                                                    if (stringToShorten.length > 40) {
                                                        isTooLong = true;
                                                        if(arrTld.length > 1)
                                                        {
                                                            stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "..." + arrTld[arrTld.length - 1];
                                                        }
                                                        else
                                                        {
                                                            stringToShorten = stringToShorten.substring(0, 40).split('.')[0] + "...";
                                                        }
                                                    }
                                                    var title = data[i].ProductName;
                                                    var shorten = "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + stringToShorten + "</div>"
                                                 
                                                    tag += '    <td class="loading js_labelShorten">' + shorten + '</td>';
                                                    tag += '    <td class="center" >';
                                                    tag += '        <img src="<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif"))%>" height="24" width="24" title="<%=Html.ResourceNotEncoded("Loading")%>" alt="<%=Html.ResourceNotEncoded("Loading")%>" />';
                                                    tag += '    </td>';
                                                    tag += '    <td class="right loading">' + data[i].ProductPrice + ' <span class="currency"><%=(string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span></td>';
                                                    tag += '    <td class="left loading">&nbsp;</td>';
                                                    tag += '</tr>';
                                                }
                                                $('#domainsDiv table tbody').append(tag);
                                                
                                                var img_url = "<%=ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"]))%>";
                                                vtip(img_url);
                                                
                                                numOfDomains = data[0].NumberOfDomains;
                                                
                                                $('#domainsLoadingDiv').hide();
                                                $('#domainsDiv').show();
                                                
                                                transactionId =  data[0].TransactionId;
                                                $(document).everyTime(300, "documentTimer", CheckResults, 100);
                                             }
                                        });  
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });
    }

    $(document).ready(function() {
        // copy html to its container
        var htmlToAppend = $("#DSPartialDiv").html();
        $("#DomainSearchContainer").append(htmlToAppend);
        $("#DSPartialDiv").remove();
        
        <%
        if (ViewData["domainsForCheck"] != null && (string) ViewData["domainsForCheck"] != String.Empty && (bool) ViewData["firstOption"])
        {
        %>
            startDomainSearching("<%=(string) ViewData["domainsForCheck"]%>");
        <%
        }
        %>
        
        $('#domaincontrol').bind('click', function() {
            CheckDomains(true);
        });
        
        $('#SearchDomain').unbind().keyup(function(e) {
            if (e.keyCode == 13) {
                CheckDomains(true);
                return false;
            } else  {
                CheckDomains();
            }
        });
        
        function CheckDomains(startSearch){
            if($('#SearchDomain').val() != "")
            {
                var regex = new RegExp("<%=ViewData["RegDomainFront"].ToString()%>");
                var regexDom = new RegExp("<%=ViewData["RegDomain"].ToString()%>");
                
                var tmp = $('#SearchDomain').val().split(' ');
                for(var i = 0; i< tmp.length; i++)
                {
                    if(tmp.length > parseInt(<%=ViewData["NumberOfDomainsAllowed"]%>))
                    {
                        $('#errorSearchDomain').text("<%=Html.ResourceNotEncoded("ErrorSearchArguments")%>");
                        return;
                    }
                    if(!(regex.test(tmp[i])) && !(regexDom.test(tmp[i])))
                    {
                        $('#errorSearchDomain').text("<%=Html.ResourceNotEncoded("ValidationErrors, ErrorInvalidDomains")%>");
                        return;
                    }
                    if(tmp[i].length > parseInt(<%=ViewData["AllowedDomainLength"]%>))
                    {
                        $('#errorSearchDomain').text("<%=Html.ResourceNotEncoded("ErrorStringLength")%>");
                        return;
                    }
                }
                
                $('#errorSearchDomain').text('');      
                
                if (typeof(startSearch) != 'undefined' && startSearch != null && startSearch) {
                    $('#domainsLoadingDiv').show();
                    $('#domainsDiv').hide();
                
                    startDomainSearching($('#SearchDomain').val());            
                }
            }
            else
            {
                $('#errorSearchDomain').text("<%=Html.ResourceNotEncoded("ErrorSearchArguments")%>");
            }
        }
    });
    </script>
<%
}
%>