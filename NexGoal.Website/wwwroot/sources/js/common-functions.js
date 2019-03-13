import React from 'react';
import moment from 'moment';

const _rootUrl = document.getElementById('RootURL').value ? document.getElementById('RootURL').value : '/';

export const resolveClientUrl = relativePath => _rootUrl + relativePath;

export const axiosError = error => {
    if (error.response) {
        let url;

        switch (error.response.status) {
            case 401:
                url = 'auth/login';
                break;

            case 404:
                url = 'error/PageNotFound';
                break;

            case 405:
                url = 'error/AccessDenied';
                break;

            case 500:
                url = 'error/InternalError';
                break;
        }

        window.location = _rootUrl + url;
    }
};

export const getCurrentUserInfo = () => {
    return {
        nickName: document.getElementById('employeeInfo-nickName').innerText,
        fullName: document.getElementById('employeeInfo-fullName').innerText,
        jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
        avatar: document.getElementById('employeeInfo-avatar').getAttribute('src'),
        isProbation: document.getElementById('employeeInfo-isProbation').checked
    };
};

export const getRoleClassName = roleId => {
    let className = '';

    switch (roleId) {
        case 2:
            className = 'role-member';
            break;

        case 3:
            className = 'role-pm';
            break;

        case 4:
            className = 'role-evaluator';
            break;

        case 5:
            className = 'role-reviewer';
            break;

        case 255:
            className = 'role-system';
            break;

        default:
            className = 'role-default';
    }

    return className;
};

export const getNominationClassName = statusId => {
    let className = '';

    switch (statusId) {
        case 1:
            className = 'status-nomination';
            break;

        case 2:
            className = 'status-shortlisted';
            break;

        case 3:
            className = 'status-winner';
            break;
    }

    return className;
};

export const getPAGradeClassName = (originalGradeId = null, gradeId = null) => {
    originalGradeId = originalGradeId === null ? 1 : originalGradeId;
    gradeId = gradeId === null ? 1 : gradeId;

    let gradeClass = '';
    if (gradeId > originalGradeId) {
        gradeClass = 'icon-increase-arrow';
    } else if (gradeId < originalGradeId) {
        gradeClass = 'icon-descrease-arrow';
    }

    return gradeClass;
};

export const calculateTimelineNode = () => {
    const width = window.innerWidth;
    if (width > 1600) { return 8; }
    if (width > 1500 && width <= 1600) { return 7; }
    if (width >= 1440 && width <= 1500) { return 6; }
    if (width >= 1366 && width <= 1440) { return 5; }
    if (width > 1280 && width < 1366) { return 4; }

    return 3;
};

export const noDataIndication = () => <span className="empty-result">No record found</span>;

export function compareArray(a, b, compareOnProp) {
    if (a === b) { return true; }

    if (a.length !== b.length) { return false; }

    if (a.some(a => !b.some(b => b[compareOnProp] === a[compareOnProp]))
        || b.some(b => !a.some(a => a[compareOnProp] === b[compareOnProp]))) {
        return false;
    }

    return true;
}

export function calendar(date) {
    return moment(date).calendar(null, {
        sameDay: '[Today at] HH:mm',
        lastDay: '[Yesterday at] HH:mm',
        lastWeek: '[Last] dddd [at] HH:mm',
        sameElse: 'DD MMM YYYY [at] HH:mm'
    });
}