var OrderPageWizzard = {
    skipFirstNSteps: 0,
    currentStep: 0,
    Validator: null,
    InitializeButtons: function (submitParams) {
        $('#orderbutton').bind('click', function (submitParams) {
            if (OrderPageWizzard.Validator !== null) {
                canSubmit = OrderPageWizzard.Validator.valid();
            }

            $("#VATValidationMessage").val($("#vatValidationInfo").text());
            onSubmit(submitParams);
        });
    },
    ValidateStep: function () {
        // contact info
        var result = true;
        switch (OrderPageWizzard.currentStep) {
			case 1:{
					
					window.hasNo = false;
					checkCurrentProducts();
					break;
				}
            case 2:{
				
                if (OrderPageWizzard.Validator !== null) {


                    if ($('#OrgNumber').length > 0) {
                        result = result & $("#submit_form").validate().element("#OrgNumber");
                    }
					if(hasNo){
						result = result 
						& OrderPageWizzard.Validator.element('#YouAreCompany') 
						& OrderPageWizzard.Validator.element('#Company') 
						& OrderPageWizzard.Validator.element('#OrgNumber')
						& OrderPageWizzard.Validator.element('#SignedName')
						& OrderPageWizzard.Validator.element('#Acceptdeclaration')
						& OrderPageWizzard.Validator.element('#DomainSpeciffic')
						& OrderPageWizzard.Validator.element('#CountryCode');
					}
                }
				}
        }

        return result;
    }
};

window.noLength = 0;
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
};
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
						
						if(OrderPageWizzard.Validator.element('#Company') 
							& OrderPageWizzard.Validator.element('#OrgNumber') 
							& OrderPageWizzard.Validator.element('#SignedName')){
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
							if(!OrderPageWizzard.Validator.element('#Company')){
								fillnames += window.ResourceValidation.Company+'<br/>';
							}
							if(!OrderPageWizzard.Validator.element('#OrgNumber')){
								fillnames += window.ResourceValidation.OrgNum+'<br/>';
							}
							if(!OrderPageWizzard.Validator.element('#SignedName')){
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
};
//BIND FUNCTION TO PRODUCT ID
window.productsFunct = {
	'DOM_NO':domainNo,
	'BHP':off365,
	'BH':off365,
	'DOM_NL':domainNL
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
        format: "#,###.00",
        decimalSep: params.DecimalSeparator,
        groupSep: params.GroupSeparator,
        locale: params.Locale,
        decimalSeparatorAlwaysShown: true
    };
};

var initializeAdditionalThemeMethods = function(params) {
    return null;
};

var initializeAdditionalThemeMethodsDocReady = function(params) {
	window.ResourceValidation = {};
	window.ResourceValidation.Required = params.ResourceValidationRequired;
	window.ResourceValidation.Declaration = params.ResourceValidationDeclaration;
	window.ResourceValidation.DeclarationFill = params.ResourceValidationDeclarationFill;
	window.ResourceValidation.Company = params.ResourceCompany;
    window.ResourceValidation.OrgNum = params.ResourceOrgNum;
    window.ResourceValidation.SignedName = params.ResourceSignedName;
    window.ResourceValidation.MustBeCompany = params.ResourceMustBeCompany;
    window.ResourceValidation.MustBeFromNorway = params.ResourceMustBeFromNorway;
	jQuery.validator.addMethod("noDomaincompany", function(value, element) {
		if($("#product_list td:contains('.no')").length != 0){
			return $('#YouAreCompany').val() == "company";
		}else{
			return true;
		}
	}, "");
	jQuery.validator.addMethod("onlyNorway", function(value, element) {
		if($("#product_list td:contains('.no')").length != 0){
			return value == "NO";
		}else{
			return true;
		}
	}, "");
	jQuery.validator.addMethod("onlyNetherlands", function(value, element) {
		if($("#product_list td:contains('.nl')").length != 0){
			return value == "NL";
		}else{
			return true;
		}
	}, "");
	jQuery.validator.addMethod("noridDeclaration", function(value, element) {
		if($("#product_list td:contains('.no')").length != 0){
			return typeof window.timeprocessed != "undefined";
		}else{
			return true;
		}
	}, "");
		$('#CountryCode').change(function(){
			$("#YouAreCompany").trigger('change');
		});
}

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
        $("#Email").focus();
        $("#Email").blur();
        $("#ContactName").focus();
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

var companyKeyUpBind = function () {
    $('#Company').blur(function (event) {
        if ($('#OrgNumber').length > 0) {
            if ($('#OrgNumber').val() != '') {
                $("#submit_form").validate().element($('#OrgNumber'));
            }
        }
    });
};

