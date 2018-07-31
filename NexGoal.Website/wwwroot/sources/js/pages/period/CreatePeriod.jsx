import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

export default class CreatePeriod extends Component {
    constructor(props) {
        super(props);
    }

    probationTarget = 1

    semiAnualTarget = 2

    initialObj = {
        name: '',
        targetTypeId: '',
        startDate: null,
        endDate: null,
        disciplineStartDate: null,
        disciplineEndDate: null,
        focusStartDate: null,
        description: '',
        isActive: true
    }

    validateForm = values => {
        let errors = {};

        if (!values.name) {
            errors.name = 'Name is required';
        }

        if (!values.targetTypeId) {
            errors.targetTypeId = 'Target type is required';
        }

        if (!values.startDate && values.targetTypeId == this.semiAnualTarget) {
            errors.startDate = 'Start Date is required';
        }

        if (!values.endDate && values.targetTypeId == this.semiAnualTarget) {
            errors.endDate = 'End Date is required';
        }

        if (!values.disciplineStartDate && values.targetTypeId == this.semiAnualTarget) {
            errors.disciplineStartDate = 'Discipline Start Date is required';
        }

        if (!values.disciplineEndDate && values.targetTypeId == this.semiAnualTarget) {
            errors.disciplineEndDate = 'Discipline End Date is required';
        }

        if (!values.focusStartDate && values.targetTypeId == this.semiAnualTarget) {
            errors.focusStartDate = 'Focus Start Date is required';
        }

        if (values.targetTypeId == this.semiAnualTarget && values.focusStartDate && values.endDate && values.focusStartDate <= values.endDate) {
            errors.focusStartDate = 'Focus Start Date must be after Period End Date';
        }

        if (!values.description) {
            errors.description = 'Description is required';
        }

        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: `/period/Create`,
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                name: values.name,
                targetTypeId: values.targetTypeId,
                startDate: values.startDate.format('YYYY-MM-DD'),
                endDate: values.endDate.format('YYYY-MM-DD'),
                disciplineStartDate: values.disciplineStartDate.format('YYYY-MM-DD'),
                disciplineEndDate: values.disciplineEndDate.format('YYYY-MM-DD'),
                focusStartDate: values.targetTypeId == this.semiAnualTarget ? values.focusStartDate.format('YYYY-MM-DD') : null,
                description: values.description,
                isActive: values.isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Create Successfully");
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

                setSubmitting(false);
            }
        }).catch(commonFunctions.axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        var onChangeRaw = (e) => {
            let date = moment(e.target.value);
            if (date.isValid()) {
                setFieldValue(e.target.name, date);
            }
        };

        return (
            <Form horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.name && errors.name && <li>{errors.name}</li>}
                            {touched.targetTypeId && errors.targetTypeId && <li>{errors.targetTypeId}</li>}
                            {touched.startDate && errors.startDate && <li>{errors.startDate}</li>}
                            {touched.endDate && errors.endDate && <li>{errors.endDate}</li>}
                            {touched.disciplineStartDate && errors.disciplineStartDate && <li>{errors.disciplineStartDate}</li>}
                            {touched.disciplineEndDate && errors.disciplineEndDate && <li>{errors.disciplineEndDate}</li>}
                            {values.targetTypeId == this.semiAnualTarget && touched.focusStartDate && errors.focusStartDate && <li>{errors.focusStartDate}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Name</Col>
                        <Col sm={9}>
                            <input type="text" name="name" value={values.name} className={`form-control  ${touched.name && errors.name && 'input-validation-error'}`} onChange={handleChange} />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Target Type</Col>
                        <Col sm={9}>
                            <select name="targetTypeId" className={`form-control  ${touched.targetTypeId && errors.targetTypeId && 'input-validation-error'}`} onChange={handleChange}>
                                <option value=""></option>
                                {
                                    this.props.targetTypes.map(e => <option key={e.targetTypeId} value={e.targetTypeId} >{e.name}</option>)
                                }
                            </select>
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className={values.targetTypeId == this.semiAnualTarget && 'required-label'} sm={3}>Start Date</Col>
                        <Col sm={3}>
                            <DatePicker autoComplete="off" className={`form-control  ${touched.startDate && errors.startDate && 'input-validation-error'}`} maxDate={values.endDate}
                                dateFormat="DD MMM, YYYY" selected={values.startDate} onChange={value => setFieldValue('startDate', value)} onChangeRaw={onChangeRaw}
                                name="startDate" showMonthDropdown showYearDropdown />
                        </Col>
                        <Col componentClass={ControlLabel} className={values.targetTypeId == this.semiAnualTarget && 'required-label'} sm={3}>End Date</Col>
                        <Col sm={3}>
                            <DatePicker autoComplete="off" className={`form-control  ${touched.endDate && errors.endDate && 'input-validation-error'}`} minDate={values.startDate}
                                dateFormat="DD MMM, YYYY" selected={values.endDate} onChange={value => setFieldValue('endDate', value)} onChangeRaw={this.onChangeRaw}
                                name="endDate" showMonthDropdown showYearDropdown />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className={values.targetTypeId == this.semiAnualTarget && 'required-label'} sm={3}>Discipline Start Date</Col>
                        <Col sm={3}>
                            <DatePicker autoComplete="off" className={`form-control  ${touched.disciplineStartDate && errors.disciplineStartDate && 'input-validation-error'}`}
                                dateFormat="DD MMM, YYYY" selected={values.disciplineStartDate} onChange={value => setFieldValue('disciplineStartDate', value)} onChangeRaw={onChangeRaw}
                                name="disciplineStartDate" showMonthDropdown showYearDropdown />
                        </Col>
                        <Col componentClass={ControlLabel} className={values.targetTypeId == this.semiAnualTarget && 'required-label'} sm={3}>Discipline End Date</Col>
                        <Col sm={3}>
                            <DatePicker autoComplete="off" className={`form-control  ${touched.disciplineEndDate && errors.disciplineEndDate && 'input-validation-error'}`}
                                dateFormat="DD MMM, YYYY" selected={values.disciplineEndDate} onChange={value => setFieldValue('disciplineEndDate', value)} onChangeRaw={onChangeRaw}
                                name="disciplineEndDate" showMonthDropdown showYearDropdown />
                        </Col>
                    </FormGroup>
                    {values.targetTypeId == this.semiAnualTarget &&
                        <FormGroup>
                            <Col componentClass={ControlLabel} className="required-label" sm={3}>Focus Start Date</Col>
                            <Col sm={3}>
                                <DatePicker autoComplete="off" className={`form-control  ${touched.focusStartDate && errors.focusStartDate && 'input-validation-error'}`} minDate={values.endDate && moment(values.endDate).add(1, 'day')}
                                    dateFormat="DD MMM, YYYY" selected={values.focusStartDate} onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                    name="focusStartDate" showMonthDropdown showYearDropdown />
                            </Col>
                        </FormGroup>
                    }

                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={3}>Active</Col>
                        <Col sm={3}>
                            <Checkbox name="isActive" onChange={handleChange} checked={values.isActive} />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Description</Col>
                        <Col sm={9}>
                            <textarea className={`form-control  ${touched.description && errors.description && 'input-validation-error'}`}
                                name="description" onChange={handleChange} value={values.description} />
                        </Col>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>Cancel</Button>
                    <Button type="submit" disabled={isSubmitting} className="btn btn-primary">Submit</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Evaluation Period</Modal.Title>
                </Modal.Header>
                <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}