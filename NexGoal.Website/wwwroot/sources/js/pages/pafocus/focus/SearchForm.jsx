import React, { Component, Fragment } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect/index';

export default class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.filter !== nextProps.filter
            || this.props.periodOptions !== nextProps.periodOptions
            || this.props.groupOptions !== nextProps.groupOptions
            || this.props.memberOptions !== nextProps.memberOptions;
    }

    handlePeriodChange = selectedOption => {
        this.props.onChange('period', selectedOption);
    };

    handleGroupChange = selectedOptions => {
        this.props.onChange('groups', selectedOptions);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onChange('employee', selectedOption);
    };

    handleExportToExcel = event => {
        event.preventDefault();
        this.props.onExportToExcel();
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        const { filter, periodOptions, groupOptions, memberOptions } = this.props;

        return (
            <Form inline className="form-filter">
                <FormGroup>
                    <ControlLabel>{getResource('EvaluationPeriod')}</ControlLabel>
                    <div className="control-content period-width">
                        <Select name="Year" isSearchable={false} isClearable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.period}
                            options={periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                {
                    groupOptions && groupOptions.length > 1 &&
                    <Fragment>
                        <FormGroup>
                            <ControlLabel>{getResource('Team')}</ControlLabel>
                            <div className="control-content team-width">
                                <Multiselect hasSearchBox
                                    items={groupOptions}
                                    onChange={this.handleGroupChange}
                                />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>{getResource('Employee')}</ControlLabel>
                            <div className="control-content employee-width">
                                <Select className="react-select-container" classNamePrefix="react-select"
                                    value={filter.employee} options={memberOptions}
                                    name="employee" onChange={this.handleEmployeeChange} isClearable
                                />
                            </div>
                        </FormGroup>
                    </Fragment>
                }
                <FormGroup className="button-block">
                    <button className="btn btn-primary" onClick={this.handleSubmit}>{getResource('Search')}</button>
                    <button className="btn btn-excel" onClick={this.handleExportToExcel}>{getResource('Excel')}</button>
                </FormGroup>
            </Form>
        );
    }
}