import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import MultiSelect from '../../../shared/components/multi-select/index';
import getResource from '../../../resourceHelper';
import { PAEditOptions } from '../../../constants';

export default class Filter extends Component {
    handlePeriodChange = (selectedOption) => {
        this.props.onFilterChange('period', selectedOption);
    }

    handleGroupChange = (selectedOption) => {
        this.props.onFilterChange('groups', selectedOption);
    }

    handleStatusChange = (selectedOption) => {
        this.props.onFilterChange('status', selectedOption);
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
                    <ControlLabel>{getResource('IsRevised')}</ControlLabel>
                    <div className="control-content">
                        <Select className="react-select-container" classNamePrefix="react-select"
                            name="status"
                            value={filter.status}
                            options={PAEditOptions}
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