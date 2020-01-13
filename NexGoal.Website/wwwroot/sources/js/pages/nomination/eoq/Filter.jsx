import React, { Component } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import isEqual from 'react-fast-compare';

import { statusOptions } from '../common';
import getResource from '../../../resourceHelper';

class Filter extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    handleYearChange = selectedOption => {
        this.props.handleYearChange(selectedOption.value);
    }

    handleStatusChange = selectedOption => {
        this.props.handleStatusChange(selectedOption.value);
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        const yearOptions = this.props.years.map(year => {
            return { value: year.toString(), label: year };
        });
        yearOptions.unshift({ value: '', label: '-- All --' });
        const yearSelected = yearOptions.find(option => option.value === this.props.year.toString());

        const statusSelected = statusOptions.find(option => option.value === this.props.statusId.toString());

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Year')}</ControlLabel>
                    <div className="control-content">
                        <Select name="Year" isSearchable={false} isClearable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={yearSelected}
                            options={yearOptions}
                            onChange={this.handleYearChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select name="StatusId" isSearchable={false} isClearable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={statusSelected}
                            options={statusOptions}
                            onChange={this.handleStatusChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <button className="btn btn-primary">{getResource('Search')}</button>
                </FormGroup>
            </Form>
        );
    }
}

export default Filter;