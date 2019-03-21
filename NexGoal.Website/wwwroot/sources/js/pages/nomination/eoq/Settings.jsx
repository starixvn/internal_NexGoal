import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { quarterOptions, validateEOQForm } from '../common';
import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoq: {
                eoqyId: 0,
                quarter: 1,
                year: new Date().getFullYear(),
                startDateForNominator: null,
                endDateForNominator: null,
                remindDateForNominator: null,
                endDateForCommittee: null,
                description: ''
            }
        };

        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        this.getEOQById();
    }

    getEOQById = () => {
        let eoqId = document.getElementById('EOQId').value;
        axios.get(resolveClientUrl(`EOQ/GetById/${eoqId}`))
            .then(response => {
                let data = response.data;

                this.setState({
                    eoq: {
                        eoqyId: data.eoqyId,
                        quarter: data.quarter,
                        year: data.year,
                        startDateForNominator: moment(data.startDateForNominator),
                        endDateForNominator: moment(data.endDateForNominator),
                        remindDateForNominator: moment(data.remindDateForNominator),
                        endDateForCommittee: moment(data.endDateForCommittee),
                        description: data.description
                    }
                });
            }).catch(axiosError);
    }

    handleSubmit = (values, { setErrors }) => {
        let eoq = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoqId: this.state.eoq.eoqyId,
            quarter: values.quarter,
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOQ/Edit'),
            method: 'POST',
            data: eoq,
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
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        const isHost = this.isHost;
        const quarterSelected = quarterOptions.find(option => option.value === values.quarter);
        const yearOptions = [
            { value: values.year, label: values.year }
        ];

        return (
            <Form id="edit-eoq-form" inline className="settings-form" onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.quarter && errors.quarter && <li>{errors.quarter}</li>}
                        {touched.year && errors.year && <li>{errors.year}</li>}
                        {touched.startDateForNominator && errors.startDateForNominator && <li>{errors.startDateForNominator}</li>}
                        {touched.endDateForNominator && errors.endDateForNominator && <li>{errors.endDateForNominator}</li>}
                        {touched.remindDateForNominator && errors.remindDateForNominator && <li>{errors.remindDateForNominator}</li>}
                        {touched.endDateForCommittee && errors.endDateForCommittee && <li>{errors.endDateForCommittee}</li>}
                        {touched.description && errors.description && <li>{errors.description}</li>}
                    </ul>
                }
                <div className="line-group">
                    <FormGroup>
                        <label className="control-label">{getResource('Quarter')}</label>
                        <div className="field control-content">
                            <Select value={quarterSelected} className="react-select-container" classNamePrefix="react-select"
                                options={quarterOptions} isDisabled
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="control-label">{getResource('Year')}</label>
                        <div className="field control-content">
                            <Select value={yearOptions[0]} className="react-select-container" classNamePrefix="react-select"
                                options={yearOptions} isDisabled
                            />
                        </div>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <div className="title-group"><strong>{getResource('ForNominator')}</strong></div>
                    <FormGroup>
                        <label className="control-label">{getResource('StartDate')}</label>
                        <div className="field control-content">
                            <DatePicker name="startDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.startDateForNominator}
                                onChange={value => setFieldValue('startDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="control-label">{getResource('EndDate')}</label>
                        <div className="field control-content">
                            <DatePicker name="endDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForNominator}
                                onChange={value => setFieldValue('endDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="control-label">{getResource('RemindDate')}</label>
                        <div className="field control-content">
                            <DatePicker name="remindDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.remindDateForNominator}
                                onChange={value => setFieldValue('remindDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <div className="title-group"><strong>{getResource('ForCommittee')}</strong></div>
                    <FormGroup>
                        <label className="control-label">{getResource('EndDate')}</label>
                        <div className="field control-content">
                            <DatePicker name="endDateForCommittee" autoComplete="off" disabled={!isHost}
                                className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForCommittee}
                                onChange={value => setFieldValue('endDateForCommittee', value)}
                            />
                        </div>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <FormGroup>
                        <label className="control-label">{getResource('Description')} (optional)</label>
                        <div className="field control-content description-content">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                rows="3" maxLength="200"
                                name="description" value={values.description || ''}
                                onChange={handleChange} disabled={isHost ? false : true}
                            />
                        </div>
                    </FormGroup>
                </div>
                {
                    isHost &&
                    <FormGroup>
                        <label className="control-label" />
                        <div className="field control-content">
                            <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }

    render() {
        return (
            <Formik enableReinitialize initialValues={this.state.eoq}
                validate={validateEOQForm}
                onSubmit={this.handleSubmit}
                render={this.renderForm}
            />
        );
    }
}

export default Settings;