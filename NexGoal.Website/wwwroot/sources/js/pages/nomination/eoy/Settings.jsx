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

import { axiosError, resolveClientUrl, validateFormikField } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import { validateEOYForm } from '../common';
import { DateFormat, InputErrorCssClass, PostRequestHeaders } from '../../../constants';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoy: {
                eoqyId: 0,
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

    getEOQById() {
        const eoyId = document.getElementById('EOYId').value;

        axios.get(resolveClientUrl(`EOY/GetById/${eoyId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    eoy: {
                        eoqyId: data.eoqyId,
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
        const eoy = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            eoyId: this.state.eoy.eoqyId,
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        });

        axios({
            url: resolveClientUrl('EOY/Edit'),
            method: 'POST',
            data: eoy,
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
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
        const isHost = this.isHost;
        const yearOptions = [
            { value: values.year, label: values.year }
        ];
        const isValidYear = validateFormikField(touched, errors, 'year'),
            isValidStartDateForMominator = validateFormikField(touched, errors, 'startDateForNominator'),
            isValidEndDateForNominator = validateFormikField(touched, errors, 'endDateForNominator'),
            isValidRemindDateForNominator = validateFormikField(touched, errors, 'remindDateForNominator'),
            isValidEndDateForCommittee = validateFormikField(touched, errors, 'endDateForCommittee'),
            isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form id="edit-eoy-form" inline className="settings-form" onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {!isValidYear && <li>{errors.year}</li>}
                        {!isValidStartDateForMominator && <li>{errors.startDateForNominator}</li>}
                        {!isValidEndDateForNominator && <li>{errors.endDateForNominator}</li>}
                        {!isValidRemindDateForNominator && <li>{errors.remindDateForNominator}</li>}
                        {!isValidEndDateForCommittee && <li>{errors.endDateForCommittee}</li>}
                        {!isValidDescription && <li>{errors.description}</li>}
                    </ul>
                }
                <div className="line-group">
                    <FormGroup>
                        <label className="control-label">{getResource('Year')}</label>
                        <div className="control-content field">
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
                        <div className="control-content field">
                            <DatePicker name="startDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${!isValidStartDateForMominator && InputErrorCssClass}`}
                                dateFormat={DateFormat}
                                selected={values.startDateForNominator}
                                onChange={value => setFieldValue('startDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="control-label">{getResource('EndDate')}</label>
                        <div className="control-content field">
                            <DatePicker name="endDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${!isValidEndDateForNominator && InputErrorCssClass}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForNominator}
                                onChange={value => setFieldValue('endDateForNominator', value)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="control-label">{getResource('RemindDate')}</label>
                        <div className="control-content field">
                            <DatePicker name="remindDateForNominator" autoComplete="off" disabled={!isHost}
                                className={`form-control ${!isValidRemindDateForNominator && InputErrorCssClass}`}
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
                                className={`form-control ${!isValidEndDateForCommittee && InputErrorCssClass}`}
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
                            <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
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
                        <div className="field">
                            <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }

    render() {
        return (
            <Formik enableReinitialize initialValues={this.state.eoy}
                validate={validateEOYForm}
                onSubmit={this.handleSubmit}
                render={this.renderForm}
            />
        );
    }
}

export default Settings;