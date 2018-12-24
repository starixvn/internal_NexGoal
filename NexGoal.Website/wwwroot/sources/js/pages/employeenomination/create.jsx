import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';

export default class Create extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeOptions: [],
            nominationPeriodOptions: [],
        };
    }

    initialObj = {
        period: null,
        employee: null,
        comment: ''
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
        let errors = {};

        if (!values.period || Array.isArray(values.period)) {
            errors.period = 'Period is required!';
        }

        if (!values.employee || Array.isArray(values.employee)) {
            errors.employee = 'Employee is required!';
        }

        return errors;
    }

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
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <form onSubmit={handleSubmit}>
                {!isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.period && errors.period && <li>{errors.period}</li>}
                        {touched.employee && errors.employee && <li>{errors.employee}</li>}
                    </ul>
                }

                <FormGroup>
                    <ControlLabel className="required-label" >{getResource('NominationPeriod')}</ControlLabel>
                    <Select name="period" className={`react-select-container ${touched.period && errors.period && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.period} onChange={value => setFieldValue('period', value)}
                        options={this.state.nominationPeriodOptions}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel className="required-label">{getResource('Employee')}</ControlLabel>
                    <Select name="employee" className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.employee} onChange={value => setFieldValue('employee', value)}
                        options={this.state.employeeOptions}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Comment')}</ControlLabel>
                    <textarea className="form-control" name="comment" value={values.comment} onChange={handleChange} />
                </FormGroup>

                <button type="submit" className="btn btn-primary">{getResource('Submit')}</button>
            </form>
        );
    }

    render() {
        return (
            <Fragment>
                <ToastLayout />
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Create new nomination</li>
                    </Breadcrumb>
                    <div className="col-sm-6">
                        <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.handleSubmit} render={this.renderForm} />
                    </div>
                </div>
            </Fragment>
        );
    }
}

ReactDOM.render(<Create />, document.getElementById('nomination-wrapper'));