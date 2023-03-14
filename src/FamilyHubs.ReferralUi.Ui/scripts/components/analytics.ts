
//todo: consent mode debugging/check: https://developers.google.com/tag-platform/devguides/consent-debugging
import { areAnalyticsAccepted } from './cookie-functions'
import { toOutcode } from './postcode'

function gtag(command: string, ...args: any[]): void {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push(arguments);
}

let GaMeasurementId: string = '';

//todo: use prototype? (or class?)
// (having an object (prototype/class) will ensure that GaMeasurementId will have already been set)
export default function initAnalytics(gaMeasurementId: string) {

    // if the environment doesn't have a measurement id, don't load analytics
    if (!Boolean(gaMeasurementId)) {
        return;
    }

    GaMeasurementId = gaMeasurementId;

    setDefaultConsent();

    loadGaScript(gaMeasurementId);

    gtag('js', new Date());

    const pageViewParams = getPiiSafePageView(gaMeasurementId);

    // set the config for auto generated events other than page_view
    gtag('config', gaMeasurementId, {
        send_page_view: true, 
        page_path: pageViewParams.page_path,
        page_location: pageViewParams.page_location,
        page_referrer: pageViewParams.referrer,
        cookie_flags: 'secure'
    });

    if (areAnalyticsAccepted()) {
        updateAnalyticsStorageConsent(true);
    }

    sendPageViewEvent();
}

function setDefaultConsent() {
    gtag('consent', 'default', {
        'analytics_storage': 'denied'
    });

    gtag('set', 'url_passthrough', true);
}

export function updateAnalyticsStorageConsent(granted: boolean, delayMs?: number) {

    let options = {
        'analytics_storage': granted ? 'granted' : 'denied'
    };

    if (typeof delayMs !== 'undefined') {
        options['wait_for_update'] = delayMs;
    }

    gtag('consent', 'update', options);
}

export function sendPageViewEvent() {
    // send the page_view event manually (https://developers.google.com/analytics/devguides/collection/gtagjs/pages#default_behavior)
    gtag('event', 'page_view', getPiiSafePageView(GaMeasurementId));
}

export function sendAnalyticsCustomEvent(accepted: boolean, source: string) {

    gtag('event', 'analytics', {
        'accepted': accepted,
        'source': source
    });
}

function loadGaScript(gaMeasurementId: string) {
    const f = document.getElementsByTagName('script')[0];
    const j = document.createElement('script');
    j.async = true;
    j.src = 'https://www.googletagmanager.com/gtag/js?id=' + gaMeasurementId;
    f.parentNode.insertBefore(j, f);
}

function getPiiSafePageView(gaMeasurementId: string) {

    const pageView = {
        page_title: document.title,
        send_to: gaMeasurementId,
        referrer: '',
        page_location: '',
        page_path: ''
    };

    //todo: set as referrer or page_referrer in pageView - does it matter? is it only picking it up from the config?
    //todo: get piisafe referrer function
    if (document.referrer !== '') {
        const referrerUrl = new URL(document.referrer);
        const piiSafeReferrerQueryString = getPiiSafeQueryString(referrerUrl.search);
        if (piiSafeReferrerQueryString == null) {
            pageView.referrer = document.referrer;
        } else {
            const urlArray = document.referrer.split('?');

            pageView.referrer = urlArray[0] + piiSafeReferrerQueryString;
        }
    }

    const piiSafeQueryString = getPiiSafeQueryString(window.location.search);

    if (piiSafeQueryString == null) {
        pageView.page_location = window.location.href;
        pageView.page_path = window.location.pathname + window.location.search;

        return pageView;
    }

    const urlArray = window.location.href.split('?');

    pageView.page_location = urlArray[0] + piiSafeQueryString;
    pageView.page_path = window.location.pathname + piiSafeQueryString;

    return pageView;
}

function getPiiSafeQueryString(queryString: string): string | null {

    //todo: for safety, convert to lowercase, so that if the user changes the case of the url, we still don't collect pii
    const queryParams = new URLSearchParams(queryString);

    let postcode = queryParams.get('postcode');
    if (postcode == null) {
        // null indicates original query params were already pii safe
        return null;
    }

    postcode = toOutcode(postcode);
    queryParams.set('postcode', postcode);
    queryParams.delete('latitude');
    queryParams.delete('longitude');

    return '?' + queryParams.toString();
}
