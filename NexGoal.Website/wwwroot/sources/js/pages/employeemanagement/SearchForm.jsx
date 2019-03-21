import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';

import Multiselect from '../../shared/components/multi-select/multi-select';
import getResource from '../../resourceHelper';

export default class SearchForm extends Component {
    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.filter) !== JSON.stringify(nextProps.filter)
            || JSON.stringify(this.props.groupOptions) !== JSON.stringify(nextProps.groupOptions)
            || JSON.stringify(this.props.roleOptions) !== JSON.stringify(nextProps.roleOptions)) {
            return true;
        }

        return false;
    }

    handleKeywordChange = event => {
        this.props.onChange('keyword', event.target.value);
    }

    handleGroupChange = selectedOption => {
        this.props.onChange('group', selectedOption);
    }

    handleRoleChange = selectedOptions => {
        this.props.onChange('roles', selectedOptions);
    }

    handleCheckboxChange = event => {
        this.props.onChange('isDisabled', event.target.checked);
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    }

    render() {
        const filter = this.props.filter;

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
                            value={filter.group} options={this.props.groupOptions}
                            isClearable={false}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Role')}</ControlLabel>
                    <div className="control-content">
                        <Multiselect name="roles" className="react-select-container" classNamePrefix="react-select"
                            value={filter.roles} options={this.props.roleOptions}
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