var countryChangeBind = function (params) {
    $("#CountryCode").change(function () {
        globalCounter++;
        $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);

        $("#submit_form").validate();
    });
};

var ValidateVATNumberOnExistence = function(value, element, params) {
    var resultAjax = true;
    //var validationMessage = '';

    if ($('#Company').val() != '') {

        if (ValidateOrgNumberCheckSum(value, element, params) == true) {
            
            var countryCode = $('#CountryCode').val();
            var vatNumber = $('#OrgNumber').val();

            if (countryCode == 'SE') {                
                vatNumber = $("#OrgNumber").val() + '01';
            }
//            vatNumber = $("#OrgNumber").val() + '01';
//            countryCode = 'SE';
            if (vatNumber.length > 0) {
                var nothingUseful = $.ajax({
                    url: validateVATNumberParams.ValidateVATNumberUrl,
                    global: false,
                    type: "Get",
                    data: {
                        sEcho: "0",
                        countryCode: countryCode,
                        VATNumber: vatNumber
                    },
                    dataType: "html",
                    async: false,
                    success: function (result) {
                        var sEcho = JSON.parse(result)['sEcho'];
                        var validationResult = JSON.parse(result)['validationResult'];
                        var success = JSON.parse(result)['success'];
                        var error = JSON.parse(result)['error'];
                        $("#vatValidationInfo").text('');

                        if (success == true) {

                            //validationResult: invalid, valid, validationerror
                            
                            if (validationResult == 'invalid') {
                                //resultAjax = false;
                                $("#vatValidationInfo").text(validateVATNumberParams.ValidationResultNotValidated);
                            }
                            else {
                                if (validationResult == 'validationerror') {
                                    $("#vatValidationInfo").text(validateVATNumberParams.ValidationResultNotValidated);
                                }
                            }

                            $.fn.AtomiaShoppingCart.globalCounter++;
                            $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
                            
                        }
                        else {
                            $("#vatValidationInfo").text(validateVATNumberParams.ValidationResultNotValidated);                            
                        }

                    }
                });
            }
        } //end of if (ValidateOrgNumberCheckSum() == true) {
    }

    return resultAjax;    

}

var emailBlurBind = function(params) {
    $("#Email").blur(function(e) {
        $(this).val($(this).val().toLowerCase());
        if ($("#submit_form").validate().element($(this))) {
        
            canSubmit = false;
            $("#email_check_loading").show();
            if (CheckEmailDomain($(this).val(), params)) {
                $("#Email").attr("class", "errorinfo");
                $("#email_domain_error").show();
            }
            else {
                $("#email_domain_error").hide();
                $("#Email").removeAttr("class");

                $("#email_check_loading").show();
                if (CheckEmail($(this).val(), params)) {
                    $("#Email").attr("class", "errorinfo");
                    $("#email_error").show();
                }
                else {
                    $("#email_error").hide();
                    $("#Email").removeAttr("class");
                    canSubmit = true;
                }
            }
            setTimeout('$("#email_check_loading").hide();', 1000);
        }
    });
};

var invoiceEmailBlurBind = function () {
    $("#InvoiceEmail").blur(function (e) {
        $(this).val($(this).val().toLowerCase());
        if ($('#secondAddressTrue').is(':checked')) {
            $("#submit_form").validate().element($("#InvoiceEmail"));
        }
    });
};

var emailKeyUpBind = function() {
    $('#Email').keyup(function(event) {

        $("#email_error").hide();
        $("#email_domain_error").hide();
        $("#Email").removeAttr("class");
    });
};

var secondAddressRadioBind = function() {
    $("#secondAddressTrue").click(function() {
        $("#secondAddress").show();
    });

    $("#secondAddressFalse").click(function() {
        $("#secondAddress").hide();
    });
};

