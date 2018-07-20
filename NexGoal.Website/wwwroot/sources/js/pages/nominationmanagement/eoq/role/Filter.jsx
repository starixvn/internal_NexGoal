import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import { nominationRoleOptions } from '../../common';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.onKeywordChange = this.onKeywordChange.bind(this);
        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleRoleChange = this.handleRoleChange.bind(this);
        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    onKeywordChange(event) {
        this.props.filter.keyword = event.target.value;
        this.props.handleFilterChange(this.props.filter);
    }

    handleGroupChange(event) {
        this.props.filter.groupIds = event.target.value;
        this.props.handleFilterChange(this.props.filter);
    }

    handleRoleChange(selectedOption) {
        this.props.filter.roleId = selectedOption.value;
        this.props.handleFilterChange(this.props.filter);
    }

    handleCheckboxChange(event) {
        this.props.filter.isShowAll = event.target.checked;
        this.props.handleFilterChange(this.props.filter);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        const filter = this.props.filter;

        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>Employee</ControlLabel>
                        <div className="control-content">
                            <input name="Keyword" className="form-control" type="text"
                                autoComplete="off"
                                placeholder="Full name or Nick name"
                                onChange={this.onKeywordChange} />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>Team</ControlLabel>
                        <div className="control-content">
                            <select className="form-control" name="GroupIds" value={filter.groupId}
                                onChange={this.handleGroupChange}>
                                <option value="">-- All --</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>Roles</ControlLabel>
                        <div className="control-content">
                            <Select name="RoleId"
                                value={nominationRoleOptions[0]}
                                options={nominationRoleOptions}
                                clearable={false}
                                searchable={false}
                                onChange={this.handleRoleChange} />
                        </div>
                    </div>
                    <div className="col">
                        <div className="checkbox">
                            <label>
                                <input type="checkbox" checked={filter.isShowAll}
                                    onChange={this.handleCheckboxChange} /> Show All Employee
                            </label>
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