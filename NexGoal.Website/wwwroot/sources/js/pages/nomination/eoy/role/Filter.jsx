import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import MultiSelect from '../../../../shared/components/multi-select/multi-select';
import getResource from '../../../../resourceHelper';

class Filter extends Component {
    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.filter) !== JSON.stringify(nextProps.filter)
            || JSON.stringify(this.props.groupOptions) !== JSON.stringify(nextProps.groupOptions)
            || JSON.stringify(this.props.nominationRoleOptions) !== JSON.stringify(nextProps.nominationRoleOptions)) {
            return true;
        }

        return false;
    }

    onKeywordChange = event => {
        this.props.onChange('keyword', event.target.value);
    }

    handleGroupChange = selectedOptions => {
        this.props.onChange('groups', selectedOptions);
    }

    handleRoleChange = selectedOption => {
        this.props.onChange('role', selectedOption);
    }

    handleCheckboxChange = event => {
        this.props.onChange('isShowAll', event.target.checked);
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
                    <div className="control-content">
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