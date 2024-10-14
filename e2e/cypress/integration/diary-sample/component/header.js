export default class HeaderTest {
  static test() {
    cy.get(".navbar-brand")
      .should("have.text", "日記")
      .should("not.have.attr", "href");

      cy.get(".nav-item")
        .eq(0)
        .children()
        .should("have.text", "Home")
        .should("have.attr", "href", "/Menu");

      cy.get(".nav-item")
        .eq(1)
        .children()
        .should("have.text", "プロフィール")
        .should("have.attr", "href", "#");

      cy.get(".nav-item")
        .eq(2)
        .children()
        .should("have.text", "設定")
        .should("have.attr", "href", "#");
  }

  static click(index) {
    cy.get(".nav-item").eq(index).children().click();
  }

  static clickTest(url) {
    context("Home", () => {
      it("メニューに遷移する", () => {
        HeaderTest.click(0);
        cy.url().should("eq", Cypress.config().baseUrl + "/Menu");
      });
    });
  
    context("プロフィール", () => {
      it("遷移なし", () => {
        HeaderTest.click(1);
        cy.url().should("eq", Cypress.config().baseUrl + url + "#");
      });
    });
  
    context("設定", () => {
      it("遷移なし", () => {
        HeaderTest.click(2);
        cy.url().should("eq", Cypress.config().baseUrl + url + "#");
      });
    });
  }
}
