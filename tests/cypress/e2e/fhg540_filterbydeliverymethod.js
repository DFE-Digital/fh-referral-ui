import { Given, Then, When } from "@badeball/cypress-cucumber-preprocessor";

Given("a user has arrived on search page", () => {
    cy.visit(`ProfessionalReferral/Search`);
});
When("user searches by a valid postcode {string}", (postcode) => {
    cy.searchbypostcode(postcode);
});
Then("the user should see delivery methods filters in-person, online and telephone", () => {
    cy.get('[data-testid="checkbox-inperson"]').should("exist");
    cy.get('[data-testid="checkbox-online"]').should("exist");
    cy.get('[data-testid="checkbox-telephone"]').should("exist");
})


When("user searches using postcode {string} and filters using delivery method 'Online'", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="checkbox-online"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the service 'aidforchildrenwithtracheostomies'", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
});

When("user searches using postcode {string} and filters using delivery method 'In person'", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="checkbox-inperson"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the service 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
});

When("user searches using postcode {string} and filters using delivery method 'Telephone'", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="checkbox-telephone"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the service 'testservice-paid-0to13yrs'", () => {
    cy.get('[data-testid="testservice-paid-0to13yrs"]').should("exist");
});

When("user searches using postcode {string} and filters using delivery methods 'In person' and 'Online'", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="checkbox-inperson"]').click();
    cy.get('[data-testid="checkbox-online"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the services 'aidforchildrenwithtracheostomies' and 'testservice-free-10to15yrs'", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
});

When("user searches using postcode {string} and filters using all delivery methods", (postcode) => {
    cy.visit(`ProfessionalReferral/Search`);
    cy.searchbypostcode(postcode);
    cy.get('[data-testid="checkbox-inperson"]').click();
    cy.get('[data-testid="checkbox-online"]').click();
    cy.get('[data-testid="checkbox-telephone"]').click();
    cy.get('[data-testid="button-apply-filters"]').click();
})
Then("the user should see the services 'aidforchildrenwithtracheostomies' , 'testservice-free-10to15yrs' and 'testservice-paid-0to13yrs'", () => {
    cy.get('[data-testid="aidforchildrenwithtracheostomies"]').should("exist");
    cy.get('[data-testid="testservice-free-10to15yrs"]').should("exist");
    cy.get('[data-testid="testservice-paid-0to13yrs"]').should("exist");
});