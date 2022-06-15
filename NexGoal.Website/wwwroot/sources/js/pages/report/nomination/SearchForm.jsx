import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect';

export default class SearchForm extends Component {
    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    };

    handleStatusChange = selectedOption => {
        this.props.onFilterChange('status', selectedOption);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    };

    handlePeriodChange = selectedOption => {
        this.props.onFilterChange('periods', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onFilterSubmit();
    };

    render() {
        const { filter, periodOptions, groupOptions, employeeOptions } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-period-label">{getResource('Period')}</ControlLabel>
                    <div className="control-content period-width">
                        <Multiselect hasSearchBox
                            onChange={this.handlePeriodChange}
                            items={periodOptions}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-team-label">{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <Multiselect hasSearchBox
                            items={groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-employee-label">{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width">
                        <Multiselect hasSearchBox
                            items={employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="nominate-report-status-label">{getResource('Status')}</ControlLabel>
                    <div className="control-content status-width">
                        <Select name="status" isSearchable={false}
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