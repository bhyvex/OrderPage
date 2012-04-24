var previousDomainSearchIDs = new Array();
var previousCartIDs = new Array();

function GenerateDomainSearchID(tld) {
    if (typeof (previousDomainSearchIDs[tld]) == 'undefined') {
        previousDomainSearchIDs[tld] = 0;
    }
    else {
        previousDomainSearchIDs[tld]++;
    }
}

function ResetDomainSearchIDGenerator() {
    previousDomainSearchIDs = new Array();
}

function GenerateDSButtonID(tld) {
    return ('ds_btn_tld_' + tld + previousDomainSearchIDs[tld]);
}

function GenerateDSDomainNameID(tld) {
    return ('ds_domainName_tld_' + tld + previousDomainSearchIDs[tld]);
}

function GenerateDSStatusID(tld) {
    return ('ds_status_tld_' + tld + previousDomainSearchIDs[tld]);
}

function GenerateDSPriceID(tld) {
    return ('ds_price_tld_' + tld + previousDomainSearchIDs[tld]);
}

function SetCartID(product) {
    var existingProductID = "";
    var result = "";
    $('#domainsDiv table tbody tr').each(function() {
        var tr = $(this);
        if (tr.find('.vtip').attr('title') == product) {
            existingProductID = tr.find('.vtip').parent().attr('id');
        }
    });
    if (existingProductID != '') {
        var splitedExistingID = existingProductID.split('_');
        result = splitedExistingID[splitedExistingID.length - 1];
    }
    else {
        $("input[name=RadioProducts]").each(
            function(i) {
                var productName = $(this).parent().find('input[type=hidden]').val(); 
                if (productName == product) {
                    result = $(this).attr('id');
                }
            }
        );
    }
    if (result == '') {
        result = product;
    }

    previousCartIDs[product] = result;
}

function GenerateCartButtonID(product) {
    return ('cart_btn_tld_' + previousCartIDs[product]);
}

function GenerateCartProductNameID(product) {
    return ('cart_productName_tld_' + previousCartIDs[product]);
}

function GenerateCartPeriodID(product) {
    return ('cart_period_tld_' + previousCartIDs[product]);
}

function GenerateCartPriceID(product) {
    return ('cart_price_tld_' + previousCartIDs[product]);
}


