import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { Async } from 'react-select';
import Select from 'react-select';

import { StatusOptions } from '../../constants';
import Multiselect from '../../shared/components/MultiSelect';
import getResource from '../../resourceHelper';

class SearchForm extends Component {
    constructor(props) {
        super(props);

        this.onFromDateChange = this.onFromDateChange.bind(this);
        this.onToDateChange = this.onToDateChange.bind(this);
        this.onChangeRaw = this.onChangeRaw.bind(this);
        this.handleStatusChange = this.handleStatusChange.bind(this);
        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleEmployeeChange = this.handleEmployeeChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    onFromDateChange(date) {
        this.props.onFilterChange('from', date);
    }

    onToDateChange(date) {
        this.props.onFilterChange('to', date);
    }

    onChangeRaw(event) {
        let date = moment(event.target.value);
        if (date.isValid()) {
            this.props.onFilterChange(event.target.name, date);
        }
    }

    handleStatusChange(selectedOption) {
        this.props.onFilterChange('isActive', selectedOption.value);
    }

    handleGroupChange(selectedOptions) {
        this.props.onFilterChange('groups', selectedOptions);
    }

    handleEmployeeChange = (selectedOption) => {
        this.props.onFilterChange('employee', selectedOption);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.onFilterSubmit();
    }

    render() {
        const filter = this.props.filter;
        const statusSelected = StatusOptions.find((option) => { return option.value === filter.isActive.toString(); });

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col" >
                        <ControlLabel>{getResource('From')}</ControlLabel>
                        <div className="control-content">
                            <DatePicker dateFormat="DD MMM, YYYY" selected={filter.from}
                                onChange={this.onFromDateChange} onChangeRaw={this.onChangeRaw}
                                name="from" className="form-control" autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="col" >
                        <ControlLabel>{getResource('To')}</ControlLabel>
                        <div className="control-content">
                            <DatePicker dateFormat="DD MMM, YYYY" selected={filter.to}
                                onChange={this.onToDateChange} onChangeRaw={this.onChangeRaw}
                                name="to" className="form-control" autoComplete="off"
                            />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>{getResource('Status')}</ControlLabel>
                        <div className="control-content">
                            <div className="control-content">
                                <Select name="isActive" className="react-select-container" classNamePrefix="react-select"
                                    value={statusSelected} options={StatusOptions}
                                    isSearchable={false} isClearable={false}
                                    onChange={this.handleStatusChange}
                                />
                            </div>
                        </div>
                    </div>
                </FormGroup>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content">
                            <Multiselect className="react-select-container" classNamePrefix="react-select"
                                value={filter.groups}
                                closeMenuOnSelect={false}
                                options={this.props.groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>{getResource('Employee')}</ControlLabel>
                        <div className="control-content">
                            <Async className="react-select-container" classNamePrefix="react-select"
                                cacheOptions value={filter.employee}
                                name="employee" onChange={this.handleEmployeeChange}
                                loadOptions={this.props.searchEmployees}
                                placeholder="-- Search an employee --"
                            />
                        </div>
                    </div>
                    <div className="col">
                        <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                    </div>
                </FormGroup>
            </Form>
        );
    }
}

export default SearchForm;