import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';
import isEqual from 'react-fast-compare';

import Multiselect from '../../shared/components/react-multiselect/index';
import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    handleKeywordChange = event => {
        this.props.onChange('keyword', event.target.value);
    };

    handleGroupChange = selectedOption => {
        this.props.onChange('group', selectedOption);
    };

    handleRoleChange = selectedOptions => {
        this.props.onChange('roles', selectedOptions);
    };

    handleCheckboxChange = event => {
        this.props.onChange('isDisabled', event.target.checked);
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        const { filter, groupOptions, roleOptions } = this.props;

        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <FormControl name="keyword" type="text"
                        value={filter.keyword}
                        placeholder={getResource('NicknameOrFullName')} autoComplete="off"
                        onChange={this.handleKeywordChange}
                    />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <Select name="groupId" className="react-select-container" classNamePrefix="react-select"
                            value={filter.group} options={groupOptions}
                            isClearable={false}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Role')}</ControlLabel>
                    <div className="control-content">
                        <Multiselect hasSearchBox
                            items={roleOptions}
                            onChange={this.handleRoleChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <div className="checkbox">
                        <input type="checkbox" name="isDisabled"
                            checked={filter.isDisabled}
                            onChange={this.handleCheckboxChange} id="chbIsResigned" className="checkbox-custom"
                        />
                        <label htmlFor="chbIsResigned">Is Resigned</label>
                    </div>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}