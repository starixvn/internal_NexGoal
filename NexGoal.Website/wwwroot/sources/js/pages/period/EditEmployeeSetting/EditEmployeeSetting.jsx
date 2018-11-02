import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import SearchForm from './SearchForm';
import Table from './Table';

export default class EditEmployeeSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                groupId: '',
                isEvaluated: '',
                paStatusId: ''
            },
            performanceStatuses: [],
            groups: [],
            employees: [],
            settings: []
        };
    }

    componentDidMount = () => {
        Promise.all([this.getPerformanceStatusesAxios(), this.getGroupsAxios(), this.getEmployeesAxios(), this.getEmployeeSettingsAxios()])
            .then(axios.spread((perfRes, groupRes, employeeRes, settingRes) => {
                var settings = settingRes.data;
                settings.map((e, i) => e.rowNum = i + 1);

                this.setState({
                    performanceStatuses: perfRes.data,
                    groups: groupRes.data.map((group) => {
                        return { value: group.groupId, label: group.name };
                    }),
                    employees: employeeRes.data.map(employee => {
                        return { value: employee.employeeId, label: `${employee.fullName} (${employee.nickName})` };
                    }),
                    settings: settings
                });
            })).catch(axiosError);
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    getPerformanceStatusesAxios = () => axios.get(resolveClientUrl('Common/GetPerformanceStatuses'));

    getGroupsAxios = () => axios(resolveClientUrl('Common/GetGroups'));

    getEmployeesAxios = () => axios(resolveClientUrl('common/GetEmployees'))

    getEmployeeSettingsAxios = () =>
        axios(resolveClientUrl('Period/GetEmployeeSettings'), {
            params: {
                periodId: this.props.periodId,
                groupId: this.state.filter.groupId,
                isEvaluated: this.state.filter.isEvaluated,
                statusId: this.state.filter.paStatusId
            }
        });

    getEmployeeSetting = (paRecordId) => {
        axios(resolveClientUrl('Period/GetEmployeeSetting'), {
            params: {
                paRecordId: paRecordId
            }
        }).then(response => {
            var settings = this.state.settings.slice(0);
            var index = settings.findIndex(e => e.paRecordId === paRecordId);

            settings[index] = { ...settings[index], ...response.data };

            this.setState({
                settings: settings
            });
        }).catch(axiosError);
    }

    getEmployeeSettings = () => {
        this.getEmployeeSettingsAxios()
            .then(response => {
                this.setState({
                    settings: response.data
                });
            })
            .catch(axiosError);
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    groups={this.state.groups}
                    evaluateStatuses={this.state.evaluateStatuses}
                    performanceStatuses={this.state.performanceStatuses}
                    onFilterChange={this.updateFilterValue}
                    onSubmit={this.getEmployeeSettings}
                />
                <Table periodId={this.props.periodId} groups={this.state.groups}
                    employeeOptions={this.state.employees}
                    data={this.state.settings}
                    refreshData={this.getEmployeeSettings}
                    getSetting={this.getEmployeeSetting}
                />
            </Fragment>
        );
    }
}