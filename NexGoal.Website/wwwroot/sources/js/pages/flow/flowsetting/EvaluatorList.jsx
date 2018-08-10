import React, { Component } from 'react';
import axios from 'axios';
import Select from 'react-select';

import commonFunctions from '../../../common-functions';
import Draggable from './Draggable';

class EvaluatorList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSelected: { value: '', label: '-- All --' },
            keyword: '',
            employees: [],
            filteredEmployees: []
        };

        this.handleOnGroupChange = this.handleOnGroupChange.bind(this);
        this.handleOnKeywordChange = this.handleOnKeywordChange.bind(this);
    }

    componentWillMount() {
        this.getEmployees(this.state.groupSelected.value);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.groupOptions !== nextProps.groupOptions
            || this.props.isHost !== nextProps.isHost
            || this.state.groupSelected !== nextState.groupSelected
            || this.state.keyword !== nextState.keyword
            || this.state.employees !== nextState.employees
            || this.state.filteredEmployees !== nextState.filteredEmployees) {
            return true;
        }

        return false;
    }

    getEmployees(groupId) {
        axios(commonFunctions.getRootUrl() + `Common/SearchEmployees?groupId=${groupId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    employees: data,
                    filteredEmployees: data
                });
            }).catch(commonFunctions.axiosError);
    }

    handleOnGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption,
            keyword: ''
        });

        this.getEmployees(selectedOption.value);
    }

    handleOnKeywordChange(event) {
        let employees;
        if (event.target.value && event.target.value.trim().length > 1) {
            const value = event.target.value.trim().toLowerCase();

            employees = this.state.employees.slice().filter((employee) =>
                employee.nickName.toLowerCase().indexOf(value) >= 0
                || employee.fullName.toLowerCase().indexOf(value) >= 0
            );
        } else {
            employees = this.state.employees.slice();
        }

        this.setState({
            keyword: event.target.value,
            filteredEmployees: employees
        });
    }

    render() {
        let groupOptions = this.props.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const employees = this.state.filteredEmployees;

        return (
            <div className="evaluator-list">
                <div className="header">Employee</div>
                <div className="content">
                    <Select value={this.state.groupSelected} options={groupOptions}
                        clearable={false} onChange={this.handleOnGroupChange} />
                    <div className="text-search">
                        <input type="text" className="form-control" placeholder="full name or nick name"
                            value={this.state.keyword} onChange={this.handleOnKeywordChange} />
                        <span className="fa fa-search"></span>
                    </div>
                    <ul>
                        {
                            employees.map((employee) => {
                                return (
                                    <Draggable key={'employee-' + employee.employeeId} enable={this.props.isHost}
                                        id={employee.employeeId}
                                        label={`${employee.nickName} (${employee.fullName})`}
                                        connectToSortable=".sortable" />)
                            })
                        }
                    </ul>
                </div>
            </div>
        );
    }
}

export default EvaluatorList;