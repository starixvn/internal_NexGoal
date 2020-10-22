import React, { Component } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import { statusOptions } from '../common';
import getResource from '../../../resourceHelper';

class Filter extends Component {
    handleStatusChange = selectedOption => {
        this.props.handleStatusChange(selectedOption.value);
    }

    handleFilterSubmit = event => {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        const statusSelected = statusOptions.find(option => option.value === this.props.statusId.toString());

        return (
            <Form inline className="form-filter" onSubmit={this.handleFilterSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select name="StatusId"
                            isSearchable={false}
                            isClearable={false}
                            options={statusOptions}
                            onChange={this.handleStatusChange}
                            className="react-select-container" classNamePrefix="react-select"
                            value={statusSelected}
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