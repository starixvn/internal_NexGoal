import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
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

    handleStatusChange = selectedOption => {
        this.props.onFilterChange('status', selectedOption);
    }

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    }

    handlePeriodChange = selectedOption => {
        this.props.onFilterChange('periods', selectedOption);
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.onFilterSubmit();
    }

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-period-label">{getResource('Period')}</ControlLabel>
                    <div className="control-content period-width">
                        <MultiSelect name="periods"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.periods}
                            onChange={this.handlePeriodChange}
                            options={this.props.periodOptions}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-team-label">{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <MultiSelect name="groups"
                            classNamePrefix="react-select"
                            className="react-select-container"
                            value={filter.groups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-employee-label">{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width">
                        <MultiSelect
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.employees}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-status-label">{getResource('Status')}</ControlLabel>
                    <div className="control-content status-width">
                        <Select name="status"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.status} options={this.props.statusOptions}
                            onChange={this.handleStatusChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}