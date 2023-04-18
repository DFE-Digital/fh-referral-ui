﻿import { getCookie } from './cookie-functions';

export function cookiesBanner() {
	let $cookiesBanner = $('.govuk-cookie-banner');
	let $cookiesBannerMessage = $('.govuk-cookie-banner__message');
	let $cookiesBannerConfirmation = $('.govuk-cookie-banner__confirmation');
	let $cookiesBannerAccepted = $('.govuk-cookie-banner__confirmation-message__accepted');
	let $cookiesBannerRejected = $('.govuk-cookie-banner__confirmation-message__rejected');
	let date = new Date();

	date.setTime(date.getTime() + 24 * 60 * 60 * 1000 * 365);

	let cookiesAccept = function () {
		document.cookie = `referral_cookies_policy=accept; expires=date; path=/`;

		$cookiesBannerMessage.addClass('govuk-hidden');
		$cookiesBannerConfirmation.removeClass('govuk-hidden');
		$cookiesBannerAccepted.removeClass('govuk-hidden');
		$('#cookies-page-accepted').prop('checked', true);
	}

	let cookiesReject = function () {
		deleteCookies();
		document.cookie = `referral_cookies_policy=reject; expires=date; path=/`;

		$cookiesBannerMessage.addClass('govuk-hidden');
		$cookiesBannerConfirmation.removeClass('govuk-hidden');
		$cookiesBannerRejected.removeClass('govuk-hidden');
		$('#cookies-page-rejected').prop('checked', true);
	}

	let deleteCookies = function () {
		let cookies = document.cookie.split(";");
		for (let i = 0; i < cookies.length; i++) {
			let equals = cookies[i].indexOf("=");
			let name = equals > -1 ? cookies[i].substr(0, equals) : cookies[i];
			document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
		}
	}

	let cookie_value = getCookie('referral_cookies_policy');

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
	} else if (cookie_value === 'accept') {
		$('#cookies-page-accepted').prop('checked', true);
	} else if (cookie_value === 'reject') {
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