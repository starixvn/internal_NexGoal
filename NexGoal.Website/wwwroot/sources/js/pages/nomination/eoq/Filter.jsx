import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import { statusOptions, renderStatusOption } from '../common';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleYearChange = this.handleYearChange.bind(this);
        this.handleStatusChange = this.handleStatusChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleYearChange(selectedOption) {
        this.props.handleYearChange(selectedOption.value);
    }

    handleStatusChange(selectedOption) {
        this.props.handleStatusChange(selectedOption.value);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        let yearOptions = this.props.years.map((year) => {
            return { value: year.toString(), label: year };
        });
        yearOptions.unshift({ value: '', label: '-- All --' });
        const yearSelected = yearOptions.find((option) => { return option.value === this.props.year.toString(); });

        const statusSelected = statusOptions.find((option) => { return option.value === this.props.statusId.toString(); });

        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>Year</ControlLabel>
                        <div className="control-content">
                            <Select name="Year"
                                value={yearSelected}
                                options={yearOptions}
                                clearable={false}
                                searchable={false}
                                onChange={this.handleYearChange} />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>Status</ControlLabel>
                        <div className="control-content">
                            <Select name="StatusId"
                                value={statusSelected}
                                options={statusOptions}
                                optionRenderer={renderStatusOption}
                                valueRenderer={renderStatusOption}
                                clearable={false}
                                searchable={false}
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