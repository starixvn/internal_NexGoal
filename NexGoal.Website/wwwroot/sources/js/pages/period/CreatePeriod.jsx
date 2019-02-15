import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Checkbox, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, TargetType } from '../../constants';

export default class CreatePeriod extends Component {
    constructor(props) {
        super(props);
    }

    initialObj = {
        name: '',
        targetTypeId: TargetType.SemiAnnual,
        startDate: null,
        endDate: null,
        disciplineStartDate: null,
        disciplineEndDate: null,
        focusStartDate: null,
        focusEndDate: null,
        description: '',
        isActive: true
    }

    validateForm = values => {
        let errors = {};

        if (!values.name) {
            errors.name = getResource('NameRequired');
        }

        if (values.targetTypeId === TargetType.SemiAnnual) {
            if (!values.startDate) {
                errors.startDate = 'Start Date is required';
            }

            if (!values.endDate) {
                errors.endDate = 'End Date is required';
            }

            if (!values.disciplineStartDate) {
                errors.disciplineStartDate = 'Discipline Start Date is required';
            }

            if (!values.disciplineEndDate) {
                errors.disciplineEndDate = 'Discipline End Date is required';
            }

            if (!values.focusStartDate) {
                errors.focusStartDate = 'Focus Start Date is required';
            }

            if (!values.focusEndDate) {
                errors.focusEndDate = 'Focus End Date is required';
            }
        }

        return errors;
    }

    onSubmit = (values, { setErrors }) => {
        axios({
            url: resolveClientUrl(`period/Create`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                name: values.name,
                targetTypeId: values.targetTypeId,
                startDate: values.startDate ? values.startDate.format('YYYY-MM-DD') : null,
                endDate: values.endDate ? values.endDate.format('YYYY-MM-DD') : null,
                disciplineStartDate: values.disciplineStartDate ? values.disciplineStartDate.format('YYYY-MM-DD') : null,
                disciplineEndDate: values.disciplineEndDate ? values.disciplineEndDate.format('YYYY-MM-DD') : null,
                focusStartDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusStartDate.format('YYYY-MM-DD') : null,
                focusEndDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusEndDate.format('YYYY-MM-DD') : null,
                description: values.description,
                isActive: values.isActive
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
                this.props.closeModal();
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
        var onChangeRaw = (e) => {
            let date = moment(Date(e.target.value));
            if (date.isValid()) {
                setFieldValue(e.target.name, date);
            }
        };

        return (
            <Form inline onSubmit={handleSubmit} className="evaluation-period-form">
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.name && errors.name && <li>{errors.name}</li>}
                            {touched.startDate && errors.startDate && <li>{errors.startDate}</li>}
                            {touched.endDate && errors.endDate && <li>{errors.endDate}</li>}
                            {touched.disciplineStartDate && errors.disciplineStartDate && <li>{errors.disciplineStartDate}</li>}
                            {touched.disciplineEndDate && errors.disciplineEndDate && <li>{errors.disciplineEndDate}</li>}
                            {values.targetTypeId === TargetType.SemiAnnual && touched.focusStartDate && errors.focusStartDate && <li>{errors.focusStartDate}</li>}
                            {values.targetTypeId === TargetType.SemiAnnual && touched.focusEndDate && errors.focusEndDate && <li>{errors.focusEndDate}</li>}
                        </ul>
                    }
                    <div className="line-group">
                        <FormGroup>
                            <label className="control-label">{getResource('Name')}</label>
                            <div class="control-content name-content">
                                <input type="text" name="name" value={values.name} autoComplete="off"
                                    className={`form-control  ${touched.name && errors.name && 'input-validation-error'}`}
                                    onChange={handleChange}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup>
                            <Col componentClass={ControlLabel}>
                                {getResource('EvaluationStartDate')}
                            </Col>
                            <Col className="control-content">
                                <DatePicker name="startDate" autoComplete="off"
                                    className={`form-control  ${touched.startDate && errors.startDate && 'input-validation-error'}`}
                                    maxDate={values.endDate}
                                    dateFormat={DateFormat} selected={values.startDate}
                                    onChange={value => setFieldValue('startDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel} >
                                {getResource('EvaluationEndDate')}
                            </Col>
                            <Col className="control-content">
                                <DatePicker name="endDate" autoComplete="off"
                                    className={`form-control  ${touched.endDate && errors.endDate && 'input-validation-error'}`}
                                    minDate={values.startDate}
                                    dateFormat={DateFormat} selected={values.endDate}
                                    onChange={value => setFieldValue('endDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup>
                            <Col componentClass={ControlLabel} >
                                Working From Date
                            </Col>
                            <Col className="control-content">
                                <DatePicker name="disciplineStartDate" autoComplete="off"
                                    className={`form-control  ${touched.disciplineStartDate && errors.disciplineStartDate && 'input-validation-error'}`}
                                    maxDate={values.disciplineEndDate}
                                    dateFormat={DateFormat} selected={values.disciplineStartDate}
                                    onChange={value => setFieldValue('disciplineStartDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel} >
                                Working To Date
                            </Col>
                            <Col className="control-content">
                                <DatePicker name="disciplineEndDate" autoComplete="off"
                                    className={`form-control  ${touched.disciplineEndDate && errors.disciplineEndDate && 'input-validation-error'}`}
                                    minDate={values.disciplineStartDate}
                                    dateFormat={DateFormat} selected={values.disciplineEndDate}
                                    onChange={value => setFieldValue('disciplineEndDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                    </div>
                    {values.targetTypeId === TargetType.SemiAnnual &&
                        <div className="line-group">
                            <FormGroup>
                                <Col componentClass={ControlLabel} className="">Focus Start Date</Col>
                                <Col className="control-content">
                                    <DatePicker name="focusStartDate" autoComplete="off"
                                        className={`form-control  ${touched.focusStartDate && errors.focusStartDate && 'input-validation-error'}`}
                                        maxDate={values.focusEndDate}
                                        dateFormat={DateFormat} selected={values.focusStartDate}
                                        onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                    />
                                </Col>
                            </FormGroup>
                            <FormGroup>
                                <Col componentClass={ControlLabel} className="">Focus End Date</Col>
                                <Col className="control-content">
                                    <DatePicker name="disciplineEndDate" autoComplete="off"
                                        className={`form-control  ${touched.focusEndDate && errors.focusEndDate && 'input-validation-error'}`}
                                        minDate={values.focusStartDate}
                                        dateFormat={DateFormat} selected={values.focusEndDate}
                                        onChange={value => setFieldValue('focusEndDate', value)} onChangeRaw={onChangeRaw}
                                    />
                                </Col>
                            </FormGroup>
                        </div>
                    }
                    <div className="line-group">
                        <FormGroup>                
                            <div className="checkbox">
                                <input type="checkbox" className="checkbox-custom" name="isActive" onChange={handleChange} checked={values.isActive} id="chb-isActive"/>
                                <label htmlFor="chb-isActive">Active</label>
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup className="description-group">
                            <Col componentClass={ControlLabel}>{getResource('Description')} (optional)</Col>
                            <Col className="">
                                <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                    name="description" rows="4"
                                    onChange={handleChange} value={values.description}
                                />
                            </Col>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AddEvaluationPeriod')}</Modal.Title>
                </Modal.Header>
                <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}