function addTermValidation(errorMessage) {
    jQuery.validator.addMethod(
        "ValidateTerm", function(value, element, params) {
            return ValidateTerm(value, element, params); 
        }
    );

    $('#errorTerm').rules("add", {
        ValidateTerm: true,
        messages: {
            ValidateTerm: errorMessage
        }
    });
}

function initializeButtons(submitParams) {
    $('#orderbutton').bind('click', function (submitParams) {
        if (window.formValidator !== null) {
            canSubmit = window.formValidator.valid();
        }

        onSubmit(submitParams);
    });
}

var focusedIn = false;
function initializeEmailChange() {
    $("#Email").focusin(function () {
        focusedIn = true;
    });
    $("#Email").focusout(function () {
        focusedIn = false;
    });
    $("#Email").change(function () {
        if (!focusedIn) {
            //let's call focus out so that validation is initiated, because browser did the autofill (because input value was changed without getting focus first).
            $("#Email").focusout();
        }
    });
}

window.productsId = {};
window.productsKey = [];
function exampleProductRestrictionFunction(exist,changed){
	if(exist){
			// Always executed if exist
			if(changed){
				//console.log('Count of product changed');
			}else{
				//console.log('Nothing changed');
			}
		}else{
			// Executed once when product get removed
		}
}
function domainNL(exist,changed){
	if(exist){
		if(typeof window.initDomainRestrictionNl == "undefined"){
			window.initDomainRestrictionNl = true;
			$('#CountryCode').rules('add', 
						 {
							onlyNetherlands:true,
							messages: {
								onlyNetherlands: "Netherlands"
							}
						 });
		}
		if(changed){
			//console.log('Count of product changed');
		}else{
			//console.log('Nothing changed');
		}
	}else{
		$('#CountryCode').rules('remove','onlyNetherlands');
	}
}
function off365(exist,changed){
	if(exist){
		if($('label[for=contact_company]').hasClass( 'required' ) == 0){
						$('label[for=contact_company]').addClass('required').html('<span>*</span>'+$('label[for=contact_company]').text());
						$('#Company').rules('add', {
							required:true,
							messages: {
								required: window.ResourceValidation.Required
							}
						
						});
					}
		if(changed){
			//console.log('Count changed');
		}else{
			//console.log('Nothing changed');
		}
	}else{
		$('label[for=contact_company]').removeClass('required');
		$('label[for=contact_company] span').remove();
		$('#Company').rules('remove', 'required');
	}
}
function domainNo(exist,changed){
	if(exist){
		if(typeof window.initDomainRestrictionNo == "undefined"){
			window.initDomainRestrictionNo = true;
			
					window.hasNo = true;
					if($('label[for=contact_company]').hasClass( 'required' ) == 0){
						$('label[for=contact_company]').addClass('required').html('<span>*</span>'+$('label[for=contact_company]').text());
						$('#Company').rules('add', {
							required:true,
							messages: {
								required: window.ResourceValidation.Required
							}
						
						});
					}
					if($('label#labelOrgNo').hasClass( 'required' ) == 0){
						$('label#labelOrgNo').addClass('required').html('<span>*</span>'+$('label#labelOrgNo').text());
						$('#OrgNumber').rules('add', {
							required:true,
							messages: {
								required: window.ResourceValidation.Required
							}
						
						});
					}
					$('#YouAreCompany').rules('add', 
					{
						noDomaincompany:true,
						messages: {
							noDomaincompany: window.ResourceValidation.MustBeCompany
						}
					 });
					 $('#CountryCode').rules('add', 
					 {
						onlyNorway:true,
						messages: {
							onlyNorway: window.ResourceValidation.MustBeFromNorway
						}
					 });
					 $('#SignedName').rules('add', 
					 {
						required:true,
						messages: {
							required: window.ResourceValidation.Required
						}
					 });
					 $('#DomainSpeciffic').hide();
					 $('#DomainSpeciffic').rules('add', 
					 {
						required:true,
						messages: {
							required: window.ResourceValidation.Declaration
						}
					 });
					 $('#Acceptdeclaration').rules('add', 
					 {
						required:true,
						messages: {
							required: window.ResourceValidation.Required
						}
					 });
					 $('#SignedName, #Acceptdeclaration').prop('disabled', false);
					 $('#DomainSpeciffic').val('');
					 $('#step_2 #nodeclaration').show();
					 function noridconfirmation(e){

					     if (window.formValidator.element('#Company')
							& window.formValidator.element('#OrgNumber')
							& window.formValidator.element('#SignedName')) {
							window.nuDomains = [];
							function isNoDomain(element, index, array) {
							  return (element.id == 'DOM_NO');
							}
							var filteredNo = cartArray.filter(isNoDomain);
							for(var x in filteredNo){
							  nuDomains.push(filteredNo[x].display);
							}
							window.timeprocessed = new Date();
							timeprocessed = timeprocessed.toISOString().slice(0, -5)+"Z";
							timeJSON = timeprocessed.slice(0,10)+" "+timeprocessed.slice(11,19);
							if($(this).prop('tagName') != 'INPUT'){
								e.preventDefault();
								window.open ("/Norid?name="+$('#SignedName').val()+"&orgid="+$('#OrgNumber').val()+"&company="+$('#Company').val()+"&domains="+nuDomains.join('|')+"&time="+timeprocessed,"mywindow","status=1");
							}else{
								$('#DomainSpeciffic').val('{"AcceptName": "'+$('#SignedName').val()+'", "AcceptDate": "'+timeJSON+'", "AcceptVersion": "2.0" }');
								$('#DomainSpeciffic').valid();
							}
						}else{
							e.preventDefault();
							var fillnames = '';
							if (!window.formValidator.element('#Company')) {
								fillnames += window.ResourceValidation.Company+'<br/>';
							}
				            if (!window.formValidator.element('#OrgNumber')) {
								fillnames += window.ResourceValidation.OrgNum+'<br/>';
							}
				            if (!window.formValidator.element('#SignedName')) {
								fillnames += window.ResourceValidation.SignedName+'<br/>';
							}
							setNotificationMessage({
								wasAnError: 1,
								NotificationText: fillnames,
								title: window.ResourceValidation.DeclarationFill
							});
						}
					 }
					 $('#nodeclaration .noriddeclaration').unbind('click').click(noridconfirmation);
					 
					 $('#Acceptdeclaration').unbind('click').bind("click",noridconfirmation);
		}			
		if(changed){
			$('#DomainSpeciffic').val('');
			$('#Acceptdeclaration').attr('checked', false);
		}else{
			//console.log('Nothing changed');
		}
		$('#SignedName, #Acceptdeclaration').prop('disabled', false);
		window.hasNo = true;
	}else{
		$('#step_2 #nodeclaration').hide();
		$('#CountryCode').rules('remove','onlyNorway');
		$('#YouAreCompany').rules('remove','noDomaincompany');
		$('label#labelOrgNo').removeClass('required');
		$('label#labelOrgNo span').remove();
		$('#OrgNumber').rules('remove', 'required');
		$('#DomainSpeciffic').val('');
		window.hasNo = false;
	}
}
//BIND FUNCTION TO PRODUCT ID
window.productsFunct = {
	'DMN_NO':domainNo,
	'DMN_NL':domainNL
};
//CHECKS ALL PRODUCTS IN THE CART AND CALL FUNCTION IF BINDED
window.checkCurrentProducts = function(){
	tmpproductsId = {};
	tmpproductsKey = [];
	for(var x in cartArray){
		if( typeof tmpproductsId[cartArray[x].id] == "undefined"){
			tmpproductsId[cartArray[x].id] = 1;
			tmpproductsKey.push(cartArray[x].id);
		}else{
			tmpproductsId[cartArray[x].id]++;
		}
	}
	for(var x in tmpproductsKey){
		if(typeof window.productsFunct[tmpproductsKey[x]] != "undefined"){
			if(typeof productsId[tmpproductsKey[x]] != "undefined"){
				if(productsId[tmpproductsKey[x]] != tmpproductsId[tmpproductsKey[x]]){
					window.productsFunct[tmpproductsKey[x]](true,true);
				}else{
					productsId[tmpproductsKey[x]] = tmpproductsId[tmpproductsKey[x]];
					window.productsFunct[tmpproductsKey[x]](true,false);
				}
			}else{
				window.productsFunct[tmpproductsKey[x]](true,true);
			}
		}
	}
	for(var x in productsKey){
		if(typeof window.productsFunct[productsKey[x]] != "undefined"){
			if(typeof tmpproductsId[productsKey[x]] == "undefined"){
				window.productsFunct[productsKey[x]](false,false);
			}
		}
	}
	productsId = tmpproductsId;
	productsKey = tmpproductsKey;
}

