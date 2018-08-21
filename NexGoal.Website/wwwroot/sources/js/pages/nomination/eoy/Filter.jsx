import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import { statusOptions } from '../common';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleStatusChange = this.handleStatusChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleStatusChange(selectedOption) {
        this.props.handleStatusChange(selectedOption.value);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        const statusSelected = statusOptions.find((option) => { return option.value === this.props.statusId.toString(); });

        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>Status</ControlLabel>
                        <div className="control-content">
                            <Select name="StatusId" isSearchable={false} isClearable={false}
                                className="react-select-container" classNamePrefix="react-select"
                                value={statusSelected}
                                options={statusOptions}
                                onChange={this.handleStatusChange} />
                        </div>
                    </div>
                    <div className="col">
                        <button className="btn btn-primary">Search</button>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;