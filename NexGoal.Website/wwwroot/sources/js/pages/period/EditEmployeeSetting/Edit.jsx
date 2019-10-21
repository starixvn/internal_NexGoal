import React, { Component } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty, uniqBy } from 'lodash';
import Select from 'react-select';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat, PerformanceProcess } from '../../../constants';
import getResource from '../../../resourceHelper';

export default class Edit extends Component {
    validateForm = values => {
        const errors = {};
        if (!values.processes.some(p => p.isSelected)) {
            errors.noSelect = 'Please select at least a process';
        }

        return errors;
    }

    onSubmit = (values, { setErrors }) => {
        const processes = values.processes.filter(p => p.processId > PerformanceProcess.ProjectManager && p.isSelected).map(p => {
            return {
                processId: p.processId,
                evaluatorId: p.evaluator ? p.evaluator.value : null,
                startDate: p.startDate ? p.startDate.format('YYYY-MM-DD') : null,
                endDate: p.endDate ? p.endDate.format('YYYY-MM-DD') : null
            };
        });

        axios({
            url: resolveClientUrl(`period/EditEmployeeSetting`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.props.periodId,
                paRecordIds: this.props.paRecordIds,
                processes: processes
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                this.props.onEditSuccess();
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
        const employees = this.props.settings.map(e => e.employee.nickName).join(', ');
        const numberOfPARecord = this.props.paRecordIds.length;
        const lbstyle = {
            display: 'inline-block',
            paddingLeft: '12px'
        };
        return (
            <Form id="edit-employee-settings" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {errors.noSelect && <li>{errors.noSelect}</li>}
                        </ul>
                    }
                    <div>
                        <label>{getResource('Employee')}</label>
                        <div style={lbstyle}  >{employees}</div>
                    </div>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                {numberOfPARecord > 1 && <th />}
                                <th />
                                <th className="col-sm-4">{getResource('Evaluator')}</th>
                                <th>{getResource('StartDate')}</th>
                                <th>{getResource('EndDate')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {values.processes.map((p, i) =>
                                <tr key={i}>
                                    {
                                        numberOfPARecord > 1 &&
                                        <td>
                                            <div className="checkbox">
                                                <input type="checkbox" checked={p.isSelected} onChange={handleChange}
                                                    id={`chb-processes${i}`}
                                                    className="checkbox-custom"
                                                    name={`processes.${i}.isSelected`}
                                                />
                                                <label htmlFor={`chb-processes${i}`} />
                                            </div>
                                        </td>
                                    }
                                    <td>{p.process}</td>
                                    <td>
                                        {
                                            p.processId !== PerformanceProcess.Self &&
                                            <Select name={`processes.${i}.evaluator`}
                                                classNamePrefix="react-select"
                                                className="react-select-container"
                                                value={p.evaluator}
                                                onChange={value => setFieldValue(`processes.${i}.evaluator`, value)}
                                                options={this.props.employeeOptions} placeholder="-- Search an employee --"
                                            />
                                        }
                                    </td>
                                    <td>
                                        {
                                            p.processId === PerformanceProcess.Reviewer &&
                                            <DatePicker autoComplete="off" className="form-control" dateFormat={DateFormat} selected={p.startDate}
                                                onChange={value => setFieldValue(`processes.${i}.startDate`, value)}
                                                name={`processes.${i}.startDate`} maxDate={p.endDate}
                                            />
                                        }
                                    </td>
                                    <td>
                                        {
                                            p.processId === PerformanceProcess.Reviewer &&
                                            <DatePicker autoComplete="off" className="form-control" dateFormat={DateFormat} selected={p.endDate}
                                                onChange={value => setFieldValue(`processes.${i}.endDate`, value)}
                                                name={`processes.${i}.endDate`} minDate={p.startDate}
                                            />
                                        }
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        const paRecordIds = this.props.settings.map(e => e.paRecordId);
        let processes = [];

        if (paRecordIds.length === 1) {
            processes = this.props.settings[0].processes.filter(p => p.processId > PerformanceProcess.ProjectManager).map(p => {
                return {
                    isSelected: true,
                    processId: p.processId,
                    process: p.process,
                    evaluator: {
                        value: p.evaluator.employeeId,
                        label: `${p.evaluator.nickName} (${p.evaluator.fullName})`
                    },
                    startDate: p.startDate ? moment(p.startDate) : null,
                    endDate: p.endDate ? moment(p.endDate) : null
                };
            });
        } else {
            processes = uniqBy(this.props.settings[0].processes.filter(p => p.processId > PerformanceProcess.ProjectManager), p => p.processId);
            processes = processes.map(p => {
                return {
                    isSelected: false,
                    processId: p.processId,
                    process: p.process,
                    evaluator: null,
                    startDate: null,
                    endDate: null
                };
            });
        }

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal}
                onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('EditEmployeeSetting')}</Modal.Title>
                </Modal.Header>
                <Formik enableReinitialize
                    initialValues={{ processes: processes }}
                    validate={this.validateForm}
                    onSubmit={this.onSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}