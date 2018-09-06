import React, { Component } from 'react';
import { Checkbox, Button, Form, FormGroup, Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

class Create extends Component {
    constructor(props) {
        super(props);
    }

    handleOnSubmit = (values, { setErrors }) => {
        let flowTemplate = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            name: values.name,
            targetTypeId: values.targetType.value,
            isActive: values.isActive,
            description: values.description
        });

        axios({
            url: resolveClientUrl('Flow/Create'),
            method: 'POST',
            data: flowTemplate,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('CreatedSuccessfully'));

                this.props.closeModal();
                this.props.onFilterSubmit();
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

    validateForm(values) {
        let errors = {};

        if (!values.name) {
            errors.name = getResource('NameRequired');
        }

        if (!values.targetType || !values.targetType.value) {
            errors.targetType = getResource('TargetTypeRequired');
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
                        <label className="col-sm-3 control-label required-label">{getResource('Name')}</label>
                        <div className="col-sm-9">
                            <input type="text" name="name" autoComplete="off"
                                className={`form-control ${touched.name && errors.name && 'input-validation-error'}`}
                                value={values.name} onChange={handleChange}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label required-label">{getResource('TargetType')}</label>
                        <div className="col-sm-9">
                            <Select name="targetType"
                                className={`react-select-container ${touched.targetType && errors.targetType && 'input-validation-error'}`}
                                classNamePrefix="react-select"
                                value={values.targetType}
                                options={targetTypeOptions}
                                isSearchable={false} isClearable={false}
                                onChange={selectedOption => setFieldValue('targetType', selectedOption)}
                            />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">{getResource('Active')}</label>
                        <div className="col-sm-9">
                            <Checkbox name="isActive" onChange={handleChange} checked={values.isActive} />
                        </div>
                    </FormGroup>
                    <FormGroup>
                        <label className="col-sm-3 control-label">{getResource('Description')}</label>
                        <div className="col-sm-9 field">
                            <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                                rows="5" maxLength="500"
                                name="description" value={values.description}
                                onChange={handleChange}
                            />
                        </div>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
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
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('CreateNominationPeriod')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={flowTemplate}
                    validate={this.validateForm}
                    onSubmit={this.handleOnSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}

export default Create;