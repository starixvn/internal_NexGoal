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

import commonFunctions from '../../../common-functions';
import { quarterOptions, validateEOQForm } from '../common';

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
            },
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getEOQById();
    }

    getEOQById() {
        let eoqId = document.getElementById('EOQId').value;
        axios.get(commonFunctions.getRootUrl() + `EOQ/GetById/${eoqId}`)
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
            }).catch(commonFunctions.axiosError);
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
            url: commonFunctions.getRootUrl() + 'EOQ/Edit',
            method: 'POST',
            data: eoq,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');
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
        }).catch(commonFunctions.axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        const isHost = this.state.isHost;
        const quarterSelected = quarterOptions.find((option) => { return option.value === values.quarter; });
        const yearOptions = [
            { value: values.year, label: values.year }
        ];

        return (
            <Form id="edit-eoq-form" horizontal onSubmit={handleSubmit}>
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

                <FormGroup>
                    <label className="col-sm-2 control-label">Quarter</label>
                    <div className="col-sm-2 field">
                        <Select value={quarterSelected}
                            options={quarterOptions}
                            disabled />
                    </div>
                    <label className="col-sm-2 control-label">Year</label>
                    <div className="col-sm-2 field">
                        <Select value={yearOptions[0]}
                            options={yearOptions}
                            disabled />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Start Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.startDateForNominator}
                            onChange={value => setFieldValue('startDateForNominator', value)}
                            name="startDateForNominator"
                            className={`form-control ${touched.startDateForNominator && errors.startDateForNominator && 'input-validation-error'}`} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForNominator}
                            onChange={value => setFieldValue('endDateForNominator', value)}
                            name="endDateForNominator"
                            className={`form-control ${touched.endDateForNominator && errors.endDateForNominator && 'input-validation-error'}`} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Remind Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.remindDateForNominator}
                            onChange={value => setFieldValue('remindDateForNominator', value)}
                            name="remindDateForNominator"
                            className={`form-control ${touched.remindDateForNominator && errors.remindDateForNominator && 'input-validation-error'}`} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Committee</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForCommittee}
                            onChange={value => setFieldValue('endDateForCommittee', value)}
                            name="endDateForCommittee"
                            className={`form-control ${touched.endDateForCommittee && errors.endDateForCommittee && 'input-validation-error'}`} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Description</label>
                    <div className="col-sm-6 field">
                        <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                            rows="3" maxLength="200"
                            name="description" value={values.description || ''}
                            onChange={handleChange} disabled={isHost ? false : true} />
                    </div>
                </FormGroup>
                {isHost &&
                    <FormGroup>
                        <label className="col-sm-2 control-label"></label>
                        <div className="col-sm-6 field">
                            <Button type="submit" className="btn btn-primary">Submit</Button>
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
                render={this.renderForm} />
        );
    }
}

export default Settings;