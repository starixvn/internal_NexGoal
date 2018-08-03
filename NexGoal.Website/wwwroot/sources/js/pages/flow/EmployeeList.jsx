import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import commonFunctions from '../../common-functions';

class EmployeeList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSelected: { value: '', label: '-- All --' },
            keyword: '',
            employees: [],
            filteredEmployees: []
        };

        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleKeywordChange = this.handleKeywordChange.bind(this);
    }

    componentWillMount() {
        this.getEmployees(this.state.groupSelected.value);
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

    handleGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption,
            keyword: ''
        });

        this.getEmployees(selectedOption.value);
    }

    handleKeywordChange(event) {
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

        this.setState({ filteredEmployees: employees });
    }

    render() {
        const groupOptions = this.props.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const employees = this.state.filteredEmployees;

        return (
            <div className="employee-list">
                <div className="header">Employee</div>
                <div className="content">
                    <Select name="" value={this.state.groupSelected} options={groupOptions}
                        clearable={false} onChange={this.handleGroupChange} />
                    <div className="search">
                        <input type="text" className="form-control" placeholder="full name or nick name"
                            value={this.state.keyword} onChange={this.handleKeywordChange} />
                        <span className="fa fa-search"></span>
                    </div>
                    <ul>
                        {employees.map((employee) => {
                            return (
                                <li key={'employee-' + employee.employeeId}>{`${employee.nickName} (${employee.fullName})`}</li>
                            )
                        })}
                    </ul>
                </div>
            </div>
        );
    }
}

export default EmployeeList;