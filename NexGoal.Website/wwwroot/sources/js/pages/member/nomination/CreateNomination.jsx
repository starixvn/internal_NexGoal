import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';
import moment from 'moment'

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat } from '../../../constants';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';

export default class CreateNomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            openForm: false,
            nominationPeriodOptions: [],
            currentDate: moment(),
        };
    }

    initialObj = {
        period: null,
        employee: null,
        comment: ''
    }

    handleOpenForm = () => {
        this.setState({ openForm: true });
    }

    handleCancel = () => {
        this.setState({ openForm: false });
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('EmployeeNomination/GetNominationPeriodsForNominator'))
        ];

        Promise.all(promises)
            .then(([nominationPeriodResponse]) => {
                this.setState({
                    nominationPeriodOptions: nominationPeriodResponse.data.map(np => {
                        return { value: np.nominationPeriodId, label: np.title }
                    })
                });
            }).catch(axiosError);
    }

    validateForm = values => {
        let errors = {};

        if (!values.period || Array.isArray(values.period)) {
            errors.period = 'Period is required!';
        }

        if ((!values.employee || Array.isArray(values.employee)) && this.props.isEmployeeSelectable) {
            errors.employee = 'Employee is required!';
        }

        return errors;
    }

    handleSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: resolveClientUrl(`EmployeeNomination/Create`),
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
        isSubmitting }) => {
        return (
            <form onSubmit={handleSubmit} className="form-filter form-performance create">
                <FormGroup>
                    <ControlLabel className="required-label" >{getResource('NominationPeriod')}</ControlLabel>
                    <Select name="period" className={`react-select-container ${touched.period && errors.period && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.period} onChange={value => setFieldValue('period', value)}
                        options={this.state.nominationPeriodOptions}
                    />
                </FormGroup>
                {
                    this.props.isEmployeeSelectable &&
                    <FormGroup>
                        <ControlLabel className="required-label">{getResource('Employee')}</ControlLabel>
                        <Select name="employee" className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`} classNamePrefix="react-select"
                            value={values.employee} onChange={value => setFieldValue('employee', value)}
                            options={this.props.employeeOptions}
                        />
                    </FormGroup>
                }
                <FormGroup>
                    <ControlLabel>{getResource('Comment')}</ControlLabel>
                    <TextareaFullscreenEditor
                        className="form-control comment-field"
                        name="comment"
                        value={values.comment}
                        onChange={handleChange} />
                </FormGroup>
                <FormGroup>
                    <Button className="btn-cancel" onClick={this.handleCancel}>{getResource('Cancel')}</Button>
                    <Button className="btn-primary" disabled={isSubmitting} type="submit">{getResource('Submit')}</Button>
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
                        <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.handleSubmit} render={this.renderForm} />
                    }
                </div>
            </div>
        );
    }
}