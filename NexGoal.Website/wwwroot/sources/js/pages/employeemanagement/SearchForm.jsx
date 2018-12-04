import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.filter) !== JSON.stringify(nextProps.filter)
            || JSON.stringify(this.props.groupOptions) !== JSON.stringify(nextProps.groupOptions)) {
            return true;
        }

        return false;
    }

    handleKeywordChange = (event) => {
        this.props.onFilterChange('keyword', event.target.value);
    }

    handleOnGroupChange = (selectedOption) => {
        this.props.onFilterChange('group', selectedOption);
    }

    handleCheckboxChange = (event) => {
        this.props.onFilterChange('isDisabled', event.target.checked);
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
                    <ControlLabel>{getResource('Keyword')}</ControlLabel>
                    <FormControl name="keyword" type="text"
                        value={filter.keyword}
                        placeholder="Full name or nickname" autoComplete="off"
                        onChange={this.handleKeywordChange}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content">
                        <Select name="groupId" className="react-select-container" classNamePrefix="react-select"
                            value={filter.group} options={this.props.groupOptions}
                            isClearable={false}
                            onChange={this.handleOnGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label>
                        <input type="checkbox" name="isDisabled"
                            checked={filter.isDisabled}
                            onChange={this.handleCheckboxChange}
                        /> Is Disabled
                    </label>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}