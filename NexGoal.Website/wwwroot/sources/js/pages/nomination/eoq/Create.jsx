import React, { Component } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { quarterOptions, validateEOQForm } from '../common';
import getResource from '../../../resourceHelper';
import { DateFormat, InputErrorCssClass } from '../../../constants';

class Create extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (values, { setErrors }) => {
        const eoq = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            quarter: values.quarter.value,
            year: values.year.value,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOQ/Create'),
            method: 'POST',
            data: eoq,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('CreatedSuccessfully'));

                this.props.closeModal();
                this.props.handleFilterSubmit();
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
        const currentYear = new Date().getFullYear();
        const yearOptions = [
            { value: currentYear, label: currentYear },
            { value: currentYear + 1, label: currentYear + 1 }
        ];

        return (
            <Form id="create-eoq-form" inline onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        !isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.quarter && errors.quarter && <li>{errors.quarter}</li>}
                            {touched.year && errors.year && <li>{errors.year}</li>}
                            {touched.startDateForNominator && errors.startDateForNominator && <li>{errors.startDateForNominator}</li>}
                            {touched.endDateForNominator && errors.endDateForNominator && <li>{errors.endDateForNominator}</li>}
                            {touched.remindDateForNominator && errors.remindDateForNominator && <li>{errors.remindDateForNominator}</li>}
                            {touched.endDateForCommittee && errors.endDateForCommittee && <li>{errors.endDateForCommittee}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }
                    <div className="line-group">
                        <FormGroup>
                            <label className="control-label">{getResource('Quarter')}</label>
                            <div className="field control-content">
                                <Select name="quarter" isSearchable={false} isClearable={false}
                                    className="react-select-container" classNamePrefix="react-select"
                                    value={values.quarter}
                                    options={quarterOptions}
                                    onChange={selectedOption => setFieldValue('quarter', selectedOption)}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="control-label">{getResource('Year')}</label>
                            <div className="field control-content">
                                <Select name="year" isSearchable={false} isClearable={false}
                                    className="react-select-container" classNamePrefix="react-select"
                                    value={values.year}
                                    options={yearOptions}
                                    onChange={selectedOption => setFieldValue('year', selectedOption)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <div className="title-group"><strong>{getResource('ForNominator')}</strong></div>
                        <FormGroup>
                            <label className="control-label">{getResource('StartDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="startDateForNominator" autoComplete="off"
                                    className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.startDateForNominator}
                                    onChange={value => setFieldValue('startDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="control-label">{getResource('EndDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="endDateForNominator" autoComplete="off"
                                    className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.endDateForNominator}
                                    onChange={value => setFieldValue('endDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="control-label">{getResource('RemindDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="remindDateForNominator" autoComplete="off"
                                    className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.remindDateForNominator}
                                    onChange={value => setFieldValue('remindDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <div className="title-group"><strong>{getResource('ForCommittee')}</strong></div>
                        <FormGroup>
                            <label className="control-label">{getResource('EndDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="endDateForCommittee" autoComplete="off"
                                    className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.endDateForCommittee}
                                    onChange={value => setFieldValue('endDateForCommittee', value)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup className="description-group">
                            <label className="control-label">{getResource('Description')} (optional)</label>
                            <div className="field control-content">
                                <textarea className={`form-control none-resize ${touched.description && errors.description && InputErrorCssClass}`}
                                    rows="5" maxLength="200"
                                    name="description" value={values.description}
                                    onChange={handleChange}
                                />
                            </div>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

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

export default Create;