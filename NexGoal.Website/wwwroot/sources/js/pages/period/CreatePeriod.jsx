import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl, validateFormikField, displayFormikErrors, axiosCreateResponseWithFormik } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, TargetType, InputErrorCssClass, PostRequestHeaders } from '../../constants';
import { validateForm } from './common-functions';

export default class CreatePeriod extends Component {
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

    onSubmit = (values, { setErrors, setSubmitting }) => {
        const dateFormat = 'YYYY-MM-DD';
        axios({
            url: resolveClientUrl(`period/Create`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                name: values.name,
                targetTypeId: values.targetTypeId,
                startDate: values.startDate ? values.startDate.format(dateFormat) : null,
                endDate: values.endDate ? values.endDate.format(dateFormat) : null,
                disciplineStartDate: values.disciplineStartDate ? values.disciplineStartDate.format(dateFormat) : null,
                disciplineEndDate: values.disciplineEndDate ? values.disciplineEndDate.format(dateFormat) : null,
                focusStartDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusStartDate.format(dateFormat) : null,
                focusEndDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusEndDate.format(dateFormat) : null,
                description: values.description,
                isActive: values.isActive
            }),
            headers: PostRequestHeaders
        }).then(response => {
            axiosCreateResponseWithFormik(setErrors, toast, response, () => {
                this.props.onCreateSuccess();
                this.props.closeModal();
            });
        }).catch(axiosError).then(() => setSubmitting(false));
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        const onChangeRaw = event => {
            const date = moment(Date(event.target.value));
            if (date.isValid()) {
                setFieldValue(event.target.name, date);
            }
        };
        const isValidName = validateFormikField(touched, errors, 'name'),
            isValidStartDate = validateFormikField(touched, errors, 'startDate'),
            isValidEndDate = validateFormikField(touched, errors, 'endDate'),
            isValidDisciplineStartDate = validateFormikField(touched, errors, 'disciplineStartDate'),
            isValidDisciplineEndDate = validateFormikField(touched, errors, 'disciplineEndDate'),
            isValidFocusStartDate = validateFormikField(touched, errors, 'focusStartDate'),
            isValidFocusEndDate = validateFormikField(touched, errors, 'focusEndDate'),
            isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form inline onSubmit={handleSubmit} className="evaluation-period-form">
                <Modal.Body>
                    {
                        displayFormikErrors(
                            touched,
                            errors,
                            ['name', 'startDate', 'endDate', 'disciplineStartDate', 'disciplineEndDate', 'focusStartDate', 'focusEndDate', 'description'])
                    }
                    <div className="line-group">
                        <FormGroup>
                            <label className="control-label">{getResource('Name')}</label>
                            <div className="control-content name-content">
                                <input type="text" name="name" value={values.name} autoComplete="off"
                                    className={`form-control ${!isValidName && InputErrorCssClass}`}
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
                                    className={`form-control ${!isValidStartDate && InputErrorCssClass}`}
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
                                    className={`form-control ${!isValidEndDate && InputErrorCssClass}`}
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
                                    className={`form-control ${!isValidDisciplineStartDate && InputErrorCssClass}`}
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
                                    className={`form-control ${!isValidDisciplineEndDate && InputErrorCssClass}`}
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
                                        className={`form-control ${!isValidFocusStartDate && InputErrorCssClass}`}
                                        dateFormat={DateFormat} selected={values.focusStartDate}
                                        onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                    />
                                </Col>
                            </FormGroup>
                            <FormGroup>
                                <Col componentClass={ControlLabel} className="">Focus End Date</Col>
                                <Col className="control-content">
                                    <DatePicker name="disciplineEndDate" autoComplete="off"
                                        className={`form-control  ${!isValidFocusEndDate && InputErrorCssClass}`}
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
                                <input type="checkbox" className="checkbox-custom" name="isActive" onChange={handleChange} checked={values.isActive} id="chb-isActive" />
                                <label htmlFor="chb-isActive">Active</label>
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup className="description-group">
                            <Col componentClass={ControlLabel}>{getResource('Description')} (optional)</Col>
                            <Col className="">
                                <textarea className={`form-control ${!isValidDescription && InputErrorCssClass}`}
                                    name="description" rows="4"
                                    onChange={handleChange} value={values.description}
                                />
                            </Col>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary" disabled={isSubmitting}>{getResource('Submit')}</Button>
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
                <Formik initialValues={this.initialObj} validate={validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}