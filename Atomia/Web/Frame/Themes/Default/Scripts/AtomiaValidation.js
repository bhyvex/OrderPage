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
                return options.length > 0 && (element.type == "select-multiple" || ($.browser.msie && !(options[0].attributes['value'].specified) ? options[0].text : options[0].value).length > 0);
            case 'input':
                if (formValidator.checkable(element))
                    return formValidator.getLength(value, element) > 0;
            default:
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

    /* Client validation for model properties annotated with AtomiaUsernameAttribute.
    * URL is either the default /Validation/CheckUsername, or set on the AtomiaUsername attribute for the property,
    * or it is possible to set it with a call to AtomiaValidation.extendUrls, e.g. during view rendering.
    */
    function AtomiaUsername(value, element, params) {
        var $oldUsername,
            formValidator = this, // jQuery Validation binds this to the form's validator object
            remoteParams,
            errorMessage = '',
            url = '';

        if (params !== undefined && params.oldUsernameField !== undefined) {
            $oldUsername = jQuery('#' + params.oldUsernameField).first();
            if ($oldUsername.length > 0 && $oldUsername.val() === value) {
                return true;
            }
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

        return jQuery.validator.methods.remote.call(formValidator, value, element, remoteParams);
    }

    // Client validation for model properties annotated with AtomiaPassword.
    function AtomiaPassword(value, element, params) {
        var $usernameField = $('#' + params.usernameField),
            username = $usernameField.length > 0 ? $usernameField.val() : null,
            placeholder = params.placeholder || null;

        if (username !== null && username !== '' && username === value) {
            return false;
        }

        if (placeholder != null && placeholder === value) {
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
                case "AtomiaPassword":
                    validationFunction = AtomiaPassword;
                    break;
                case "AtomiaRepeatPassword":
                    validationFunction = AtomiaRepeatPassword;
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

    return {
        init: init,
        extendUrls: extendUrls,
        validateImmediately: validateImmediately,
        initValidationTrigger: initValidationTrigger
    };
})(jQuery);
