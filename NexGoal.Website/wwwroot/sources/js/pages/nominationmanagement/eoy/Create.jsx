import React, { Component } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

import commonFunctions from '../../../common-functions';
import { validateEOYForm } from '../common';

class CreateEOY extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (values, { setErrors }) => {
        let eoy = {
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        };

        let formData = commonFunctions.objectToFormData(eoy);
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);

        axios({
            url: commonFunctions.getRootUrl() + 'EOY/Create',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
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
        }).catch(commonFunctions.axiosError);
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
            <Form id="create-eoy-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        !isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.year && errors.year && <li>{errors.year}</li>}
                            {touched.startDateForNominator && errors.startDateForNominator && <li>{errors.startDateForNominator}</li>}
                            {touched.endDateForNominator && errors.endDateForNominator && <li>{errors.endDateForNominator}</li>}
                            {touched.remindDateForNominator && errors.remindDateForNominator && <li>{errors.remindDateForNominator}</li>}
                            {touched.endDateForCommittee && errors.endDateForCommittee && <li>{errors.endDateForCommittee}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }

                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Year</label>
                        <div className="col-sm-3 field">
                            <Select name="year"
                                value={values.year}
                                options={yearOptions}
                                clearable={false}
                                searchable={false}
                                onChange={selectedOption => setFieldValue('year', selectedOption)} />
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
                                className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`} />
                        </div>
                        <label className="col-sm-3 control-label required-label">End Date For Nominator</label>
                        <div className="col-sm-3 field">
                            <DatePicker autoComplete="off"
                                dateFormat="DD MMM, YYYY"
                                selected={values.endDateForNominator}
                                onChange={value => setFieldValue('endDateForNominator', value)}
                                name="endDateForNominator"
                                className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`} />
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
                                className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`} />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">Description</label>
                        <div className="col-sm-9 field">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                rows="5" maxLength="200"
                                name="description" value={values.description}
                                onChange={handleChange} />
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
        const eoy = {
            year: { value: currentYear, label: currentYear },
            startDateForNominator: null,
            endDateForNominator: null,
            remindDateForNominator: null,
            endDateForCommittee: null,
            description: ''
        };

        return (
            <Modal show={this.props.showModal} keyboard={false}
                onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Nomination Period</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={eoy}
                    validate={validateEOYForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm} />
            </Modal>
        );
    }
}

export default CreateEOY;