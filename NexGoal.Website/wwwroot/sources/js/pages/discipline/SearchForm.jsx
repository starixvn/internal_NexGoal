import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import Select from 'react-select';

import { StatusOptions } from '../../constants';
import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

class SearchForm extends Component {
    onFromDateChange = (date) => {
        this.props.onChange('from', date);
    }

    onToDateChange = (date) => {
        this.props.onChange('to', date);
    }

    onChangeRaw = (event) => {
        let date = moment(event.target.value);
        if (date.isValid()) {
            this.props.onChange(event.target.name, date);
        }
    }

    handleStatusChange = (selectedOption) => {
        this.props.onChange('isActive', selectedOption);
    }

    handleGroupChange = (selectedOptions) => {
        this.props.onChange('group', selectedOptions);
    }

    handleEmployeeChange = (selectedOption) => {
        this.props.onChange('employee', selectedOption);
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
                    <ControlLabel>{getResource('From')}</ControlLabel>
                    <div className="control-content">
                        <DatePicker dateFormat={DateFormat} selected={filter.from}
                            onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                            name="from" className="form-control" autoComplete="off"
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('To')}</ControlLabel>
                    <div className="control-content">
                        <DatePicker dateFormat={DateFormat} selected={filter.to}
                            onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                            name="to" className="form-control" autoComplete="off"
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <div className="control-content">
                            <Select name="isActive"
                                className="react-select-container" classNamePrefix="react-select"
                                value={filter.isActive}
                                options={StatusOptions}
                                isSearchable={false} isClearable={false}
                                onChange={this.handleStatusChange}
                            />
                        </div>
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content">
                        <Select name="group"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.group}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content">
                        <Select name="employee"
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.employee}
                            options={this.props.employeeOptions}
                            onChange={this.handleEmployeeChange} isClearable
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

export default SearchForm;