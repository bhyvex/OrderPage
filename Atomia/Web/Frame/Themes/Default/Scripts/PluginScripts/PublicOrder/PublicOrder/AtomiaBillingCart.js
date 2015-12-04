//-----------------------------------------------------------------------
// <copyright file="zAtomiaBillingShoppingCart.js" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Dusan Milenkovic </author>
//-----------------------------------------------------------------------

// This file must be copied to plugin where cart is rendered, this copy here is for developers and must be updated together with plugin copy

var cartArray = new Array();

var globalCounter = 0;

$.postJSON = function(url, data, callback) {
    $.post(url, data, callback, "json");
}; 

(function($) {
    $.fn.AtomiaShoppingCart = function(options) {

        var defaults = {
            ProductName: { display: true, thead: { attr: { 'scope': 'col' }, css: {}, displayText: "Specification" }, tbody: { attr: { "padding": "10px;", "class": "js_labelShorten" }, css: {}} },
            ProductPeriod: { display: false, change: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Period" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductNumberOfItems: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Items" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductFullPrice: { display: false, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Full Price" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductPrice: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Price" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductDiscount: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Discount" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductCommission: { display: false, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Commission" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductTotalPrice: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Amount" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
            ProductAction: { display: false, thead: { attr: { 'scope': 'col', "class": "center" }, css: {}, displayText: " " }, tbody: { attr: { "class": "center" }, css: {}, displayText: "Del"} },
            OrderSubAmount: { display: true, tfoot: [{ displayText: "Subtotal", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
            OrderTaxes: { display: true, tfoot: [{ displayText: "VAT", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
            OrderTotal: { display: true, tfoot: [{ displayText: "Amount to pay", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
            ServerURL: "",
            ChosenCountryId: "",
            PricesIncludingVAT: false,
            OrderCustomAttributes: [],
            AddOrderAddressData: false,
            ChangePeriodFunction: function(htmlElement, oldproductID, oldProductDesc, oldProductQuantity, oldProductRenewalPeriodId, oldProductIsPackage, newProductID, newProductDesc, newProductQuantity, newProductRenewalPeriodId, newProductIsPackage)  {
                $.fn.AtomiaShoppingCart.SwitchItem(oldproductID, oldProductDesc, oldProductQuantity, oldProductRenewalPeriodId, oldProductIsPackage, newProductID, newProductDesc, newProductQuantity, newProductRenewalPeriodId, newProductIsPackage, true);
            },
            DeleteButtonFunction: function(htmlElement, productID, productDisplayName, productQuantity) {
                $.fn.AtomiaShoppingCart.RemoveItem(productID, productDisplayName, productQuantity, true);
            },
            TermsOfServicesRenderFunction: function(resName, resValue) {
            },
            InitialRender: false,
            CampaignCodeRender: function() {
                return false;
            },
            CampaignCodeBeforeHtml: function() {
                return '';
            },
            CampaignCodeAfterHtml: function() {
                return '';
            },
            CampaignCodeLabel: "Campaign code:",
            CampaignCodeFieldId: "cartCampaignCode",
            vtipImagePath: "",
            processingImageLocation: "",
            cartTotal: 0,
            OnAfterRecalculation: function(data) { },
            CartUpdatedEventName: "AtomiaShoppingCartUpdated"
        };
        var options = $.extend(defaults, options);

        $.fn.AtomiaShoppingCart.options = options;

        return this.each(function() {
            $.fn.AtomiaShoppingCart.options.htmlElement = this;
            if ($.fn.AtomiaShoppingCart.options.InitialRender) {
                $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
            }
        });
    };

    $.fn.AtomiaShoppingCart.defaults = {
        ProductName: { display: true, thead: { attr: { 'scope': 'col' }, css: {}, displayText: "Specification" }, tbody: { attr: { "padding": "10px;", "class": "js_labelShorten" }, css: {}} },
        ProductPeriod: { display: false, change: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Period" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductNumberOfItems: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Items" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductFullPrice: { display: false, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Full Price" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductPrice: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Price" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductDiscount: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Discount" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductCommission: { display: false, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Commission" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductTotalPrice: { display: true, thead: { attr: { 'scope': 'col', "class": "right" }, css: {}, displayText: "Amount" }, tbody: { attr: { "class": "right", style: "padding: 10px;" }, css: {}} },
        ProductAction: { display: false, thead: { attr: { 'scope': 'col', "class": "center" }, css: {}, displayText: " " }, tbody: { attr: { "class": "center" }, css: {}, displayText: "Del"} },
        OrderSubAmount: { display: true, tfoot: [{ displayText: "Subtotal", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        OrderTaxes: { display: true, tfoot: [{ displayText: "VAT", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        OrderTotal: { display: true, tfoot: [{ displayText: "Amount to pay", attr: { colspan: "4", style: "padding: 5px 10px;"} }, { attr: { style: "padding: 5px 10px;"}}] },
        ServerURL: "",
        ChosenCountryId: "",
        PricesIncludingVAT: false,
        OrderCustomAttributes: [],
        AddOrderAddressData: false,
        ChangePeriodFunction: function(htmlElement, oldproductID, oldProductDesc, oldProductQuantity, oldProductRenewalPeriodId, oldProductIsPackage, newProductID, newProductDesc, newProductQuantity, newProductRenewalPeriodId, newProductIsPackage)  {
            $.fn.AtomiaShoppingCart.SwitchItem(oldproductID, oldProductDesc, oldProductQuantity, oldProductRenewalPeriodId, oldProductIsPackage, newProductID, newProductDesc, newProductQuantity, newProductRenewalPeriodId, newProductIsPackage, true);
        },
        DeleteButtonFunction: function(htmlElement, productID, productDisplayName, productQuantity) {
            $.fn.AtomiaShoppingCart.RemoveItem(productID, productDisplayName, productQuantity, true);
        },
        TermsOfServicesRenderFunction: function(resName, resValue) {
        },
        InitialRender: false,
        CampaignCodeRender: function() {
            return false;
        },
        CampaignCodeBeforeHtml: function() {
            return '';
        },
        CampaignCodeAfterHtml: function() {
            return '';
        },
        CampaignCodeLabel: "Campaign code:",
        CampaignCodeFieldId: "cartCampaignCode",
        vtipImagePath: "",
        processingImageLocation: "",
        cartTotal: 0,
        OnAfterRecalculation: function(data) { }
    };

    $.fn.AtomiaShoppingCart.GetOrderAddressDataAsJson = function() {
        // this method will be implemented for PublicOrder api address data, if you want it in BCUP override this method in specific Cart.ascx file and set AddOrderAddressData: true
        var result = "";
        var tmpArray = [];
        if (typeof ($('#ContactName').val()) != 'undefined' && $('#ContactName').val() != '') {
            tmpArray[tmpArray.length] = "'FirstName': '" + $('#ContactName').val() + "'";
        }
        if (typeof ($('#ContactLastName').val()) != 'undefined' && $('#ContactLastName').val() != '') {
            tmpArray[tmpArray.length] = "'LastName': '" + $('#ContactLastName').val() + "'";
        }
        if (typeof ($('#Company').val()) != 'undefined' && $('#Company').val() != '') {
            tmpArray[tmpArray.length] = "'Company': '" + $('#Company').val() + "'";
        }
        if (typeof ($('#OrgNumber').val()) != 'undefined' && $('#OrgNumber').val() != '') {
            tmpArray[tmpArray.length] = "'CompanyNumber': '" + $('#OrgNumber').val() + "'";
        }
        if (typeof ($('#Address').val()) != 'undefined' && $('#Address').val() != '') {
            tmpArray[tmpArray.length] = "'Address': '" + $('#Address').val() + "'";
        }
        if (typeof ($('#Address2').val()) != 'undefined' && $('#Address2').val() != '') {
            tmpArray[tmpArray.length] = "'Address2': '" + $('#Address2').val() + "'";
        }
        if (typeof ($('#PostNumber').val()) != 'undefined' && $('#PostNumber').val() != '') {
            tmpArray[tmpArray.length] = "'Zip': '" + $('#PostNumber').val() + "'";
        }
        if (typeof ($('#City').val()) != 'undefined' && $('#City').val() != '') {
            tmpArray[tmpArray.length] = "'City': '" + $('#City').val() + "'";
        }
        if (typeof ($('#TelephoneProcessed').val()) != 'undefined' && $('#TelephoneProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'Phone': '" + $('#TelephoneProcessed').val() + "'";
        }
        if (typeof ($('#MobileProcessed').val()) != 'undefined' && $('#MobileProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'Mobile': '" + $('#MobileProcessed').val() + "'";
        }
        if (typeof ($('#FaxProcessed').val()) != 'undefined' && $('#FaxProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'Fax': '" + $('#FaxProcessed').val() + "'";
        }
        if (typeof ($('#Email').val()) != 'undefined' && $('#Email').val() != '') {
            tmpArray[tmpArray.length] = "'Email': '" + $('#Email').val() + "'";
        }
        if (typeof ($('#InvoiceAddress').val()) != 'undefined' && $('#InvoiceAddress').val() != '') {
            tmpArray[tmpArray.length] = "'BillingAddress': '" + $('#InvoiceAddress').val() + "'";
        }
        if (typeof ($('#InvoiceAddress2').val()) != 'undefined' && $('#InvoiceAddress2').val() != '') {
            tmpArray[tmpArray.length] = "'BillingAddress2': '" + $('#InvoiceAddress2').val() + "'";
        }
        if (typeof ($('#InvoicePostNumber').val()) != 'undefined' && $('#InvoicePostNumber').val() != '') {
            tmpArray[tmpArray.length] = "'BillingZip': '" + $('#InvoicePostNumber').val() + "'";
        }
        if (typeof ($('#InvoiceCity').val()) != 'undefined' && $('#InvoiceCity').val() != '') {
            tmpArray[tmpArray.length] = "'BillingCity': '" + $('#InvoiceCity').val() + "'";
        }
        if (typeof ($('#InvoiceTelephoneProcessed').val()) != 'undefined' && $('#InvoiceTelephoneProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'BillingPhone': '" + $('#InvoiceTelephoneProcessed').val() + "'";
        }
        if (typeof ($('#InvoiceMobileProcessed').val()) != 'undefined' && $('#InvoiceMobileProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'BillingMobile': '" + $('#InvoiceMobileProcessed').val() + "'";
        }
        if (typeof ($('#InvoiceEmail').val()) != 'undefined' && $('#InvoiceEmail').val() != '') {
            tmpArray[tmpArray.length] = "'BillingEmail': '" + $('#InvoiceEmail').val() + "'";
        }
        if (typeof ($('#InvoiceFaxProcessed').val()) != 'undefined' && $('#InvoiceFaxProcessed').val() != '') {
            tmpArray[tmpArray.length] = "'BillingFax': '" + $('#InvoiceFaxProcessed').val() + "'";
        }
        if (typeof ($('#InvoiceCountryCode').val()) != 'undefined' && $('#InvoiceCountryCode').val() != '') {
            tmpArray[tmpArray.length] = "'BillingCountry': '" + $('#InvoiceCountryCode').val() + "'";
        }
        if (typeof ($('#VATNumber').val()) != 'undefined' && $('#VATNumber').val() != '') {
            tmpArray[tmpArray.length] = "'LegalNumber': '" + $('#VATNumber').val() + "'";
        }

        if (tmpArray.length > 0) {
            result = "{";
            for (var i = 0; i < tmpArray.length; i++) {
                if (i < tmpArray.length - 1) {
                    result += tmpArray[i] + ", ";
                } else {
                    result += tmpArray[i] + "}";
                }
            }
        }

        return result;
    };
    $.fn.AtomiaShoppingCart.AddOrderCustomAttribute = function(name, value) {
        if (typeof (name) != 'undefined' && name != '' && typeof (value) != 'undefined' && value != '') {
            for (var i = 0; i < $.fn.AtomiaShoppingCart.options.OrderCustomAttributes.length; i++) {
                if ($.fn.AtomiaShoppingCart.options.OrderCustomAttributes[i] == '{"Name":"' + name + '","Value":"' + value + '"}') {
                    return;
                }
            }
            $.fn.AtomiaShoppingCart.options.OrderCustomAttributes[$.fn.AtomiaShoppingCart.options.OrderCustomAttributes.length] = '{"Name":"' + name + '","Value":"' + value + '"}';
        }
    };
    $.fn.AtomiaShoppingCart.RemoveOrderCustomAttribute = function(name, value) {
        if (typeof (name) != 'undefined' && name != '' && typeof (value) != 'undefined' && value != '') {
            var tmpArray = $.fn.AtomiaShoppingCart.options.OrderCustomAttributes;
            for (var i = 0; i < $.fn.AtomiaShoppingCart.options.OrderCustomAttributes.length; i++) {
                if ($.fn.AtomiaShoppingCart.options.OrderCustomAttributes[i] == '{"Name":"' + name + '","Value":"' + value + '"}') {
                    tmpArray.splice(i, 1);
                }
            }
            $.fn.AtomiaShoppingCart.options.OrderCustomAttributes = tmpArray;
        }
    };
    $.fn.AtomiaShoppingCart.AddUpdateOrderCustomAttribute = function(name, value) {
        if (typeof (name) != 'undefined' && name != '' && typeof (value) != 'undefined' && value != '') {
            var updated = false;
            for (var i = 0; i < $.fn.AtomiaShoppingCart.options.OrderCustomAttributes.length; i++) {
                if (JSON.parse($.fn.AtomiaShoppingCart.options.OrderCustomAttributes[i]).Name == name) {
                    $.fn.AtomiaShoppingCart.options.OrderCustomAttributes[i] ='{"Name":"' + name + '","Value":"' + value + '"}';
                    updated = true;
                }
            }
            if (!updated) {
                $.fn.AtomiaShoppingCart.options.OrderCustomAttributes[$.fn.AtomiaShoppingCart.options.OrderCustomAttributes.length] = '{"Name":"' + name + '","Value":"' + value + '"}';
            }
        }
    };
    $.fn.AtomiaShoppingCart.GetCustomAttributesAsJson = function() {
        return '[' + $.fn.AtomiaShoppingCart.options.OrderCustomAttributes + ']';
    };
    $.fn.AtomiaShoppingCart.AddItem = function(item, itemdisplay, itemquantity, doRecalculation, renewalPeriod, isPackage, addToTheBeginning) {
        if (typeof renewalPeriod == 'undefined' || renewalPeriod == '')
        {
            renewalPeriod = '00000000-0000-0000-0000-000000000000';
        }
		if($.fn.AtomiaShoppingCart.CheckDuplicate(item,itemdisplay)){
			if (typeof addToTheBeginning != 'undefined' && addToTheBeginning) {
				cartArray.splice(0, 0, { id: item, display: itemdisplay, quantity: itemquantity, renewalPeriod: renewalPeriod, isPackage: isPackage });
			} else {
				cartArray[cartArray.length] = { id: item, display: itemdisplay, quantity: itemquantity, renewalPeriod: renewalPeriod, isPackage: isPackage };
			}        

			if (typeof doRecalculation != 'undefined' && doRecalculation) {
				globalCounter++;
				$.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
			}
		}
		
		$.fn.AtomiaShoppingCart.TriggerCartUpdated();
    };
	$.fn.AtomiaShoppingCart.CheckDuplicate = function(item,itemdisplay){
		for( x in cartArray ){
			if(cartArray[x].id == item && cartArray[x].display == itemdisplay){
				return false;
			}
		}
		return true;
	};
    $.fn.AtomiaShoppingCart.RemoveItem = function(item, itemdisplay, itemquantity, doRecalculation) {
        for (var i = 0; i < cartArray.length; i++) {
            if (cartArray[i].id == item && cartArray[i].display == itemdisplay && cartArray[i].quantity == itemquantity) {
                cartArray.splice(i, 1);
            }
        }
        if (typeof doRecalculation != 'undefined' && doRecalculation) {
            globalCounter++;
            $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
        }
		
		$.fn.AtomiaShoppingCart.TriggerCartUpdated();
    };
    $.fn.AtomiaShoppingCart.SwitchItem = function(oldId, olddisplay, oldquantity, oldRenewalPeriod, oldIsPackage, newId, newdisplay, newquantity, newRenewalPeriod, newIsPackage, doRecalculation) {
        for (var i = 0; i < cartArray.length; i++) {
            if (cartArray[i].id == oldId && cartArray[i].display == olddisplay && cartArray[i].quantity == oldquantity && cartArray[i].renewalPeriod == oldRenewalPeriod) {
                cartArray.splice(i, 1, { id: newId, display: newdisplay, quantity: newquantity, renewalPeriod: newRenewalPeriod, isPackage: newIsPackage });
            }
        }
        if (typeof doRecalculation != 'undefined' && doRecalculation) {
            globalCounter++;
            $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
        }
		
		$.fn.AtomiaShoppingCart.TriggerCartUpdated();
    };
    $.fn.AtomiaShoppingCart.ResetCart = function(doRecalculation) {
        cartArray = new Array();
        if (typeof doRecalculation != 'undefined' && doRecalculation) {
            globalCounter++;
            $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
        }
		
		$.fn.AtomiaShoppingCart.TriggerCartUpdated();
    };
    $.fn.AtomiaShoppingCart.RecalculateCart = function(counter) {
        var productIds = '';
        var productdisplayNames = '';
        var productQuantities = '';
        var renewalPeriods = '';

        for (var i = 0; i < cartArray.length; i++) {
            productIds += cartArray[i].id + "|";
            productdisplayNames += cartArray[i].display + "|";
            productQuantities += cartArray[i].quantity + "|";
            if (typeof cartArray[i].renewalPeriod == 'undefined' || cartArray[i].renewalPeriod == '') {
                renewalPeriods += "00000000-0000-0000-0000-000000000000|";
            }
            else {
                renewalPeriods += cartArray[i].renewalPeriod + "|";
            }
        }

        var htmlElement = $.fn.AtomiaShoppingCart.options.htmlElement;
        
        // check if processingImageDiv is added after table, if not add it
        if ($(htmlElement).next().length == 0 || $(htmlElement).next().attr('id') != 'processingImageDiv') {
            $(htmlElement).after('<div id="processingImageDiv" class="dataTables_processing" id="domainlist_processing" style="position: absolute">Processing...</div>');
        } 

        if ($(paymentMethodSelector).length > 0) {
            var paymentMethod = $(paymentMethodSelector).attr('value');
            if (paymentMethod == 'InvoiceByEmail' || paymentMethod == 'InvoiceByPost') {
                paymentMethod = 'PayWithInvoice';
            }
            $.fn.AtomiaShoppingCart.AddUpdateOrderCustomAttribute('PaymentMethod', paymentMethod);
        }

        $.postJSON(
            $.fn.AtomiaShoppingCart.options.ServerURL,
            {
                arrayOfProducts: productIds,
                arrayOfProductNames: productdisplayNames,
                arrayOfProductQuantities: productQuantities,
                arrayOfRenewalPeriods: renewalPeriods,
                displayProductName: $.fn.AtomiaShoppingCart.options.ProductName.display,
                displayProductPeriod: $.fn.AtomiaShoppingCart.options.ProductPeriod.display,
                displayProductNumberOfItems: $.fn.AtomiaShoppingCart.options.ProductNumberOfItems.display,
                displayProductPrice: $.fn.AtomiaShoppingCart.options.ProductPrice.display,
                displayProductDiscount: $.fn.AtomiaShoppingCart.options.ProductDiscount.display,
                displayProductTotalPrice: $.fn.AtomiaShoppingCart.options.ProductTotalPrice.display,
                displayOrderSubAmount: $.fn.AtomiaShoppingCart.options.OrderSubAmount.display,
                displayOrderTaxes: $.fn.AtomiaShoppingCart.options.OrderTaxes.display,
                displayOrderTotal: $.fn.AtomiaShoppingCart.options.OrderTotal.display,
                chosenCountry: ($.fn.AtomiaShoppingCart.options.ChosenCountryId.length > 0) ? $('#' + $.fn.AtomiaShoppingCart.options.ChosenCountryId).val() : $('#CountryCode').val(),
                globalCounter: counter,
                campaignCode: ($.fn.AtomiaShoppingCart.options.CampaignCodeRender() && $('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).length > 0) ? $('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).val() : '',
                pricesIncludingVAT: $.fn.AtomiaShoppingCart.options.PricesIncludingVAT,
                orderCustomAttributes: $.fn.AtomiaShoppingCart.GetCustomAttributesAsJson(),
                orderAddress: $.fn.AtomiaShoppingCart.options.AddOrderAddressData ? $.fn.AtomiaShoppingCart.GetOrderAddressDataAsJson() : ''
            },
            function(data) {
                
                if (counter == globalCounter) {
                    var htmlElement = $.fn.AtomiaShoppingCart.options.htmlElement;

                    var theadElement = $(document.createElement('thead'));

                    var theadtr = $(document.createElement('tr'));

                    var theadArray = [];

                    if ($.fn.AtomiaShoppingCart.options.ProductName.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductName;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductPeriod.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductPeriod;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductNumberOfItems.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductNumberOfItems;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductFullPrice.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductFullPrice;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductPrice.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductPrice;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductDiscount.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductDiscount;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductCommission.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductCommission;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductTotalPrice.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductTotalPrice;
                    }
                    if ($.fn.AtomiaShoppingCart.options.ProductAction.display) {
                        theadArray[theadArray.length] = $.fn.AtomiaShoppingCart.options.ProductAction;
                    }

                    jQuery.each(theadArray, function() {
                        CreateTHElement(this, theadtr);
                    });

                    $(theadElement).append($(theadtr));

                    var tbodyElement = $(document.createElement('tbody'));

                    CreateTBodyElement(tbodyElement, data);

                    var tfootElement = $(document.createElement('tfoot'));

                    jQuery.each($.fn.AtomiaShoppingCart.options, function(i, val) {
                        switch (i) {  
                            case "OrderSubAmount":                          
                            case "OrderTaxes":
                                if (($.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller == 'false') || ($.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller == undefined) || ($.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller == ''))
                                {
                                    CreateTFootElement(i, val, tfootElement, data);
                                }
                                break;                            
                            case "OrderTotal":
                                CreateTFootElement(i, val, tfootElement, data);
                                break;
                            default:
                                break;
                        }
                    });

                    // delete process iamge div and table clear
                    $('#processingImageDiv').remove();
                    $(htmlElement).html('');
                    $(htmlElement).append($(theadElement)).append($(tfootElement)).append($(tbodyElement));

                    var oTimerId;

                    if ($.fn.AtomiaShoppingCart.options.CampaignCodeRender()) {
                        if ($('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).length < 1) {

                            var couponCodeHtml = '';

                            couponCodeHtml += $.fn.AtomiaShoppingCart.options.CampaignCodeBeforeHtml();

                            couponCodeHtml += '<div class="formrow"><h5><label for="cartCouponCode">';
                            couponCodeHtml += $.fn.AtomiaShoppingCart.options.CampaignCodeLabel;
                            couponCodeHtml += '</label></h5><div class="col2row ">';
                            couponCodeHtml += '<input type="text" value="" name="' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId + '" id="' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId + '">';
                            couponCodeHtml += '</div><br class="clear"></div>';

                            couponCodeHtml += $.fn.AtomiaShoppingCart.options.CampaignCodeAfterHtml();

                            $(htmlElement).after(couponCodeHtml);
                        }

                        $('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).unbind('keyup').bind('keyup', function() {
                            window.clearTimeout(oTimerId);

                            oTimerId = window.setTimeout(function() {
                                $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
                            }, 500);
                        });

                    } else if ($('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).length > 0) {
                        $('#' + $.fn.AtomiaShoppingCart.options.CampaignCodeFieldId).remove();
                    }

                    if ($.fn.AtomiaShoppingCart.options.ProductAction.display) {
                        var table_id = $($.fn.AtomiaShoppingCart.options.htmlElement).attr('id');

                        var dButtonClass = $.fn.AtomiaShoppingCart.options.ProductAction.tbody.displayButtonClass;

                        if (dButtonClass.indexOf(' ') != -1) {
                            dButtonClass = dButtonClass.replace(/ /g, '.');
                        }

                        $('#' + table_id + ' .' + dButtonClass).click(
                            function() {
                                var trIndex = $(this).parent().parent()[0].rowIndex;
                                $.fn.AtomiaShoppingCart.options.DeleteButtonFunction($(this), cartArray[trIndex - 1].id, cartArray[trIndex - 1].display, cartArray[trIndex - 1].quantity);
                            }
                          );
                    }

                    if ($.fn.AtomiaShoppingCart.options.ProductPeriod.display && $.fn.AtomiaShoppingCart.options.ProductPeriod.change) {
                        var table_id = $($.fn.AtomiaShoppingCart.options.htmlElement).attr('id');

                        $('#' + table_id + ' select').change(
                            function() {
                                var trIndex = $(this).parent().parent()[0].rowIndex;
                                var selectedOptionData = $(this).val().split('|');
                                $.fn.AtomiaShoppingCart.options.ChangePeriodFunction($(this), cartArray[trIndex - 1].id, cartArray[trIndex - 1].display, cartArray[trIndex - 1].quantity, cartArray[trIndex - 1].renewalPeriod, cartArray[trIndex - 1].isPackage, selectedOptionData[0], cartArray[trIndex - 1].display, cartArray[trIndex - 1].quantity, selectedOptionData[1], cartArray[trIndex - 1].isPackage);
                            }
                          );
                    }

                    $.fn.AtomiaShoppingCart.options.TermsOfServicesRenderFunction(data.ItemsTermsResName, data.ItemsTermsResValue);

                }

                triggerShortening($.fn.AtomiaShoppingCart.options.vtipImagePath);

                // storing cart total
                $.fn.AtomiaShoppingCart.options.cartTotal = data.ShoppingCartTotal;
                $.fn.AtomiaShoppingCart.options.OnAfterRecalculation(data);
            }
        );
    };
    $.fn.AtomiaShoppingCart.TriggerCartUpdated = function () {
        $($.fn.AtomiaShoppingCart.options.htmlElement).trigger($.fn.AtomiaShoppingCart.options.CartUpdatedEventName);
    };
})(jQuery);

function CreateTHElement(elementFromJSON, trElement) {
    if (elementFromJSON.display) {
        var theadTH = $(document.createElement('th'));

        $(theadTH).attr(elementFromJSON.thead.attr);

        jQuery.each(elementFromJSON.thead.attr, function(i, valInner) {
            if (i == "colspan") {
                $(theadTH)[0].colSpan = valInner;
            }
        });

        $(theadTH).css(elementFromJSON.thead.css);

        $(theadTH).html(elementFromJSON.thead.displayText);

        $(trElement).append($(theadTH));
    }
}

function CreateTBodyElement(tbodyElement, data) {
    if (typeof data.ShoppingCartItems != 'undefined' && data.ShoppingCartItems.length > 0) {
        jQuery.each(data.ShoppingCartItems, function(itemKey, itemVal) {

            var tbodyTR = $(document.createElement('tr'));

            $(tbodyTR).attr('style', 'display: table-row;').attr('class', (itemKey % 2 == 0) ? 'even' : 'odd');

            jQuery.each($.fn.AtomiaShoppingCart.options, function(i, val) {
                if (val.display && typeof val.tbody != 'undefined') {
                    var tbodyTD = $(document.createElement('td'));

                    $(tbodyTD).attr(val.tbody.attr);
                    jQuery.each(val.tbody.attr, function(i, valInner) {
                        if (i == "colspan") {
                            $(tbodyTD)[0].colSpan = valInner;
                        }
                    });
                    $(tbodyTD).css(val.tbody.css);

                    switch (i) {
                        case "ProductName":
                            $(tbodyTD).html(itemVal.ProductName);
                            SetCartID(itemVal.ProductName);
                            $(tbodyTD).attr('id', GenerateCartProductNameID(itemVal.ProductName)); 
                            $(tbodyTD).addClass('js_atomiaTagged');
                            break;
                        case "ProductPeriod":
                            if (typeof itemVal.AvailablePeriodList != 'undefined' && itemVal.AvailablePeriodList != null && itemVal.AvailablePeriodList.length > 0) {
                                if (itemVal.AvailablePeriodList.length > 1 && $.fn.AtomiaShoppingCart.options.ProductPeriod.change) {
                                    var selectListHTML = $(document.createElement('select'));
                                    $(selectListHTML).attr('style', 'width: 65px;');

                                    jQuery.each(itemVal.AvailablePeriodList, function(optionIndex, optionValue) {
                                        var selectListOptionHTML = $(document.createElement('option'));
                                        selectListOptionHTML.val(optionValue.OptionID + '|' + optionValue.RenewalPeriodId);
                                        selectListOptionHTML.html(optionValue.OptionText);
                                        if (optionValue.OptionSelected) {
                                            selectListOptionHTML.attr('selected', 'selected');
                                        }

                                        $(selectListHTML).append($(selectListOptionHTML));
                                    });

                                    $(tbodyTD).append($(selectListHTML));
                                }
                                else {
                                    $(tbodyTD).attr('style', '');
                                    $(tbodyTD).html(itemVal.AvailablePeriodList[0].OptionText);
                                }
                                $(tbodyTD).attr('id', GenerateCartPeriodID(itemVal.ProductName)); 
                                $(tbodyTD).addClass('js_atomiaTagged');
                            }
                            break;
                        case "ProductNumberOfItems":
                            $(tbodyTD).html(itemVal.NumberOfItems);
                            break;
                        case "ProductFullPrice":
                            $(tbodyTD).html(itemVal.ProductFullPrice + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                            break;
                        case "ProductPrice":
                            $(tbodyTD).html(itemVal.ProductPrice + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>'); 
                            break;
                        case "ProductDiscount":
                            $(tbodyTD).html(itemVal.ProductDiscount + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                            break;
                        case "ProductCommission":
                            $(tbodyTD).html(itemVal.ProductResellerCommission + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                            break;
                        case "ProductTotalPrice":
                            $(tbodyTD).html(itemVal.ProductTotal + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                            $(tbodyTD).attr('id', GenerateCartPriceID(itemVal.ProductName));
                            $(tbodyTD).addClass('js_atomiaTagged');
                            break;
                        case "ProductAction":
                            if (itemVal.IsRemovable) {
                                $(tbodyTD).html('<a class="' + val.tbody.displayButtonClass + '" href="javascript:void(0);">' + val.tbody.displayButtonText + '</a>');
                                $(tbodyTD).attr('id', GenerateCartButtonID(itemVal.ProductName));
                                $(tbodyTD).addClass('js_atomiaTagged');
                            }
                            else {
                                $(tbodyTD).html('&nbsp');
                            }
                            break;
                    }

                    $(tbodyTR).append($(tbodyTD));
                }
            });

            $(tbodyElement).append($(tbodyTR));

        });
    } else {
        cartArray = new Array();
    }
}
function CreateTFootElement(elementName, elementFromJSON, tfootElement, data) {
    if (elementFromJSON.display) {

        switch (elementName) {
            case "OrderSubAmount":
                var tfootTR = $(document.createElement('tr'));

                jQuery.each(elementFromJSON.tfoot, function(i, val) {
                    var tfootTD = $(document.createElement('td'));

                    $(tfootTD).attr(val.attr);
                    jQuery.each(val.attr, function(i, valInner) {
                        if (i == "colspan") {
                            $(tfootTD)[0].colSpan = valInner;
                        }
                    });

                    if (typeof val.displayText != 'undefined') {
                        $(tfootTD).html(val.displayText + ':');
                    } else {
                        switch (i) {
                            case 0:
                            case 1:
                                if (typeof data.ShoppingCartSubtotal != 'undefined') {
                                    $(tfootTD).html(data.ShoppingCartSubtotal + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    $(tfootTR).append($(tfootTD));

                });

                $(tfootElement).append($(tfootTR));

                break;

            case "OrderTaxes":
                if (typeof data.ShoppingCartTaxes != 'undefined' && data.ShoppingCartTaxes.length > 0) {
                    jQuery.each(data.ShoppingCartTaxes, function(taxKey, taxVal) {

                        var tfootTR = $(document.createElement('tr'));

                        jQuery.each(elementFromJSON.tfoot, function(i, val) {
                            var tfootTD = $(document.createElement('td'));

                            $(tfootTD).attr(val.attr);

                            jQuery.each(val.attr, function(i, valInner) {
                                if (i == "colspan") {
                                    $(tfootTD)[0].colSpan = valInner;
                                }
                            });

                            switch (i) {
                                case 0:
                                    if (typeof taxVal.TaxVAT != 'undefined') {
                                        $(tfootTD).html((typeof val.displayText != 'undefined' ? val.displayText : taxVal.TaxName));
                                    }
                                    break;
                                case 1:
                                    if (typeof taxVal.TaxAmount != 'undefined') {
                                        $(tfootTD).html(taxVal.TaxAmount + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                                    }
                                    break;
                                default:
                                    break;
                            }

                            $(tfootTR).append($(tfootTD));

                        });

                        $(tfootElement).append($(tfootTR));

                    });
                }
                break;
            case "OrderTotal":
                var tfootTR = $(document.createElement('tr'));

                jQuery.each(elementFromJSON.tfoot, function(i, val) {
                    var tfootTD = $(document.createElement('td'));

                    $(tfootTD).attr(val.attr);
                    jQuery.each(val.attr, function(i, valInner) {
                        if (i == "colspan") {
                            $(tfootTD)[0].colSpan = valInner;
                        }
                    });

                    if (typeof val.displayText != 'undefined') {
                        $(tfootTD).html(val.displayText + ':'); 
                    } else {
                        switch (i) {
                            case 0:
                            case 1:
                                if (typeof data.ShoppingCartTotal != 'undefined') {
                                    $(tfootTD).html(data.ShoppingCartTotal + ' <span class="currency">' + data.ShoppingCartCurrency + '</span>');
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    $(tfootTR).append($(tfootTD));

                });

                $(tfootElement).append($(tfootTR));

                break;
            default:
                break;
        }
    }
}
function triggerShortening(img_url) {
    $('.js_labelShortenCart').each(function() {
        var stringToShorten = $(this).text();
        var arrTld = $(this).text().split('.');
        if (stringToShorten.length > 15) {
            if (arrTld.length > 1) {
                stringToShorten = stringToShorten.substring(0, 15).split('.')[0] + "..." + arrTld[arrTld.length - 1];
            }
            else {
                stringToShorten = stringToShorten.substring(0, 15) + "...";
            }
        }
        var title = $(this).html();
        var shorten = "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + stringToShorten + "</div>";

        $(this).html(shorten);
    });
    vtip(img_url);
}