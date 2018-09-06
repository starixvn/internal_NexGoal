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
import getResource from '../../../resourceHelper';
import { validateEOYForm } from '../common';

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
            },
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getEOQById();
    }

    getEOQById() {
        const eoyId = document.getElementById('EOYId').value;

        axios.get(resolveClientUrl(`EOY/GetById/${eoyId}`))
            .then(response => {
                let data = response.data;

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
        let eoy = stringify({
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
            headers: {
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
        const isHost = this.state.isHost;
        const yearOptions = [
            { value: values.year, label: values.year }
        ];

        return (
            <Form id="edit-eoy-form" horizontal onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.year && errors.year && <li>{errors.year}</li>}
                        {touched.startDateForNominator && errors.startDateForNominator && <li>{errors.startDateForNominator}</li>}
                        {touched.endDateForNominator && errors.endDateForNominator && <li>{errors.endDateForNominator}</li>}
                        {touched.remindDateForNominator && errors.remindDateForNominator && <li>{errors.remindDateForNominator}</li>}
                        {touched.endDateForCommittee && errors.endDateForCommittee && <li>{errors.endDateForCommittee}</li>}
                        {touched.description && errors.description && <li>{errors.description}</li>}
                    </ul>
                }

                <FormGroup>
                    <label className="col-sm-2 control-label">{getResource('Year')}</label>
                    <div className="col-sm-2 field">
                        <Select value={yearOptions[0]} className="react-select-container" classNamePrefix="react-select"
                            options={yearOptions}
                            isDisabled={true}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">{getResource('StartDateForNominator')}</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.startDateForNominator}
                            onChange={value => setFieldValue('startDateForNominator', value)}
                            name="startDateForNominator"
                            className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`}
                        />
                    </div>
                    <label className="col-sm-2 control-label required-label">{getResource('EndDateForNominator')}</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForNominator}
                            onChange={value => setFieldValue('endDateForNominator', value)}
                            name="endDateForNominator"
                            className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">{getResource('RemindDateForNominator')}</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.remindDateForNominator}
                            onChange={value => setFieldValue('remindDateForNominator', value)}
                            name="remindDateForNominator"
                            className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && 'input-validation-error'}`}
                        />
                    </div>
                    <label className="col-sm-2 control-label required-label">{getResource('EndDateForCommittee')}</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForCommittee}
                            onChange={value => setFieldValue('endDateForCommittee', value)}
                            name="endDateForCommittee"
                            className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">{getResource('Description')}</label>
                    <div className="col-sm-6 field">
                        <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                            rows="3" maxLength="200"
                            name="description" value={values.description || ''}
                            onChange={handleChange} disabled={isHost ? false : true}
                        />
                    </div>
                </FormGroup>
                {isHost &&
                    <FormGroup>
                        <label className="col-sm-2 control-label" />
                        <div className="col-sm-6 field">
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