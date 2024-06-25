import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, axiosCreateResponseWithFormik } from '../../../common-functions';
import { validateEOQForm } from '../common';
import getResource from '../../../resourceHelper';
import { PostRequestHeaders } from '../../../constants';
import CreateNominationPeriodForm from '../CreateNominationPeriodForm';

export default class Create extends Component {
    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <CreateNominationPeriodForm isEOQ
                values={values} errors={errors} touched={touched}
                setFieldValue={setFieldValue}
                onChange={handleChange}
                onSubmit={handleSubmit}
                onClose={this.props.closeModal}
            />
        );
    };

    handleSubmit = (values, { setErrors }) => {
        const dateFormat = 'YYYY-MM-DD';
        const eoq = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            quarter: values.quarter.value,
            year: values.year.value,
            startDateForNominator: values.startDateForNominator.format(dateFormat),
            endDateForNominator: values.endDateForNominator.format(dateFormat),
            remindDateForNominator: values.remindDateForNominator.format(dateFormat),
            endDateForCommittee: values.endDateForCommittee.format(dateFormat),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOQ/Create'),
            method: 'POST',
            data: eoq,
            headers: PostRequestHeaders
        }).then(response => {
            axiosCreateResponseWithFormik(setErrors, toast, response, () => {
                this.props.closeModal();
                this.props.handleFilterSubmit();
            });
        }).catch(axiosError);
    };

    render() {
        const currentYear = new Date().getFullYear();
        const eoq = {
            quarter: { value: 1, label: '1st' },
            year: { value: currentYear, label: currentYear },
            startDateForNominator: null,
            endDateForNominator: null,
            remindDateForNominator: null,
            endDateForCommittee: null,
            description: ''
        };

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AddNominationPeriod')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={eoq}
                    validate={validateEOQForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}