var paymentMethodEmailBind = function(params) {
    $('#PaymentMethodEmail').bind('click', function(event, preventRecalculation) {
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
    $("#PaymentMethodPost").click(function() {
        $("#cc_paymentDiv").hide();
        $('#BillingText').html($('#BillingTextPostContainer').html());
        $('#ActivationText').text(params.ActivationTextPost);
        $.fn.AtomiaShoppingCart.dontShowTaxesForThisReseller = $("#dontShowTaxesForThisResellerHidden").val();
        $.fn.AtomiaShoppingCart.AddOrderCustomAttribute('PayByInvoice', 'true');
        globalCounter++;
        $.fn.AtomiaShoppingCart.RecalculateCart(globalCounter);
    });
};

var paymentMethodCarBind = function(params) {
	$("#PaymentMethodCard").click(function() {
		$('p.paymentNeededNotification').hide();
		$('#paymentPluginList').hide();
		$('#paymentPluginPayPal').hide();
		$('input[name="pluginSelector"][value="CCPayment"], input[name="pluginSelector"][value="PayExRedirect"], input[name="pluginSelector"][value="WorldPayRedirect"],  input[name="pluginSelector"][value="WorldPayXmlRedirect"], input[name="pluginSelector"][value="DibsFlexwin"], input[name="pluginSelector"][value="AdyenHpp"]').attr('checked', 'checked');
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
    $("#PaymentMethodPayPal").click(function() {
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

var fillPaymentMethod = function(model) {
    if (model == 'email') {
        $("#PaymentMethodEmail").click();
    } else if (model == 'post') {
        $("#PaymentMethodPost").click();
    } else if (model == 'card') {
        $("#PaymentMethodCard").click();
    }
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

var onSubmit = function(params) {
    if ($('#CountryCode').val() == 'SE') {
        if ($("#Company").val() == '') {
            $("#VATNumber").val('');
        } else {
            // company or organisation
            $("#VATNumber").val('SE' + $("#OrgNumber").val() + '01');
        }
    }

    var ids = "";
    var productsCounter = 0;

    // From cart partial .js, contains info about products in cart
    for (var i = 0; i < cartArray.length; i++) {
        ids += cartArray[i].id + '|' + cartArray[i].display + '|' + cartArray[i].renewalPeriod + '|';
        productsCounter++;
    }

    var domains = "";
    $('#domainsDiv table tbody tr[id]').each(function() {
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

    if (canSubmit) {
        $("#submit_form").submit();
    }
};

var CheckEmail = function(email, params) {
    if (typeof (email) != 'undefined' && email != '') {

        var sentData = new Object();

        sentData.email = email;

        var response = $.ajax({
            async: false,
            type: 'POST',
            url: params.CheckEmailAction,
            data: sentData
        }).responseText;

        return response == 'true';
    } else {
        return false;
    }
};


var CheckEmailDomain = function(email, params) {
    if (typeof (email) != 'undefined' && email != '') {
        var response = $.ajax({
            async: false,
            type: 'POST',
            url: params.CheckEmailDomainAction,
            data: 'email=' + email
        }).responseText;

        return response == 'false';
    } else {
        return false;
    }
};

// Validation methods
var ValidateFax = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#Fax').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#FaxProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#FaxProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#FaxProcessed').val(phone + '|' + $('#CountryCode').val());
        }
        else {
            $('#FaxProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
};

var ValidateInvoiceFax = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#InvoiceFax').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#InvoiceFaxProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#InvoiceFaxProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#InvoiceFaxProcessed').val(phone + '|' + $('#InvoiceCountryCode').val());
        }
        else {
            $('#InvoiceFaxProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
}

var ValidateTelephoneEx = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#Telephone').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#TelephoneProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#TelephoneProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#TelephoneProcessed').val(phone + '|' + $('#CountryCode').val());
        }
        else {
            $('#TelephoneProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
};

var ValidateInvoiceTelephoneEx = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#InvoiceTelephone').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#InvoiceTelephoneProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#InvoiceTelephoneProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#InvoiceTelephoneProcessed').val(phone + '|' + $('#InvoiceCountryCode').val());
        }
        else {
            $('#InvoiceTelephoneProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
};

var ValidateMobileEx = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#Mobile').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#MobileProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#MobileProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#MobileProcessed').val(phone + '|' + $('#CountryCode').val());
        }
        else {
            $('#MobileProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
};

var ValidateInvoiceMobileEx = function(value, element, params) {
    var regex1 = /^\+[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex2 = /^[1-9][0-9-.,()\s\/]+[0-9)]$/;
    var regex3 = /^[0-9][0-9-.,()\s\/]+[0-9)]$/;
    var phone = TrimSpace($('#InvoiceMobile').val());

    var ok = false;

    if (regex1.test(phone)) {
        ok = true;
        $('#InvoiceMobileProcessed').val(phone.replace('+', ''));
    }
    if (regex2.test(phone)) {
        ok = true;
        $('#InvoiceMobileProcessed').val(phone);
    }
    if (regex3.test(phone)) {
        ok = true;
        if (phone[0] == '0' && phone[1] != '0') {
            $('#InvoiceMobileProcessed').val(phone + '|' + $('#InvoiceCountryCode').val());
        }
        else {
            $('#InvoiceMobileProcessed').val(phone);
        }
    }

    if (!ok) {
        return false;
    }

    return true;
};

var ValidateOrgNumberEx = function(value, element, params) {
    var regex = /^\d{10}$/;
    var org = TrimSpace($('#OrgNumber').val());

    var ok = false;

    org = org.replace('-', '');
    if (regex.test(org)) {
        ok = true;
    }
    if (!ok) {
        return false;
    }
    return ok;
};

var ValidateOrgNumberCheckSum = function(value, element, params) {
    var org = TrimSpace($('#OrgNumber').val());
    org = org.replace('-', '');
    org = org.substr(0, 6) + '-' + org.substr(6);
    //checksum
    if ($('#Company').val() == '') {
        return TestPrivateOrgNumber(org);
    }
    else {
        if (TestCompanyOrgNumber(org)) {
            return true;
        }
        else {
            return TestPrivateOrgNumber(org);
        }
    }
};

function TestPrivateOrgNumber(org) {
    if (!org.match(/^(\d{2})(\d{2})(\d{2})\-(\d{4})$/)) {
        return false;
    }

    var now = new Date();
    var nowFullYear = now.getFullYear() + "";
    var nowCentury = nowFullYear.substring(0, 2);
    var nowShortYear = nowFullYear.substring(2, 4);
    var year = RegExp.$1;
    var month = RegExp.$2;
    var day = RegExp.$3;
    var controldigits = RegExp.$4;
    var fullYear = (year * 1 <= nowShortYear * 1) ? (nowCentury + year) * 1 : ((nowCentury * 1 - 1) + year) * 1;
    var months = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
    if (fullYear % 400 == 0 || fullYear % 4 == 0 && fullYear % 100 != 0) {
        months[1] = 29;
    }

    if (month * 1 < 1 || month * 1 > 12 || day * 1 < 1 || day * 1 > months[month * 1 - 1]) {
        return false;
    }

    var alldigits = year + month + day + controldigits;
    var nn = "";
    for (var n = 0; n < alldigits.length; n++) {
        nn += ((((n + 1) % 2) + 1) * alldigits.substring(n, n + 1));
    }
    var checksum = 0;
    for (var n = 0; n < nn.length; n++) {
        checksum += nn.substring(n, n + 1) * 1;
    }
    return checksum % 10 == 0;
}

function TestCompanyOrgNumber(org) {
    if (!org.match(/^(\d{1})(\d{5})\-(\d{4})$/)) {
        return false;
    }

    var group = RegExp.$1;
    var controldigits = RegExp.$3;
    var alldigits = group + RegExp.$2 + controldigits;
    if (alldigits.substring(2, 3) < 2) {
        return false;
    }

    var nn = "";
    for (var n = 0; n < alldigits.length; n++) {
        nn += ((((n + 1) % 2) + 1) * alldigits.substring(n, n + 1));
    }
    var checksum = 0;
    for (var n = 0; n < nn.length; n++) {
        checksum += nn.substring(n, n + 1) * 1;
    }
    return checksum % 10 == 0;
}

var ValidateVATNumberEx = function(value, element, params) {
    if ($('#VATNumber').val() != '') {
        var EUCountries = params.EUCountries.split(' ');
        var check = false;
        for(var i =0; i< EUCountries.length; i++)
        {
            if($('#CountryCode').val() == EUCountries[i]) 
            {
                check = true;
            }   
        }
        if(check)
        {
            var vatNum = TrimSpace($('#VATNumber').val());
            if (checkVATNumber (vatNum)) {
                $('#VATNumber').val(checkVATNumber (vatNum));
                return true; 
            }  
            else {
                return false;
            }
        }
        else
        {
            return true;
        }
    } else return true;
};

var ValidatePostNumberEx = function(value, element, params) {
    var regex;

    if (typeof (params) != 'undefined' && typeof (params.DefaultCountryCode) != 'undefined' && $('#CountryCode').val() == params.DefaultCountryCode) {
        regex = /^\d{5}$/;
    }
    else {
        regex = /^\d+$/;
    }
    var org = TrimSpace($('#PostNumber').val());

    var ok = false;

    org = org.replace(' ', '');
    if (regex.test(org)) {
        ok = true;
    }
    if (!ok) {
        return false;
    }

    return true;
};

var setSubmitFormAdditionalThemeRules = function(params) {
    
};

bindSecondAddressCheckBoxClick = function () {
    $('#secondAddressTrue').unbind().bind('click', function () {
        AddRequiredValidationRulesForInvoiceFields();
    });
}

addBillingCustomerDataBlur = function(defaultString) {
    $("#ContactName,#ContactLastName,#Company,#InvoiceContactName,#InvoiceContactLastName,#InvoiceCompany").blur(function() {
        if( $("#secondAddressFalse:checked").length == 1)
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

addTechCustomerDataBlur = function(defaultString) {
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
