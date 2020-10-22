import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import MultiSelect from '../../../shared/components/multi-select/multi-select';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    }

    handlePeriodChange = selectedOption => {
        this.props.onFilterChange('period', selectedOption);
    }

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    }

    handleSubmit = event => {
        event.preventDefault();
    }

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-period-label">{getResource('Period')}</ControlLabel>
                    <div className="control-content period-width">
                        <Select name="period"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.period}
                            options={this.props.periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-team-label">{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <MultiSelect name="groups"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.groups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-employee-label">
                        {getResource('Employee')}
                    </ControlLabel>
                    <div className="control-content employee-width">
                        <MultiSelect
                            classNamePrefix="react-select"
                            className="react-select-container"
                            value={filter.employees}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <a className="btn btn-excel" href={this.props.exportUrl} >{getResource('Excel')}</a>
                </FormGroup>
            </Form>
        );
    }
}