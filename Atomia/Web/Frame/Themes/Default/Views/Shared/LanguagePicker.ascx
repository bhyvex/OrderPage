<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[Atomia.Web.Frame.Models.LanguagePickerFormData, Atomia.Web.Frame]]" %>
<%@ Import Namespace="Atomia.Web.Frame.Models" %>
<%@ Import Namespace="System.Globalization" %>
<%  if (!String.IsNullOrEmpty(Model.ReturnUrl) && Model.Languages != null && Model.Languages.Length > 1)
    {

        LanguageModel defaultLanguage = Model.Languages.FirstOrDefault(l => l.IsDefault) ?? Model.Languages[0];
        string inputDescription = Html.Resource(defaultLanguage.Name) + " - " + Html.Resource(defaultLanguage.Name + "Description");
%>
    <div id="languagePickerDiv" class="adv_combo_box lng_list">
        <span class="flg_<%= defaultLanguage.Code.ToLowerInvariant() %>"></span>
        <input type="text" readonly="readonly" value="<%= inputDescription %>" title="<%= inputDescription %>" />
        <ul style="display: none;">
            <% foreach (LanguageModel language in Model.Languages)
            {
            %>

                <li>
                    <span class="flg_<%= language.Code.ToLowerInvariant() %> large"></span>
                    <p class="title"><%= Html.Resource(language.Name)%></p>
                    <p class="description"><%= Html.Resource(language.Name + "Description")%></p>
                    <%= Html.Hidden("languageCode", language.IsDefault ? "Default" : language.Code )%>
                </li>
            <% } %>
        </ul>
    </div>
    <% Html.BeginForm("LanguagePicker", "Home", new { area = "root" }, FormMethod.Post, new { @id = "languages_form", autocomplete = "off" }); %>
        <%= Html.Hidden("languagePickerFormData.ReturnUrl", Model.ReturnUrl)%>
        <%= Html.Hidden("languagePickerFormData.SelectedLanguage")%>
    <% Html.EndForm();%>
    <script type="text/javascript">
        $(document).ready(function() {
//            $('#languagePickerDiv li').bind('click', function() {
//                var language = $(this).attr('rel');
//                if (language != 'Default') {
//                    $('#languagePickerFormData_SelectedLanguage').val(language);
//                    $("#languages_form").submit();
//                }
//            });

            $('.adv_combo_box').advCombo();
        });

        //!| ADVANCED DROPDOWN

        $.fn.advCombo = function () {
            var advComboInput = $(this).children('input');
            var advComboMenu = $(this).children('ul');
            var advComboMenuItem = $(this).find('li');
            var advComboMenuItemLink = $(this).find('li a');

            advComboMenu.hide();

            // open combobox
            advComboInput.click(function (e) {
                e.stopPropagation();
                var adv_menu = $(this).next();
                var position = $(this).position();
                if ($.browser.msie && parseInt($.browser.version, 10) <= 7) { absPosIE(advComboMenu, position); };

                if ($(adv_menu).is(':visible') && !$(this).hasClass('.disabled')) {
                    advComboMenu.hide();
                    advComboMenu.children('.hover').removeClass('hover');
                } else {
                    advComboMenu.hide();
                    advComboMenu.children('.hover').removeClass('hover');
                    $(adv_menu).show();
                    if (!adv_menu.children('li:first').hasClass('disabled')) {
                        adv_menu.children('li:first').addClass('hover');
                    };
                }

                $(document).click(function () {
                    $(adv_menu).hide();
                    adv_menu.children('.hover').removeClass('hover');
                });
            });

            // click on item
            advComboMenuItem.click(function (e) {
                e.stopPropagation();

                if (!$(this).hasClass('.disabled') && ($(this).children('label').length == 0)) {
                    if ($(this).children('.title').length > 0) {
                        $(this).parents('.adv_combo_box').children('input').val($(this).children('.title').text());
                        if ($(this).parents('.adv_combo_box').hasClass('lng_list')) {
                            selectedLang = $(this).children('.title').text() + ' - ' + $(this).children('.description').text();
                            $(this).parents('.adv_combo_box').children('input').val(selectedLang).attr('title', selectedLang);
                            $(this).parents('.adv_combo_box').children('span').attr('class', $(this).children('span').attr('class')).removeClass('large');
                        };
                        if ($(this).parents('.adv_combo_box').hasClass('cc_list')) {
                            $(this).parents('.adv_combo_box').children('input').val($(this).children('.title').text() + ' ' + $(this).children('.description').text());
                        };
                    } else {
                        $(this).parents('.adv_combo_box').children('input').val($(this).text());
                    };
                    $(this).parents('ul').hide();
                    $(this).parents('ul').children('.hover').removeClass('hover');
                    $(this).parents('ul').prev().focus();

                    // submit form
                    var language = $(this).find('input').attr('value');
                    if (language != 'Default') {
                        $('#languagePickerFormData_SelectedLanguage').val(language);
                        $("#languages_form").submit();
                    }
                };
            });

            advComboMenuItem.hover(function () {
                $(this).parent().children('.hover').removeClass('hover');
                if (!$(this).hasClass('disabled')) {
                    $(this).addClass('hover');
                };
            });

            advComboInput.keyup(function (kc) {
                var myMeny = $(this).next();

                switch (kc.keyCode + '') {
                    case '27':   /* escape */
                        if (myMeny.is(':visible')) {
                            myMeny.hide();
                            myMeny.children('.hover').removeClass('hover');
                        };
                        break;

                    case '13':   /* enter */
                        myMeny.children('.hover').click();
                        break;

                    case '40':   /* down arrow */
                        if (myMeny.is(':hidden')) {
                            myMeny.show();
                            myMeny.children('li:first').addClass('hover');
                        } else {
                            if (myMeny.children('.hover').next().length > 0) {
                                myMeny.children('.hover').next().addClass('hover_prep');
                                myMeny.children('.hover').removeClass('hover');
                                myMeny.children('.hover_prep').addClass('hover').removeClass('hover_prep');
                            };
                        };
                        break;

                    case '38':   /* up arrow */
                        if (myMeny.is(':hidden')) {
                            myMeny.show();
                            myMeny.children('li:last').addClass('hover');
                        } else {
                            if (myMeny.children('.hover').prev().length > 0) {
                                myMeny.children('.hover').prev().addClass('hover_prep');
                                myMeny.children('.hover').removeClass('hover');
                                myMeny.children('.hover_prep').addClass('hover').removeClass('hover_prep');
                            };
                        };
                        break;

                    default:   /* typing */
                        if ($(this).val().length > 2) {
                            if (myMeny.is(':hidden')) {
                                myMeny.show();
                                myMeny.children('li:first').addClass('hover');
                            };
                        };
                };
            });
        }
    </script>
<%
    }
%>