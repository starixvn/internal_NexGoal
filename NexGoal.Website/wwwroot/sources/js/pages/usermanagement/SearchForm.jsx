import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';

class SearchForm extends Component {
    constructor(props) {
        super(props);

        this.handleKeywordChange = this.handleKeywordChange.bind(this);
        this.handleOnGroupChange = this.handleOnGroupChange.bind(this);
        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleKeywordChange = (event) => {
        this.props.onFilterChange('keyword', event.target.value);
    }

    handleOnGroupChange(selectedOption) {
        this.props.onFilterChange('group', selectedOption);
    }

    handleCheckboxChange = (event) => {
        this.props.onFilterChange('isDisabledUser', event.target.checked);
    }

    handleSubmit(event) {
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
                        placeholder="Full name or Nick name" autoComplete="off"
                        onChange={this.handleKeywordChange}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <Select name="groupId" className="react-select-container" classNamePrefix="react-select"
                        value={filter.group} options={this.props.groupOptions}
                        isClearable={false}
                        onChange={this.handleOnGroupChange}
                    />
                </FormGroup>
                <FormGroup>
                    <label>
                        <input type="checkbox" name="isDisabledUser"
                            checked={filter.isDisabledUser}
                            onChange={this.handleCheckboxChange}
                        /> Is Disabled User
                    </label>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}

export default SearchForm;