/* INFOTIPS START */
$('#submit_form input').focus(function() {
    if ($('#infotip' + $(this).attr('id')).css('display') == 'none') {
        $('#infotip' + $(this).attr('id')).css('display', 'block');
    }
});
$('#submit_form input').blur(function() {
    if ($('#infotip' + $(this).attr('id')).css('display') != 'none') {
        $('#infotip' + $(this).attr('id')).css('display', 'none');
    };
});
/* INFOTIPS END */

var initializeDecimalParser = function(params) {
    jQuery.fn.parse.defaults = {
        locale: params.Locale,
        decimalSep: params.DecimalSeparator,
        groupSep: params.GroupSeparator,
        decimalSeparatorAlwaysShown: true
    };

    jQuery.fn.format.defaults = {
        format: params.CurrencyDecimalPlacesFormat,
        decimalSep: params.DecimalSeparator,
        groupSep: params.GroupSeparator,
        locale: params.Locale,
        decimalSeparatorAlwaysShown: true
    };
};

var initializeAdditionalThemeMethods = function(params) {
    return null;
};

var initializeAdditionalThemeMethodsDocReady = function (params) {
    window.ResourceValidation = {};
    window.ResourceValidation.Required = params.ResourceValidationRequired;
    window.ResourceValidation.Declaration = params.ResourceValidationDeclaration;
    window.ResourceValidation.DeclarationFill = params.ResourceValidationDeclarationFill;
    window.ResourceValidation.Company = params.ResourceCompany;
    window.ResourceValidation.OrgNum = params.ResourceOrgNum;
    window.ResourceValidation.SignedName = params.ResourceSignedName;
    window.ResourceValidation.MustBeCompany = params.ResourceMustBeCompany;
    window.ResourceValidation.MustBeFromNorway = params.ResourceMustBeFromNorway;
    jQuery.validator.addMethod("noDomaincompany", function (value, element) {
        if ($("#product_list td:contains('.no')").length != 0) {
            return $('#YouAreCompany').val() == "company";
        } else {
            return true;
        }
    }, "");
    jQuery.validator.addMethod("onlyNorway", function (value, element) {
        if ($("#product_list td:contains('.no')").length != 0) {
            return value == "NO";
        } else {
            return true;
        }
    }, "");
    jQuery.validator.addMethod("onlyNetherlands", function (value, element) {
        if ($("#product_list td:contains('.nl')").length != 0) {
            return value == "NL";
        } else {
            return true;
        }
    }, "");
    jQuery.validator.addMethod("noridDeclaration", function (value, element) {
        if ($("#product_list td:contains('.no')").length != 0) {
            return typeof window.timeprocessed != "undefined";
        } else {
            return true;
        }
    }, "");
    $('#CountryCode').change(function () {
        $("#YouAreCompany").trigger('change');
    });
};

