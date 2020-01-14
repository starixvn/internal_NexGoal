import React from 'react';
import moment from 'moment';
import { isEmpty } from 'lodash';

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
    const originalGrade = originalGradeId === null ? 1 : originalGradeId;
    const currentGrade = gradeId === null ? 1 : gradeId;

    let gradeClass = '';
    if (currentGrade > originalGrade) {
        gradeClass = 'icon-increase-arrow';
    } else if (currentGrade < originalGrade) {
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

export const compareArray = function (a, b, compareOnProp) {
    if (a === b) { return true; }

    if (a.length !== b.length) { return false; }

    if (a.some(ae => !b.some(be => be[compareOnProp] === ae[compareOnProp]))
        || b.some(be => !a.some(ae => ae[compareOnProp] === be[compareOnProp]))) {
        return false;
    }

    return true;
};

export const calendar = function (date) {
    return moment(date).calendar(null, {
        sameDay: '[Today at] HH:mm',
        lastDay: '[Yesterday at] HH:mm',
        lastWeek: '[Last] dddd [at] HH:mm',
        sameElse: 'DD MMM YYYY [at] HH:mm'
    });
};

export const getUrlByActionTypeId = function (actionTypeId, recordId, NotificationActionType) {
    let url = '';
    switch (actionTypeId) {
        case NotificationActionType.MyPageWithCreatePerformanceCheckin:
            url = resolveClientUrl('?action=newrecord');
            break;

        case NotificationActionType.MyPageWithSpecificPerformanceCheckin:
            url = resolveClientUrl(`?recordId=${recordId}`);
            break;

        case NotificationActionType.MemberPageWithCreatePerformanceCheckin:
            url = resolveClientUrl('member?action=newrecord');
            break;

        case NotificationActionType.MemberPageWithSpecificPerformanceCheckin:
            url = resolveClientUrl(`member?recordId=${recordId}`);
            break;

        case NotificationActionType.MemberPageWithActiveNominationTab:
            url = resolveClientUrl('member?activeTab=2');
            break;

        case NotificationActionType.MemberPageWithActivePerformanceAppraisalTab:
            url = resolveClientUrl('member?activeTab=3');
            break;

        case NotificationActionType.PAFocus:
            url = resolveClientUrl('pafocus');
            break;
    }

    return url;
};

export const groupBy = function (array, keyGetter) {
    const map = new Map();

    for (let i = 0; i < array.length; i++) {
        const item = array[i];
        const key = keyGetter(item);

        const collection = map.get(key);
        if (!collection) {
            map.set(key, [item]);
        } else {
            collection.push(item);
        }
    }

    return [...map];
};

export const validateFormikField = (touched, errors, fieldName) => touched[fieldName] && errors[fieldName] ? false : true;

export const displayFormikErrors = (touched, errors, fieldNames = []) => {
    return (
        !isEmpty(errors)
        && !isEmpty(touched)
        && <ul className="list-message-error">
            {
                fieldNames.map(field => {
                    const isValid = validateFormikField(touched, errors, field);
                    return !isValid && <li>{errors[field]}</li>;
                })
            }
        </ul>
    );
};