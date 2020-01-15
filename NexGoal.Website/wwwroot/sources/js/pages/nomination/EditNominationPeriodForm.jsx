import React, { PureComponent } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import Select from 'react-select';

import { validateFormikField, displayFormikErrors } from '../../common-functions';
import { quarterOptions } from './common';
import getResource from '../../resourceHelper';
import { DateFormat, InputErrorCssClass } from '../../constants';

export default class EditNominationPeriodForm extends PureComponent {
    render() {
        const { values, errors, touched, isHost } = this.props;
        const quarterSelected = quarterOptions.find(option => option.value === values.quarter);
        const yearOptions = [
            { value: values.year, label: values.year }
        ];
        const isValidStartDateForMominator = validateFormikField(touched, errors, 'startDateForNominator'),
            isValidEndDateForNominator = validateFormikField(touched, errors, 'endDateForNominator'),
            isValidRemindDateForNominator = validateFormikField(touched, errors, 'remindDateForNominator'),
            isValidEndDateForCommittee = validateFormikField(touched, errors, 'endDateForCommittee'),
            isValidDescription = validateFormikField(touched, errors, 'description');

        return (
            <Form id="edit-nomination-period" inline className="settings-form" onSubmit={this.props.onSubmit}>
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
                                <Select value={quarterSelected} className="react-select-container" classNamePrefix="react-select"
                                    options={quarterOptions} isDisabled
                                />
                            </div>
                        </FormGroup>
                    }
                    <FormGroup>
                        <label className="control-label">{getResource('Year')}</label>
                        <div className="field control-content">
                            <Select value={yearOptions[0]} className="react-select-container" classNamePrefix="react-select"
                                options={yearOptions} isDisabled
                            />
                        </div>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <div className="title-group"><strong>{getResource('ForNominator')}</strong></div>
                    <FormGroup>
                        <label className="control-label">{getResource('StartDate')}</label>
                        <div className="field control-content">
                            <DatePicker name="startDateForNominator" autoComplete="off" disabled={!isHost}
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
                            <DatePicker name="endDateForNominator" autoComplete="off" disabled={!isHost}
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
                            <DatePicker name="remindDateForNominator" autoComplete="off" disabled={!isHost}
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
                            <DatePicker name="endDateForCommittee" autoComplete="off" disabled={!isHost}
                                className={`form-control ${!isValidEndDateForCommittee && InputErrorCssClass}`}
                                dateFormat={DateFormat}
                                selected={values.endDateForCommittee}
                                onChange={value => this.props.setFieldValue('endDateForCommittee', value)}
                            />
                        </div>
                    </FormGroup>
                </div>
                <div className="line-group">
                    <FormGroup>
                        <label className="control-label">{getResource('Description')} (optional)</label>
                        <div className="field control-content description-content">
                            <textarea className={`form-control none-resize ${!isValidDescription && InputErrorCssClass}`}
                                rows="3" maxLength="200"
                                name="description" value={values.description}
                                onChange={this.props.onChange} disabled={isHost ? false : true}
                            />
                        </div>
                    </FormGroup>
                </div>
                {
                    isHost &&
                    <FormGroup>
                        <label className="control-label" />
                        <div className="field control-content">
                            <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }
}