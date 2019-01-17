import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import MultiSelect from '../../../shared/components/multi-select/index';
import getResource from '../../../resourceHelper';

export default class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeOptions: props.employeeOptions,
            errors: {}
        };
    }

    handlePeriodChange = (selectedOption) => {
        console.log(selectedOption);
        this.props.onFilterChange('periods', selectedOption);
    }

    handleGroupChange = (selectedOption) => {
        this.props.onFilterChange('groups', selectedOption);
    }

    handleEmployeeChange = (selectedOption) => {
        this.props.onFilterChange('employee', selectedOption);
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.props.onSubmit();
    }

    render() {
        const filter = this.props.filter;
        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Period')}</ControlLabel>
                    <div className="control-content">
                        <MultiSelect className="react-select-container" classNamePrefix="react-select"
                            name="periods"
                            value={filter.periods}
                            options={this.props.periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                {
                    this.props.groupOptions.length > 1 &&
                    <FormGroup>
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content team">
                            <MultiSelect className="react-select-container" classNamePrefix="react-select"
                                name="groups"
                                value={filter.groups}
                                options={this.props.groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </FormGroup>
                }
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content">
                        <Select className="react-select-container" classNamePrefix="react-select"
                            name="employee"
                            value={filter.employee}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
                <FormGroup>
                    <Button type="button" bsStyle="success" onClick={this.props.exportExcel}>{getResource('Excel')}</Button>
                </FormGroup>
            </Form>
        );
    }
}