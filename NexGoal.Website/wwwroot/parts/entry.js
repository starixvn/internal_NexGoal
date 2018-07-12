const path = require("path");
const { projectPath } = require("./constant");

const { clientInputPath, clientOutputPath } = projectPath;
const pagePath = path.join(clientInputPath, "pages");

module.exports = {
    client: {
        cssmain: "./sources/styles/main.scss",
        csserror: "./sources/styles/pages/error.scss",
        cssassignpermission: "./sources/styles/pages/rolemanagement.scss",
        cssusermanagement: "./sources/styles/pages/usermanagement.scss",
        cssnominationmanagement: "./sources/styles/pages/nominationmanagement.scss",
        cssdiscipline: "./sources/styles/pages/discipline/discipline.scss",
        //
        main: "./sources/js/main.js",
        home: "./sources/js/pages/home/index.jsx",
        assignpermission: './sources/js/pages/role/index.jsx',
        usermanagement: './sources/js/pages/usermanagement/index.jsx',
        discipline: './sources/js/pages/discipline/index.jsx',
        nominationmanagement: './sources/js/pages/nominationmanagement/index.jsx'
    }
};