var bindSelectClick = function (radioBtnCnt, protectedContainers, inputFieldContainers, pContainerToShow) {
    if ($("#" + radioBtnCnt).length > 0) {
        $("#" + radioBtnCnt).click(function () {

            for (var i = 0; i < protectedContainers.length; i++) {
                if (i == pContainerToShow) {
                    $("#" + protectedContainers[i]).show();
                } else {
                    $("#" + protectedContainers[i]).hide();
                }
            }

            for (var i = 0; i < inputFieldContainers.length; i++) {
                $("#" + inputFieldContainers[i]).val('');
            }
        });
    }
};

var bindOrderbuttonClick = function() {
    $('#orderbutton').bind('click', function() {
        $("#submit_form").submit();
    });
};

var setNotificationMessage = function(notificationParams) {
    // Regular error
    var message = "";
    if (notificationParams.wasAnError == 1) {
        message = notificationParams.NotificationText;
        $('#notification').notification('notify', notificationParams.title, message);
    }
    // allowed number of domains searched exceeded
    else if (notificationParams.wasAnError == 2) {
        message = notificationParams.ValidationErrorsErrorNumDomains;
        $('#notification').notification('notify', notificationParams.title, message);
    }
    // domains contain characters that are not allowed or number of characters exeeds allowed
    else if (notificationParams.wasAnError == 3) {
        message = notificationParams.NotificationTextInvalidDomain;
        $('#notification').notification('notify', notificationParams.title, message);
    }
};

// Validation method

// generic method for server side validation
var ValidateValueServerSide = function (values, element, url, httpMethod) {
    var returnData = false;
    
    $.ajax({
        async: false,
        type: httpMethod,
        url: url,
        data: (values),
        success: function (responseData) {
            returnData = responseData;
        },

        error: function () {
            returnData = false;
        },

        dataType: 'json'
    });

    return returnData;
}

var ValidateDomainsLength = function(value, element, params) {
    if ($('#first').is(':checked')) {
        var data = $('#Domains').val();
        var pom = data.split("\n");
        var i = 0;

        for (i = 0; i < pom.length; i++) {
            if (pom[i] == '') {
                continue;
            }
            if (TrimSpace(pom[i]).length > parseInt(params.AllowedDomainLength, 10)) {
                return false;
            }
        }
    }
    return true;
};

var ValidateGroupOfDomains = function(value, element, params) {
    if ($('#first').is(':checked')) {
        var data = $('#Domains').val();
        var pom = data.split("\n");
        var i = 0;
        var re = new RegExp(params.RegDomainFront);
        var re2 = new RegExp(params.RegDomain);

        for (i = 0; i < pom.length; i++) {
            if (pom[i] == '') {
                continue;
            }
            var tmpStr = TrimSpace(pom[i]);
            if (!re.test(tmpStr) && !re2.test(tmpStr)) {
                return false;
            }
        }
    }
    return true;
};

var ValidateNumOfDomains = function(value, element, params) {
    if ($('#first').is(':checked')) {
        var data = $('#Domains').val();
        var pom = data.split("\n");
        var i = 0;
        var domCounter = 0;

        for (i = 0; i < pom.length; i++) {
            if (pom[i] === '') {
                continue;
            }
            if (++domCounter > parseInt(params.NumberOfDomainsAllowed, 10)) {
                return false;
            }
        }
    }
    return true;
};

var setIndexFormDomainsRules = function(requiredMessage) {
    $("#Domains").rules("add", {
        required: function(element) {
            return $('#first').is(':checked');
        },
        messages: {
            required: requiredMessage
        }
    });
};

var setIndexFormDomainRules = function(message) {
    $("#Domain").rules("add", {
        required: function(element) {
            return $('#second').is(':checked');
        },
        messages: {
            required: message
        }
    });
};