import React, { Component } from 'react';
import { Modal, Button, FormGroup, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

import { axiosError, resolveClientUrl, validateFormikField } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, InputErrorCssClass, PostRequestHeaders } from '../../constants';

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
        const isValidEffectiveDate = validateFormikField(touched, errors, 'effectiveDate');
        const isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form className="discipline-form" id="edit-form" inline onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        !isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {!isValidEffectiveDate && <li>{errors.effectiveDate}</li>}
                            {!isValidDescription && <li>{errors.description}</li>}
                        </ul>
                    }

                    <div className="line-group">
                        <FormGroup>
                            <label className="control-label required-label">{getResource('IssueDate')}</label>
                            <div className="control-content">
                                <DatePicker className={`form-control  ${!isValidEffectiveDate && InputErrorCssClass}`}
                                    dateFormat={DateFormat} selected={values.effectiveDate}
                                    onChange={value => setFieldValue('effectiveDate', value)}
                                    onChangeRaw={this.onChangeRaw}
                                    name="effectiveDate" autoComplete="off"
                                />
                            </div>
                        </FormGroup>
                        <FormGroup className="description-group">
                            <label className="control-label required-label">{getResource('Content')}</label>
                            <div className="control-content">
                                <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
                                    name="description" rows="10"
                                    onChange={handleChange}
                                    value={values.description}
                                />
                            </div>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.onClose}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
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