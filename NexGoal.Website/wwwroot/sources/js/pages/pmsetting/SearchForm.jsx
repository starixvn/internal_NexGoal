import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';
import MultiSelect from '../../shared/components/multi-select/multi-select';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleGroupChange = selectedOption => {
        this.props.onFilterChange('pms', selectedOption);
    }

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employee', selectedOption);
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
                    <ControlLabel>{getResource('ProjectManager')}</ControlLabel>
                    <div className="control-content team-width">
                        <MultiSelect name="pms"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.pms}
                            options={this.props.pmOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content team-width">
                        <Select name="employee"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.employee} isClearable
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange}
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