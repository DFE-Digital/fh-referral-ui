"use strict";
exports.__esModule = true;
var analytics_1 = require("./components/analytics");
var cookie_banner_1 = require("./components/cookie-banner");
analytics_1["default"](window.GA_MEASUREMENT_ID);
(function ($) {
    'use strict';
    function fhgov() {
        this.init = function () {
            showHideFilters();
            cookie_banner_1.cookiesBanner();
        };
        var showHideFilters = function () {
            $('.js-show-hide-filters').click(function (evt) {
                evt.preventDefault();
                $('.filters-component').toggleClass('app-results-filter-overlay');
                $('body').toggleClass('app-results-filter-overlay-active');
            });
        };
    }
    window.fhgov = new fhgov();
})(jQuery);
//todo: move into $(document).ready()?
window.fhgov.init();
