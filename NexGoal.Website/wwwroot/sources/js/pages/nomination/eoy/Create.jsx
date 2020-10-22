import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl, axiosCreateResponseWithFormik } from '../../../common-functions';
import { validateEOYForm } from '../common';
import getResource from '../../../resourceHelper';
import { PostRequestHeaders } from '../../../constants';
import CreateNominationPeriodForm from '../CreateNominationPeriodForm';

class Create extends Component {
    handleSubmit = (values, { setErrors }) => {
        const eoy = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            year: values.year.value,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOY/Create'),
            method: 'POST',
            data: eoy,
            headers: PostRequestHeaders
        }).then(response => {
            axiosCreateResponseWithFormik(setErrors, toast, response, () => {
                this.props.closeModal();
                this.props.handleFilterSubmit();
            });
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
            <CreateNominationPeriodForm isEOQ={false}
                values={values} errors={errors} touched={touched}
                setFieldValue={setFieldValue}
                onChange={handleChange}
                onSubmit={handleSubmit}
                onClose={this.props.closeModal}
            />
        );
    }

    render() {
        const currentYear = new Date().getFullYear();
        const eoy = {
            year: { value: currentYear, label: currentYear },
            remindDateForNominator: null,
            endDateForCommittee: null,
            startDateForNominator: null,
            endDateForNominator: null,
            description: ''
        };

        return (
            <Modal id="eoy-create-modal" keyboard={false} backdrop="static"
                show={this.props.showModal}
                onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AddNominationPeriod')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={eoy}
                    validate={validateEOYForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}

export default Create;