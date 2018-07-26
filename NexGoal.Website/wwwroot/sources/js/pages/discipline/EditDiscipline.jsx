import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { Async } from 'react-select';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

export default class AddEditDiscipline extends Component {
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
        axios({
            url: `/discipline/${values.disciplineId === 0 ? 'add' : 'edit'}`,
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                disciplineId: values.disciplineId,
                employeeId: values.employee.employeeId,
                effectiveDate: values.effectiveDate.format('YYYY-MM-DD'),
                content: values.content,
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Update Successfully");
                this.props.onEditDiscipline(values.disciplineId, response.data);
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
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        return (
            <Form id="edit-user-role-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.employee && errors.employee && <li>{errors.employee}</li>}
                            {touched.effectiveDate && errors.effectiveDate && <li>{errors.effectiveDate}</li>}
                            {touched.content && errors.content && <li>{errors.content}</li>}
                        </ul>
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={2}>Employee</Col>
                        <Col sm={10}>
                            <Async className={touched.employee && errors.employee && 'input-validation-error'} disabled={values.disciplineId > 0} valueKey="employeeId" labelKey="name" autoload={false} value={values.employee} name="employee"
                                onChange={value => setFieldValue('employee', value)} loadOptions={this.searchEmployees} placeholder="-- Search an employee --" />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={2}>Issue Date</Col>
                        <Col sm={10}>
                            <DatePicker className={`form-control  ${touched.effectiveDate && errors.effectiveDate && 'input-validation-error'}`}
                                dateFormat="DD MMM, YYYY" selected={values.effectiveDate} onChange={value => setFieldValue('effectiveDate', value)} onChangeRaw={this.onChangeRaw}
                                name="effectiveDate" showMonthDropdown showYearDropdown />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={2}>Content</Col>
                        <Col sm={10}>
                            <textarea className={`form-control  ${touched.content && errors.content && 'input-validation-error'}`}
                                name="content" onChange={handleChange} value={values.content} />
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
        var title = this.props.discipline && this.props.discipline.disciplineId === 0 ? 'Add Discipline' : 'Edit Discipline';

        return (
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Formik initialValues={this.props.discipline} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}