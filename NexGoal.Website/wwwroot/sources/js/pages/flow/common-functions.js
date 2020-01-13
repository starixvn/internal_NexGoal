import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceProcess } from '../../constants';

export const getEmployeeById = (employeeId, callback) => {
    axios(resolveClientUrl(`Flow/GetEmployeeById?employeeId=${employeeId}`))
        .then(response => {
            callback(response.data);
        }).catch(axiosError);
};

export const handleEvaluatorChange = (vm, evaluatorId) => {
    getEmployeeById(evaluatorId, employee => {
        const evaluator = {
            ...vm.state.evaluator,
            ...{
                processId: PerformanceProcess.Evaluator,
                evaluatorId: employee.employeeId,
                nickName: employee.nickName,
                fullName: employee.fullName,
                code: employee.code,
                avatar: employee.avatar
            }
        };
        vm.setState({ evaluator });
    });
};

export const handleReviewerChange = (vm, reviewerId) => {
    getEmployeeById(reviewerId, employee => {
        const reviewer = {
            ...vm.state.reviewer,
            ...{
                processId: PerformanceProcess.Reviewer,
                evaluatorId: employee.employeeId,
                nickName: employee.nickName,
                fullName: employee.fullName,
                code: employee.code,
                avatar: employee.avatar
            }
        };
        vm.setState({ reviewer });
    });
};