// https://angular.io/guide/build#proxying-to-a-backend-server

const PROXY_CONFIG = {
  '/api/**': {
    target: "http://10.1.1.18:8080",//process.env.services__server__https__0 || process.env.services__server__http__0,
    changeOrigin: true,
    secure: false,
    logLevel: 'debug'
  },
};

module.exports = PROXY_CONFIG;
