import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { Async } from 'react-select';

import { Formik } from 'formik';

export default class EditDiscipline extends Component {
    constructor(props) {
        super(props);
    }

    validateForm = values => {
        let errors = {};

        if (!values.employee) {
            errors.employee = 'Employee is required';
        }

        if (!values.effectiveDate) {
            errors.effectiveDate = 'Issue Date is required';
        }

        if (!values.content) {
            errors.content = 'Content is required';
        }

        return errors;
    }

    onSubmit = (values, { setSubmitting, setErrors }) => {
        debugger;
        setTimeout(() => {
            alert(JSON.stringify(values, null, 2));
            setSubmitting(false);
        }, 1000);
    }

    searchEmployees = (input, callback) => {
        if (input.length > 2) {
            return axios(`/common/SearchEmployees?keyword=${input}`)
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            employeeId: item.employeeId,
                            name: `${item.fullName} (${item.nickName})`
                        });
                    });

                    return { options };
                });
        }

        callback(null, { options: [] });
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleBlur,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        return (
            <Form id="edit-user-role-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={2}>Employee</Col>
                        <Col sm={10}>
                            <Async valueKey="employeeId" labelKey="name" autoload={false} value={values.employee} name="employee" onBlur={handleBlur}
                                onChange={value => setFieldValue('employee', value)} loadOptions={this.searchEmployees} placeholder="-- Search an employee --" />

                            {touched.employee && errors.employee && <span className="text-danger">{errors.employee}</span>}
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={2}>Issue Date</Col>
                        <Col sm={10}>

                            <DatePicker dateFormat="DD MMM, YYYY" selected={values.effectiveDate} onChange={value => setFieldValue('effectiveDate', value)} onBlur={handleBlur} onChangeRaw={this.onChangeRaw}
                                name="effectiveDate" className="form-control" showMonthDropdown showYearDropdown />
                            {touched.effectiveDate && errors.effectiveDate && <span className="text-danger">{errors.effectiveDate}</span>}
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} sm={2}>Content</Col>
                        <Col sm={10}>
                            <textarea className="form-control" name="content" onChange={handleChange} onBlur={handleBlur} value={values.content} />
                            {touched.content && errors.content && <span className="text-danger">{errors.content}</span>}
                        </Col>
                    </FormGroup>
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
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Assign Roles To User </Modal.Title>
                </Modal.Header>
                <Formik initialValues={{
                    employee: null, effectiveDate: moment(), content: ''
                }} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}