import getResource from '../../resourceHelper';
import { TargetType } from '../../constants';

export function validateForm(values) {
    const errors = {};
    if (!values.name) {
        errors.name = getResource('NameRequired');
    }

    if (values.targetTypeId === TargetType.SemiAnnual) {
        if (!values.startDate) {
            errors.startDate = 'Start Date is required';
        }

        if (!values.endDate) {
            errors.endDate = 'End Date is required';
        }

        if (values.startDate && values.endDate && values.startDate.isAfter(values.endDate)) {
            errors.startDate = 'Start Date must be before End Date';
        }

        if (!values.disciplineStartDate) {
            errors.disciplineStartDate = 'Discipline Start Date is required';
        }

        if (!values.disciplineEndDate) {
            errors.disciplineEndDate = 'Discipline End Date is required';
        }

        if (values.disciplineStartDate && values.disciplineEndDate && values.disciplineStartDate.isAfter(values.disciplineEndDate)) {
            errors.disciplineStartDate = 'Discipline Start Date must be before Discipline End Date';
        }

        if (!values.focusStartDate) {
            errors.focusStartDate = 'Focus Start Date is required';
        }

        if (!values.focusEndDate) {
            errors.focusEndDate = 'Focus End Date is required';
        }

        if (values.focusStartDate && values.focusEndDate && values.focusStartDate.isAfter(values.focusEndDate)) {
            errors.focusStartDate = 'Focus Start Date must be before Focus End Date';
        }
    }

    return errors;
}