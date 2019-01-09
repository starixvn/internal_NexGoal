import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import MultiSelect from '../../../shared/components/multi-select/index';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleStatusChange = (selectedOption) => {
        this.props.onFilterChange('status', selectedOption);
    }

    handleGroupChange = (selectedOption) => {
        this.props.onFilterChange('groups', selectedOption);
    }

    handlePeriodChange = (selectedOption) => {
        this.props.onFilterChange('periods', selectedOption);
    }

    handleEmployeeChange = (selectedOption) => {
        this.props.onFilterChange('employee', selectedOption);
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.props.onFilterSubmit();
    }

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Period')}</ControlLabel>
                    <div className="control-content period-width">
                        <MultiSelect name="periods"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.periods}
                            options={this.props.periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
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
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width">
                        <Select name="employee"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.employee}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange} isClearable
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content status-width">
                        <Select name="status" className="react-select-container" classNamePrefix="react-select"
                            value={filter.status} options={this.props.statusOptions} onChange={this.handleStatusChange}
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