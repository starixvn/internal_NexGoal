import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect/index';

export default class SearchForm extends Component {
    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    };

    handleNoPRChange = selectedOption => {
        this.props.onFilterChange('noPR', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        const { filter, groupOptions, employeeOptions, noPROptions } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                {
                    groupOptions.length > 1 &&
                    <FormGroup>
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content team-width pr-tracking-search-form">
                            <Multiselect hasSearchBox
                                items={groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </FormGroup>
                }

                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width pr-tracking-search-form">
                        <Multiselect hasSearchBox
                            items={employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>No PR</ControlLabel>
                    <div className="control-content select-noPR">
                        <Select name="noPR"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.noPR}
                            options={noPROptions}
                            onChange={this.handleNoPRChange}
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