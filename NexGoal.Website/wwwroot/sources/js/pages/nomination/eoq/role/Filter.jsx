import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import isEqual from 'react-fast-compare';

import MultiSelect from '../../../../shared/components/multi-select/multi-select';
import getResource from '../../../../resourceHelper';

class Filter extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    handleGroupChange = selectedOptions => {
        this.props.onChange('groups', selectedOptions);
    }

    onKeywordChange = event => {
        this.props.onChange('keyword', event.target.value);
    }

    handleRoleChange = selectedOption => {
        this.props.onChange('role', selectedOption);
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    }
    handleCheckboxChange = event => {
        this.props.onChange('isShowAll', event.target.checked);
    }

    render() {
        const filter = this.props.filter;

        return (
            <Form inline className="form-filter filter-role-management" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content">
                        <input name="Keyword" className="form-control" type="text"
                            autoComplete="off"
                            placeholder={getResource('NicknameOrFullName')}
                            onChange={this.onKeywordChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Team')}</ControlLabel>
                    <div className="control-content team-width">
                        <MultiSelect className="react-select-container" classNamePrefix="react-select"
                            value={filter.groups}
                            options={this.props.groupOptions}
                            onChange={this.handleGroupChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Roles')}</ControlLabel>
                    <div className="control-content">
                        <Select name="RoleId" className="react-select-container" classNamePrefix="react-select"
                            value={filter.role}
                            options={this.props.nominationRoleOptions}
                            isSearchable={false} isClearable={false}
                            onChange={this.handleRoleChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <div className="checkbox">
                        <input type="checkbox" checked={filter.isShowAll} className="checkbox-custom"
                            onChange={this.handleCheckboxChange} id="chbShowAllEmp"
                        />
                        <label htmlFor="chbShowAllEmp">{getResource('ShowAllEmployee')}</label>
                    </div>
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                </FormGroup>
            </Form>
        );
    }
}

export default Filter;