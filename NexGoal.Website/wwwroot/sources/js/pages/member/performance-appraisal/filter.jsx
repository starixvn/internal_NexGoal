import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import MultiSelect from '../../../shared/components/multi-select/index';
import getResource from '../../../resourceHelper';
import { StatusOptions } from '../../../constants';

export default class Filter extends Component {
    constructor(props) {
        super(props);
    }

    handlePeriodChange = (selectedOption) => {
        this.props.onFilterChange('period', selectedOption);
    }

    handleGroupChange = (selectedOption) => {
        this.props.onFilterChange('groups', selectedOption);
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
                    <ControlLabel>{getResource('Period')}</ControlLabel>
                    <div className="control-content">
                        <Select className="react-select-container" classNamePrefix="react-select"
                            name="period"
                            value={filter.period}
                            options={this.props.periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content team">
                        <MultiSelect className="react-select-container" classNamePrefix="react-select"
                            name="groups"
                            value={filter.groups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select name="IsActive" className="react-select-container" classNamePrefix="react-select"
                            value={statusSelected} options={StatusOptions}
                            isSearchable={false} isClearable={false}
                            onChange={this.handleStatusChange}
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