import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';

import getResource from '../../../resourceHelper';

export default class SearchForm extends Component {
    handleGroupIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handleEvaluatedStatusIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handlePAStatusIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handleSubmit = e => {
        e.preventDefault();
        this.props.onSubmit();
    }

    renderGroupOption = option => <option key={option.value} value={option.value}>{option.label}</option>

    renderPerformanceStatusOption = option => <option key={option.id} value={option.id}>{option.name}</option>

    render() {
        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content">
                        <select value={this.props.filter.groupId} name="groupId" className="form-control" onChange={this.handleGroupIdChange}>
                            <option value="">-- All --</option>
                            {this.props.groups.map(this.renderGroupOption)}
                        </select>
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>Evaluated Status</ControlLabel>
                    <div className="control-content">
                        <select value={this.props.filter.evaluatedStatusId} name="evaluatedStatusId" className="form-control" onChange={this.handleEvaluatedStatusIdChange}>
                            <option value="">-- All --</option>
                            <option value="true">Evaluated</option>
                            <option value="false">Not Evaluated</option>
                        </select>
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>PA Status</ControlLabel>
                    <div className="control-content">
                        <select value={this.props.filter.paStatusId} name="paStatusId" className="form-control" onChange={this.handlePAStatusIdChange}>
                            <option value="">-- All --</option>performanceStatuses
                                {this.props.performanceStatuses.map(this.renderPerformanceStatusOption)}
                        </select>
                    </div>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}