import React, { Component } from 'react';
import Select from 'react-select';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    handleStatusChange = selectedOption => this.props.onStatusChange(selectedOption)

    render() {
        return (
            <Form inline className="form-filter" onSubmit={this.props.onSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select value={this.props.filter.isActive} name="isActive" onChange={this.handleStatusChange}
                            className="react-select-container" classNamePrefix="react-select"
                            options={this.props.statusOptions}
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