const { defineConfig } = require('cypress')

module.exports = defineConfig({
  projectId: 'xxxxxx',
  e2e: {
    // We've imported your old cypress plugins here.
    // You may want to clean this up later by importing these.
    setupNodeEvents(on, config) {
      return require('./cypress/plugins/index.js')(on, config)
    },
    baseUrl: 'http://localhost',
    specPattern: 'cypress/integration/diary-sample/**/*.{js,jsx,ts,tsx}',
  },
})
