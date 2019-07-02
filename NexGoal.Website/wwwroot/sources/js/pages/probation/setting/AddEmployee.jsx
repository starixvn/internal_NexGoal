import React, { Component } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import Select from 'react-select';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';

export default class AddEmployee extends Component {
    constructor(props) {
        super(props);
    }

    initialObj = {
        employees: []
    }

    validateForm = values => {
        let errors = {};

        if (!values.employees || values.employees.length === 0) {
            errors.employees = 'Please select member';
        }

        return errors;
    }

    onSubmit = values => {
        axios({
            url: resolveClientUrl(`Probation/AddNewEmployees`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeIds: values.employees.map(e => e.value)
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('AddedSuccessfully'));
                this.props.onAddEmployeesuccess();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleSubmit,
        setFieldValue }) => {
        return (
            <Form onSubmit={handleSubmit} className="add-member-form">
                <Modal.Body>
                    <div className="lb-member">{getResource('Employees')}</div>
                    <Select name="employees"
                        className={`react-select-container ${touched.employees && errors.employees && 'input-validation-error'}`}
                        classNamePrefix="react-select" isClearable
                        options={this.props.employeeOptions} placeholder="Nickname or Full name"
                        value={values.employees} isMulti closeMenuOnSelect={false}
                        onChange={value => setFieldValue('employees', value)}
                    />
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md add-new-employee-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Assign new employee(s)</Modal.Title>
                </Modal.Header>
                <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}