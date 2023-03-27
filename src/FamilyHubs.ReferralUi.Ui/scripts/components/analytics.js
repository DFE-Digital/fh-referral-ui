"use strict";
exports.__esModule = true;
exports.sendAnalyticsCustomEvent = exports.sendPageViewEvent = exports.updateAnalyticsStorageConsent = void 0;
var cookie_functions_1 = require("./cookie-functions");
var postcode_1 = require("./postcode");
function gtag(command) {
    var args = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        args[_i - 1] = arguments[_i];
    }
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push(arguments);
}
var GaMeasurementId = '';
function initAnalytics(gaMeasurementId) {
    // if the environment doesn't have a measurement id, don't load analytics
    if (!Boolean(gaMeasurementId)) {
        return;
    }
    GaMeasurementId = gaMeasurementId;
    setDefaultConsent();
    loadGaScript(gaMeasurementId);
    gtag('js', new Date());
    var pageViewParams = getPiiSafePageView(gaMeasurementId);
    // set the config for auto generated events other than page_view
    gtag('config', gaMeasurementId, {
        send_page_view: false,
        page_path: pageViewParams.page_path,
        page_location: pageViewParams.page_location,
        page_referrer: pageViewParams.referrer,
        cookie_flags: 'secure'
    });
    if (cookie_functions_1.areAnalyticsAccepted()) {
        updateAnalyticsStorageConsent(true);
    }
    sendPageViewEvent();
}
exports["default"] = initAnalytics;
function setDefaultConsent() {
    gtag('consent', 'default', {
        'analytics_storage': 'denied'
    });
    gtag('set', 'url_passthrough', true);
}
function updateAnalyticsStorageConsent(granted, delayMs) {
    var options = {
        'analytics_storage': granted ? 'granted' : 'denied'
    };
    if (typeof delayMs !== 'undefined') {
        options['wait_for_update'] = delayMs;
    }
    gtag('consent', 'update', options);
}
exports.updateAnalyticsStorageConsent = updateAnalyticsStorageConsent;
function sendPageViewEvent() {
    // send the page_view event manually (https://developers.google.com/analytics/devguides/collection/gtagjs/pages#default_behavior)
    gtag('event', 'page_view', getPiiSafePageView(GaMeasurementId));
}
exports.sendPageViewEvent = sendPageViewEvent;
function sendAnalyticsCustomEvent(accepted, source) {
    gtag('event', 'analytics', {
        'accepted': accepted,
        'source': source
    });
}
exports.sendAnalyticsCustomEvent = sendAnalyticsCustomEvent;
function loadGaScript(gaMeasurementId) {
    var f = document.getElementsByTagName('script')[0];
    var j = document.createElement('script');
    j.async = true;
    j.src = 'https://www.googletagmanager.com/gtag/js?id=' + gaMeasurementId;
    f.parentNode.insertBefore(j, f);
}
function getPiiSafePageView(gaMeasurementId) {
    var pageView = {
        page_title: document.title,
        send_to: gaMeasurementId,
        referrer: '',
        page_location: '',
        page_path: ''
    };
    //todo: set as referrer or page_referrer in pageView - does it matter? is it only picking it up from the config?
    //todo: get piisafe referrer function
    if (document.referrer !== '') {
        var referrerUrl = new URL(document.referrer);
        var piiSafeReferrerQueryString = getPiiSafeQueryString(referrerUrl.search);
        if (piiSafeReferrerQueryString == null) {
            pageView.referrer = document.referrer;
        }
        else {
            var urlArray_1 = document.referrer.split('?');
            pageView.referrer = urlArray_1[0] + piiSafeReferrerQueryString;
        }
    }
    var piiSafeQueryString = getPiiSafeQueryString(window.location.search);
    if (piiSafeQueryString == null) {
        pageView.page_location = window.location.href;
        pageView.page_path = window.location.pathname + window.location.search;
        return pageView;
    }
    var urlArray = window.location.href.split('?');
    pageView.page_location = urlArray[0] + piiSafeQueryString;
    pageView.page_path = window.location.pathname + piiSafeQueryString;
    return pageView;
}
function getPiiSafeQueryString(queryString) {
    //todo: for safety, convert to lowercase, so that if the user changes the case of the url, we still don't collect pii
    var queryParams = new URLSearchParams(queryString);
    var postcode = queryParams.get('postcode');
    if (postcode == null) {
        // null indicates original query params were already pii safe
        return null;
    }
    postcode = postcode_1.toOutcode(postcode);
    queryParams.set('postcode', postcode);
    queryParams["delete"]('latitude');
    queryParams["delete"]('longitude');
    return '?' + queryParams.toString();
}
