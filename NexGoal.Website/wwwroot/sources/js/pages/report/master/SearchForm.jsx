import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect/index';

export default class SearchForm extends Component {
    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    };

    handlePeriodChange = selectedOption => {
        this.props.onFilterChange('period', selectedOption);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
    };

    render() {
        const { filter, periodOptions, groupOptions, employeeOptions, exportUrl } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-period-label">{getResource('Period')}</ControlLabel>
                    <div className="control-content period-width">
                        <Select name="period"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.period}
                            options={periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-team-label">{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <Multiselect hasSearchBox
                            items={groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel htmlFor="master-report-employee-label">
                        {getResource('Employee')}
                    </ControlLabel>
                    <div className="control-content employee-width">
                        <Multiselect hasSearchBox
                            items={employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <a className="btn btn-excel" href={exportUrl} >{getResource('Excel')}</a>
                </FormGroup>
            </Form>
        );
    }
}