var setNotificationMessage = function(notificationParams) {
    // Regular error
    var message = "";
    if (notificationParams.wasAnError == 1) {
        message = notificationParams.NotificationText;
        $('#notification').notification('notify', notificationParams.title, message);
    }
    // Payment error
    else if (notificationParams.wasAnError == 2) {
        message = notificationParams.NotificationTextPayment;
        $('#notification').notification('notify', notificationParams.title, message);
    }
};

var setOrderByPost = function(model) {
    if (model == 'post') {
        OrderByPostSelected = true;
    }
};

var initializeVtip = function(img_url) {
    vtip(img_url);
};

var countryChangeBind = function (params) {
    $("#CountryCode").change(function () {
        globalCounter++;
        $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
    });
};

var paymentMethodEmailBind = function(params) {
    $('#InvoiceByEmail').bind('click', function (event, preventRecalculation) {
        $("#cc_paymentDiv").hide();
        $('#BillingText').html($('#BillingTextEmailContainer').html());
        $('#ActivationText').text(params.ActivationTextMail);
        $.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller = $("#dontShowTaxesForThisResellerHidden").val();
        $.fn.AtomiaShoppingCart.AddOrderCustomAttribute('PayByInvoice', 'true');
        if (typeof (preventRecalculation) == 'undefined' || preventRecalculation == null || !preventRecalculation) {
            globalCounter++;
            $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
        }
    });
};

