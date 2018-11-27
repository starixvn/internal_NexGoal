import React, { Component } from 'react';
import { FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../../constants';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';

export default class CreateNomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            openForm: false,
            nominationPeriodOptions: props.nominationPeriods.map(np => {
                return { value: np.nominationPeriodId, label: np.title };
            }),
            currentDate: moment(),
            eoqQuestions: [],
            eoyQuestions: [],
            initialObj: {
                period: null,
                employee: null,
                comment: ''
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetEOQQuestions')),
            axios(resolveClientUrl('Common/GetEOYQuestions'))
        ];

        Promise.all(promises)
            .then(([eoqResponse, eoyResponse]) => {
                this.setState({
                    eoqQuestions: eoqResponse.data,
                    eoyQuestions: eoyResponse.data
                });
            }).catch(axiosError);
    }

    handleOpenForm = () => {
        this.setState({ openForm: true });
    }

    handleCloseForm = () => {
        this.setState({ openForm: false });
    }

    validateForm = values => {
        let errors = {};

        if (!values.period) {
            errors.period = 'Period is required!';
        }

        if (!values.employee && this.props.isEmployeeSelectable) {
            errors.employee = 'Employee is required!';
        }

        if (!values.comment) {
            errors.comment = 'Comment is required!';
        }

        return errors;
    }

    handleSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: resolveClientUrl(`MemberNomination/Create`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                nominationPeriodId: values.period.value,
                nomineeId: this.props.isEmployeeSelectable ? values.employee.value : this.props.nomineeId,
                comment: values.comment
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                this.handleCloseForm();
                this.props.onCreateSuccess(data.objectIds);
                toast.success(getResource('CreatedSuccessfully'));
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
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        submitForm,
        isSubmitting }) => {
        let onPeriodChange = (value) => {
            var period = this.props.nominationPeriods.find(x => x.nominationPeriodId === value.value);
            let questions = '';
            if (period.quarter) {
                questions = this.state.eoqQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            } else {
                questions = this.state.eoyQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            }

            setFieldValue('period', value);
            setFieldValue('comment', questions);
        };
        return (
            <form onSubmit={handleSubmit} className="form-filter form-performance create">
                <FormGroup>
                    <div className="nomination-period">
                        <ControlLabel className="required-label" >{getResource('NominationPeriod')}</ControlLabel>
                        <Select name="period" className={`react-select-container ${touched.period && errors.period && 'input-validation-error'}`} classNamePrefix="react-select"
                            value={values.period} onChange={onPeriodChange}
                            options={this.state.nominationPeriodOptions}
                        />
                    </div>
                    {
                        this.props.isEmployeeSelectable &&
                        <div className="employee">
                            <ControlLabel className="required-label">{getResource('Employee')}</ControlLabel>
                            <Select name="employee" className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`} classNamePrefix="react-select"
                                value={values.employee} onChange={value => setFieldValue('employee', value)}
                                options={this.props.employeeOptions}
                            />
                        </div>
                    }
                </FormGroup>
                <FormGroup>
                    <TextareaFullscreenEditor
                        className={`form-control comment-field ${touched.comment && errors.comment && 'input-validation-error'}`}
                        name="comment"
                        value={values.comment}
                        onChange={handleChange}
                    />
                </FormGroup>
                <FormGroup>
                    <Button className="btn-cancel" onClick={this.handleCloseForm}>{getResource('Cancel')}</Button>
                    <Button className="btn-primary" disabled={isSubmitting} type="submit">{getResource('Publish')}</Button>
                </FormGroup>
            </form>
        );
    }

    render() {
        let currentUser = this.props.currentUser;

        return (
            <div className="record record-your-mind">
                <div className="avatar-user">
                    <img src={currentUser.avatar} className="avatar" />
                    <div className="person-info">
                        <span className="role-member">{currentUser.nickName}</span>
                        <div className="submit-date">{this.state.currentDate.format(DateFormat)}</div>
                    </div>
                </div>
                <div className="comment">
                    {
                        !this.state.openForm &&
                        <input className="form-control"
                            style={{ height: 50, marginBottom: 15 }}
                            placeholder="What's on your mind?"
                            onClick={this.handleOpenForm}
                        />
                    }
                    {
                        this.state.openForm &&
                        <Formik
                            initialValues={this.state.initialObj}
                            validate={this.validateForm}
                            onSubmit={this.handleSubmit}
                            render={this.renderForm}
                        />
                    }
                </div>
            </div>
        );
    }
}