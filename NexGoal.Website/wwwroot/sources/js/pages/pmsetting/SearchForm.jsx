import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';
import Multiselect from '../../shared/components/react-multiselect';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleGroupChange = selectedOption => {
        this.props.onFilterChange('pms', selectedOption);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employee', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onFilterSubmit();
    };

    render() {
        const { filter, pmOptions } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('ProjectManager')}</ControlLabel>
                    <div className="control-content team-width">
                        <Multiselect hasSearchBox
                            items={pmOptions}
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