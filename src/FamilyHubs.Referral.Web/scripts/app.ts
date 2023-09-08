export { };

declare global {
    interface Window {
        fhgov: any;
    }
}

function fhgov() {
    this.init = function () {
        showHideFilters();
    };

    let showHideFilters = function () {
        const button = document.querySelector('.js-show-hide-filters');
        const filters = document.querySelector('.filters-component');
        const body = document.querySelector('body');
        if (button) {
            button.addEventListener('click',
                function(evt) {
                    evt.preventDefault();
                    filters.classList.toggle('app-results-filter-overlay');
                    body.classList.toggle('app-results-filter-overlay-active');
                });
        }
    }
}

window.fhgov = new fhgov();

document.addEventListener('DOMContentLoaded', function () {
    window.fhgov.init();
});
