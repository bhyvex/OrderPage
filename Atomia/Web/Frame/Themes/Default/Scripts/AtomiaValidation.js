/***********************************************************************************************
*                                                                                             *
* N.B. ALL CHANGES TO THIS FILE SHOULD BE DONE IN THE Atomia.Web.Plugin.Validation project.   *
* THEN COPY THE LATEST VERSION FROM THERE TO WHERE IT IS NEEDED.                              *
*                                                                                             *
***********************************************************************************************/

var AtomiaValidation = (function (jQuery) {
    var urls = {},
        $ = jQuery;


    // Client validation for model properties annotated with AtomiaRegularExpressionAttribute.
    function AtomiaRegularExpression(value, element, params) {
        var regex = new RegExp(params.pattern);
        var match = regex.exec(value);
        return match != null;
    }

    // Client validation for model properties annotated with AtomiaRequiredAttribute.
    function AtomiaRequired(value, element, params) {
        var formValidator = this; // jQuery Validation binds this to the form's validator object
        switch (element.nodeName.toLowerCase()) {
            case 'select':
                var options = $("option:selected", element);
                return options.length > 0 && (element.type == "select-multiple" || (!(options[0].attributes['value'].specified) ? options[0].text : options[0].value).length > 0);
            case 'input':
                if (formValidator.checkable(element))
                    return formValidator.getLength(value, element) > 0;
            default:
                return $.trim(value).length > 0;
        }
    }

    // Client validation for model properties annotated with AtomiaUsernameRequiredAttribute.
    function AtomiaUsernameRequired(value, element, params) {
        var formValidator = this;
        var skipValidation = false;

        // Skip validation if we don't have separated username and email.
        if (params !== undefined &&
            params.separateUsernameAndEmail !== undefined &&
            params.separateUsernameAndEmail === 'false') {
            skipValidation = true;
        }

        if (skipValidation) {
            return true;
        } else if (formValidator.checkable(element)) {
            return formValidator.getLength(value, element) > 0;
        } else {
            return $.trim(value).length > 0;
        }
    }

    // Client validation for model properties annotated with AtomiaStringLengthAttribute.
    function AtomiaStringLength(value, element, params) {
        var formValidator = this; // jQuery Validation binds this to the form's validator object
        return !AtomiaRequired.call(formValidator, value, element, params) || formValidator.getLength($.trim(value), element) <= params.maxLength;
    }

    // Client validation for model properties annotated with AtomiaRangeAttribute.
    function AtomiaRange(value, element, params) {
        var formValidator = this; // jQuery Validation binds this to the form's validator object
        return !AtomiaRequired.call(formValidator, value, element, params) || (value >= params.minimum && value <= params.maximum);
    }

    // Client validation for model properties annotated with the AtomiaLoginAttribute.
    function AtomiaLogin(value, element, params) {
        var remoteParams = {
            url: params.validateLoginUrl,
            type: 'post',
            data: {
                username: $('#' + params.usernameField).val(),
                password: $('#' + params.passwordField).val(),
                errorMessage: params.errorMessage
            }
        };

        if ($("#ResellerRootDomain").length > 0 && $("#ResellerRootDomain").val() != "") {
            remoteParams.data.resellerRootDomain = $("#ResellerRootDomain").val();
        }

        var formValidator = this;

        return jQuery.validator.methods.remote.call(formValidator, value, element, remoteParams);
    }

    /* Client validation for model properties annotated with AtomiaUsernameAttribute.
    * URL is either the default /Validation/CheckUsername, or set on the AtomiaUsername attribute for the property,
    * or it is possible to set it with a call to AtomiaValidation.extendUrls, e.g. during view rendering.
    */
    function AtomiaUsername(value, element, params) {
        var $oldUsername,
            formValidator = this, // jQuery Validation binds this to the form's validator object
            remoteParams,
            errorMessage = '',
            url = '',
            skipValidation = false;

        if (params !== undefined && params.oldUsernameField !== undefined) {
            $oldUsername = jQuery('#' + params.oldUsernameField).first();
            if ($oldUsername.length > 0 && $oldUsername.val() === value) {
                return true;
            }
        }

        // Skip validation for the Email field if we separate username and email
        if (params !== undefined &&
            params.type !== undefined &&
            params.type === 'Email' &&
            params.separateUsernameAndEmail !== undefined &&
            params.separateUsernameAndEmail === 'true') {
            skipValidation = true;
        }

        // Skip validation for the Username field if we don't separate username and email
        if (params !== undefined &&
            params.type !== undefined &&
            params.type === 'Username' &&
            params.separateUsernameAndEmail !== undefined &&
            params.separateUsernameAndEmail === 'false') {
            skipValidation = true;
        }

        if (params !== undefined && params.errorMessage !== undefined) {
            errorMessage = params.errorMessage
        }

        if (urls.checkUsername !== undefined) {
            url = urls.checkUsername;
        }
        else if (params !== undefined && params.checkUsernameUrl !== undefined) {
            url = params.checkUsernameUrl;
        }

        remoteParams = {
            url: url,
            type: "post",
            data: {
                username: value,
                errorMessage: errorMessage
            }
        };

        if ($("#ResellerRootDomain").length > 0 && $("#ResellerRootDomain").val() != "") {
            remoteParams.data.resellerRootDomain = $("#ResellerRootDomain").val();
        }

        if (skipValidation) {
            return true;
        } else {
            return jQuery.validator.methods.remote.call(formValidator, value, element, remoteParams);
        }
    }

    function AtomiaUsernameExists(value, element, params) {
        var formValidator = this, // jQuery Validation binds this to the form's validator object
            remoteParams,
            errorMessage = '',
            url = '';

        if (params !== undefined && params.errorMessage !== undefined) {
            errorMessage = params.errorMessage
        }

        if (urls.checkUsername !== undefined) {
            url = urls.checkUsername;
        }
        else if (params !== undefined && params.checkUsernameUrl !== undefined) {
            url = params.checkUsernameUrl;
        }

        remoteParams = {
            url: url,
            type: "post",
            data: {
                username: value,
                errorMessage: errorMessage
            }
        };

        if ($("#ResellerRootDomain").length > 0 && $("#ResellerRootDomain").val() != "") {
            remoteParams.data.resellerRootDomain = $("#ResellerRootDomain").val();
        }

        return jQuery.validator.methods.remote.call(formValidator, value, element, remoteParams);
    }

    // Client validation for model properties annotated with AtomiaPassword.
    function AtomiaPassword(value, element, params) {
        var $usernameField = $('#' + params.usernameField),
            username = $usernameField.length > 0 ? $usernameField.val() : null;

        if (username !== null && username !== '' && username === value) {
            return false;
        }

        if (params.placeholder === value) {
            return true;
        }

        return new RegExp(params.pattern).test(value);
    }

    // Client validation for model properties annotated with AtomiaRepeatPassword.
    function AtomiaRepeatPassword(value, element, params) {
        var $passwordField = $('#' + params.passwordField),
            password = $passwordField.length > 0 ? $passwordField.val() : null;

        if (password !== null && password !== '' && password !== value) {
            return false;
        }

        return true;
    }



    /*
    *      PUBLIC FUNCTIONS
    */

    // Extend urls cofiguration object.
    function extendUrls(urlsToAdd) {
        $.extend(urls, urlsToAdd);
    }

    /* Initialize the custom jQuery Validation methods that are need by view.
    * E.g. AtomiaValidation.init('AtomiaUsername', 'AtomiaRequired');
    */
    function init() {
        var i,
            validationFunction,
            validatorMethodName,
            argumentsLength = arguments.length;

        for (i = 0; i < argumentsLength; i += 1) {
            validatorMethodName = arguments[i];

            switch (validatorMethodName) {
                case "AtomiaRegularExpression":
                    validationFunction = AtomiaRegularExpression;
                    break;
                case "AtomiaRequired":
                    validationFunction = AtomiaRequired;
                    break;
                case "AtomiaStringLength":
                    validationFunction = AtomiaStringLength;
                    break;
                case "AtomiaRange":
                    validationFunction = AtomiaRange;
                    break;
                case "AtomiaUsername":
                    validationFunction = AtomiaUsername;
                    break;
                case "AtomiaUsernameExists":
                    validationFunction = AtomiaUsernameExists;
                    break;
                case "AtomiaPassword":
                    validationFunction = AtomiaPassword;
                    break;
                case "AtomiaRepeatPassword":
                    validationFunction = AtomiaRepeatPassword;
                    break;
                case "AtomiaLogin":
                    validationFunction = AtomiaLogin;
                    break;
                case "AtomiaUsernameRequired":
                    validationFunction = AtomiaUsernameRequired;
                    break;
                default:
                    validationFunction = null;
                    break;
            }

            if (validationFunction !== null && !jQuery.validator.methods.hasOwnProperty(validatorMethodName)) {
                jQuery.validator.addMethod(validatorMethodName, validationFunction);
            }
        }
    }

    /* Override jQuery Validation's default behavior to not validate required fields until they have seen some input or been submitted.
    * Otherwise an empty pre-filled required field (e.g. in edit form) does not get validated until the whole form is submitted.
    */
    function validateImmediately(formId) {
        var $form = $('#' + formId),
            $validator = $form.validate();

        $validator.settings.onfocusout = function (element) {
            if (!$validator.checkable(element)) {
                $validator.element(element);
            }
        };
    }

    function initValidationTrigger(fieldSelector, triggerSelector) {
        jQuery(triggerSelector).on('change', function () {
            var rules,
                $a = jQuery(fieldSelector);

            if ($a.val() !== '') {
                $a.valid();
            } else {
                /* Empty fields have validation triggered only if not required. For required fields that 
                * are empty it is assumed they will be filled in later. */
                rules = $a.rules();
                if (!rules.hasOwnProperty('required') && !rules.hasOwnProperty('AtomiaRequired')) {
                    $a.valid();
                }
            }
        });
    }

    // initializes handler for the event triggered when form validation fails
    function initOnValidationFailed(formId, funcOnValidationFailed) {
        var $form = $('#' + formId);
        $form.bind("invalid-form.validate", funcOnValidationFailed);
    }

    return {
        init: init,
        extendUrls: extendUrls,
        validateImmediately: validateImmediately,
        initValidationTrigger: initValidationTrigger,
        initOnValidationFailed: initOnValidationFailed
    };
})(jQuery);
