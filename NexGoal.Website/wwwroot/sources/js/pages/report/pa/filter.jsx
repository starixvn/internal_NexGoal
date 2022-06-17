import React, { Component } from 'react';
import Select from 'react-select';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Multiselect from '../../../shared/components/react-multiselect/index';
import getResource from '../../../resourceHelper';
import { PAEditOptions } from '../../../constants';

export default class Filter extends Component {
    handlePeriodChange = selectedOptions => {
        this.props.onFilterChange('periods', selectedOptions);
    };

    handleGroupChange = selectedOptions => {
        this.props.onFilterChange('groups', selectedOptions);
    };

    handleEmployeeChange = selectedOptions => {
        this.props.onFilterChange('employees', selectedOptions);
    };

    handleIsRevisedChange = selectedOption => {
        this.props.onFilterChange('isRevised', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        const { filter, periodOptions, groupOptions, employeeOptions, displayMemberFilter } = this.props;
        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Period')}</ControlLabel>
                    <div className="control-content">
                        <Multiselect
                            items={periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                {
                    groupOptions.length > 1 &&
                    <FormGroup>
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content team-width">
                            <Multiselect hasSearchBox
                                items={groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </FormGroup>
                }
                {
                    groupOptions.length > 0 && displayMemberFilter &&
                    <FormGroup>
                        <ControlLabel>{getResource('Employee')}</ControlLabel>
                        <div className="control-content employee-width">
                            <Multiselect hasSearchBox
                                items={employeeOptions}
                                onChange={this.handleEmployeeChange}
                            />
                        </div>
                    </FormGroup>
                }
                {
                    groupOptions.length > 0 && displayMemberFilter &&
                    <FormGroup>
                        <ControlLabel>{getResource('IsRevised')}</ControlLabel>
                        <div className="control-content">
                            <Select className="react-select-container" classNamePrefix="react-select"
                                name="status" isSearchable={false}
                                value={filter.isRevised}
                                options={PAEditOptions}
                                onChange={this.handleIsRevisedChange}
                            />
                        </div>
                    </FormGroup>
                }
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
                <FormGroup>
                    <Button type="button" bsStyle="excel" onClick={this.props.exportExcel}>{getResource('Excel')}</Button>
                </FormGroup>
            </Form>
        );
    }
}