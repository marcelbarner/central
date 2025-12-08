// https://angular.io/guide/build#proxying-to-a-backend-server

const PROXY_CONFIG = {
  '/api/**': {
    target: process.env.services__server__https__0 || process.env.services__server__http__0,
    changeOrigin: true,
    secure: false,
    logLevel: 'debug'
  },
};

module.exports = PROXY_CONFIG;
