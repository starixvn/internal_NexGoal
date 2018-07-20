import React from 'react';

export const quarterOptions = [
    { value: 1, label: '1st' },
    { value: 2, label: '2nd' },
    { value: 3, label: '3rd' },
    { value: 4, label: '4th' }
];

export const statusOptions = [
    { value: '', label: '-- All --' },
    { value: '0', label: 'Not Ready' },
    { value: '1', label: 'In Progress' },
    { value: '2', label: 'Closed' }
];

export const nominationRoleOptions = [
    { value: '', label: '-- All --' },
    { value: 1, label: 'Nominator' },
    { value: 2, label: 'Committee' }
];

export function renderStatusOption(option) {
    let cssClass = 'nomination-period-status';

    switch (option.value) {
        case '0':
            cssClass += ' not-started';
            break;
        case '1':
            cssClass += ' in-progress';
            break;
            cssClass += ' closed';
        case '2':
            break;
    }

    return (<span className={cssClass}>{option.label}</span>);
}