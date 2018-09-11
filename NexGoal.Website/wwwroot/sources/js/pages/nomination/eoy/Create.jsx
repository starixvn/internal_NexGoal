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
import { validateEOYForm } from '../common';
import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';

class CreateEOY extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (values, { setErrors }) => {
        let eoy = stringify({
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
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('CreatedSuccessfully'));

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
                        <label className="col-sm-3 control-label required-label">{getResource('Year')}</label>
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
                        <label className="col-sm-3 control-label required-label">{getResource('StartDateForNominator')}</label>
                        <div className="col-sm-3 field">
                            <DatePicker name="startDateForNominator" autoComplete="off"
                                className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.startDateForNominator}
                                onChange={value => setFieldValue('startDateForNominator', value)}
                            />
                        </div>
                        <label className="col-sm-3 control-label required-label">{getResource('EndDateForNominator')}</label>
                        <div className="col-sm-3 field">
                            <DatePicker name="endDateForNominator" autoComplete="off"
                                className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForNominator}
                                onChange={value => setFieldValue('endDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">{getResource('RemindDateForNominator')}</label>
                        <div className="col-sm-3 field">
                            <DatePicker name="remindDateForNominator" autoComplete="off"
                                className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.remindDateForNominator}
                                onChange={value => setFieldValue('remindDateForNominator', value)}
                            />
                        </div>
                        <label className="col-sm-3 control-label required-label">{getResource('EndDateForCommittee')}</label>
                        <div className="col-sm-3 field">
                            <DatePicker name="endDateForCommittee" autoComplete="off"
                                className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForCommittee}
                                onChange={value => setFieldValue('endDateForCommittee', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">{getResource('Description')}</label>
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
                    <Button className="btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
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
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('CreateNominationPeriod')}</Modal.Title>
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

export default CreateEOY;