import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, Checkbox, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import TargetType from './TargetType';

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
        axios.get(commonFunctions.getRootUrl() + 'Period/Get/' + this.props.periodId)
            .then(response => {
                var data = response.data;
                data.startDate = data.startDate ? moment(data.startDate) : null;
                data.endDate = data.endDate ? moment(data.endDate) : null;
                data.disciplineStartDate = data.disciplineStartDate ? moment(data.disciplineStartDate) : null;
                data.disciplineEndDate = data.disciplineEndDate ? moment(data.disciplineEndDate) : null;
                data.focusStartDate = data.focusStartDate ? moment(data.focusStartDate) : null;
                this.setState({ basicInformation: data });
            }).catch(commonFunctions.axiosError);
    }

    validateForm = values => {
        let errors = {};

        if (!values.name) {
            errors.name = 'Name is required';
        }

        if (!values.startDate && values.targetTypeId === TargetType.semiAnual) {
            errors.startDate = 'Start Date is required';
        }

        if (!values.endDate && values.targetTypeId === TargetType.semiAnual) {
            errors.endDate = 'End Date is required';
        }

        if (!values.disciplineStartDate && values.targetTypeId === TargetType.semiAnual) {
            errors.disciplineStartDate = 'Discipline Start Date is required';
        }

        if (!values.disciplineEndDate && values.targetTypeId === TargetType.semiAnual) {
            errors.disciplineEndDate = 'Discipline End Date is required';
        }

        if (!values.focusStartDate && values.targetTypeId === TargetType.semiAnual) {
            errors.focusStartDate = 'Focus Start Date is required';
        }

        if (values.targetTypeId === TargetType.semiAnual && values.focusStartDate && values.endDate && values.focusStartDate <= values.endDate) {
            errors.focusStartDate = 'Focus Start Date must be after Period End Date';
        }

        if (!values.description) {
            errors.description = 'Description is required';
        }

        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: '/period/edit',
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
                focusStartDate: values.targetTypeId === TargetType.semiAnual ? values.focusStartDate.format('YYYY-MM-DD') : null,
                description: values.description,
                isActive: values.isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Update Successfully");
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

            setSubmitting(false);
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
                {!isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.name && errors.name && <li>{errors.name}</li>}
                        {touched.startDate && errors.startDate && <li>{errors.startDate}</li>}
                        {touched.endDate && errors.endDate && <li>{errors.endDate}</li>}
                        {touched.disciplineStartDate && errors.disciplineStartDate && <li>{errors.disciplineStartDate}</li>}
                        {touched.disciplineEndDate && errors.disciplineEndDate && <li>{errors.disciplineEndDate}</li>}
                        {values.targetTypeId === TargetType.semiAnual && touched.focusStartDate && errors.focusStartDate && <li>{errors.focusStartDate}</li>}
                        {touched.description && errors.description && <li>{errors.description}</li>}
                    </ul>
                }
                <FormGroup>
                    <Col componentClass={ControlLabel} className="required-label" sm={2}>Name</Col>
                    <Col sm={6}>
                        <input type="text" name="name" value={values.name} autoComplete="off"
                            className={`form-control  ${touched.name && errors.name && 'input-validation-error'}`} onChange={handleChange} />
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col componentClass={ControlLabel} sm={2}>Target Type</Col>
                    <Col sm={2}><ControlLabel>{values.targetType}</ControlLabel></Col>
                </FormGroup>
                <FormGroup>
                    <Col componentClass={ControlLabel} className={values.targetTypeId === TargetType.semiAnual && 'required-label'} sm={2}>Start Date</Col>
                    <Col sm={2}>
                        <DatePicker autoComplete="off" className={`form-control  ${touched.startDate && errors.startDate && 'input-validation-error'}`} maxDate={values.endDate}
                            dateFormat="DD MMM, YYYY" selected={values.startDate} onChange={value => setFieldValue('startDate', value)} onChangeRaw={onChangeRaw}
                            name="startDate" showMonthDropdown showYearDropdown />
                    </Col>
                    <Col componentClass={ControlLabel} className={values.targetTypeId === TargetType.semiAnual && 'required-label'} sm={2}>End Date</Col>
                    <Col sm={2}>
                        <DatePicker autoComplete="off" className={`form-control  ${touched.endDate && errors.endDate && 'input-validation-error'}`} minDate={values.startDate}
                            dateFormat="DD MMM, YYYY" selected={values.endDate} onChange={value => setFieldValue('endDate', value)} onChangeRaw={this.onChangeRaw}
                            name="endDate" showMonthDropdown showYearDropdown />
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col componentClass={ControlLabel} className={values.targetTypeId === TargetType.semiAnual && 'required-label'} sm={2}>Discipline Start Date</Col>
                    <Col sm={2}>
                        <DatePicker autoComplete="off" className={`form-control  ${touched.disciplineStartDate && errors.disciplineStartDate && 'input-validation-error'}`}
                            dateFormat="DD MMM, YYYY" selected={values.disciplineStartDate} onChange={value => setFieldValue('disciplineStartDate', value)} onChangeRaw={onChangeRaw}
                            name="disciplineStartDate" showMonthDropdown showYearDropdown />
                    </Col>
                    <Col componentClass={ControlLabel} className={values.targetTypeId === TargetType.semiAnual && 'required-label'} sm={2}>Discipline End Date</Col>
                    <Col sm={2}>
                        <DatePicker autoComplete="off" className={`form-control  ${touched.disciplineEndDate && errors.disciplineEndDate && 'input-validation-error'}`}
                            dateFormat="DD MMM, YYYY" selected={values.disciplineEndDate} onChange={value => setFieldValue('disciplineEndDate', value)} onChangeRaw={onChangeRaw}
                            name="disciplineEndDate" showMonthDropdown showYearDropdown />
                    </Col>
                </FormGroup>
                {values.targetTypeId === TargetType.semiAnual &&
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={2}>Focus Start Date</Col>
                        <Col sm={2}>
                            <DatePicker autoComplete="off" className={`form-control  ${touched.focusStartDate && errors.focusStartDate && 'input-validation-error'}`} minDate={values.endDate && moment(values.endDate).add(1, 'day')}
                                dateFormat="DD MMM, YYYY" selected={values.focusStartDate} onChange={value => setFieldValue('focusStartDate', value)} onChangeRaw={onChangeRaw}
                                name="focusStartDate" showMonthDropdown showYearDropdown />
                        </Col>
                    </FormGroup>
                }

                <FormGroup>
                    <Col componentClass={ControlLabel} sm={2}>Active</Col>
                    <Col sm={2}>
                        <Checkbox name="isActive" onChange={handleChange} checked={values.isActive} />
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col componentClass={ControlLabel} className="required-label" sm={2}>Description</Col>
                    <Col sm={6}>
                        <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                            name="description" rows="4"
                            onChange={handleChange} value={values.description} />
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col smOffset={2} sm={6}>
                        <Button type="submit" disabled={isSubmitting} className="btn btn-primary">Save</Button>
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