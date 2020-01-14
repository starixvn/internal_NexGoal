import React, { Component } from 'react';
import { Modal, Button, FormGroup, Form } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import Select from 'react-select';

import { validateFormikField, displayFormikErrors } from '../../common-functions';
import getResource from '../../resourceHelper';
import { DateFormat, InputErrorCssClass } from '../../constants';

export default class CreateEditForm extends Component {
    render() {
        const { touched, errors, values, isCreate } = this.props;
        const isValidEmployee = validateFormikField(touched, errors, 'employee'),
            isValidEffectiveDate = validateFormikField(touched, errors, 'effectiveDate'),
            isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form className="discipline-form" id="create-edit-form" inline onSubmit={this.props.onSubmit}>
                <Modal.Body>
                    {displayFormikErrors(touched, errors, ['employee', 'effectiveDate', 'description'])}

                    <div className="line-group">
                        {
                            isCreate &&
                            <FormGroup>
                                <label className="control-label required-label">{getResource('Employee')}</label>
                                <div className="control-content">
                                    <Select name="employee"
                                        className={`react-select-container ${!isValidEmployee && InputErrorCssClass}`}
                                        classNamePrefix="react-select"
                                        isDisabled={values.disciplineId > 0} isClearable
                                        options={this.props.employeeOptions}
                                        value={values.employee}
                                        onChange={value => this.props.setFieldValue('employee', value)}
                                    />
                                </div>
                            </FormGroup>
                        }
                        <FormGroup>
                            <label className="required-label control-label">{getResource('IssueDate')}</label>
                            <div className="control-content">
                                <DatePicker className={`form-control  ${!isValidEffectiveDate && InputErrorCssClass}`}
                                    dateFormat={DateFormat} selected={values.effectiveDate}
                                    onChange={value => this.props.setFieldValue('effectiveDate', value)}
                                    onChangeRaw={this.onChangeRaw}
                                    name="effectiveDate" autoComplete="off"
                                />
                            </div>
                        </FormGroup>
                        <FormGroup className="description-group">
                            <label className="required-label control-label">{getResource('Content')}</label>
                            <div className="control-content">
                                <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
                                    name="description" rows="10"
                                    onChange={this.props.onChange}
                                    value={values.description}
                                />
                            </div>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.onClose}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }
}