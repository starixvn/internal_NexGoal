import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';
import DatePicker from 'react-datepicker';

import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';
import Multiselect from '../../../shared/components/react-multiselect/index';

export default class SearchForm extends Component {
    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    };

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    };

    handleFromDateChange = date => {
        this.props.onFilterChange('fromDate', date);
    };

    handleToDateChange = date => {
        this.props.onFilterChange('toDate', date);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    handleEvaluatorResultChange = selectedOption => {
        this.props.onFilterChange('isQualifiedByEvaluator', selectedOption);
    };

    handleReviewerResultChange = selectedOption => {
        this.props.onFilterChange('isQualified', selectedOption);
    };

    render() {
        const { filter, groupOptions, employeeOptions, probationResultOptions, exportUrl } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('FromDate')}</ControlLabel>
                    <div className="control-content period-width">
                        <DatePicker autoComplete="off" dateFormat={DateFormat} selected={filter.fromDate}
                            onChange={this.handleFromDateChange}
                            name='fromDate' maxDate={filter.toDate}
                            className='form-control'
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('ToDate')}</ControlLabel>
                    <div className="control-content period-width">
                        <DatePicker autoComplete="off" dateFormat={DateFormat} selected={filter.toDate}
                            onChange={this.handleToDateChange}
                            name='toDate' minDate={filter.fromDate}
                            className='form-control'
                        />
                    </div>
                </FormGroup>
                {
                    this.props.showTeam &&
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
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width">
                        <Multiselect hasSearchBox
                            items={employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>Evaluator's Result</ControlLabel>
                    <div className="control-content">
                        <Select isSearchable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.isQualifiedByEvaluator}
                            options={probationResultOptions}
                            onChange={this.handleEvaluatorResultChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>Reviewer's Result</ControlLabel>
                    <div className="control-content">
                        <Select isSearchable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.isQualified}
                            options={probationResultOptions}
                            onChange={this.handleReviewerResultChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <button type="submit" className="btn btn-primary">{getResource('Search')}</button>
                </FormGroup>
                <FormGroup>
                    <a className="btn btn-excel" href={exportUrl} >{getResource('Excel')}</a>
                </FormGroup>
            </Form>
        );
    }
}