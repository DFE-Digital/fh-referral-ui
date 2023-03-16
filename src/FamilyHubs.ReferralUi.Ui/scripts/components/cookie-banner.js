"use strict";
exports.__esModule = true;
exports.cookiesBanner = void 0;
var cookie_functions_1 = require("./cookie-functions");
function cookiesBanner() {
    var $cookiesBanner = $('.govuk-cookie-banner');
    var $cookiesBannerMessage = $('.govuk-cookie-banner__message');
    var $cookiesBannerConfirmation = $('.govuk-cookie-banner__confirmation');
    var $cookiesBannerAccepted = $('.govuk-cookie-banner__confirmation-message__accepted');
    var $cookiesBannerRejected = $('.govuk-cookie-banner__confirmation-message__rejected');
    var date = new Date();
    date.setTime(date.getTime() + 24 * 60 * 60 * 1000 * 365);
    var cookiesAccept = function () {
        document.cookie = "service_directory_cookies_policy=accept; expires=date; path=/";
        $cookiesBannerMessage.addClass('govuk-hidden');
        $cookiesBannerConfirmation.removeClass('govuk-hidden');
        $cookiesBannerAccepted.removeClass('govuk-hidden');
        $('#cookies-page-accepted').prop('checked', true);
    };
    var cookiesReject = function () {
        deleteCookies();
        document.cookie = "service_directory_cookies_policy=reject; expires=date; path=/";
        $cookiesBannerMessage.addClass('govuk-hidden');
        $cookiesBannerConfirmation.removeClass('govuk-hidden');
        $cookiesBannerRejected.removeClass('govuk-hidden');
        $('#cookies-page-rejected').prop('checked', true);
    };
    var deleteCookies = function () {
        var cookies = document.cookie.split(";");
        for (var i = 0; i < cookies.length; i++) {
            var equals = cookies[i].indexOf("=");
            var name_1 = equals > -1 ? cookies[i].substr(0, equals) : cookies[i];
            document.cookie = name_1 + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
        }
    };
    var cookie_value = cookie_functions_1.getCookie('service_directory_cookies_policy');
    if (typeof cookie_value === 'undefined') {
        $cookiesBanner.removeClass('govuk-hidden');
        $('#cookies-banner-accept').click(function () {
            cookiesAccept();
        });
        $('#cookies-banner-reject').click(function () {
            cookiesReject();
        });
        $('#cookies-banner-close').click(function () {
            $cookiesBanner.addClass('govuk-hidden');
        });
    }
    else if (cookie_value === 'accept') {
        $('#cookies-page-accepted').prop('checked', true);
    }
    else if (cookie_value === 'reject') {
        $('#cookies-page-rejected').prop('checked', true);
    }
    $('#cookies-page-save').click(function () {
        if ($('#cookies-page-accepted').is(':checked')) {
            cookiesAccept();
            $('#cookies-saved-notification').removeClass('govuk-hidden');
            $('html, body').animate({ scrollTop: 0 }, 'fast');
        }
        if ($('#cookies-page-rejected').is(':checked')) {
            cookiesReject();
            $('#cookies-saved-notification').removeClass('govuk-hidden');
            $('html, body').animate({ scrollTop: 0 }, 'fast');
        }
    });
}
exports.cookiesBanner = cookiesBanner;
