import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';

export default class CreatePeriod extends Component {
    constructor(props) {
        super(props);
    }

    validateForm = values => {
        let errors = {};

        if (!values.name) {
            errors.name = 'Name is required';
        }

        if (!values.targetTypeId) {
            errors.targetTypeId = 'Target type is required';
        }

        if (!values.startDate) {
            errors.startDate = 'Start Date is required';
        }

        if (!values.endDate) {
            errors.endDate = 'End Date is required';
        }

        if (!values.content) {
            errors.description = 'Description is required';
        }

        return errors;
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
                            {touched.name && errors.name && <li>{errors.name}</li>}
                            {touched.targetTypeId && errors.targetTypeId && <li>{errors.targetTypeId}</li>}
                            {touched.startDate && errors.startDate && <li>{errors.startDate}</li>}
                            {touched.endDate && errors.endDate && <li>{errors.endDate}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Name</Col>
                        <Col sm={9}>
                            <input type="text" name="name" value={values.name} className={`form-control  ${touched.name && errors.name && 'input-validation-error'}`} onChange={handleChange} />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Target Type</Col>
                        <Col sm={9}>
                            <select name="targetTypeId" className={`form-control  ${touched.targetTypeId && errors.targetTypeId && 'input-validation-error'}`} onChange={handleChange}>
                                {
                                    this.props.targetTypes.map(e => <option key={e.targetTypeId} value={e.targetTypeId} >{e.name}</option>)
                                }
                            </select>
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Start Date</Col>
                        <Col sm={3}>
                            <DatePicker className={`form-control  ${touched.startDate && errors.startDate && 'input-validation-error'}`}
                                dateFormat="DD MMM, YYYY" selected={values.startDate} onChange={value => setFieldValue('startDate', value)} onChangeRaw={this.onChangeRaw}
                                name="startDate" showMonthDropdown showYearDropdown />
                        </Col>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>End Date</Col>
                        <Col sm={3}>
                            <DatePicker className={`form-control  ${touched.endDate && errors.endDate && 'input-validation-error'}`}
                                dateFormat="DD MMM, YYYY" selected={values.endDate} onChange={value => setFieldValue('endDate', value)} onChangeRaw={this.onChangeRaw}
                                name="endDate" showMonthDropdown showYearDropdown />
                        </Col>
                    </FormGroup>
                    <FormGroup>
                        <Col componentClass={ControlLabel} className="required-label" sm={3}>Description</Col>
                        <Col sm={9}>
                            <textarea className={`form-control  ${touched.description && errors.description && 'input-validation-error'}`}
                                name="description" onChange={handleChange} value={values.description} />
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
        let obj = {
            name: '',
            targetTypeId: '',
            startDate: null,
            endDate: null,
            description: ''
        };

        return (
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Create Evaluation Period</Modal.Title>
                </Modal.Header>
                <Formik initialValues={obj} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}