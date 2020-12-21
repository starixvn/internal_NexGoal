import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    handleStatusChange = selectedOption => {
        this.props.onFilterChange('status', selectedOption);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onFilterSubmit();
    };

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select name="status" className="react-select-container" classNamePrefix="react-select"
                            value={filter.status} options={this.props.statusOptions} onChange={this.handleStatusChange}
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