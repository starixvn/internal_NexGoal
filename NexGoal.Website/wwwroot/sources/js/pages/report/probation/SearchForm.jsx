import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel } from 'react-bootstrap';
import Select from 'react-select';
import DatePicker from 'react-datepicker';

import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';
import MultiSelect from '../../../shared/components/multi-select/multi-select';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleGroupChange = selectedOption => {
        this.props.onFilterChange('groups', selectedOption);
    }

    handleEmployeeChange = selectedOption => {
        this.props.onFilterChange('employees', selectedOption);
    }

    handleFromDateChange = date => {
        this.props.onFilterChange('fromDate', date);
    }

    handleToDateChange = date => {
        this.props.onFilterChange('toDate', date);
    }

    handleSubmit = event => {
        event.preventDefault();
    }

    render() {
        const filter = this.props.filter;

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
                        <MultiSelect
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.employees}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <button type="submit" className="btn btn-primary">{getResource('Search')}</button>
                </FormGroup>
                <FormGroup>
                    <a className="btn btn-excel" href={this.props.exportUrl} >{getResource('Excel')}</a>
                </FormGroup>
            </Form>
        );
    }
}