var paymentMethodPostBind = function(params) {
    $("#InvoiceByPost").click(function () {
        $("#cc_paymentDiv").hide();
        $('#BillingText').html($('#BillingTextPostContainer').html());
        $('#ActivationText').text(params.ActivationTextPost);
        $.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller = $("#dontShowTaxesForThisResellerHidden").val();
        $.fn.AtomiaShoppingCart.AddOrderCustomAttribute('PayByInvoice', 'true');
        globalCounter++;
        $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
    });
};

var paymentMethodCardBind = function(params, container) {
    container.click(function () {
		$('p.paymentNeededNotification').hide();
		$('#paymentPluginList').hide();
		$('#paymentPluginPayPal').hide();
		$('#paymentPluginCCPayment, #paymentPluginPayExRedirect').show();
		$("#cc_paymentDiv").show();
		$('#BillingText').html($('#BillingTextCCContainer').html());
		$('#ActivationText').text(params.ActivationTextCC);
		$.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller = $("#dontShowTaxesForThisResellerHidden").val();
		$.fn.AtomiaShoppingCart.RemoveOrderCustomAttribute('PayByInvoice', 'true');
		globalCounter++;
		$.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
	});
};

var paymentMethodPayPalBind = function(params) {
    $("#PayPal").click(function () {
        $('p.paymentNeededNotification').hide();
        $('#paymentPluginList').hide();
        $('#paymentPluginCCPayment, #paymentPluginPayExRedirect').hide();
        $('input[name="pluginSelector"][value="PayPal"]').attr('checked','checked');
        $('#paymentPluginPayPal').show();
        $("#cc_paymentDiv").show();
        $('#BillingText').html($('#BillingTextPayPalContainer').html());
        $('#ActivationText').text(params.ActivationTextCC);
        $.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller = $("#dontShowTaxesForThisResellerHidden").val();
        $.fn.AtomiaShoppingCart.RemoveOrderCustomAttribute('PayByInvoice', 'true');
        globalCounter++;
        $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
    });
};

// dissallow clicking submit button more then once
var submitOnceUnbind = function() {
    $('#submit_form').submit(function() {
        if ($("#submit_form").valid()) {
            $('#submit_form input,select').unbind().keydown(function(e) {
                if (e.keyCode == 13) {
                    return false;
                }
            });
            $('#orderbutton').unbind().bind('click', function() {
                return false;
            });
        }
    });
};

var onSubmit = function (params) {
    var ids = "";
    var productsCounter = 0;

    // From cart partial .js, contains info about products in cart
    for (var i = 0; i < cartArray.length; i++) {
        ids += cartArray[i].id + '|' + cartArray[i].display + '|' + cartArray[i].renewalPeriod + '|';
        productsCounter++;
    }

    var domains = "";
    $('#domainsDiv table tbody tr[id]').each(function () {
        domains += $(this).find('.vtip').attr('title') + " ";
    });

    if (productsCounter < 2 && params.IsFirstOption == 'True') {
        $('#ArrayOfProducts').val('');
        $('#SearchDomains').val(domains);
    } else {
        $('#ArrayOfProducts').val(ids);
        $('#SearchDomains').val(domains);
    }

    if (typeof ($('#PostNumber')) != 'undefined' && $('#PostNumber').val() !== null) {
        $('#PostNumber').val($('#PostNumber').val().toUpperCase());
    }

    if (typeof ($('#InvoicePostNumber')) != 'undefined' && $('#InvoicePostNumber').val() !== null) {
        $('#InvoicePostNumber').val($('#InvoicePostNumber').val().toUpperCase());
    }

    AtomiaOrderForm.copyAllFromMainAddress();
    AtomiaOrderForm.showInvalidAddresses();

    if (canSubmit) {
        $("#submit_form").submit();
    }
};

