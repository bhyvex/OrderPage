(function($) {
    var start;
    $.fn.extend({
        notification: function(options, title, message) {
            // Is it a command or initialization
            if (typeof (options) != "string") {
                options = $.extend({}, $.notification.defaults, {}, options);
                return this.each(function() {
                    new $.notification(this, options);
                });
            }
            else {
                if (options == 'notify') {
                    // Change title and message
                    $($(this).attr("titleSelector")).html(title);
                    $($(this).attr("messageSelector")).html(message.split("&lt;").join("<").split("&gt;").join(">"));

                    if ($(this).css('display') == 'none') {
                        var that = this;
                        $(this).show('drop', { direction: "up" }, that.showTimeout, function() {
                            if ($(that).attr('hideTimeout') != -1) {
                                start = setTimeout(function() {
                                    $(that).hide('drop', { direction: "up" }, $(that).attr('showTimeout'));
                                }, $(that).attr('hideTimeout'));
                            }
                        });
                    }
                }
            }
        }
    });

    $.notification = function(div, options) {
        $(options.closeSelector).click(function() {
            if ($(div).css('display') == 'block') {
                $(div).hide('drop', { direction: "up" }, options.showTimeout);
                clearTimeout(start);
            }
        });

        // Attach properties required for commands
        $(div).attr('showTimeout', options.showTimeout);
        $(div).attr('hideTimeout', options.hideTimeout);
        $(div).attr('titleSelector', options.titleSelector);
        $(div).attr('messageSelector', options.messageSelector);
    }

    $.notification.defaults = {
        titleSelector: "#notification-title",
        messageSelector: "#notification-message",
        closeSelector: "#notification-close",
        showTimeout: 1000,
        hideTimeout: 15000
    };
})(jQuery);