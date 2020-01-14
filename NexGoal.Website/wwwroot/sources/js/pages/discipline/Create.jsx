import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase } from 'lodash';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { PostRequestHeaders } from '../../constants';
import CreateEditForm from './CreateEditForm';

export default class Create extends Component {
    validateForm = values => {
        const errors = {};

        if (!values.employee) {
            errors.employee = getResource('EmployeeRequired');
        }

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
            url: resolveClientUrl('discipline/create'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: values.disciplineId,
                employeeId: values.employee.value,
                effectiveDate: values.effectiveDate.format('YYYY-MM-DD'),
                content: values.description
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('CreatedSuccessfully'));

                this.props.onCreateSuccess();
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
                isCreate
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
        const discipline = {
            employee: null,
            effectiveDate: moment(),
            description: ''
        };

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal}
                onHide={this.props.onClose}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AddDiscipline')}</Modal.Title>
                </Modal.Header>
                <Formik initialValues={discipline}
                    validate={this.validateForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}