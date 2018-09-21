const jsSourcePath = './sources/js/pages';
const jsDistPath = 'js';
const styleSourcePath = './sources/styles';
const styleDistPath = 'styles';

module.exports = {
    client: {
        [`${styleDistPath}/main`]: `${styleSourcePath}/main.scss`,
        [`${styleDistPath}/error`]: `${styleSourcePath}/pages/error.scss`,
        [`${styleDistPath}/assignpermission`]: `${styleSourcePath}/pages/rolemanagement.scss`,
        [`${styleDistPath}/usermanagement`]: `${styleSourcePath}/pages/usermanagement.scss`,
        [`${styleDistPath}/nominationmanagement`]: `${styleSourcePath}/pages/nomination/index.scss`,
        [`${styleDistPath}/editeoqy`]: `${styleSourcePath}/pages/nomination/edit.scss`,
        [`${styleDistPath}/discipline`]: `${styleSourcePath}/pages/discipline/discipline.scss`,
        [`${styleDistPath}/evaluationflow`]: `${styleSourcePath}/pages/flow/index.scss`,
        [`${styleDistPath}/editevaluationflow`]: `${styleSourcePath}/pages/flow/edit.scss`,
        [`${styleDistPath}/period`]: `${styleSourcePath}/pages/period/period.scss`,
        [`${styleDistPath}/editperiod`]: `${styleSourcePath}/pages/period/edit.scss`,
        [`${styleDistPath}/selfevaluation`]: `${styleSourcePath}/pages/SelfEvaluation/self-evaluation.scss`,
        [`${styleDistPath}/evaluation`]: `${styleSourcePath}/pages/evaluation.scss`,
        [`${styleDistPath}/create-employee-nomination`]: `${styleSourcePath}/pages/employeenomination/create.scss`,
        //
        [`${jsDistPath}/main`]: "./sources/js/main.js",
        [`${jsDistPath}/home`]: `${jsSourcePath}/home/index.jsx`,
        [`${jsDistPath}/assignpermission`]: `${jsSourcePath}/role/index.jsx`,
        [`${jsDistPath}/usermanagement`]: `${jsSourcePath}/usermanagement/index.jsx`,
        [`${jsDistPath}/discipline`]: `${jsSourcePath}/discipline/index.jsx`,
        [`${jsDistPath}/nominationmanagement`]: `${jsSourcePath}/nomination/index.jsx`,
        [`${jsDistPath}/editeoq`]: `${jsSourcePath}/nomination/eoq/edit.jsx`,
        [`${jsDistPath}/editeoy`]: `${jsSourcePath}/nomination/eoy/edit.jsx`,
        [`${jsDistPath}/evaluationflow`]: `${jsSourcePath}/flow/index.jsx`,
        [`${jsDistPath}/editevaluationflow`]: `${jsSourcePath}/flow/edit.jsx`,
        [`${jsDistPath}/period`]: `${jsSourcePath}/period/index.jsx`,
        [`${jsDistPath}/editperiod`]: `${jsSourcePath}/period/edit.jsx`,
        [`${jsDistPath}/selfevaluation`]: `${jsSourcePath}/selfevaluation/index.jsx`,
        [`${jsDistPath}/createperformancecheckin`]: `${jsSourcePath}/selfevaluation/pci/create.jsx`,
        [`${jsDistPath}/editperformancecheckin`]: `${jsSourcePath}/selfevaluation/pci/edit.jsx`,
        [`${jsDistPath}/createmembercheckin`]: `${jsSourcePath}/evaluation/index.jsx`,
        [`${jsDistPath}/editmembercheckin`]: `${jsSourcePath}/evaluation/edit.jsx`,
        [`${jsDistPath}/create-employee-nomination`]: `${jsSourcePath}/employeenomination/create.jsx`,
    }
};