var setSubmitFormAdditionalThemeRules = function(params) {
    
};

var addBillingCustomerDataBlur = function(defaultString) {
    $("#ContactName,#ContactLastName,#Company,#InvoiceContactName,#InvoiceContactLastName,#InvoiceCompany").blur(function() {
        if ($('#SecondAddress').val() == 'false')
        {
            if($("#Company").val() != "")
            {
                $("#BillingContactCustomerText").text($("#Company").val());
            }
            else if($("#ContactName").val() != "" && $("#ContactLastName").val() != "")
            {
                $("#BillingContactCustomerText").text($("#ContactName").val() + " " + $("#ContactLastName").val());
            }
            else
            {
                $("#BillingContactCustomerText").text(defaultString);
            }
        }
        else
        {
            if($("#InvoiceCompany").val() != "")
            {
                $("#BillingContactCustomerText").text($("#InvoiceCompany").val());
            }
            else if($("#InvoiceContactName").val() != "" && $("#InvoiceContactLastName").val() != "")
            {
                $("#BillingContactCustomerText").text($("#InvoiceContactName").val() + " " + $("#InvoiceContactLastName").val());
            }
            else
            {
                $("#BillingContactCustomerText").text(defaultString);
            }
        }
    });
};

var addTechCustomerDataBlur = function(defaultString) {
    $("#ContactName,#ContactLastName,#Company").blur(function() {
        if ($("#Company").val() != "") {
            $("#TechContactCustomerText").text($("#Company").val());
        }
        else if ($("#ContactName").val() != "" && $("#ContactLastName").val() != "") {
            $("#TechContactCustomerText").text($("#ContactName").val() + " " + $("#ContactLastName").val());
        }
        else {
            $("#TechContactCustomerText").text(defaultString);
        }
    });
};


