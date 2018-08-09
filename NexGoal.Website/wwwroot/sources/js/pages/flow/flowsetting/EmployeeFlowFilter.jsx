import React, { Component } from 'react';
import axios from 'axios';
import Select from 'react-select';

import commonFunctions from '../../../common-functions';

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

        this.handleGroupChange = this.handleGroupChange.bind(this);
        this.handleKeywordChange = this.handleKeywordChange.bind(this);
        this.handleEmployeeUnsettingChange = this.handleEmployeeUnsettingChange.bind(this);
        this.handleEmployeeSettingChange = this.handleEmployeeSettingChange.bind(this);
        this.handleMoveRightClick = this.handleMoveRightClick.bind(this);
        this.handleMoveLeftClick = this.handleMoveLeftClick.bind(this);
    }

    componentWillMount() {
        const groupId = this.state.groupSelected.value;
        this.getEmployeeUnsetting(groupId);
        this.getEmployeeSetting(groupId);
    }

    getEmployeeUnsetting(groupId) {
        axios(commonFunctions.getRootUrl() + `EmployeeFlowSetting/GetEmployeeDoNotHaveSetting?flowTemplateId=${this.props.flowTemplateId}&groupId=${groupId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    employeeUnsettings: data,
                    filteredEmployees: data
                });
            }).catch(commonFunctions.axiosError);
    }

    getEmployeeSetting(groupId) {
        axios(commonFunctions.getRootUrl() + `EmployeeFlowSetting/GetEmployeeHaveSetting?flowTemplateId=${this.props.flowTemplateId}&groupId=${groupId}`)
            .then(response => {
                const data = response.data;

                this.setState({
                    employeeSettings: data
                });
            }).catch(commonFunctions.axiosError);
    }

    handleGroupChange(selectedOption) {
        this.setState({
            groupSelected: selectedOption,
            keyword: ''
        });

        this.getEmployeeUnsetting(selectedOption.value);
        this.getEmployeeSetting(selectedOption.value);
    }

    handleKeywordChange(event) {
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

    handleEmployeeUnsettingChange(event) {
        this.setState({ employeeUnsettingSelected: event.target.value });
    }

    handleEmployeeSettingChange(event) {
        this.setState({ employeeSettingSelected: event.target.value });

        this.props.onEmployeeSelected(event.target.value);
    }

    handleMoveRightClick() {
        let employeeUnsettings = this.state.employeeUnsettings.slice();
        let employeeSettings = this.state.employeeSettings.slice();
        let filteredEmployees = this.state.filteredEmployees.slice();

        let index = filteredEmployees.map((employee) => { return employee.employeeId.toString() }).indexOf(this.state.employeeUnsettingSelected);
        filteredEmployees.splice(index, 1);

        index = employeeUnsettings.map((employee) => { return employee.employeeId.toString() }).indexOf(this.state.employeeUnsettingSelected);
        employeeSettings.push(employeeUnsettings[index]);
        employeeUnsettings.splice(index, 1);

        this.setState({
            employeeUnsettingSelected: null,
            employeeUnsettings: employeeUnsettings,
            employeeSettings: employeeSettings,
            filteredEmployees: filteredEmployees
        });
    }

    handleMoveLeftClick() {
        let employeeUnsettings = this.state.employeeUnsettings.slice();
        let employeeSettings = this.state.employeeSettings.slice();

        let index = employeeSettings.map((employee) => { return employee.employeeId.toString() }).indexOf(this.state.employeeSettingSelected);
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
                <Select value={this.state.groupSelected} options={groupOptions}
                    clearable={false} onChange={this.handleGroupChange} />
                <div className="text-search">
                    <input type="text" className="form-control" placeholder="full name or nick name"
                        value={this.state.keyword} onChange={this.handleKeywordChange} />
                    <span className="fa fa-search"></span>
                </div>
                <div className="employee-setting">
                    <select className="form-control" size="10" onChange={this.handleEmployeeUnsettingChange}>
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
                            onClick={this.handleMoveRightClick} />
                        <button className="btn btn-primary fa fa-step-backward" title="Move Left"
                            disabled={!(isHost && this.state.employeeSettingSelected && !hasSetting)}
                            onClick={this.handleMoveLeftClick} />
                    </div>
                    <select className="form-control" size="10" onChange={this.handleEmployeeSettingChange}>
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