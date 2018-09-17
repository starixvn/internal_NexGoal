import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
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
        let errors = {};
        if (!values.processes.some(p => p.isSelected)) {
            errors.noSelect = "Please select at least a process";
        }

        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        let processes = [];
        let adminProcess = values.processes.find(p => p.processId === PerformanceProcess.HumanResource && p.isSelected);

        if (adminProcess) {
            let adminProcessEvaluators = adminProcess.evaluator;
            if (adminProcessEvaluators && adminProcessEvaluators.length > 0) {
                processes = adminProcessEvaluators.map(e => {
                    return {
                        processId: PerformanceProcess.HumanResource,
                        evaluatorId: e.value,
                        startDate: adminProcess.startDate ? adminProcess.startDate.format('YYYY-MM-DD') : null,
                        endDate: adminProcess.endDate ? adminProcess.endDate.format('YYYY-MM-DD') : null
                    };
                });
            } else {
                processes.push({
                    processId: PerformanceProcess.HumanResource,
                    evaluatorId: null,
                    startDate: adminProcess.startDate ? adminProcess.startDate.format('YYYY-MM-DD') : null,
                    endDate: adminProcess.endDate ? adminProcess.endDate.format('YYYY-MM-DD') : null
                });
            }
        }

        axios({
            url: resolveClientUrl(`period/EditEmployeeSetting`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.props.periodId,
                paRecordIds: this.props.paRecordIds,
                processes: processes.concat(values.processes.filter(p => p.processId > PerformanceProcess.HumanResource && p.isSelected).map(p => {
                    return {
                        processId: p.processId,
                        evaluatorId: p.evaluator ? p.evaluator.value : null,
                        startDate: p.startDate ? p.startDate.format('YYYY-MM-DD') : null,
                        endDate: p.endDate ? p.endDate.format('YYYY-MM-DD') : null
                    };
                }))
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                this.props.onEditSuccess();
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

                setSubmitting(false);
            }
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
        var onChangeRaw = (e) => {
            let date = moment(e.target.value);
            if (date.isValid()) {
                setFieldValue(e.target.name, date);
            }
        };
        const employees = this.props.settings.map(e => e.employee.nickName).join(', ');
        let numberOfPARecord = this.props.paRecordIds.length;

        return (
            <Form id="edit-employee-settings" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {errors.noSelect && <li>{errors.noSelect}</li>}
                        </ul>
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={3}>{getResource('Employee')}</Col>
                        <Col sm={9} style={{ paddingTop: '7px' }}  >{employees}</Col>
                    </FormGroup>

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
                                    {numberOfPARecord > 1 &&
                                        <td>
                                            <input type="checkbox" checked={p.isSelected} onChange={handleChange} name={`processes.${i}.isSelected`} />
                                        </td>}
                                    <td>{p.process}</td>
                                    <td>
                                        {
                                            p.processId !== PerformanceProcess.Self &&
                                            <Select name={`processes.${i}.evaluator`}
                                                classNamePrefix="react-select"
                                                className={`react-select-container ${p.processId === PerformanceProcess.HumanResource ? 'human-resource' : ''}`}
                                                value={p.evaluator} isMulti={p.processId === PerformanceProcess.HumanResource}
                                                onChange={value => setFieldValue(`processes.${i}.evaluator`, value)}
                                                options={this.props.employeeOptions} placeholder="-- Search an employee --"
                                            />
                                        }
                                    </td>
                                    <td>
                                        {
                                            (p.processId === PerformanceProcess.HumanResource || p.processId === PerformanceProcess.Reviewer) &&
                                            <DatePicker autoComplete="off" className="form-control" dateFormat={DateFormat} selected={p.startDate}
                                                onChange={value => setFieldValue(`processes.${i}.startDate`, value)} onChangeRaw={onChangeRaw}
                                                name={`processes.${i}.startDate`}
                                            />
                                        }
                                    </td>
                                    <td>
                                        {
                                            (p.processId === PerformanceProcess.HumanResource || p.processId === PerformanceProcess.Reviewer) &&
                                            <DatePicker autoComplete="off" className="form-control" dateFormat={DateFormat} selected={p.endDate}
                                                onChange={value => setFieldValue(`processes.${i}.endDate`, value)} onChangeRaw={onChangeRaw}
                                                name={`processes.${i}.endDate`}
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
                    <Button type="submit" disabled={isSubmitting} className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        const paRecordIds = this.props.settings.map(e => e.paRecordId);
        let processes = [];

        if (paRecordIds.length === 1) {
            let adminProcesses = this.props.settings[0].processes.filter(p => p.processId === PerformanceProcess.HumanResource);

            processes = [{
                isSelected: true,
                processId: 1,
                process: adminProcesses[0].process,
                evaluator: adminProcesses.map(e => {
                    return {
                        value: e.evaluator.employeeId,
                        label: `${e.evaluator.fullName} (${e.evaluator.nickName})`
                    };
                }),
                startDate: adminProcesses[0].startDate ? moment(adminProcesses[0].startDate) : null,
                endDate: adminProcesses[0].endDate ? moment(adminProcesses[0].endDate) : null
            }];

            processes = processes.concat(this.props.settings[0].processes.filter(p => p.processId > PerformanceProcess.ProjectManager)
                .map(p => {
                    return {
                        isSelected: true,
                        processId: p.processId,
                        process: p.process,
                        evaluator: {
                            value: p.evaluator.employeeId,
                            label: `${p.evaluator.fullName} (${p.evaluator.nickName})`
                        },
                        startDate: p.startDate ? moment(p.startDate) : null,
                        endDate: p.endDate ? moment(p.endDate) : null
                    };
                }));
        } else {
            processes = uniqBy(
                this.props.settings[0].processes.filter(p => p.processId !== PerformanceProcess.Self && p.processId !== PerformanceProcess.ProjectManager),
                p => p.processId);
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