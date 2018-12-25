import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import Select from 'react-select';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class Create extends Component {
    validateForm = values => {
        let errors = {};

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
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('CreatedSuccessfully'));

                this.props.onCreateSuccess();
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
        return (
            <Form id="create-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        !isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.employee && errors.employee && <li>{errors.employee}</li>}
                            {touched.effectiveDate && errors.effectiveDate && <li>{errors.effectiveDate}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>{getResource('Employee')}</Col>
                        <Col sm={9}>
                            <Select name="employee"
                                className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`}
                                classNamePrefix="react-select"
                                isDisabled={values.disciplineId > 0} isClearable
                                options={this.props.employeeOptions}
                                value={values.employee}
                                onChange={value => setFieldValue('employee', value)}
                            />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>{getResource('IssueDate')}</Col>
                        <Col sm={9}>
                            <DatePicker className={`form-control  ${touched.effectiveDate && errors.effectiveDate && 'input-validation-error'}`}
                                dateFormat={DateFormat} selected={values.effectiveDate}
                                onChange={value => setFieldValue('effectiveDate', value)}
                                onChangeRaw={this.onChangeRaw}
                                name="effectiveDate" autoComplete="off"
                            />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>{getResource('Content')}</Col>
                        <Col sm={9}>
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                name="description" rows="10"
                                onChange={handleChange}
                                value={values.description}
                            />
                        </Col>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.onClose}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        let discipline = {
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