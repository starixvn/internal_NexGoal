import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl, validateFormikField, axiosCreateResponseWithFormik } from '../../common-functions';
import { PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';

export default class Create extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeOptions: [],
            nominationPeriodOptions: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetEmployees')),
            axios(resolveClientUrl('EmployeeNomination/GetNominationPeriodsForNominator'))
        ];

        Promise.all(promises)
            .then(([employeeResponse, nominationPeriodResponse]) => {
                this.setState({
                    employeeOptions: employeeResponse.data.map(employee => {
                        return { value: employee.employeeId, label: `${employee.nickName} (${employee.fullName})` };
                    }),
                    nominationPeriodOptions: nominationPeriodResponse.data.map(np => {
                        return { value: np.nominationPeriodId, label: np.title };
                    })
                });
            }).catch(axiosError);
    }

    validateForm = values => {
        const errors = {};

        if (!values.period || Array.isArray(values.period)) {
            errors.period = 'Period is required!';
        }

        if (!values.employee || Array.isArray(values.employee)) {
            errors.employee = 'Employee is required!';
        }

        return errors;
    };

    handleSubmit = (values, { setErrors }) => {
        axios({
            url: resolveClientUrl(`EmployeeNomination/Create`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                nominationPeriodId: values.period.value,
                nomineeId: values.employee.value,
                comment: values.comment
            }),
            headers: PostRequestHeaders
        }).then(response => {
            axiosCreateResponseWithFormik(setErrors, toast, response);
        }).catch(axiosError);
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        const isValidPeriod = validateFormikField(touched, errors, 'period');
        const isValidEmployee = validateFormikField(touched, errors, 'employee');

        return (
            <form onSubmit={handleSubmit} >
                {!isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {!isValidPeriod && <li>{errors.period}</li>}
                        {!isValidEmployee && <li>{errors.employee}</li>}
                    </ul>
                }

                <FormGroup>
                    <ControlLabel className="required-label" >{getResource('NominationPeriod')}</ControlLabel>
                    <Select name="period" className={`react-select-container ${!isValidPeriod && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.period} onChange={value => setFieldValue('period', value)}
                        options={this.state.nominationPeriodOptions}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel className="required-label">{getResource('Employee')}</ControlLabel>
                    <Select name="employee" className={`react-select-container ${!isValidEmployee && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.employee} onChange={value => setFieldValue('employee', value)}
                        options={this.state.employeeOptions}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Comment')}</ControlLabel>
                    <textarea className="form-control" name="comment" value={values.comment} onChange={handleChange} />
                </FormGroup>

                <button type="submit" className="btn btn-primary">{getResource('Submit')}</button>
            </form >
        );
    };

    render() {
        const initialObj = {
            period: null,
            employee: null,
            comment: ''
        };

        return (
            <Fragment>
                <ToastLayout />
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Create new nomination</li>
                    </Breadcrumb>
                    <div className="col-sm-6">
                        <Formik initialValues={initialObj} validate={this.validateForm} onSubmit={this.handleSubmit} render={this.renderForm} />
                    </div>
                </div>
            </Fragment>
        );
    }
}

ReactDOM.render(<Create />, document.getElementById('nomination-wrapper'));