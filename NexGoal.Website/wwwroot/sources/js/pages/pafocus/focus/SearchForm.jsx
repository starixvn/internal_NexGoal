import React, { Component } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/multi-select/index';

export default class SearchForm extends Component {
    handlePeriodChange = (selectedOption) => {
        this.props.onChange('period', selectedOption);
    }

    handleGroupChange = (selectedOptions) => {
        this.props.onChange('groups', selectedOptions);
    }

    handleEmployeeChange = (selectedOption) => {
        this.props.onChange('employee', selectedOption);
    }

    handleExportToExcel = (event) => {
        event.preventDefault();
        this.props.onExportToExcel();
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.props.onSubmit();
    }

    render() {
        const filter = this.props.filter;
        const periodOptions = this.props.periodOptions;
        const groupOptions = this.props.groupOptions;
        const memberOptions = this.props.memberOptions;

        return (
            <Form inline className="form-filter">
                <FormGroup>
                    <ControlLabel>{getResource('EvaluationPeriod')}</ControlLabel>
                    <div className="control-content">
                        <Select name="Year" isSearchable={false} isClearable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.period}
                            options={periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content">
                        <Multiselect className="react-select-container" classNamePrefix="react-select"
                            value={filter.groups}
                            options={groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content">
                        <Select className="react-select-container" classNamePrefix="react-select"
                            value={filter.employee} options={memberOptions}
                            name="employee" onChange={this.handleEmployeeChange} isClearable
                        />
                    </div>
                </FormGroup>
                <FormGroup className="button-block">
                    <button className="btn btn-primary" onClick={this.handleSubmit}>{getResource('Search')}</button>
                    <button className="btn btn-excel" onClick={this.handleExportToExcel}>{getResource('Excel')}</button>
                </FormGroup>
            </Form>
        );
    }
}