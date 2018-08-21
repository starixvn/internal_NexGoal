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

export function validateEOYForm(values) {
    let errors = {};

    if (!values.year) {
        errors.year = 'Year is required!';
    }

    if (!values.startDateForNominator) {
        errors.startDateForNominator = 'Start Date For Nominator is required!';
    }

    if (!values.endDateForNominator) {
        errors.endDateForNominator = 'End Date For Nominator is required!';
    }

    if (!values.remindDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator is required!';
    }

    if (!values.endDateForCommittee) {
        errors.endDateForCommittee = 'End Date For Committee is required!';
    }

    if (values.endDateForNominator < values.startDateForNominator) {
        errors.endDateForNominator = 'End Date For Nominator must be greater than Start Date For Nominator.';
    }

    if (values.remindDateForNominator < values.startDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator must be greater than or equal to Start Date For Nominator.';
    }

    if (values.endDateForNominator < values.remindDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator must be less than or equal to End Date For Nominator.';
    }

    if (values.endDateForCommittee < values.endDateForNominator) {
        errors.endDateForCommittee = 'End Date For Committee must be greater than End Date For Nominator.';
    }

    return errors;
}

export function validateEOQForm(values) {
    let errors = {};

    if (!values.quarter) {
        errors.quarter = 'Quarter is required!';
    }

    if (!values.year) {
        errors.year = 'Year is required!';
    }

    if (!values.startDateForNominator) {
        errors.startDateForNominator = 'Start Date For Nominator is required!';
    }

    if (!values.endDateForNominator) {
        errors.endDateForNominator = 'End Date For Nominator is required!';
    }

    if (!values.remindDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator is required!';
    }

    if (!values.endDateForCommittee) {
        errors.endDateForCommittee = 'End Date For Committee is required!';
    }

    if (values.endDateForNominator < values.startDateForNominator) {
        errors.endDateForNominator = 'End Date For Nominator must be greater than Start Date For Nominator.';
    }

    if (values.remindDateForNominator < values.startDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator must be greater than or equal to Start Date For Nominator.';
    }

    if (values.endDateForNominator < values.remindDateForNominator) {
        errors.remindDateForNominator = 'Remind Date For Nominator must be less than or equal to End Date For Nominator.';
    }

    if (values.endDateForCommittee < values.endDateForNominator) {
        errors.endDateForCommittee = 'End Date For Committee must be greater than End Date For Nominator.';
    }

    return errors;
}