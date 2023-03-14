/* Name of the cookie to save users cookie preferences to. */
const CONSENT_COOKIE_NAME = 'service_directory_cookies_policy';

export function getCookie(name: string) {
	let cookieArr = document.cookie.split(";");
	for (let i = 0; i < cookieArr.length; i++) {
		let cookiePair = cookieArr[i].split("=");
		if (name == cookiePair[0].trim()) {
			return decodeURIComponent(cookiePair[1]);
		}
	}
}

export function areAnalyticsAccepted(): boolean {
    var consentCookie = getCookie(CONSENT_COOKIE_NAME);
	console.log(consentCookie);
    if (consentCookie == 'accept') {
        return true;
    } 

    return false;
}