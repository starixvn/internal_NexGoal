import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import SearchForm from './SearchForm';
import Table from './Table';

const defaultOption = { value: '', label: '-- All --' };

export default class EditEmployeeSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                group: defaultOption,
                isEvaluated: defaultOption,
                paStatus: defaultOption
            },
            settings: []
        };

        this.groupOptions = [];
        this.multiGroupOptions = [];
        this.employeeOptions = [];
        this.performanceStatusOptions = [];
        this.evaluateStatuses = [
            defaultOption,
            { value: true, label: 'Evaluated' },
            { value: false, label: 'Not Evaluated' }
        ];
    }

    componentDidMount = () => {
        Promise.all([this.getPerformanceStatusesAxios(), this.getGroupsAxios(), this.getEmployeesAxios(), this.getEmployeeSettingsAxios()])
            .then(axios.spread((perfRes, groupRes, employeeRes, settingRes) => {
                const groups = groupRes.data.map(group => {
                    return { value: group.groupId, label: group.name };
                });

                this.multiGroupOptions = groups;
                this.groupOptions = [defaultOption, ...groups];

                this.employeeOptions = [defaultOption, ...employeeRes.data.map(employee => {
                    return { value: employee.employeeId, label: `${employee.nickName} (${employee.fullName})` };
                })];

                this.performanceStatusOptions = [defaultOption, ...perfRes.data.map(
                    perf => ({ value: perf.id, label: perf.name })
                )];

                this.setState({
                    settings: settingRes.data
                });
            })).catch(axiosError);
    }

    updateFilterValue = (name, value) => {
        const filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    getPerformanceStatusesAxios = () => axios.get(resolveClientUrl('Common/GetPerformanceStatuses'));

    getGroupsAxios = () => axios(resolveClientUrl('Common/GetGroups'));

    getEmployeesAxios = () => axios(resolveClientUrl('common/GetEmployees'))

    getEmployeeSettingsAxios = () =>
        axios(resolveClientUrl('Period/GetEmployeeSettings'), {
            params: {
                periodId: this.props.periodId,
                groupId: this.state.filter.group.value,
                isEvaluated: this.state.filter.isEvaluated.value,
                statusId: this.state.filter.paStatus.value
            }
        });

    getEmployeeSetting = paRecordId => {
        axios(resolveClientUrl('Period/GetEmployeeSettingById'), {
            params: {
                paRecordId: paRecordId
            }
        }).then(response => {
            const settings = this.state.settings.slice(0);
            const index = settings.findIndex(e => e.paRecordId === paRecordId);

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
                    groups={this.groupOptions}
                    evaluateStatuses={this.evaluateStatuses}
                    performanceStatuses={this.performanceStatusOptions}
                    onFilterChange={this.updateFilterValue}
                    onSubmit={this.getEmployeeSettings}
                />
                <Table periodId={this.props.periodId} groups={this.multiGroupOptions}
                    employeeOptions={this.employeeOptions}
                    data={this.state.settings}
                    refreshData={this.getEmployeeSettings}
                    getSetting={this.getEmployeeSetting}
                />
            </Fragment>
        );
    }
}