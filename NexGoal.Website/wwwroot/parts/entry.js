const path = require("path");
const { projectPath } = require("./constant");

const { clientInputPath, clientOutputPath } = projectPath;
const pagePath = path.join(clientInputPath, "pages");

module.exports = {
    client: {
        cssmain: "./sources/styles/main.scss",
        cssassign_permission: "./sources/styles/pages/role/index.scss",
        cssusermanagement: "./sources/styles/pages/usermanagement/usermanagement.scss",
        //
        main: "./sources/js/main.js",
        home: "./sources/js/home/index.jsx",
        assign_permission: './sources/js/role/index.js',
        usermanagement: './sources/js/usermanagement/index.js'
    }
};