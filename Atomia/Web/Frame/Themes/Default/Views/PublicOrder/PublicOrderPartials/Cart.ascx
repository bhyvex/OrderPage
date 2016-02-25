<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<div id="CartPartialDiv">                                                     

</div>
           
<script type="text/javascript">
var switchedId;
var showCommission = ('<%= Session["resellerAccountData"] != null %>').toLowerCase() == 'true';
var footerColspan = '<%= Session["resellerAccountData"] != null ? 5 : 4 %>';
$('#product_list').AtomiaShoppingCart(
    {
        OrderSubAmount: { display: true, tfoot: [ { "displayText" : <%= Html.ResourceJavascript("Sum") %>, attr : { "colspan":"4", "style":"padding: 5px 10px;" } }, { attr : { "style":"padding: 5px 10px;" } }, { attr: { "style":"padding: 5px 10px;" } } ] },
        OrderTaxes: { display: true, tfoot: [ { attr : { "colspan":"4", style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } } ] },
        OrderTotal: { display: true, tfoot: [ { displayText : <%= Html.ResourceJavascript("Total") %>, attr : { "colspan":"4", style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } } ] },
        ServerURL: "<%= Url.Action("RecalculateCart", new { area = "PublicOrder", controller = "PublicOrder" }) %>",
        ProductName: { display : true, thead : { attr : { 'scope': 'col' }, css : { }, displayText : <%= Html.ResourceJavascript("Name") %> }, tbody: { attr : { }, css: { } } },
        ProductAction: { display: true, thead: { attr : { 'scope': 'col', "class":"center" }, css: { }, displayText: " " }, tbody: { attr: { "class":"center", style:"min-width: 45px;" }, css: { }, displayButtonText: <%= Html.ResourceJavascript("Remove") %>, displayButtonClass: "b_s_delete button small red" } },
        ProductPeriod: { display: true, change: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: <%= Html.ResourceJavascript("Period") %> }, tbody: { attr: { "class":"right", style:"padding: 10px;"}, css: { } } },
        ProductPrice: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: <%= Html.ResourceJavascript("Price") %> }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductDiscount: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: <%= Html.ResourceJavascript("Discount") %> }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductCommission: { display: showCommission, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Commission" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductTotalPrice: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: <%= Html.ResourceJavascript("Amount") %> }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductNumberOfItems: { display: false, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "Items" }, tbody: { attr: { "class":"right", style:"padding: 10px;"}, css: { } } },
        OrderSubAmount: { display: true, tfoot: [{ displayText: <%= Html.ResourceJavascript("Subtotal") %>, attr: { colspan: footerColspan, style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        OrderTaxes: { display: true, tfoot: [{ displayText: <%= Html.ResourceJavascript("VAT") %>, attr: { colspan: footerColspan, style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        OrderTotal: { display: true, tfoot: [{ displayText: <%= Html.ResourceJavascript("AmountToPay") %>, attr: { colspan: footerColspan, style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        PricesIncludingVAT: true,
        AddOrderAddressData: true,
		vtipImagePath: "<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"])) %>",
        processingImageLocation: "<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif")) %>",
        DeleteButtonFunction: function(htmlElement, productID, productDisplayName, productQuantity) 
        {
            var trIndex = $(htmlElement).parent().parent()[0].rowIndex;
			var productID = cartArray[trIndex-1].id;
            var productDisplayName = cartArray[trIndex-1].display;
           
           $('#MainDomainSelect option[value="'+productDisplayName+'"]').remove();
            if($('#MainDomainSelect').children().length > 1) 
            {
                $('#MainDomainHeader:hidden, #MainDomainWrapper:hidden').show('blind', 500);
            } 
            else 
            {
                $('#MainDomainHeader:visible, #MainDomainWrapper:visible').hide('blind', 500);
            }
            
            //switch button in upper div
           $('#domainsDiv table tbody tr[id='+productID+']').each(function() {
                var tr = $(this);
                if (tr.find('.vtip').attr('title') == productDisplayName)
                {
                    tr.find('a').attr({ rel: "add" });
                    tr.find('a').text("<%=Html.ResourceNotEncoded("Add")%>").removeClass("red").addClass("green");
                } 
            });
            
            $.fn.AtomiaShoppingCart.RemoveItem(productID, productDisplayName, 1, true); 
        },
		ChangePeriodFunction: function(htmlElement, oldproductID, oldProductDesc, oldProductQuantity, oldProductRenewalPeriodId, oldProductIsPackage, newProductID, newProductDesc, newProductQuantity, newProductRenewalPeriodId, newProductIsPackage) 
		{
			var trIndex = $(htmlElement).parent().parent()[0].rowIndex;
			var oldProductID = cartArray[trIndex-1].id;
            var oldProductDisplayName = cartArray[trIndex-1].display;
            var isPackage = false;
			
            if (typeof cartArray[trIndex-1].isPackage != 'undefined' && cartArray[trIndex-1].isPackage) {
                isPackage = true;

                var switched = switchedId.split('|');
                switchedId = newProductID + '|' + oldProductDisplayName + '|' + newProductRenewalPeriodId;

                for (var i = 3; i < switched.length; i += 3) {
                    switchedId += '|' + switched[i] + '|' + switched[i + 1] + '|' + switched[i + 2];
                }
            }			
			
			$.fn.AtomiaShoppingCart.SwitchItem(oldProductID, oldProductDisplayName, 1, oldProductRenewalPeriodId, isPackage, newProductID, oldProductDisplayName, 1, newProductRenewalPeriodId, isPackage, true); 
			
			$(htmlElement).parent().parent().attr({ "id": newProductID });
			 
			$('#domainsDiv table tbody tr[id]').each(function() {
				var tr = $(this);
				if (tr.find('.vtip').attr('title') == oldProductDisplayName) 
				{
					tr.attr({ "id": newProductID });
				} 
			});
		},
		OnAfterRecalculation: function(data) 
		{ 
			if(parseFloat(data.ShoppingCartTotal) == 0)
			{
			    if (typeof(data.ShoppingCartItems) != 'undefined' && data.ShoppingCartItems != null && data.ShoppingCartItems.length > 0) {
			        $('#InvoiceByEmail').trigger('click', [true]);
			    }
				
				$('#PaymentDiv').hide();
			}
			else
			{
				$('#PaymentDiv').show();
			}
			
			$('#totalPrice').html(data.ShoppingCartTotal + ' <span class="currency"><%= (string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span>');
			
			$('p.paymentNeededNotification').hide();
			
			if ($('#InvoiceByEmail').is(':checked') || $('#InvoiceByPost').is(':checked')) {
				 if (typeof(data.ShoppingCartItems) != 'undefined' && data.ShoppingCartItems != null && data.ShoppingCartItems.length > 0) {
					var found = false;
					for (var i=0; i<data.ShoppingCartItems.length; i++) {
						if (data.ShoppingCartItems[i] != null && typeof(data.ShoppingCartItems[i].ProvisioningAllowedType) != 'undefined' && data.ShoppingCartItems[i].ProvisioningAllowedType != null && data.ShoppingCartItems[i].ProvisioningAllowedType == 1) {
							found = true;
							break;
						}
					}
					
					if (found) {
						$('p.paymentNeededNotification').show();
					}
				 }
			}
			checkCurrentProducts();
		},
        TermsOfServicesRenderFunction: function(resName, resValue) {
            $('#termsList').html('');
            if(resName != '' && resValue != '' && typeof (resName) != 'undefined' && typeof (resValue) != 'undefined' && resValue != null)
            {
				var tmpTextArr = resValue.split('|');
                var label = "";
                
                var tag = '<div class="formrow">';
                tag += '    <h5>';
                tag += '        <label class="required"><span>*</span><%= Html.Resource("Confirmation")%>:';
                tag += '        </label>';
                tag += '    </h5>';
                tag += '    <div class="col2row">';
                
                for(var i=0; i<tmpTextArr.length; i++)
                {
                    label = tmpTextArr[i];
                    
		            tag += '        <%= Html.CheckBox("termCheck' + i + '", new { @id = "termCheck' + i + '", @class = "js_termCheck" })%><label for="termCheck' + i + '">' + label + '</label>';
		            tag += '        <br class="clear" />';
                }
                tag += '    </div>';
                tag += '</div>';
                tag += '<br class="clear" />';
                
                $('#termsList').html(tag);
                
                // Form id and errorTerm hidden field id are from Select page where cart is displayed
                $(".js_termCheck").change(function() {
                    $("#submit_form").validate().element($('#errorTerm'));
                });
                
                $('#termsDiv').show();
            }
            else
		    {
		        $('#termsDiv').hide();
		    }
        }
    });

function firstRenderCart(cartData) {
    
    if (cartData != null) {
        var tmpArr = cartData.split('|');
        for (var i = 0; i<tmpArr.length; i+=4)
        {
			if(i < tmpArr.length - 4)
			{
				$.fn.AtomiaShoppingCart.AddItem(tmpArr[i], tmpArr[i+1], 1, false, tmpArr[i+2], tmpArr[i+3]);
			}
			else
			{
				$.fn.AtomiaShoppingCart.AddItem(tmpArr[i], tmpArr[i+1], 1, true, tmpArr[i+2], tmpArr[i+3]);
			}
        }
    }    

	<% 
	var splittedTemp = ViewData["switchedId"].ToString().Split('|');
    var tempSwitchedId = "";
    for (int i = 0; i < splittedTemp.Length; i += 3)
    {
        tempSwitchedId += splittedTemp[i] + "|" +
                         splittedTemp[i + 1] +
                         '|' + splittedTemp[i + 2];
        if (i < splittedTemp.Length - 3)
        {
            tempSwitchedId += "|";
        }
    }
	
	%>
	
    switchedId = "<%= tempSwitchedId %>";
}

function ValidateTerm(value, element, params) {
    var exit = false;
    $('.js_termCheck').each(function() {
        if(!$(this).is(':checked'))
        {   
            exit = true;
        }
    });
    if(exit)
    {
        return false;
    }
    
    return true;
}

function RemoveOrderByPost() {
    if(OrderByPostSelected) 
    {
        var id = "<%= ViewData["OrderByPostId"].ToString()%>";
        var text = <%= Html.ResourceJavascript(String.Format("{0}Common, PostOrderTitleDesc", Session["Theme"])) %>;
		$.fn.AtomiaShoppingCart.RemoveItem(id, text, 1, true);
    } 
}

$(document).ready(function() {
    // copy html to its container
    var cartHtmlToAppend = $("#CartPartialDiv").html();
    $("#CartContainer").append(cartHtmlToAppend);
    $("#CartPartialDiv").remove();

    <%
    if(ViewData["CartProducts"] != null)
    {
        var cartProds = String.Empty;
        var splittedProds = ViewData["CartProducts"].ToString().Split('|');

            for (var i = 0; i < splittedProds.Length; i += 4)
            {
                var productDesc = splittedProds[i + 1];

                if (i < splittedProds.Length - 4)
                {
                    cartProds += splittedProds[i] + "|" + productDesc + "|" + splittedProds[i + 2] + "|" + splittedProds[i + 3] + "|";
                }
                else
                {
                    cartProds += splittedProds[i] + "|" + productDesc + "|" + splittedProds[i + 2] + "|" + splittedProds[i + 3];
            }
        }
    %>
        firstRenderCart('<%=cartProds%>');
    <%
    }
    %>
      
    $("input[name=RadioProducts]").each(
        function( i ){
            $( this ).bind(
                "click",
                function(){
                    var id = $(this).val();
                    var productName = $(this).parent().find('input[name="RadioProductsName"]').val();
                    var renewalPeriodId = $(this).parent().find('input[name="RadioProductsRenewalPeriod"]').val();
                    var setupFeeId = $(this).parent().find('input[name="RadioProductsSetupFeeId"]').val();
                    var setupFeeDescription = $(this).parent().find('input[name="RadioProductsSetupFeeDescription"]').val();
                    var setupFeeRenewalPeriodId = $(this).parent().find('input[name="RadioProductsSetupFeeRenewalPeriodId"]').val();
    	            
    	            var switched = switchedId.split('|');
    	            switchedId = id + "|" + productName + "|" + renewalPeriodId;
                    
                    // Remove replaced items from cart
                    for (var i = 0; i < switched.length; i+=3) {
                        $.fn.AtomiaShoppingCart.RemoveItem(switched[i], switched[i + 1], 1, false);
                    }

                    // Add new items and recalculate
                    if (typeof setupFeeId != 'undefined' && setupFeeId != '') {
                        $.fn.AtomiaShoppingCart.AddItem(setupFeeId, setupFeeDescription, 1, false, setupFeeRenewalPeriodId, false, true);
                        switchedId += "|" + setupFeeId + "|" + setupFeeDescription + "|" + setupFeeRenewalPeriodId;
                    }

                    $.fn.AtomiaShoppingCart.AddItem(id, productName, 1, true, renewalPeriodId, true, true);
                }
            );
        }
    );

    $("input[name=RadioBillingContact]").each(function(i) {
        $(this).bind('click', function() {
            var value = $(this).val();
            $.fn.AtomiaShoppingCart.AddUpdateOrderCustomAttribute('BillingContact', value);
            globalCounter++;
		    $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
        });
    });

    $('input[name="RadioBillingContact"]:checked').trigger('click');
    
    $("#InvoiceByPost").click(function() {
        OrderByPostSelected = true;
        var id = "<%= ViewData["OrderByPostId"].ToString()%>";
        var text = <%= Html.ResourceJavascript(String.Format("{0}Common, PostOrderTitleDesc", Session["Theme"])) %>;
        // Remove if in the cart
        $.fn.AtomiaShoppingCart.RemoveItem(id, text, 1, false);
		$.fn.AtomiaShoppingCart.AddItem(id, text, 1, true, false);
    });
    
    $("#InvoiceByEmail").click(function() {
        RemoveOrderByPost();
    });
    
    $("#PayPal").click(function() {
        RemoveOrderByPost();
    });
    
    $("#CCPayment").click(function() {
        RemoveOrderByPost();
    });
    
    $("#PayExRedirect").click(function() {
        RemoveOrderByPost();
    });
    
    $("#WorldPayRedirect").click(function() {
        RemoveOrderByPost();
    });
    
    $("#DibsFlexwin").click(function() {
        RemoveOrderByPost();
    });
    
    $("#WorldPayXmlRedirect").click(function() {
        RemoveOrderByPost();
    });
    
    $("#AdyenHpp").click(function() {
        RemoveOrderByPost();
    });
});
</script>
   
