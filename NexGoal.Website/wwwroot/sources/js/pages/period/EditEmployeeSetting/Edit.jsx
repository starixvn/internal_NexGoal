import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, FormGroup, ControlLabel, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik, FieldArray, Field } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import Async from 'react-select/lib/Async';
import commonFunctions from '../../../common-functions';

export default class Edit extends Component {
    constructor(props) {
        super(props);

        var paRecordIds = props.settings.map(e => e.paRecordId);
        var employees = props.settings.map(e => e.nickName).join(', ');

        this.processes = props.settings[0].processes;

        this.state = {
            paRecordIds: paRecordIds,
            employees: employees,
        };
    }

    searchEmployees = (input, callback) => {
        if (input.length > 2) {
            return axios(`/common/SearchEmployees?keyword=${input}`)
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
        axios({
            url: `/period/Create`,
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                name: values.name,
                targetTypeId: values.targetTypeId,
                startDate: values.startDate ? values.startDate.format('YYYY-MM-DD') : null,
                endDate: values.endDate ? values.endDate.format('YYYY-MM-DD') : null,
                disciplineStartDate: values.disciplineStartDate ? values.disciplineStartDate.format('YYYY-MM-DD') : null,
                disciplineEndDate: values.disciplineEndDate ? values.disciplineEndDate.format('YYYY-MM-DD') : null,
                focusStartDate: values.targetTypeId == TargetType.semiAnual ? values.focusStartDate.format('YYYY-MM-DD') : null,
                description: values.description,
                isActive: values.isActive
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Create Successfully");
                this.props.onCreateSuccess();
                this.props.closeModal();
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
        }).catch(commonFunctions.axiosError);
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

        debugger;

        return (
            <Form horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                        </ul>
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
                                <th>Start Date</th>
                                <th>End Date</th>
                            </tr>
                        </thead>
                        <tbody>
                            {values.processes.map((p, i) =>
                                <tr key={i}>
                                    <td>{p.process}</td>
                                    <td>
                                        <Async
                                            className={`react-select-container`} classNamePrefix="react-select"
                                            value={{ label: p.evaluator, value: p.evaluatorId }}
                                            onChange={value => setFieldValue(`processes.${i}.evaluator`, value.label)}
                                            loadOptions={this.searchEmployees}
                                            placeholder="-- Search an employee --" />
                                    </td>
                                    <td></td>
                                    <td></td>
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