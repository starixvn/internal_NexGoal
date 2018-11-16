import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handlePeriodChange = (selectedOption) => {
        this.props.onFilterChange('nominationPeriod', selectedOption);
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.props.onFilterSubmit();
    }

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Period')}</ControlLabel>
                    <div className="control-content">
                        <Select name="nominationPeriod" className="react-select-container" classNamePrefix="react-select"
                            value={filter.nominationPeriod} options={this.props.periodOptions}
                            isClearable={false}
                            onChange={this.handlePeriodChange}
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