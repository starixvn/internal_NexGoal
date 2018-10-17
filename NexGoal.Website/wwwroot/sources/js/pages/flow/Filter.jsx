import React, { Component } from 'react';
import { Button, ControlLabel, Form, FormGroup } from 'react-bootstrap';
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
            <Form inline className="form-filter" onSubmit={this.handleOnSubmit}>
                <FormGroup>
                    <ControlLabel>{getResource('TargetType')}</ControlLabel>
                    <div className="control-content">
                        <Select name="TargetTypeId" className="react-select-container" classNamePrefix="react-select"
                            value={targetTypeSelected} options={targetTypeOptions}
                            isSearchable={false} isClearable={false}
                            onChange={this.handleTargetTypeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Status')}</ControlLabel>
                    <div className="control-content">
                        <Select name="IsActive" className="react-select-container" classNamePrefix="react-select"
                            value={statusSelected} options={StatusOptions}
                            isSearchable={false} isClearable={false}
                            onChange={this.handleStatusChange}
                        />
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