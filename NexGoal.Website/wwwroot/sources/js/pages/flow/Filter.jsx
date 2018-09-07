import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../resourceHelper';
import { StatusOptions } from '../../constants';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleTargetTypeChange = this.handleTargetTypeChange.bind(this);
        this.handleStatusChange = this.handleStatusChange.bind(this);
        this.handleOnSubmit = this.handleOnSubmit.bind(this);
    }

    handleTargetTypeChange(selectedOption) {
        this.props.filter.targetTypeId = selectedOption.value.toString();
        this.props.onFilterChange(this.props.filter);
    }

    handleStatusChange(selectedOption) {
        this.props.filter.isActive = selectedOption.value;
        this.props.onFilterChange(this.props.filter);
    }

    handleOnSubmit(event) {
        event.preventDefault();
        this.props.onSubmit();
    }

    render() {
        const filter = this.props.filter;
        let targetTypeOptions = this.props.targetTypes.map((targetType) => {
            return { value: targetType.targetTypeId.toString(), label: targetType.name };
        });
        targetTypeOptions.unshift({ value: '', label: '-- All --' });
        const targetTypeSelected = targetTypeOptions.find((option) => { return option.value === filter.targetTypeId.toString(); });

        const statusSelected = StatusOptions.find((option) => { return option.value === filter.isActive.toString(); });

        return (
            <form className="from-inline form-filter" onSubmit={this.handleOnSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>{getResource('TargetType')}</ControlLabel>
                        <div className="control-content">
                            <Select name="TargetTypeId" className="react-select-container" classNamePrefix="react-select"
                                value={targetTypeSelected} options={targetTypeOptions}
                                isSearchable={false} isClearable={false}
                                onChange={this.handleTargetTypeChange}
                            />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>{getResource('Status')}</ControlLabel>
                        <div className="control-content">
                            <Select name="IsActive" className="react-select-container" classNamePrefix="react-select"
                                value={statusSelected} options={StatusOptions}
                                isSearchable={false} isClearable={false}
                                onChange={this.handleStatusChange}
                            />
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