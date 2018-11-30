const jsSourcePath = './sources/js/pages';
const jsDistPath = 'js';
const styleSourcePath = './sources/styles';
const styleDistPath = 'styles';

module.exports = {
    client: {
        [`${styleDistPath}/main`]: `${styleSourcePath}/main.scss`,
        [`${styleDistPath}/error`]: `${styleSourcePath}/pages/error.scss`,
        [`${styleDistPath}/home`]: `${styleSourcePath}/pages/home/home.scss`,
        [`${styleDistPath}/assignpermission`]: `${styleSourcePath}/pages/rolemanagement.scss`,
        [`${styleDistPath}/employeemanagement`]: `${styleSourcePath}/pages/employeemanagement.scss`,
        [`${styleDistPath}/nominationmanagement`]: `${styleSourcePath}/pages/nomination/index.scss`,
        [`${styleDistPath}/editeoqy`]: `${styleSourcePath}/pages/nomination/edit.scss`,
        [`${styleDistPath}/discipline`]: `${styleSourcePath}/pages/discipline/discipline.scss`,
        [`${styleDistPath}/evaluationflow`]: `${styleSourcePath}/pages/flow/index.scss`,
        [`${styleDistPath}/editevaluationflow`]: `${styleSourcePath}/pages/flow/edit.scss`,
        [`${styleDistPath}/period`]: `${styleSourcePath}/pages/period/period.scss`,
        [`${styleDistPath}/editperiod`]: `${styleSourcePath}/pages/period/edit.scss`,
        [`${styleDistPath}/evaluation`]: `${styleSourcePath}/pages/evaluation.scss`,
        [`${styleDistPath}/member-index`]: `${styleSourcePath}/pages/member/index.scss`,
        [`${styleDistPath}/member-details`]: `${styleSourcePath}/pages/member/details.scss`,
        [`${styleDistPath}/pafocus`]: `${styleSourcePath}/pages/pafocus/index.scss`,
        [`${styleDistPath}/nomination-assessment`]: `${styleSourcePath}/pages/nominationassessment/index.scss`,
        //
        [`${jsDistPath}/main`]: "./sources/js/main.js",
        [`${jsDistPath}/search-member`]: "./sources/js/SearchMember.jsx",
        [`${jsDistPath}/home`]: `${jsSourcePath}/home/index.jsx`,
        [`${jsDistPath}/assignpermission`]: `${jsSourcePath}/role/index.jsx`,
        [`${jsDistPath}/employeemanagement`]: `${jsSourcePath}/employeemanagement/index.jsx`,
        [`${jsDistPath}/discipline`]: `${jsSourcePath}/discipline/index.jsx`,
        [`${jsDistPath}/nominationmanagement`]: `${jsSourcePath}/nomination/index.jsx`,
        [`${jsDistPath}/editeoq`]: `${jsSourcePath}/nomination/eoq/edit.jsx`,
        [`${jsDistPath}/editeoy`]: `${jsSourcePath}/nomination/eoy/edit.jsx`,
        [`${jsDistPath}/evaluationflow`]: `${jsSourcePath}/flow/index.jsx`,
        [`${jsDistPath}/editevaluationflow`]: `${jsSourcePath}/flow/edit.jsx`,
        [`${jsDistPath}/period`]: `${jsSourcePath}/period/index.jsx`,
        [`${jsDistPath}/editperiod`]: `${jsSourcePath}/period/edit.jsx`,
        [`${jsDistPath}/member-index`]: `${jsSourcePath}/member/index.jsx`,
        [`${jsDistPath}/member-details`]: `${jsSourcePath}/member/details.jsx`,
        [`${jsDistPath}/pafocus`]: `${jsSourcePath}/pafocus/index.jsx`,
        [`${jsDistPath}/nomination-assessment`]: `${jsSourcePath}/nominationassessment/index.jsx`
    }
};