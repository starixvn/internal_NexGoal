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
        cssnominationmanagement: "./sources/styles/pages/nomination/index.scss",
        cssediteoqy: "./sources/styles/pages/nomination/edit.scss",
        cssdiscipline: "./sources/styles/pages/discipline/discipline.scss",
        cssevaluationflow: "./sources/styles/pages/flow/index.scss",
        csseditevaluationflow: "./sources/styles/pages/flow/edit.scss",
        cssperiod: "./sources/styles/pages/period/period.scss",
        csseditperiod: "./sources/styles/pages/period/edit.scss",
        cssselfevaluation: "./sources/styles/pages/selfevaluation/view.scss"
        //
        main: "./sources/js/main.js",
        home: "./sources/js/pages/home/index.jsx",
        assignpermission: './sources/js/pages/role/index.jsx',
        usermanagement: './sources/js/pages/usermanagement/index.jsx',
        discipline: './sources/js/pages/discipline/index.jsx',
        nominationmanagement: './sources/js/pages/nomination/index.jsx',
        editeoq: './sources/js/pages/nomination/eoq/edit.jsx',
        editeoy: './sources/js/pages/nomination/eoy/edit.jsx',
        evaluationflow: './sources/js/pages/flow/index.jsx',
        editevaluationflow: './sources/js/pages/flow/edit.jsx',
        period: './sources/js/pages/period/index.jsx',
        editperiod: './sources/js/pages/period/edit.jsx'
    }
};