import React, { Component } from 'react';
import { Form, FormGroup, ControlLabel, Button } from 'react-bootstrap';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import Multiselect from '../../../shared/components/react-multiselect/index';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
        const NoPROptions = [
            { value: 2, text: 'Self' },
            { value: 3, text: 'Project Manager' },
            { value: 4, text: 'Evaluator' },
            { value: 5, text: 'Reviewer' }
        ];

        this.state = {
            filter: {
                groups: props.groupOptions,
                employees: props.employeeOptions,
                noPR: null
            },
        employees : props.employees,
        employeeOptions: props.employeeOptions
        };

        this.noPROptions = NoPROptions.map(ns => ({ value: ns.value, label: ns.text }));
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            const employeeChoosens = this.state.employees.filter(e => value.some(v => v.id === e.groupId))
                .map(e => ({
                    id: e.employeeId,
                    text: `${e.nickName} (${e.fullName})`,
                    checked: true
                }));

            this.setState({
                employeeOptions: employeeChoosens,
                filter: {
                    ...this.state.filter,
                    [name]: value,
                    employees: employeeChoosens
                }
            });
        } else {
            this.setState({
                filter: {
                    ...this.state.filter,
                    [name]: value
                }
            });
        }
    };

    handleGroupChange = selectedOption => this.handleFilterChange('groups', selectedOption);

    handleEmployeeChange = selectedOption => { this.handleFilterChange('employees', selectedOption);};

    handleNoPRChange = selectedOption => { this.handleFilterChange('noPR', selectedOption);};

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit(this.state.filter);
    };

    render() {
        const { groupOptions } = this.props;
        return (
            <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                {
                    groupOptions.length > 1 &&
                    <FormGroup>
                        <ControlLabel>{getResource('Team')}</ControlLabel>
                        <div className="control-content team-width pr-tracking-search-form">
                            <Multiselect hasSearchBox
                                items={groupOptions}
                                onChange={this.handleGroupChange}
                            />
                        </div>
                    </FormGroup>
                }
                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    <div className="control-content employee-width pr-tracking-search-form">
                        <Multiselect hasSearchBox
                            items={this.state.employeeOptions}
                            onChange={this.handleEmployeeChange}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>No PR</ControlLabel>
                    <div className="control-content select-noPR">
                        <Select name="noPR"
                            className="react-select-container" classNamePrefix="react-select"
                            value={this.state.filter.noPR}
                            options={this.noPROptions}
                            onChange={this.handleNoPRChange} isClearable
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