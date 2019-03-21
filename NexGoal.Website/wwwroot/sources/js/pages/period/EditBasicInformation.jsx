import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
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

export default class EditBasicInformation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            basicInformation: null
        };
    }

    componentDidMount() {
        this.getPeriod();
    }

    getPeriod = () => {
        axios.get(resolveClientUrl('Period/GetById/' + this.props.periodId))
            .then(response => {
                var data = response.data;
                data.startDate = data.startDate ? moment(data.startDate) : null;
                data.endDate = data.endDate ? moment(data.endDate) : null;
                data.disciplineStartDate = data.disciplineStartDate ? moment(data.disciplineStartDate) : null;
                data.disciplineEndDate = data.disciplineEndDate ? moment(data.disciplineEndDate) : null;
                data.focusStartDate = data.focusStartDate ? moment(data.focusStartDate) : null;
                data.focusEndDate = data.focusEndDate ? moment(data.focusEndDate) : null;
                this.setState({ basicInformation: data });
            }).catch(axiosError);
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

            if (values.startDate && values.endDate && values.startDate.isAfter(values.endDate)) {
                errors.startDate = 'Start Date must be before End Date';
            }

            if (!values.disciplineStartDate) {
                errors.disciplineStartDate = 'Discipline Start Date is required';
            }

            if (!values.disciplineEndDate) {
                errors.disciplineEndDate = 'Discipline End Date is required';
            }

            if (values.disciplineStartDate && values.disciplineEndDate && values.disciplineStartDate.isAfter(values.disciplineEndDate)) {
                errors.disciplineStartDate = 'Discipline Start Date must be before Discipline End Date';
            }

            if (!values.focusStartDate) {
                errors.focusStartDate = 'Focus Start Date is required';
            }

            if (!values.focusEndDate) {
                errors.focusEndDate = 'Focus End Date is required';
            }

            if (values.focusStartDate && values.focusEndDate && values.focusStartDate.isAfter(values.focusEndDate)) {
                errors.focusStartDate = 'Focus Start Date must be before Focus End Date';
            }
        }

        return errors;
    }

    onSubmit = (values, { setErrors, setSubmitting }) => {
        axios({
            url: resolveClientUrl('period/edit'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: values.periodId,
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
                toast.success(getResource('UpdatedSuccessfully'));
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
        var onChangeRaw = e => {
            let date = moment(Date(e.target.value));
            if (date.isValid()) {
                setFieldValue(e.target.name, date);
            }
        };

        return (
            <Form inline className="settings-form evaluation-period-form" onSubmit={handleSubmit} >
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
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
                        <Col componentClass={ControlLabel}>{getResource('Name')}</Col>
                        <Col className="control-content name-content">
                            <input type="text" name="name" value={values.name} autoComplete="off"
                                className={`form-control  ${touched.name && errors.name && 'input-validation-error'}`} onChange={handleChange}
                            />
                        </Col>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <FormGroup>
                        <Col componentClass={ControlLabel} >
                            {getResource('EvaluationStartDate')}
                        </Col>
                        <Col className="control-content">
                            <DatePicker name="startDate" autoComplete="off"
                                className={`form-control  ${touched.startDate && errors.startDate && 'input-validation-error'}`}
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
                                dateFormat={DateFormat} selected={values.endDate}
                                onChange={value => setFieldValue('endDate', value)} onChangeRaw={onChangeRaw}
                            />
                        </Col>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <FormGroup>
                        <Col componentClass={ControlLabel} >Working From Date</Col>
                        <Col className="control-content">
                            <DatePicker name="disciplineStartDate" autoComplete="off"
                                className={`form-control  ${touched.disciplineStartDate && errors.disciplineStartDate && 'input-validation-error'}`}
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
                                dateFormat={DateFormat} selected={values.disciplineEndDate}
                                onChange={value => setFieldValue('disciplineEndDate', value)} onChangeRaw={onChangeRaw}
                            />
                        </Col>
                    </FormGroup>
                </div>
                {values.targetTypeId === TargetType.SemiAnnual &&
                    <div className="line-group">
                        <FormGroup>
                            <Col componentClass={ControlLabel}>Focus Start Date</Col>
                            <Col className="control-content">
                                <DatePicker name="focusStartDate" autoComplete="off"
                                    className={`form-control  ${touched.focusStartDate && errors.focusStartDate && 'input-validation-error'}`}
                                    dateFormat={DateFormat} selected={values.focusStartDate}
                                    onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel}>Focus End Date</Col>
                            <Col className="control-content">
                                <DatePicker name="disciplineEndDate" autoComplete="off"
                                    className={`form-control  ${touched.focusEndDate && errors.focusEndDate && 'input-validation-error'}`}
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
                    <FormGroup>
                        <Col componentClass={ControlLabel}>{getResource('Description')}</Col>
                        <Col className="description-content">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                name="description" rows="4"
                                onChange={handleChange} value={values.description}
                            />
                        </Col>
                    </FormGroup>
                </div>
                <FormGroup>
                    <Col>
                        <Button type="submit" className="btn btn-primary" disabled={isSubmitting}>{getResource('Submit')}</Button>
                    </Col>
                </FormGroup>
            </Form>
        );
    }

    render() {
        if (this.state.basicInformation) {
            return (
                <Formik initialValues={this.state.basicInformation} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            );
        } else {
            return '';
        }
    }
}