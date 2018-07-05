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
        home: "./sources/js/pages/home/index.jsx",
        assign_permission: './sources/js/pages/role/index.jsx'
        usermanagement: './sources/js/usermanagement/index.js'
    }
};