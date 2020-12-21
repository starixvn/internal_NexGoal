import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl, validateFormikField, displayFormikErrors, axiosEditResponseWithFormik } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, TargetType, InputErrorCssClass, PostRequestHeaders } from '../../constants';
import { validateForm } from './common-functions';

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
                const data = response.data;
                data.startDate = data.startDate ? moment(data.startDate) : null;
                data.endDate = data.endDate ? moment(data.endDate) : null;
                data.disciplineStartDate = data.disciplineStartDate ? moment(data.disciplineStartDate) : null;
                data.disciplineEndDate = data.disciplineEndDate ? moment(data.disciplineEndDate) : null;
                data.focusStartDate = data.focusStartDate ? moment(data.focusStartDate) : null;
                data.focusEndDate = data.focusEndDate ? moment(data.focusEndDate) : null;
                this.setState({ basicInformation: data });
            }).catch(axiosError);
    };

    onSubmit = (values, { setErrors, setSubmitting }) => {
        const dateFormat = 'YYYY-MM-DD';

        axios({
            url: resolveClientUrl('period/edit'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: values.periodId,
                name: values.name,
                description: values.description,
                targetTypeId: values.targetTypeId,
                startDate: values.startDate ? values.startDate.format(dateFormat) : null,
                endDate: values.endDate ? values.endDate.format(dateFormat) : null,
                disciplineStartDate: values.disciplineStartDate ? values.disciplineStartDate.format(dateFormat) : null,
                disciplineEndDate: values.disciplineEndDate ? values.disciplineEndDate.format(dateFormat) : null,
                focusStartDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusStartDate.format(dateFormat) : null,
                focusEndDate: values.targetTypeId === TargetType.SemiAnnual ? values.focusEndDate.format(dateFormat) : null,
                isActive: values.isActive
            }),
            headers: PostRequestHeaders
        }).then(response => {
            axiosEditResponseWithFormik(setErrors, toast, response);
        }).catch(axiosError).then(() => setSubmitting(false));
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        const onChangeRaw = e => {
            const date = moment(Date(e.target.value));
            if (date.isValid()) {
                setFieldValue(e.target.name, date);
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
            <Form inline className="settings-form evaluation-period-form" onSubmit={handleSubmit} >
                {
                    displayFormikErrors(
                        touched,
                        errors,
                        ['name', 'startDate', 'endDate', 'disciplineStartDate', 'disciplineEndDate', 'focusStartDate', 'focusEndDate', 'description'])
                }
                <div className="line-group">
                    <FormGroup>
                        <Col componentClass={ControlLabel}>{getResource('Name')}</Col>
                        <Col className="control-content name-content">
                            <input type="text" name="name" value={values.name} autoComplete="off"
                                className={`form-control  ${!isValidName && InputErrorCssClass}`} onChange={handleChange}
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
                                dateFormat={DateFormat} selected={values.startDate}
                                className={`form-control  ${!isValidStartDate && InputErrorCssClass}`}
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
                                dateFormat={DateFormat} selected={values.endDate}
                                onChange={value => setFieldValue('endDate', value)} onChangeRaw={onChangeRaw}
                                className={`form-control  ${!isValidEndDate && InputErrorCssClass}`}
                            />
                        </Col>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <FormGroup>
                        <Col componentClass={ControlLabel} >Working From Date</Col>
                        <Col className="control-content">
                            <DatePicker name="disciplineStartDate" autoComplete="off"
                                className={`form-control  ${!isValidDisciplineStartDate && InputErrorCssClass}`}
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
                                className={`form-control  ${!isValidDisciplineEndDate && InputErrorCssClass}`}
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
                                    className={`form-control  ${!isValidFocusStartDate && InputErrorCssClass}`}
                                    dateFormat={DateFormat} selected={values.focusStartDate}
                                    onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel}>Focus End Date</Col>
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
                    <FormGroup>
                        <Col componentClass={ControlLabel}>{getResource('Description')}</Col>
                        <Col className="description-content">
                            <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
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
    };

    render() {
        if (this.state.basicInformation) {
            return (
                <Formik initialValues={this.state.basicInformation} validate={validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            );
        } else {
            return '';
        }
    }
}