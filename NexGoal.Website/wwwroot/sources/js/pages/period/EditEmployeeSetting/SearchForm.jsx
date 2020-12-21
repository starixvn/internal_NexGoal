import React, { Component } from 'react';
import Select from 'react-select';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';

import getResource from '../../../resourceHelper';

export default class SearchForm extends Component {
    handleGroupChange = selectedOption => this.props.onFilterChange('group', selectedOption);

    handleEvaluatedStatusChange = selectedOption => this.props.onFilterChange('isEvaluated', selectedOption);

    handlePAStatusChange = selectedOption => this.props.onFilterChange('paStatus', selectedOption);

    handleSubmit = e => {
        e.preventDefault();
        this.props.onSubmit();
    };

    renderGroupOption = option => <option key={option.value} value={option.value}>{option.label}</option>;

    renderPerformanceStatusOption = option => <option key={option.id} value={option.id}>{option.name}</option>;

    render() {
        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content">
                        <Select name="group" className="react-select-container" classNamePrefix="react-select"
                            value={this.props.filter.group}
                            options={this.props.groups}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>Evaluated Status</ControlLabel>
                    <div className="control-content">
                        <Select name="isEvaluated" className="react-select-container" classNamePrefix="react-select"
                            value={this.props.filter.isEvaluated}
                            options={this.props.evaluateStatuses}
                            onChange={this.handleEvaluatedStatusChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>PA Status</ControlLabel>
                    <div className="control-content">
                        <Select name="paStatus" className="react-select-container" classNamePrefix="react-select"
                            value={this.props.filter.paStatus}
                            options={this.props.performanceStatuses}
                            onChange={this.handlePAStatusChange}
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