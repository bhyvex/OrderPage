<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<div id="CartPartialDiv">                                                     

</div>
           
<script type="text/javascript">
var switchedId;

$('#product_list').AtomiaShoppingCart(
    {
        OrderSubAmount: { display: true, tfoot: [ { "displayText" : "<%= Html.ResourceNotEncoded("Sum")%>", attr : { "colspan":"4", "style":"padding: 5px 10px;" } }, { attr : { "style":"padding: 5px 10px;" } }, { attr: { "style":"padding: 5px 10px;" } } ] },
        OrderTaxes: { display: true, tfoot: [ { attr : { "colspan":"4", style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } } ] },
        OrderTotal: { display: true, tfoot: [ { displayText : "<%= Html.ResourceNotEncoded("Total")%>", attr : { "colspan":"4", style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } }, { attr : { style:"padding: 5px 10px;" } } ] },
        ServerURL: "<%= Url.Action("RecalculateCart", new { area = "PublicOrder", controller = "PublicOrder" }) %>",
        ProductName: { display : true, thead : { attr : { 'scope': 'col' }, css : { }, displayText : "<%= Html.ResourceNotEncoded("Name") %>" }, tbody: { attr : { }, css: { } } },
        ProductAction: { display: true, thead: { attr : { 'scope': 'col', "class":"center" }, css: { }, displayText: " " }, tbody: { attr: { "class":"center", style:"min-width: 45px;" }, css: { }, displayButtonText: "<%= Html.ResourceNotEncoded("Remove")%>", displayButtonClass: "b_s_delete button small red" } },
        ProductPeriod: { display: true, change: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "<%= Html.ResourceNotEncoded("Period")%>" }, tbody: { attr: { "class":"right", style:"padding: 10px;"}, css: { } } },
        ProductPrice: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "<%= Html.ResourceNotEncoded("Price") %>" }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductDiscount: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "<%= Html.ResourceNotEncoded("Discount") %>" }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductTotalPrice: { display: true, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "<%= Html.ResourceNotEncoded("Amount") %>" }, tbody: { attr: { "class":"right" }, css: { } } },
        ProductNumberOfItems: { display: false, thead: { attr : { 'scope': 'col', "class":"right" }, css: { }, displayText: "Items" }, tbody: { attr: { "class":"right", style:"padding: 10px;"}, css: { } } },
        PricesIncludingVAT: true,
        AddOrderAddressData: true,
		vtipImagePath: "<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/gui", Session["Theme"])) %>",
        processingImageLocation: "<%= ResolveClientUrl(string.Format("~/Themes/{0}/Content/img/icons/{1}", Session["Theme"], "icn_processing_transparent.gif")) %>",
        DeleteButtonFunction: function(htmlElement, productID, productDisplayName, productQuantity) 
        {
            var trIndex = $(htmlElement).parent().parent()[0].rowIndex;
			var productID = cartArray[trIndex-1].id;
            var productDisplayName = cartArray[trIndex-1].display;
		   
		   $('#MainDomainSelect option[value='+productDisplayName+']').remove();
			if($('#MainDomainSelect').children().length > 1) 
			{
				$('#MainDomainHeader').show();
				$('#MainDomainWrapper').show();
			} 
			else 
			{
				$('#MainDomainHeader').hide();
				$('#MainDomainWrapper').hide();
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
		ChangePeriodFunction: function(htmlElement, oldproductID, oldProductDesc, oldProductQuantity, newProductID, newProductDesc, newProductQuantity) 
		{
			var trIndex = $(htmlElement).parent().parent()[0].rowIndex;
			var oldProductID = cartArray[trIndex-1].id;
            var oldProductDisplayName = cartArray[trIndex-1].display;
			var newProductId = $(htmlElement).val();
			
			switchedId = newProductId + '|' + oldProductDisplayName;
			
			$.fn.AtomiaShoppingCart.SwitchItem(oldProductID, oldProductDisplayName, 1, newProductId, oldProductDisplayName, 1, true); 
			
			$(htmlElement).parent().parent().attr({ "id": newProductId });
			 
			$('#domainsDiv table tbody tr[id]').each(function() {
				var tr = $(this);
				if (tr.find('.vtip').attr('title') == oldProductDisplayName) 
				{
					tr.attr({ "id": newProductId });
				} 
			});
		},
		OnAfterRecalculation: function(data) 
		{ 
			if(parseFloat(data.ShoppingCartTotal) == 0)
			{
			    if (typeof(data.ShoppingCartItems) != 'undefined' && data.ShoppingCartItems != null && data.ShoppingCartItems.length > 0) {
			        $('#PaymentMethodEmail').trigger('click', [true]);
			    }
				
				$('#PaymentDiv').hide();
			}
			else
			{
				$('#PaymentDiv').show();
			}
			
			$('#totalPrice').html(data.ShoppingCartTotal + ' <span class="currency"><%= (string)this.Session["OrderCurrencyResource"] ?? Html.ResourceNotEncoded(String.Format("{0}Common, Currency", Session["Theme"]))%></span>');
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
        for (var i = 0; i<tmpArr.length; i+=2)
        {
			if(i < tmpArr.length - 2)
			{
				$.fn.AtomiaShoppingCart.AddItem(tmpArr[i], tmpArr[i+1], 1, false);
			}
			else
			{
				$.fn.AtomiaShoppingCart.AddItem(tmpArr[i], tmpArr[i+1], 1, true);
			}
        }
    }    

	<% 
	var splittedTemp = ViewData["switchedId"].ToString().Split('|');
	var tempSwitchedId = splittedTemp[0] + "|" + Html.ResourceNotEncoded(String.Format("{0}Common, " + splittedTemp[1], Session["Theme"]));
	%>
	
    switchedId = "<%= tempSwitchedId %>";
}

function ValidateTerm(value, element, params) {
    var exit = false;
    $('.js_termCheck').each(function() {
        if(!$(this).attr('checked'))
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

            for (var i = 0; i < splittedProds.Length; i += 2)
            {
                var productDesc = Html.ResourceNotEncoded(String.Format("{0}Common, " + splittedProds[i + 1], Session["Theme"]));
                if (String.IsNullOrEmpty(productDesc))
                {
                    // Domains ProductDescription is domain itself
                    productDesc = splittedProds[i + 1];
            }

                if (i < splittedProds.Length - 2)
                {
                    cartProds += splittedProds[i] + "|" + productDesc + "|";
                }
                else
                {
                    cartProds += splittedProds[i] + "|" + productDesc;
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
                    var productName = $(this).parent().find('input[type=hidden]').val();
    	            
    	            var switched = switchedId.split('|');
    	            switchedId = id + "|" + productName;
    	           
					$.fn.AtomiaShoppingCart.SwitchItem(switched[0], switched[1], 1, id, productName, 1, true);
                }
            );
        }
    );
    
    $("#PaymentMethodEmail").click(function() {
        if(OrderByPostSelected)
        {
            var id = "<%= ViewData["OrderByPostId"].ToString()%>";
            var text = '<%= Html.ResourceNotEncoded(String.Format("{0}Common, PostOrderTitleDesc", Session["Theme"]))%>';
			$.fn.AtomiaShoppingCart.RemoveItem(id, text, 1, true);
        } 
    });
    
    $("#PaymentMethodPost").click(function() {
        OrderByPostSelected = true;
        var id = "<%= ViewData["OrderByPostId"].ToString()%>";
        var text = '<%= Html.ResourceNotEncoded(String.Format("{0}Common, PostOrderTitleDesc", Session["Theme"]))%>';
        // Remove if in the cart
        $.fn.AtomiaShoppingCart.RemoveItem(id, text, 1, false);
		$.fn.AtomiaShoppingCart.AddItem(id, text, 1, true);
    });
    
    $("#PaymentMethodCard").click(function() {
        if(OrderByPostSelected)
        {
            var id = "<%= ViewData["OrderByPostId"].ToString()%>";
            var text = '<%= Html.ResourceNotEncoded(String.Format("{0}Common, PostOrderTitleDesc", Session["Theme"]))%>';
			$.fn.AtomiaShoppingCart.RemoveItem(id, text, 1, true);
        }
    });  
});
</script>
   