var AtomiaOrderForm = (function (jQuery) {
    var $ = jQuery,
        fieldPrefix = '',
        isSecondAddressVisible = false,
        isWhoisContactVisible = false,
        selectedArticleNumbers = new Set(),
        selectedProductCategories = new Set(),
        productsChangedEvent = "ProductsChanged",
        itemCategories;

    function Set() {
        this.hash = {};
    }
    Set.prototype.add = function (val) {
        if (!this.hash.hasOwnProperty(val)) {
            this.hash[val] = 0;
        }

        this.hash[val] += 1;
    };
    Set.prototype.remove = function (val) {
        if (this.hash.hasOwnProperty(val)) {
            if (this.hash[val] > 0) {
                this.hash[val] -= 1;
            } else {
                delete this.hash[val];
            }
        }
    };
    Set.prototype.toList = function () {
        var property,
		    hashList = [];

        for (property in this.hash) {
            if (this.hash.hasOwnProperty(property) && this.hash[property] > 0) {
                hashList.push(property);
            }
        }

        return hashList;
    };

    function showSecondAddress() {
        $("#secondAddress").show('blind', 500);
        $('#billing-text-open').hide();
        $('#billing-text-close').show();
        $("#SecondAddress").val(true);
        isSecondAddressVisible = true;
    }

    function showWhoisContact() {
        $("#whoisContactAddress").show('blind', 500);
        $('#whois-text-open').hide();
        $('#whois-text-close').show();
        $('#WhoisContact').val(true);
        isWhoisContactVisible = true;
    }

    // Copy all content from main address to other addresses, if those are not visible
    function copyAllFromMainAddress() {
        // No need to copy anything
        if (isSecondAddressVisible && isWhoisContactVisible) {
            return;
        }

        $("#mainAddress input, #mainAddress select").each(function () {

            var $mainField = $(this),
                mainFieldId = $mainField.attr("id"),
                $secondAddressField,
                $whoisContactField;


            if (!isSecondAddressVisible) {
                $secondAddressField = $('#' + 'Invoice' + mainFieldId);
                if ($secondAddressField.length !== 0) {
                    $secondAddressField.val($mainField.val());
                }
            }

            if (!isWhoisContactVisible) {
                $whoisContactField = $('#' + 'DomainReg' + mainFieldId);
                if ($whoisContactField.length !== 0) {
                    $whoisContactField.val($mainField.val());
                }
            }
        });

        if (!isSecondAddressVisible) {
            $("#secondAddress input, #secondAddress select").each(function () {
                $(this).blur();
            });
        }

        if (!isWhoisContactVisible) {
            $("#whoisContactAddress input, #whoisContactAddress select").each(function () {
                $(this).blur();
            });
        }
    }

    function showInvalidAddresses() {
        var secondAddressValid = true,
            whoisContactValid = true;

        if (!isSecondAddressVisible) {
            $("#secondAddress input, #secondAddress select").each(function () {
                var isValid = $(this).valid();

                if (!isValid) {
                    secondAddressValid = false;
                }
            });
        }

        if (!secondAddressValid && !isSecondAddressVisible) {
            showSecondAddress();
        }

        if (!isWhoisContactVisible) {
            $("#whoisContactAddress input, #whoisContactAddress select").each(function () {
                var isValid = $(this).valid();

                if (!isValid) {
                    whoisContactValid = false;
                }
            });
        }

        if (!whoisContactValid && !isWhoisContactVisible) {
            showWhoisContact();
        }
    }

    function secondAddressInit() {
        $("#billing-trigger-open").click(function () {
            copyAllFromMainAddress();
            showSecondAddress();
        });

        $('#billing-trigger-close').click(function () {
            $("#secondAddress").hide('blind', 500);
            $('#billing-text-open').show();
            $('#billing-text-close').hide();
            $("#SecondAddress").val(false);
            isSecondAddressVisible = false;
        });
    }

    function whoisContactInit(enabled) {
        if (enabled === false) {
            $("#whoisContactAddress input, #whoisContactAddress select").each(function () {
                $(this).rules("remove");
            });
        } else {
            $("#whois-trigger-open").click(function () {
                copyAllFromMainAddress();
                showWhoisContact()
            });

            $("#whois-trigger-close").click(function () {
                $("#whoisContactAddress").hide('blind', 500);
                $('#whois-text-open').show();
                $('#whois-text-close').hide();
                $('#WhoisContact').val(false);
                isWhoisContactVisible = false;
            });
        }
    }

    function getSelectedArticleNumbers() {
        return selectedArticleNumbers.toList();
    }

    function getSelectedProductCategories() {
        return selectedProductCategories.toList();
    }

    function cartChangeBind(cartChangeEventName) {
        $("#product_list").on(cartChangeEventName, function () {
            var cartLength = cartArray.length,
                i,
                artno;

            selectedArticleNumbers = new Set();
            selectedProductCategories = new Set();

            for (i = 0; i < cartLength; i += 1) {
                artno = cartArray[i].id;
                selectedArticleNumbers.add(artno);
                selectedProductCategories.add(itemCategories[artno]);
            }

            $("#product_list").trigger(productsChangedEvent);
        });
    }

    function setItemCategories(val) {
        itemCategories = val;
    }

    return {
        copyAllFromMainAddress: copyAllFromMainAddress,
        secondAddressInit: secondAddressInit,
        whoisContactInit: whoisContactInit,
        getSelectedArticleNumbers: getSelectedArticleNumbers,
        getSelectedProductCategories: getSelectedProductCategories,
        setItemCategories: setItemCategories,
        productsChangedEvent: productsChangedEvent,
        cartChangeBind: cartChangeBind,
        showInvalidAddresses: showInvalidAddresses
    };
})(jQuery);

