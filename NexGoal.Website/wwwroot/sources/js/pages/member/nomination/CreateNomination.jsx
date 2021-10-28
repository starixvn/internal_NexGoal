import React, { Component } from 'react';
import { FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { InputErrorCssClass, PostRequestHeaders } from '../../../constants';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';

export default class CreateNomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            openForm: false,
            eoqQuestions: [],
            eoyQuestions: []
        };

        this.initialObj = {
            period: null,
            employee: null,
            comment: ''
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetEOQQuestions')),
            axios(resolveClientUrl('Common/GetEOYQuestions'))
        ];

        Promise.all(promises).then(([eoqResponse, eoyResponse]) => {
            this.setState({
                eoqQuestions: eoqResponse.data,
                eoyQuestions: eoyResponse.data
            });
        }).catch(axiosError);

        window.addEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    onLeavingPageEventListener = event => {
        if (this.refs.form) {
            const isDirty = this.refs.form.getFormikComputedProps().dirty;
            if (isDirty) {
                event.returnValue = ' ';
                return ' ';
            }
        }

        return null;
    };

    handleOpenForm = () => {
        this.setState({ openForm: true });
    };

    handleCloseForm = () => {
        this.setState({ openForm: false });
    };

    handleSubmit = (values, { setErrors, setSubmitting }) => {
        axios({
            url: resolveClientUrl(`MemberNomination/Create`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                nominationPeriodId: values.period.value,
                nomineeId: this.props.isEmployeeSelectable ? values.employee.value : this.props.nomineeId,
                comment: values.comment.trim()
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                this.handleCloseForm();
                this.props.onCreateSuccess(data.objectIds);
                toast.success(getResource('CreatedSuccessfully'));
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
        }).catch(axiosError).then(() => setSubmitting(false));
    };

    validateForm = values => {
        const errors = {};

        if (!values.period) {
            errors.period = 'Period is required!';
        }

        if (!values.employee && this.props.isEmployeeSelectable) {
            errors.employee = 'Employee is required!';
        }

        if (!values.comment || values.comment.trim() === '') {
            errors.comment = 'Comment is required!';
        }

        return errors;
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const onPeriodCreateNominationChange = value => {
            const currentPeriod = this.props.nominationPeriodOptions.find(x => x.value === value.value);
            let questionData = '';
            if (currentPeriod.quarter) {
                questionData = this.state.eoqQuestions.map(e => `${e.resourceId}. ${e.value}`).join('\r\n\r\n');
            } else {
                questionData = this.state.eoyQuestions.map(e => `${e.value}`).join('\r\n\r\n');
            }

            setFieldValue('period', value);
            setFieldValue('comment', questionData);
        };

        return (
            <form onSubmit={handleSubmit} className="form-filter form-performance create">
                <FormGroup>
                    <div className="nomination-period">
                        <ControlLabel>{getResource('NominationPeriod')}</ControlLabel>
                        <Select name="period"
                            classNamePrefix="react-select"
                            className={`react-select-container ${touched.period && errors.period && InputErrorCssClass}`}
                            value={values.period} onChange={onPeriodCreateNominationChange}
                            options={this.props.nominationPeriodOptions}
                        />
                    </div>
                    {
                        this.props.isEmployeeSelectable &&
                        <div className="employee">
                            <ControlLabel>{getResource('Employee')}</ControlLabel>
                            <Select name="employee"
                                classNamePrefix="react-select"
                                className={`react-select-container ${touched.employee && errors.employee && InputErrorCssClass}`}
                                value={values.employee} onChange={value => setFieldValue('employee', value)}
                                options={this.props.employeeOptions}
                            />
                        </div>
                    }
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Comment')}</ControlLabel>
                    <TextareaFullscreenEditor
                        className={`form-control comment-field ${touched.comment && errors.comment && InputErrorCssClass}`}
                        name="comment"
                        value={values.comment}
                        onChange={handleChange}
                    />
                </FormGroup>
                <FormGroup className="text-right">
                    <Button className="btn-cancel" onClick={this.handleCloseForm}>{getResource('Cancel')}</Button>
                    <Button className="btn-primary" type="submit" disabled={isSubmitting}>{getResource('Publish')}</Button>
                </FormGroup>
            </form>
        );
    };

    render() {
        const currentUser = this.props.currentUser;

        return (
            <div className="record record-your-mind">
                <div className="avatar-user">
                    <img src={currentUser.avatar} className="avatar" />
                    <div className="person-info">
                        <span className="role-member">{currentUser.nickName}</span>
                    </div>
                </div>
                <div className="comment">
                    {
                        !this.state.openForm &&
                        <input className="form-control"
                            style={{ height: 50 }}
                            placeholder="Who has contributed the most to the company, recently? Vote for them now!"
                            onClick={this.handleOpenForm}
                        />
                    }
                    {
                        this.state.openForm &&
                        <Formik ref="form"
                            initialValues={this.initialObj}
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