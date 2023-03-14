export { };

declare global {
    interface Window {
        fhgov: any;
        GA_MEASUREMENT_ID: string;
        GA_CONTAINER_ID: string;
        dataLayer: any[];
    }
}

import initAnalytics from './components/analytics';
import { cookiesBanner } from './components/cookie-banner';

initAnalytics(window.GA_MEASUREMENT_ID);

(function ($) {
	'use strict';
	function fhgov() {
		this.init = function () {
			showHideFilters();
			cookiesBanner();
		};

		let showHideFilters = function () {
			$('.js-show-hide-filters').click(function (evt) {
				evt.preventDefault();

				$('.filters-component').toggleClass('app-results-filter-overlay');
				$('body').toggleClass('app-results-filter-overlay-active');
			});
		}

	}

	window.fhgov = new fhgov();
}
)(jQuery);

//todo: move into $(document).ready()?
window.fhgov.init();