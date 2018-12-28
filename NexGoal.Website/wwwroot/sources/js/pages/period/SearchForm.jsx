import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    handleStatusChange = event => this.props.onStatusChange(event.target.value)

    render() {
        return (
            <Form inline className="form-filter" onSubmit={this.props.onSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <select value={this.props.isActive} name="isActive" className="form-control" onChange={this.handleStatusChange}>
                            <option value="">-- All --</option>
                            <option value="true">Active</option>
                            <option value="false">Inactive</option>
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