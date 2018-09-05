import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik, FieldArray, Field } from 'formik';
import { camelCase, isEmpty, uniqBy } from 'lodash';
import Async from 'react-select/lib/Async';
import { axiosError, resolveClientUrl } from '../../../common-functions';

export default class Edit extends Component {
    constructor(props) {
        super(props);

        var paRecordIds = props.settings.map(e => e.paRecordId);
        var employees = props.settings.map(e => e.nickName).join(', ');

        if (paRecordIds.length === 1) {
            let adminProcesses = props.settings[0].processes.filter(p => p.processId === 1);

            this.processes = [{
                processId: 1,
                process: adminProcesses[0].process,
                evaluator: adminProcesses.map(e => { return { label: e.evaluator, value: e.evaluatorId }; }),
                startDate: adminProcesses[0].startDate ? moment(adminProcesses[0].startDate) : null,
                endDate: adminProcesses[0].endDate ? moment(adminProcesses[0].endDate) : null,
            }];

            this.processes = this.processes.concat(props.settings[0].processes.filter(p => p.processId !== 1).map(p => {
                return {
                    processId: p.processId,
                    process: p.process,
                    evaluator: { label: p.evaluator, value: p.evaluatorId },
                    startDate: p.startDate ? moment(p.startDate) : null,
                    endDate: p.endDate ? moment(p.endDate) : null,
                }
            }));
        } else {
            let processes = uniqBy(props.settings[0].processes, p => p.processId)
            this.processes = processes.map(p => {
                return {
                    paRecordId: p.paRecordId,
                    processId: p.processId,
                    process: p.process,
                    evaluator: null,
                    startDate: null,
                    endDate: null,
                }
            });
        }

        this.state = {
            employees: employees,
        };
    }

    searchEmployees = (input, callback) => {
        if (input.length > 2) {
            return axios(resolveClientUrl(`common/SearchEmployees?keyword=${input}`))
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: item.nickName
                        });
                    });

                    return callback(options);
                });
        }

        return callback([]);
    }

    validateForm = values => {
        let errors = {};
        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        var processes = [];
        var adminProcessEvaluators = values.processes.find(p => p.processId === 1).evaluator;
        if (adminProcessEvaluators) {
            processes = adminProcessEvaluators.map(e => { return { processId: 1, evaluatorId: e.value }; });
        }

        axios({
            url: resolveClientUrl(`period/EditEmployeeSetting`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordIds: this.props.paRecordIds,
                processes: processes.concat(values.processes.filter(p => p.processId > 1 && p.evaluator !== null).map(p => {
                    return {
                        processId: p.processId,
                        evaluatorId: p.evaluator.value
                    };
                }))
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Create Successfully");
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

        return (
            <Form horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error" />
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={3}>Employee</Col>
                        <Col sm={9} style={{ paddingTop: '7px' }}  >{this.state.employees}</Col>
                    </FormGroup>

                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Evaluator</th>
                            </tr>
                        </thead>
                        <tbody>
                            {values.processes.map((p, i) =>
                                <tr key={i}>
                                    <td>{p.process}</td>
                                    <td>
                                        {p.processId !== 2 &&
                                            <Async className={`react-select-container`} classNamePrefix="react-select"
                                                value={p.evaluator} isMulti={p.processId === 1}
                                                onChange={value => setFieldValue(`processes.${i}.evaluator`, value)}
                                                loadOptions={this.searchEmployees} placeholder="-- Search an employee --" />
                                        }
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>Cancel</Button>
                    <Button type="submit" disabled={isSubmitting} className="btn btn-primary">Submit</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        return (
            <Modal backdrop="static" show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Edit Employee Setting</Modal.Title>
                </Modal.Header>
                <Formik initialValues={{ processes: this.processes }} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}