import React, { PureComponent } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import Select from 'react-select';

import { validateFormikField, displayFormikErrors } from '../../common-functions';
import { quarterOptions } from './common';
import getResource from '../../resourceHelper';
import { DateFormat, InputErrorCssClass } from '../../constants';

export default class CreateNominationPeriodForm extends PureComponent {
    render() {
        const { values, errors, touched } = this.props;
        const currentYear = new Date().getFullYear();
        const yearOptions = [
            { value: currentYear, label: currentYear },
            { value: currentYear + 1, label: currentYear + 1 }
        ];
        const isValidStartDateForMominator = validateFormikField(touched, errors, 'startDateForNominator'),
            isValidEndDateForNominator = validateFormikField(touched, errors, 'endDateForNominator'),
            isValidRemindDateForNominator = validateFormikField(touched, errors, 'remindDateForNominator'),
            isValidEndDateForCommittee = validateFormikField(touched, errors, 'endDateForCommittee'),
            isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form id="create-nomination-period" inline onSubmit={this.props.onSubmit}>
                <Modal.Body>
                    {
                        displayFormikErrors(
                            touched,
                            errors,
                            ['quarter', 'year', 'startDateForNominator', 'endDateForNominator', 'remindDateForNominator', 'endDateForCommittee', 'description'])
                    }

                    <div className="line-group">
                        {
                            this.props.isEOQ &&
                            <FormGroup>
                                <label className="control-label">{getResource('Quarter')}</label>
                                <div className="field control-content">
                                    <Select name="quarter" isSearchable={false} isClearable={false}
                                        className="react-select-container" classNamePrefix="react-select"
                                        value={values.quarter}
                                        options={quarterOptions}
                                        onChange={selectedOption => this.props.setFieldValue('quarter', selectedOption)}
                                    />
                                </div>
                            </FormGroup>
                        }
                        <FormGroup>
                            <label className="control-label">{getResource('Year')}</label>
                            <div className="field control-content">
                                <Select name="year" isSearchable={false} isClearable={false}
                                    className="react-select-container" classNamePrefix="react-select"
                                    value={values.year}
                                    options={yearOptions}
                                    onChange={selectedOption => this.props.setFieldValue('year', selectedOption)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <div className="title-group"><strong>{getResource('ForNominator')}</strong></div>
                        <FormGroup>
                            <label className="control-label">{getResource('StartDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="startDateForNominator" autoComplete="off"
                                    className={`form-control ${!isValidStartDateForMominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.startDateForNominator}
                                    onChange={value => this.props.setFieldValue('startDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="control-label">{getResource('EndDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="endDateForNominator" autoComplete="off"
                                    className={`form-control ${!isValidEndDateForNominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.endDateForNominator}
                                    onChange={value => this.props.setFieldValue('endDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="control-label">{getResource('RemindDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="remindDateForNominator" autoComplete="off"
                                    className={`form-control ${!isValidRemindDateForNominator && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.remindDateForNominator}
                                    onChange={value => this.props.setFieldValue('remindDateForNominator', value)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <div className="title-group"><strong>{getResource('ForCommittee')}</strong></div>
                        <FormGroup>
                            <label className="control-label">{getResource('EndDate')}</label>
                            <div className="field control-content">
                                <DatePicker name="endDateForCommittee" autoComplete="off"
                                    className={`form-control ${!isValidEndDateForCommittee && InputErrorCssClass}`}
                                    dateFormat={DateFormat}
                                    selected={values.endDateForCommittee}
                                    onChange={value => this.props.setFieldValue('endDateForCommittee', value)}
                                />
                            </div>
                        </FormGroup>
                    </div>
                    <div className="line-group">
                        <FormGroup className="description-group">
                            <label className="control-label">{getResource('Description')} (optional)</label>
                            <div className="field control-content">
                                <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
                                    rows="5" maxLength="200"
                                    name="description" value={values.description}
                                    onChange={this.props.onChange}
                                />
                            </div>
                        </FormGroup>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onClose}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }
}