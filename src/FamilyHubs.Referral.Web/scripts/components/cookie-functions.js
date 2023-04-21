"use strict";
exports.__esModule = true;
exports.areAnalyticsAccepted = exports.getCookie = void 0;
/* Name of the cookie to save users cookie preferences to. */
var CONSENT_COOKIE_NAME = 'referral_cookies_policy';
function getCookie(name) {
    var cookieArr = document.cookie.split(";");
    for (var i = 0; i < cookieArr.length; i++) {
        var cookiePair = cookieArr[i].split("=");
        if (name == cookiePair[0].trim()) {
            return decodeURIComponent(cookiePair[1]);
        }
    }
}
exports.getCookie = getCookie;
function areAnalyticsAccepted() {
    var consentCookie = getCookie(CONSENT_COOKIE_NAME);
    console.log(consentCookie);
    if (consentCookie == 'accept') {
        return true;
    }
    return false;
}
exports.areAnalyticsAccepted = areAnalyticsAccepted;
