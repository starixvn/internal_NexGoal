import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import TargetType from './TargetType';

export default class EditBasicInformation extends Component {
    constructor(props) {
        super(props);

        this.setState({
            basicInformation: {}
        });
    }

    validateForm = values => {
        let errors = {};

        if (!values.name) {
            errors.name = 'Name is required';
        }

        if (!values.startDate && values.targetTypeId == TargetType.semiAnual) {
            errors.startDate = 'Start Date is required';
        }

        if (!values.endDate && values.targetTypeId == TargetType.semiAnual) {
            errors.endDate = 'End Date is required';
        }

        if (!values.disciplineStartDate && values.targetTypeId == TargetType.semiAnual) {
            errors.disciplineStartDate = 'Discipline Start Date is required';
        }

        if (!values.disciplineEndDate && values.targetTypeId == TargetType.semiAnual) {
            errors.disciplineEndDate = 'Discipline End Date is required';
        }

        if (!values.focusStartDate && values.targetTypeId == TargetType.semiAnual) {
            errors.focusStartDate = 'Focus Start Date is required';
        }

        if (values.targetTypeId == TargetType.semiAnual && values.focusStartDate && values.endDate && values.focusStartDate <= values.endDate) {
            errors.focusStartDate = 'Focus Start Date must be after Period End Date';
        }

        if (!values.description) {
            errors.description = 'Description is required';
        }

        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: `/discipline/${values.disciplineId === 0 ? 'create' : 'edit'}`,
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: values.disciplineId,
                employeeId: values.employee.employeeId,
                effectiveDate: values.effectiveDate.format('YYYY-MM-DD'),
                content: values.content,
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Update Successfully");
                this.props.onEditDiscipline(values.disciplineId, response.data);
                this.props.closeModal();
            } else {
                if (data.errors) {
                    let errors = {};
                    data.errors.map(error => {
                        errors[camelCase(error.key)] = error.errorMessage;
                    });

                    setErrors(errors);
                } else {
                    toast.error(data.message);
                }
                setSubmitting(false);
            }
        }).catch(commonFunctions.axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        return (
            <Form horizontal onSubmit={handleSubmit}>
                {!isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.name && errors.name && <li>{errors.name}</li>}
                        {touched.startDate && errors.startDate && <li>{errors.startDate}</li>}
                        {touched.endDate && errors.endDate && <li>{errors.endDate}</li>}
                        {touched.disciplineStartDate && errors.disciplineStartDate && <li>{errors.disciplineStartDate}</li>}
                        {touched.disciplineEndDate && errors.disciplineEndDate && <li>{errors.disciplineEndDate}</li>}
                        {values.targetTypeId == TargetType.semiAnual && touched.focusStartDate && errors.focusStartDate && <li>{errors.focusStartDate}</li>}
                        {touched.description && errors.description && <li>{errors.description}</li>}
                    </ul>
                }
            </Form>
        );
    }

    render() {
        return (
            <Formik initialValues={this.state.basicInformation} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
        );
    }
}