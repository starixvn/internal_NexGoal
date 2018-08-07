import React, { Component } from 'react';
import { Checkbox, Button, Form, FormGroup, Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import commonFunctions from '../../common-functions';

class Create extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (values, { setErrors }) => {
        let flowTemplate = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            name: values.name,
            targetTypeId: values.targetType.value,
            isActive: values.isActive,
            description: values.description
        });

        axios({
            url: commonFunctions.getRootUrl() + 'Flow/Create',
            method: 'POST',
            data: flowTemplate,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Created successfully!');

                this.props.closeModal();
                this.props.handleFilterSubmit();
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
        }).catch(commonFunctions.axiosError);
    }

    validateForm(values) {
        let errors = {};

        if (!values.name) {
            errors.name = 'Name is required!';
        }

        if (!values.targetType || !values.targetType.value) {
            errors.targetType = 'Target Type is required!';
        }

        return errors;
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        let targetTypeOptions = this.props.targetTypes.map((targetType) => {
            return { value: targetType.targetTypeId, label: targetType.name };
        });
        targetTypeOptions.unshift({ value: '', label: '-- All --' });

        return (
            <Form id="create-flowtemplate-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        !isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {touched.name && errors.name && <li>{errors.name}</li>}
                            {touched.targetType && errors.targetType && <li>{errors.targetType}</li>}
                            {touched.description && errors.description && <li>{errors.description}</li>}
                        </ul>
                    }

                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Name</label>
                        <div className="col-sm-9">
                            <input type="text" name="name" autoComplete="off"
                                className={`form-control ${touched.name && errors.name && 'input-validation-error'}`}
                                value={values.name} onChange={handleChange} />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">Target Type</label>
                        <div className="col-sm-9">
                            <Select name="targetType" className={`${touched.targetType && errors.targetType && 'input-validation-error'}`}
                                value={values.targetType}
                                options={targetTypeOptions}
                                clearable={false}
                                searchable={false}
                                onChange={selectedOption => setFieldValue('targetType', selectedOption)} />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">Active</label>
                        <div className="col-sm-9">
                            <Checkbox name="isActive" onChange={handleChange} checked={values.isActive} />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">Description</label>
                        <div className="col-sm-9 field">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                rows="5" maxLength="500"
                                name="description" value={values.description}
                                onChange={handleChange} />
                        </div>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.closeModal}>Cancel</Button>
                    <Button type="submit" className="btn btn-primary">Submit</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        const flowTemplate = {
            name: '',
            targetType: { value: '', label: '-- All --' },
            isActive: true,
            description: ''
        };

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Nomination Period</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={flowTemplate}
                    validate={this.validateForm}
                    onSubmit={this.handleSubmit}
                    render={this.renderForm} />
            </Modal>
        );
    }
}

export default Create;