import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import MultiSelect from '../../../../shared/components/multi-select/MultiSelect';
import getResource from '../../../../resourceHelper';

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

    handleGroupChange(selectedOptions) {
        this.props.filter.groups = selectedOptions;
        this.props.handleFilterChange(this.props.filter);
    }

    handleRoleChange(selectedOption) {
        this.props.filter.role = selectedOption;
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
            <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>{getResource('Employee')}</ControlLabel>
                        <div className="control-content">
                            <input name="Keyword" className="form-control" type="text"
                                autoComplete="off"
                                placeholder="Full name or Nick name"
                                onChange={this.onKeywordChange}
                            />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content">
                            <MultiSelect className="react-select-container" classNamePrefix="react-select"
                                value={filter.groups}
                                options={this.props.groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>{getResource('Roles')}</ControlLabel>
                        <div className="control-content">
                            <Select name="RoleId" className="react-select-container" classNamePrefix="react-select"
                                value={filter.role}
                                options={this.props.nominationRoleOptions}
                                isSearchable={false} isClearable={false}
                                onChange={this.handleRoleChange}
                            />
                        </div>
                    </div>
                    <div className="col">
                        <div className="checkbox">
                            <label>
                                <input type="checkbox" checked={filter.isShowAll}
                                    onChange={this.handleCheckboxChange}
                                /> {getResource('ShowAllEmployee')}
                            </label>
                        </div>
                    </div>
                    <div className="col">
                        <button className="btn btn-primary">{getResource('Search')}</button>
                    </div>
                </FormGroup>
            </form>
        );
    }
}

export default Filter;