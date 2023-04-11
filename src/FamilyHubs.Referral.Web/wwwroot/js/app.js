const CONSENT_COOKIE_NAME="referral_cookies_policy";function getCookie(e){let o=document.cookie.split(";");for(let t=0;t<o.length;t++){let n=o[t].split("=");if(e==n[0].trim())return decodeURIComponent(n[1])}}function areAnalyticsAccepted(){var e=getCookie(CONSENT_COOKIE_NAME);return console.log(e),"accept"==e}const INCODE_REGEX=/\d[a-z]{2}$/i,POSTCODE_REGEX=/^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$/i,SPACE_REGEX=/\s+/gi,sanitize=e=>e.replace(SPACE_REGEX,"").toUpperCase(),isValid=e=>null!==e.match(POSTCODE_REGEX),toOutcode=e=>{return isValid(e)?(o=e,o.replace(SPACE_REGEX,"").toUpperCase()).replace(INCODE_REGEX,""):null;var o};function gtag(e,...o){window.dataLayer=window.dataLayer||[],window.dataLayer.push(arguments)}let GaMeasurementId="";function initAnalytics(e){if(!Boolean(e))return;GaMeasurementId=e,setDefaultConsent(),loadGaScript(e),gtag("js",new Date);const o=getPiiSafePageView(e);gtag("config",e,{send_page_view:!1,page_path:o.page_path,page_location:o.page_location,page_referrer:o.referrer,cookie_flags:"secure"}),areAnalyticsAccepted()&&updateAnalyticsStorageConsent(!0),sendPageViewEvent()}function setDefaultConsent(){gtag("consent","default",{analytics_storage:"denied"}),gtag("set","url_passthrough",!0)}function updateAnalyticsStorageConsent(e,o){let t={analytics_storage:e?"granted":"denied"};void 0!==o&&(t.wait_for_update=o),gtag("consent","update",t)}function sendPageViewEvent(){gtag("event","page_view",getPiiSafePageView(GaMeasurementId))}function loadGaScript(e){const o=document.getElementsByTagName("script")[0],t=document.createElement("script");t.async=!0,t.src="https://www.googletagmanager.com/gtag/js?id="+e,o.parentNode.insertBefore(t,o)}function getPiiSafePageView(e){const o={page_title:document.title,send_to:e,referrer:"",page_location:"",page_path:""};if(""!==document.referrer){const e=getPiiSafeQueryString(new URL(document.referrer).search);if(null==e)o.referrer=document.referrer;else{const t=document.referrer.split("?");o.referrer=t[0]+e}}const t=getPiiSafeQueryString(window.location.search);if(null==t)return o.page_location=window.location.href,o.page_path=window.location.pathname+window.location.search,o;const n=window.location.href.split("?");return o.page_location=n[0]+t,o.page_path=window.location.pathname+t,o}function getPiiSafeQueryString(e){const o=new URLSearchParams(e);let t=o.get("postcode");return null==t?null:(t=toOutcode(t),o.set("postcode",t),o.delete("latitude"),o.delete("longitude"),"?"+o.toString())}function cookiesBanner(){let e=$(".govuk-cookie-banner"),o=$(".govuk-cookie-banner__message"),t=$(".govuk-cookie-banner__confirmation"),n=$(".govuk-cookie-banner__confirmation-message__accepted"),i=$(".govuk-cookie-banner__confirmation-message__rejected"),a=new Date;a.setTime(a.getTime()+31536e6);let c=function(){document.cookie="referral_cookies_policy=accept; expires=date; path=/",o.addClass("govuk-hidden"),t.removeClass("govuk-hidden"),n.removeClass("govuk-hidden"),$("#cookies-page-accepted").prop("checked",!0)},r=function(){s(),document.cookie="referral_cookies_policy=reject; expires=date; path=/",o.addClass("govuk-hidden"),t.removeClass("govuk-hidden"),i.removeClass("govuk-hidden"),$("#cookies-page-rejected").prop("checked",!0)},s=function(){let e=document.cookie.split(";");for(let o=0;o<e.length;o++){let t=e[o].indexOf("="),n=t>-1?e[o].substr(0,t):e[o];document.cookie=n+"=;expires=Thu, 01 Jan 1970 00:00:00 GMT"}},d=getCookie("referral_cookies_policy");void 0===d?(e.removeClass("govuk-hidden"),$("#cookies-banner-accept").click((function(){c()})),$("#cookies-banner-reject").click((function(){r()})),$("#cookies-banner-close").click((function(){e.addClass("govuk-hidden")}))):"accept"===d?$("#cookies-page-accepted").prop("checked",!0):"reject"===d&&$("#cookies-page-rejected").prop("checked",!0),$("#cookies-page-save").click((function(){$("#cookies-page-accepted").is(":checked")&&(c(),$("#cookies-saved-notification").removeClass("govuk-hidden"),$("html, body").animate({scrollTop:0},"fast")),$("#cookies-page-rejected").is(":checked")&&(r(),$("#cookies-saved-notification").removeClass("govuk-hidden"),$("html, body").animate({scrollTop:0},"fast"))}))}initAnalytics(window.GA_MEASUREMENT_ID),function(e){window.fhgov=new function(){this.init=function(){o(),cookiesBanner()};let o=function(){e(".js-show-hide-filters").click((function(o){o.preventDefault(),e(".filters-component").toggleClass("app-results-filter-overlay"),e("body").toggleClass("app-results-filter-overlay-active")}))}}}(jQuery),window.fhgov.init();
//# sourceMappingURL=app.js.map