import React, { Component } from 'react';
import { Checkbox, Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import commonFunctions from '../../common-functions';

class EditInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            targetTypes: [],
            flowTemplate: {
                flowTemplateId: 0,
                name: '',
                targetTypeId: 0,
                isActive: false,
                description: ''
            }
        };
    }

    componentDidMount() {
        this.getTargetTypes();
        this.getFlowTemplateById();
    }

    getTargetTypes() {
        axios.get(commonFunctions.getRootUrl() + 'Common/GetTargetTypes')
            .then(response => {
                this.setState({ targetTypes: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getFlowTemplateById() {
        axios.get(commonFunctions.getRootUrl() + `Flow/GetById/${this.props.flowTemplateId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    flowTemplate: {
                        flowTemplateId: data.flowTemplateId,
                        name: data.name,
                        targetTypeId: data.targetTypeId,
                        isActive: data.isActive,
                        description: data.description
                    }
                });
            }).catch(commonFunctions.axiosError);
    }

    handleOnSubmit = (values, { setErrors }) => {
        const flowTemplate = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            flowTemplateId: this.state.flowTemplate.flowTemplateId,
            name: values.name,
            targetTypeId: values.targetType.value,
            isActive: values.isActive,
            description: values.description
        });

        axios({
            url: commonFunctions.getRootUrl() + 'Flow/Edit',
            method: 'POST',
            data: flowTemplate,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');
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
        const isHost = this.props.isHost;
        const targetTypeOptions = this.state.targetTypes.map((targetType) => {
            return { value: targetType.targetTypeId.toString(), label: targetType.name };
        });

        return (
            <Form id="edit-evaluationflow-form" horizontal onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.name && errors.name && <li>{errors.name}</li>}
                        {touched.targetType && errors.targetType && <li>{errors.targetType}</li>}
                        {touched.description && errors.description && <li>{errors.description}</li>}
                    </ul>
                }

                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Name</label>
                    <div className="col-sm-6">
                        <input type="text" name="name" autoComplete="off" disabled={!isHost}
                            className={`form-control ${touched.name && errors.name && 'input-validation-error'}`}
                            value={values.name} onChange={handleChange} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Target Type</label>
                    <div className="col-sm-6">
                        <Select name="targetType" className={`${touched.targetType && errors.targetType && 'input-validation-error'}`}
                            value={values.targetType}
                            options={targetTypeOptions}
                            clearable={false} searchable={false} disabled={!isHost}
                            onChange={selectedOption => setFieldValue('targetType', selectedOption)} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Active</label>
                    <div className="col-sm-6">
                        <Checkbox name="isActive" onChange={handleChange} checked={values.isActive} disabled={!isHost} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Description</label>
                    <div className="col-sm-6">
                        <textarea className={`form-control none-resize ${touched.description && errors.description && 'input-validation-error'}`}
                            rows="3" maxLength="200"
                            name="description" value={values.description || ''}
                            onChange={handleChange} disabled={isHost ? false : true} />
                    </div>
                </FormGroup>
                {isHost &&
                    <FormGroup>
                        <label className="col-sm-2 control-label"></label>
                        <div className="col-sm-6">
                            <Button type="submit" className="btn btn-primary">Submit</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }

    render() {
        const targetTypeOptions = this.state.targetTypes.map((targetType) => {
            return { value: targetType.targetTypeId.toString(), label: targetType.name };
        });
        const targetTypeSelected = targetTypeOptions.find((option) => { return option.value === this.state.flowTemplate.targetTypeId.toString(); });
        const flowTemplate = {
            name: this.state.flowTemplate.name,
            targetType: targetTypeSelected,
            isActive: this.state.flowTemplate.isActive,
            description: this.state.flowTemplate.description
        };

        return (
            <Formik enableReinitialize initialValues={flowTemplate}
                validate={this.validateForm}
                onSubmit={this.handleOnSubmit}
                render={this.renderForm} />
        );
    }
}

export default EditInfo;