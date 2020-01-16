import { camelCase } from 'lodash';
import getResource from '../../resourceHelper';

export function validateCreateEditSelfPerformance(vm) {
    const errors = {},
        comment = vm.state.performanceRecord.comment;
    let isValid = true;

    if (!comment || comment.trim() === '') {
        errors.comment = getResource('CommentRequired');
        isValid = false;
    }

    vm.setState({ errors });

    return isValid;
}

export function validateCreateEditMemberPerformance(vm) {
    const errors = {},
        record = vm.state.performanceRecord;
    let isValid = true;

    if (record.employees.length === 0 && vm.props.isMultiple) {
        errors.employee = getResource('EmployeeRequired');
        isValid = false;
    }

    if (!record.comment || record.comment.trim() === '') {
        errors.comment = getResource('CommentRequired');
        isValid = false;
    }

    vm.setState({ errors });

    return isValid;
}

export function handleClickEditPerformanceRecord(vm, performanceRecordId) {
    vm.setState({
        editPerformanceRecord: {
            isShowModal: true,
            performanceRecordId: performanceRecordId,
            isDraft: false
        }
    });
}

export function handleCancelEditPerformanceRecord(vm) {
    vm.setState({
        editPerformanceRecord: {
            isShowModal: false,
            performanceRecordId: 0
        }
    });
}

export function handleClickEditDraftPerformanceRecord(vm, performanceRecordId) {
    vm.setState({
        editPerformanceRecord: {
            isShowModal: true,
            performanceRecordId: performanceRecordId,
            isDraft: true
        }
    });
}

export function handleClickDeletePerformanceRecord(vm, performanceRecordId) {
    vm.setState({
        deletePerformanceRecord: {
            isShowModal: true,
            performanceRecordId: performanceRecordId
        }
    });
}

export function handleCancelDeletePerformanceRecord(vm) {
    vm.setState({
        deletePerformanceRecord: {
            isShowModal: false,
            performanceRecordId: 0
        }
    });
}

export const axiosCreatePerformanceResponse = (vm, toastObj, axiosResponse, successCallback) => {
    const data = axiosResponse.data;
    if (data.isSuccessful) {
        toastObj.success(getResource('CreatedSuccessfully'));

        if (successCallback && typeof successCallback === 'function') {
            successCallback();
        }
    } else {
        if (data.errors) {
            const errors = {};
            data.errors.map(error => {
                errors[camelCase(error.key)] = error.errorMessage;
            });

            vm.setState({ errors });
        } else {
            toastObj.error(data.message);
        }
    }
};

export const axiosEditPerformanceResponse = (vm, toastObj, axiosResponse, successCallback) => {
    const data = axiosResponse.data;
    if (data.isSuccessful) {
        toastObj.success(getResource('UpdatedSuccessfully'));

        if (successCallback && typeof successCallback === 'function') {
            successCallback();
        }
    } else {
        if (data.errors) {
            const errors = {};
            data.errors.map(error => {
                errors[camelCase(error.key)] = error.errorMessage;
            });

            vm.setState({ errors });
        } else {
            toastObj.error(data.message);
        }
    }
};

export function loadMorePerformanceRecord(vm, page, callback) {
    const records = vm.state.performanceRecords.slice(0, page * vm.itemPerPage);
    vm.setState({
        displayedPerformanceRecords: records,
        hasMore: records.length < vm.state.performanceRecords.length
    }, () => {
        if (callback && typeof callback === 'function') {
            callback();
        }
    });
}