import React, { Component } from 'react';
import { Form, ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect';

export default class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.filter !== nextProps.filter
            || this.props.periodOptions !== nextProps.periodOptions
            || this.props.groupOptions !== nextProps.groupOptions) {
            return true;
        }

        return false;
    }

    handlePeriodChange = selectedOption => {
        this.props.onChange('period', selectedOption);
    };

    handleGroupChange = selectedOptions => {
        this.props.onChange('groups', selectedOptions);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        const { filter, periodOptions, groupOptions } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('EvaluationPeriod')}</ControlLabel>
                    <div className="control-content">
                        <Select name="Year" isSearchable={false} isClearable={false}
                            className="react-select-container" classNamePrefix="react-select"
                            value={filter.period}
                            options={periodOptions}
                            onChange={this.handlePeriodChange}
                        />
                    </div>
                </FormGroup>
                {
                    groupOptions && groupOptions.length > 1 &&
                    <FormGroup>
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content">
                            <Multiselect hasSearchBox
                                items={groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </FormGroup>
                }
                <FormGroup>
                    <button className="btn btn-primary">{getResource('Search')}</button>
                </FormGroup>
            </Form>
        );
    }
}