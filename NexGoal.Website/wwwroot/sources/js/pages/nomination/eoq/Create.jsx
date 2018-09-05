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

class CreateEOQ extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (values, { setErrors }) => {
        let eoq = stringify({
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
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Created successfully!');

                this.props.closeModal();
                this.props.handleFilterSubmit();
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
            <Form id="create-eoq-form" horizontal onSubmit={handleSubmit}>
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

                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Quarter</label>
                        <div className="col-sm-3 field">
                            <Select name="quarter" isSearchable={false} isClearable={false}
                                className="react-select-container" classNamePrefix="react-select"
                                value={values.quarter}
                                options={quarterOptions}
                                onChange={selectedOption => setFieldValue('quarter', selectedOption)}
                            />
                        </div>
                        <label className="col-sm-3 control-label required-label">Year</label>
                        <div className="col-sm-3 field">
                            <Select name="year" isSearchable={false} isClearable={false}
                                className="react-select-container" classNamePrefix="react-select"
                                value={values.year}
                                options={yearOptions}
                                onChange={selectedOption => setFieldValue('year', selectedOption)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Start Date For Nominator</label>
                        <div className="col-sm-3 field">
                            <DatePicker autoComplete="off"
                                dateFormat="DD MMM, YYYY"
                                selected={values.startDateForNominator}
                                onChange={value => setFieldValue('startDateForNominator', value)}
                                name="startDateForNominator"
                                className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`}
                            />
                        </div>
                        <label className="col-sm-3 control-label required-label">End Date For Nominator</label>
                        <div className="col-sm-3 field">
                            <DatePicker autoComplete="off"
                                dateFormat="DD MMM, YYYY"
                                selected={values.endDateForNominator}
                                onChange={value => setFieldValue('endDateForNominator', value)}
                                name="endDateForNominator"
                                className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Remind Date For Nominator</label>
                        <div className="col-sm-3 field">
                            <DatePicker autoComplete="off"
                                dateFormat="DD MMM, YYYY"
                                selected={values.remindDateForNominator}
                                onChange={value => setFieldValue('remindDateForNominator', value)}
                                name="remindDateForNominator"
                                className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && 'input-validation-error'}`} />
                        </div>
                        <label className="col-sm-3 control-label required-label">End Date For Committee</label>
                        <div className="col-sm-3 field">
                            <DatePicker autoComplete="off"
                                dateFormat="DD MMM, YYYY"
                                selected={values.endDateForCommittee}
                                onChange={value => setFieldValue('endDateForCommittee', value)}
                                name="endDateForCommittee"
                                className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">Description</label>
                        <div className="col-sm-9 field">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                rows="5" maxLength="200"
                                name="description" value={values.description}
                                onChange={handleChange}
                            />
                        </div>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.closeModal}>Cancel</Button>
                    <Button type="submit" className="btn btn-primary">Submit</Button>
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
                    <Modal.Title>Create Nomination Period</Modal.Title>
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

export default CreateEOQ;