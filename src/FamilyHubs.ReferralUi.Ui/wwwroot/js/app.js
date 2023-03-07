const INCODE_REGEX=/\d[a-z]{2}$/i,POSTCODE_REGEX=/^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$/i,SPACE_REGEX=/\s+/gi,sanitize=e=>e.replace(SPACE_REGEX,"").toUpperCase(),isValid=e=>null!==e.match(POSTCODE_REGEX),toOutcode=e=>{return isValid(e)?(o=e,o.replace(SPACE_REGEX,"").toUpperCase()).replace(INCODE_REGEX,""):null;var o};function gtag(e,...o){window.dataLayer=window.dataLayer||[],window.dataLayer.push(arguments)}let GaMeasurementId="";function initAnalytics(e){if(!Boolean(e))return;GaMeasurementId=e,setDefaultConsent(),loadGaScript(e),gtag("js",new Date);const o=getPiiSafePageView(e);gtag("config",e,{send_page_view:!1,page_path:o.page_path,page_location:o.page_location,page_referrer:o.referrer,cookie_flags:"secure"}),sendPageViewEvent()}function setDefaultConsent(){gtag("consent","default",{analytics_storage:"denied"}),gtag("set","url_passthrough",!0)}function sendPageViewEvent(){gtag("event","page_view",getPiiSafePageView(GaMeasurementId))}function loadGaScript(e){const o=document.getElementsByTagName("script")[0],t=document.createElement("script");t.async=!0,t.src="https://www.googletagmanager.com/gtag/js?id="+e,o.parentNode.insertBefore(t,o)}function getPiiSafePageView(e){const o={page_title:document.title,send_to:e,referrer:"",page_location:"",page_path:""};if(""!==document.referrer){const e=getPiiSafeQueryString(new URL(document.referrer).search);if(null==e)o.referrer=document.referrer;else{const t=document.referrer.split("?");o.referrer=t[0]+e}}const t=getPiiSafeQueryString(window.location.search);if(null==t)return o.page_location=window.location.href,o.page_path=window.location.pathname+window.location.search,o;const i=window.location.href.split("?");return o.page_location=i[0]+t,o.page_path=window.location.pathname+t,o}function getPiiSafeQueryString(e){const o=new URLSearchParams(e);let t=o.get("postcode");return null==t?null:(t=toOutcode(t),o.set("postcode",t),o.delete("latitude"),o.delete("longitude"),"?"+o.toString())}initAnalytics(window.GA_MEASUREMENT_ID),function(e){window.fhgov=new function(){this.init=function(){o(),t()};let o=function(){e(".js-show-hide-filters").click((function(o){o.preventDefault(),e(".filters-component").toggleClass("app-results-filter-overlay"),e("body").toggleClass("app-results-filter-overlay-active")}))},t=function(){let o=e(".govuk-cookie-banner"),t=e(".govuk-cookie-banner__message"),i=e(".govuk-cookie-banner__confirmation"),n=e(".govuk-cookie-banner__confirmation-message__accepted"),a=e(".govuk-cookie-banner__confirmation-message__rejected"),c=new Date;c.setTime(c.getTime()+31536e6);let r=function(){document.cookie="service_directory_cookies_policy=accept; expires=date; path=/",t.addClass("govuk-hidden"),i.removeClass("govuk-hidden"),n.removeClass("govuk-hidden"),e("#cookies-page-accepted").prop("checked",!0)},s=function(){d(),document.cookie="service_directory_cookies_policy=reject; expires=date; path=/",t.addClass("govuk-hidden"),i.removeClass("govuk-hidden"),a.removeClass("govuk-hidden"),e("#cookies-page-rejected").prop("checked",!0)},d=function(){let e=document.cookie.split(";");for(let o=0;o<e.length;o++){let t=e[o].indexOf("="),i=t>-1?e[o].substr(0,t):e[o];document.cookie=i+"=;expires=Thu, 01 Jan 1970 00:00:00 GMT"}},l=function(e){let o=document.cookie.split(";");for(let t=0;t<o.length;t++){let i=o[t].split("=");if(e==i[0].trim())return decodeURIComponent(i[1])}}("service_directory_cookies_policy");void 0===l?(o.removeClass("govuk-hidden"),e("#cookies-banner-accept").click((function(){r()})),e("#cookies-banner-reject").click((function(){s()})),e("#cookies-banner-close").click((function(){o.addClass("govuk-hidden")}))):"accept"===l?e("#cookies-page-accepted").prop("checked",!0):"reject"===l&&e("#cookies-page-rejected").prop("checked",!0),e("#cookies-page-save").click((function(){e("#cookies-page-accepted").is(":checked")&&(r(),e("#cookies-saved-notification").removeClass("govuk-hidden"),e("html, body").animate({scrollTop:0},"fast")),e("#cookies-page-rejected").is(":checked")&&(s(),e("#cookies-saved-notification").removeClass("govuk-hidden"),e("html, body").animate({scrollTop:0},"fast"))}))}}}(jQuery),window.fhgov.init();
//# sourceMappingURL=app.js.map
