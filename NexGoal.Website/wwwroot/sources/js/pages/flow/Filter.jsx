import React, { Component } from 'react';
import { ControlLabel, FormGroup } from 'react-bootstrap';
import Select from 'react-select';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.handleTargetTypeChange = this.handleTargetTypeChange.bind(this);
        this.handleStatusChange = this.handleStatusChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleTargetTypeChange(selectedOption) {
        this.props.filter.targetTypeId = selectedOption.value.toString();
        this.props.handleFilterChange(this.props.filter);
    }

    handleStatusChange(selectedOption) {
        this.props.filter.isActive = selectedOption.value;
        this.props.handleFilterChange(this.props.filter);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleFilterSubmit();
    }

    render() {
        const filter = this.props.filter;
        const targetTypeOptions = this.props.targetTypes.map((targetType) => {
            return { value: targetType.targetTypeId.toString(), label: targetType.name };
        });
        targetTypeOptions.unshift({ value: '', label: '-- All --' });
        const targetTypeSelected = targetTypeOptions.find((option) => { return option.value === filter.targetTypeId.toString(); });

        const statusOptions = [
            { value: '', label: '-- All --' },
            { value: 'true', label: 'Active' },
            { value: 'false', label: 'Inactive' }
        ];
        const statusSelected = statusOptions.find((option) => { return option.value === filter.isActive.toString() });

        return (
            <form className="from-inline form-filter" onSubmit={this.handleSubmit}>
                <FormGroup>
                    <div className="col">
                        <ControlLabel>Type</ControlLabel>
                        <div className="control-content">
                            <Select name="TargetTypeId"
                                value={targetTypeSelected} options={targetTypeOptions}
                                clearable={false} searchable={false}
                                onChange={this.handleTargetTypeChange} />
                        </div>
                    </div>
                    <div className="col">
                        <ControlLabel>Status</ControlLabel>
                        <div className="control-content">
                            <Select name="IsActive"
                                value={statusSelected} options={statusOptions}
                                clearable={false} searchable={false}
                                onChange={this.handleStatusChange} />
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