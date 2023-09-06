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
    initializeBackButtons();
});

//todo: needs to live in the fh packages

function initializeBackButtons() {

    // Check if the page wasn't opened in a new tab or a standalone window
    if (history.length > 1) {

        // Select all elements with the data-model attribute set to "fh-back-button"
        const backButtonElements = document.querySelectorAll('[data-model="fh-back-button"]');

        // Loop through each matching element and apply the logic
        backButtonElements.forEach((backButton: HTMLElement) => {
            backButton.style.display = "block";

            // Add an event listener to handle the back button click
            backButton.addEventListener("click", () => {
                // Go back to the previous page in the browser's history
                window.history.back();
            });
        });
    }
}