import React, { Component } from 'react';
import { Modal, Button, FormGroup, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

import { axiosError, resolveClientUrl, validateFormikField, displayFormikErrors } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, InputErrorCssClass, PostRequestHeaders } from '../../constants';
import CreateEditForm from './CreateEditForm';

export default class Edit extends Component {
    validateForm = values => {
        const errors = {};

        if (!values.effectiveDate) {
            errors.effectiveDate = getResource('IssueDateRequired');
        }

        if (!values.description) {
            errors.description = getResource('ContentRequired');
        }

        return errors;
    }

    handleSubmit = (values, { setErrors }) => {
        axios({
            url: resolveClientUrl('discipline/edit'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: values.disciplineId,
                employeeId: values.employeeId,
                effectiveDate: values.effectiveDate.format('YYYY-MM-DD'),
                content: values.description
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));

                this.props.onEditSuccess(values);
            } else {
                if (data.errors) {
                    const errors = {};
                    data.errors.map(error => {
                        errors[camelCase(error.key)] = error.errorMessage;
                    });

                    setErrors(errors);
                } else {
                    toast.error(data.message);
                }
            }
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <CreateEditForm
                isCreate={false}
                values={values}
                touched={touched}
                errors={errors}
                employeeOptions={this.props.employeeOptions}
                setFieldValue={setFieldValue}
                onChange={handleChange}
                onSubmit={handleSubmit}
                onClose={this.props.onClose}
            />
        );
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal}
                onHide={this.props.onClose}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{`Edit ${this.props.discipline.employee.nickName}'s Discipline`}</Modal.Title>
                </Modal.Header>
                <Formik initialValues={{ ...this.props.discipline }}
                    validate={this.validateForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}