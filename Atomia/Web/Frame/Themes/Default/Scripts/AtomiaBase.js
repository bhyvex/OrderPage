$.postJSON = function(url, data, callback) {
    $.post(url, data, callback, "json");
}; 

function AtomiaTriggerShortening(img_url, numberOfChars) {
    $('.labelShorten').each(function() {
        var maxLength = (typeof (numberOfChars) == 'undefined' || numberOfChars == '') ? 40 : parseInt(numberOfChars);
        var stringToShorten = $(this).text();
        var isTooLong = false;
        if (stringToShorten.length > maxLength) {
            isTooLong = true;
            stringToShorten = stringToShorten.substring(0, maxLength) + "...";
        }
        var title = isTooLong ? $(this).html() : "";
        var shorten = isTooLong ? "<div class='vtip' style='white-space: nowrap' title='" + title + "'>" + stringToShorten + "</div>" : stringToShorten;

        $(this).html(shorten);
    });
    vtip(img_url);
}

function AtomiaCheckable(element) {
    return /radio|checkbox/i.test(element.type);
}

function AtomiaFindByName(name) {
    // select by name and filter by form for performance over form.find("[name=...]")
    return $(document.getElementsByName(name)).map(function(index, element) {
        return element.name == name && element || null;
    });
}

function AtomiaGetLength(value, element) {
    switch (element.nodeName.toLowerCase()) {
        case 'select':
            return $("option:selected", element).length;
        case 'input':
            if (AtomiaCheckable(element))
                return AtomiaFindByName(element.name).filter(':checked').length;
    }
    return value.length;
}

function AtomiaRegularExpression(value, element, params) {
    return !AtomiaRequired(value, element, params) || AtomiaGetReqularExpresionValidationResult(value, element, params);
}

function AtomiaGetReqularExpresionValidationResult(value, element, params) {
    if (typeof (params) != 'undefined' && typeof (params.pattern) != 'undefined') {
        var regex = new RegExp(params.pattern);
        var match = regex.exec(value);
        return match != null;
    } else {
        return false;
    }
}

function AtomiaRequired(value, element, params) {
    switch (element.nodeName.toLowerCase()) {
        case 'select':
            var options = $("option:selected", element);
            return options.length > 0 && (element.type == "select-multiple" || ($.browser.msie && !(options[0].attributes['value'].specified) ? options[0].text : options[0].value).length > 0);
        case 'input':
            if (AtomiaCheckable(element))
                return AtomiaGetLength(value, element) > 0;
        default:
            return $.trim(value).length > 0;
    }
}

function AtomiaStringLength(value, element, params) {
    return !AtomiaRequired(value, element, params) || AtomiaGetLength($.trim(value), element) <= params.maxLength;
}

function AtomiaGetQueryString(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}

function TrimSpace(passedString) {
    if (typeof (passedString) != 'undefined' && passedString != '') {
        while (passedString[0] == ' ') {
            passedString = passedString.toString().substring(1);
        }
        while (passedString[passedString.length - 1] == ' ') {
            passedString = passedString.toString().substring(0, passedString.length - 1);
        }
        return passedString;
    } else {
        return '';
    }
}

jQuery.validator.addMethod(
    "AtomiaRegularExpression", function (value, element, params) {
        return AtomiaRegularExpression(value, element, params);
    }
);

jQuery.validator.addMethod(
        "AtomiaRequired", function (value, element, params) {
            return AtomiaRequired(value, element, params);
        }
);

jQuery.validator.addMethod(
    "AtomiaStringLength", function (value, element, params) {
        return AtomiaStringLength(value, element, params);
    }
);
