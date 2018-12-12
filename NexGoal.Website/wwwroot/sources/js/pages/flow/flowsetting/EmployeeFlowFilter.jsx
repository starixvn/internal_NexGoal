import React, { Component } from 'react';
import axios from 'axios';
import Select from 'react-select';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';

class EmployeeFlowFilter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupSelected: { value: '', label: '-- All --' },
            keyword: '',
            employeeUnsettings: [],
            employeeSettings: [],
            filteredEmployees: [],
            employeeUnsettingSelected: null,
            employeeSettingSelected: null
        };
    }

    componentDidMount() {
        const groupId = this.state.groupSelected.value;
        this.getEmployeeUnsetting(groupId);
        this.getEmployeeSetting(groupId);
    }

    getEmployeeUnsetting = (groupId) => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetEmployeeUnsetting?flowTemplateId=${this.props.flowTemplateId}&groupId=${groupId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    employeeUnsettings: data,
                    filteredEmployees: data
                });
            }).catch(axiosError);
    }

    getEmployeeSetting = (groupId) => {
        axios(resolveClientUrl(`EmployeeFlowSetting/GetEmployeeHaveSetting?flowTemplateId=${this.props.flowTemplateId}&groupId=${groupId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    employeeSettings: data
                });
            }).catch(axiosError);
    }

    handleOnGroupChange = (selectedOption) => {
        this.setState({
            groupSelected: selectedOption,
            keyword: ''
        });

        this.getEmployeeUnsetting(selectedOption.value);
        this.getEmployeeSetting(selectedOption.value);
    }

    handleOnKeywordChange = (event) => {
        let employees;
        if (event.target.value && event.target.value.trim().length > 1) {
            const value = event.target.value.trim().toLowerCase();

            employees = this.state.employeeUnsettings.slice().filter((employee) =>
                employee.nickName.toLowerCase().indexOf(value) >= 0
                || employee.fullName.toLowerCase().indexOf(value) >= 0
            );
        } else {
            employees = this.state.employeeUnsettings.slice();
        }

        this.setState({
            keyword: event.target.value,
            filteredEmployees: employees
        });
    }

    handleOnEmployeeUnsettingChange = (event) => {
        this.setState({ employeeUnsettingSelected: event.target.value });
    }

    handleOnEmployeeSettingChange = (event) => {
        this.setState({ employeeSettingSelected: event.target.value });

        this.props.onEmployeeSelected(event.target.value);
    }

    handleOnMoveRightClick = () => {
        let employeeUnsettings = this.state.employeeUnsettings.slice();
        let employeeSettings = this.state.employeeSettings.slice();
        let filteredEmployees = this.state.filteredEmployees.slice();

        let index = filteredEmployees.findIndex(employee => employee.employeeId.toString() === this.state.employeeUnsettingSelected);
        filteredEmployees.splice(index, 1);

        index = employeeUnsettings.findIndex(employee => employee.employeeId.toString()=== this.state.employeeUnsettingSelected);
        employeeSettings.push(employeeUnsettings[index]);
        employeeUnsettings.splice(index, 1);

        this.setState({
            employeeUnsettingSelected: null,
            employeeUnsettings: employeeUnsettings,
            employeeSettings: employeeSettings,
            filteredEmployees: filteredEmployees
        });
    }

    handleOnMoveLeftClick = () => {
        let employeeUnsettings = this.state.employeeUnsettings.slice();
        let employeeSettings = this.state.employeeSettings.slice();

        let index = employeeSettings.findIndex(employee => employee.employeeId.toString() === this.state.employeeSettingSelected);
        employeeUnsettings.push(employeeSettings[index]);
        employeeSettings.splice(index, 1);

        this.setState({
            employeeSettingSelected: null,
            employeeUnsettings: employeeUnsettings,
            employeeSettings: employeeSettings,
            filteredEmployees: employeeUnsettings,
            keyword: ''
        });
    }

    render() {
        const isHost = this.props.isHost;
        const hasSetting = this.props.hasSetting;
        let groupOptions = this.props.groupOptions.map((group) => {
            return { value: group.groupId, label: group.name };
        });
        groupOptions.unshift({ value: '', label: '-- All --' });

        const employees = this.state.filteredEmployees;
        const employeeSettings = this.state.employeeSettings;

        return (
            <div className="filter">
                <Select className="react-select-container" classNamePrefix="react-select"
                    value={this.state.groupSelected} options={groupOptions}
                    isClearable={false}
                    onChange={this.handleOnGroupChange}
                />
                <div className="text-search">
                    <input type="text" className="form-control" placeholder={getResource('FullNameOrNickname')}
                        value={this.state.keyword} onChange={this.handleOnKeywordChange}
                    />
                    <span className="fa fa-search" />
                </div>
                <div className="employee-setting">
                    <select className="form-control" size="10" onChange={this.handleOnEmployeeUnsettingChange}>
                        {
                            employees.map((employee) =>
                                <option key={employee.employeeId} value={employee.employeeId}>
                                    {`${employee.nickName} (${employee.fullName})`}
                                </option>
                            )
                        }
                    </select>
                    <div className="control">
                        <button className="btn btn-primary fa fa-step-forward" title="Move Right"
                            disabled={!(isHost && this.state.employeeUnsettingSelected)}
                            onClick={this.handleOnMoveRightClick}
                        />
                        <button className="btn btn-primary fa fa-step-backward" title="Move Left"
                            disabled={!(isHost && this.state.employeeSettingSelected && !hasSetting)}
                            onClick={this.handleOnMoveLeftClick}
                        />
                    </div>
                    <select className="form-control" size="10" onChange={this.handleOnEmployeeSettingChange}>
                        {
                            employeeSettings.map((employee) =>
                                <option key={employee.employeeId} value={employee.employeeId}>
                                    {`${employee.nickName} (${employee.fullName})`}
                                </option>
                            )
                        }
                    </select>
                </div>
            </div>
        );
    }
}

export default EmployeeFlowFilter;