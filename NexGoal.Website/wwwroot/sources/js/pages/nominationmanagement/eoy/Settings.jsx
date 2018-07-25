import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase } from 'lodash';

import commonFunctions from '../../../common-functions';
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

    componentWillMount() {
        this.getEOQById();
    }

    getEOQById() {
        const eoyId = document.getElementById('EOYId').value;

        axios.get(commonFunctions.getRootUrl() + `EOY/GetById/${eoyId}`)
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
            }).catch(commonFunctions.axiosError);
    }

    handleSubmit = (values, { setErrors }) => {
        let eoy = {
            eoyId: this.state.eoy.eoqyId,
            year: values.year,
            startDateForNominator: values.startDateForNominator.toDate(),
            endDateForNominator: values.endDateForNominator.toDate(),
            remindDateForNominator: values.remindDateForNominator.toDate(),
            endDateForCommittee: values.endDateForCommittee.toDate(),
            description: values.description
        };

        let formData = commonFunctions.objectToFormData(eoy);
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);

        axios({
            url: commonFunctions.getRootUrl() + 'EOY/Edit',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
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
        const yearOptions = [
            { value: values.year, label: values.year }
        ];

        return (
            <Form id="edit-eoy-form" horizontal onSubmit={handleSubmit}>
                {
                    ((touched.year && errors.year)
                        || (touched.startDateForNominator && errors.startDateForNominator)
                        || (touched.endDateForNominator && errors.endDateForNominator)
                        || (touched.remindDateForNominator && errors.remindDateForNominator)
                        || (touched.endDateForCommittee && errors.endDateForCommittee)
                        || (touched.description && errors.description)) &&
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
                            className={touched.startDateForNominator && errors.startDateForNominator ? 'form-control input-validation-error' : 'form-control'} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForNominator}
                            onChange={value => setFieldValue('endDateForNominator', value)}
                            name="endDateForNominator"
                            className={touched.endDateForNominator && errors.endDateForNominator ? 'form-control input-validation-error' : 'form-control'} />
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
                            className={touched.remindDateForNominator && errors.remindDateForNominator ? 'form-control input-validation-error' : 'form-control'} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Committee</label>
                    <div className="col-sm-2 field">
                        <DatePicker autoComplete="off" disabled={!isHost}
                            dateFormat="DD MMM, YYYY"
                            selected={values.endDateForCommittee}
                            onChange={value => setFieldValue('endDateForCommittee', value)}
                            name="endDateForCommittee"
                            className={touched.endDateForCommittee && errors.endDateForCommittee ? 'form-control input-validation-error' : 'form-control'} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Description</label>
                    <div className="col-sm-6 field">
                        <textarea className={touched.description && errors.description ? 'form-control none-resize input-validation-error' : 'form-control none-resize'}
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
            <Formik enableReinitialize initialValues={this.state.eoy}
                validate={validateEOYForm}
                onSubmit={this.handleSubmit}
                render={this.renderForm} />
        );
    }
}

export default Settings;