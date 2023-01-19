// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
	'use strict';
	function fhgov() {
		this.init = function () {
			showHideFilters();
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

fhgov.init();