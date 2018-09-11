import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleStatusChange = event => this.props.onStatusChange(event.target.value)

    render() {
        return (
            <Form inline className="form-filter" onSubmit={this.props.onSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>{getResource('Status')}</ControlLabel>
                        <div className="control-content">
                            <select value={this.props.isActive} name="isActive" className="form-control" onChange={this.handleStatusChange}>
                                <option value="">-- All --</option>
                                <option value="true">Active</option>
                                <option value="false">Inactive</option>
                            </select>
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