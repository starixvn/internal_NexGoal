import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import commonFunctions from '../../../common-functions';
import { toast } from 'react-toastify';
import { camelCase, range } from 'lodash';
import SearchForm from './SearchForm'
import Table from './Table';

export default class EditEmployeeSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                groupId: '',
                isLocal: '',
                isEvaluated: '',
                paStatusId: '',
            },
            performanceStatuses: [],
            groups: [],
            settings: []
        }
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    componentDidMount = () => {
        axios.all([this.getPerformanceStatusesAxios(), this.getGroupsAxios(), this.getEmployeeSettingsAxios()])
            .then(axios.spread((perfRes, groupRes, settingRes) => {
                var settings = settingRes.data;
                settings.map((e, i) => e.rowNum = i + 1);

                this.setState({
                    performanceStatuses: perfRes.data,
                    groups: groupRes.data.map((group) => {
                        return { value: group.groupId, label: group.name };
                    }),
                    settings: settings
                });
            })).catch(commonFunctions.axiosError);
    }

    getPerformanceStatusesAxios = () => axios.get(commonFunctions.getRootUrl() + 'Common/GetPerformanceStatuses');

    getGroupsAxios = () => axios(commonFunctions.getRootUrl() + 'Common/GetGroups')

    getEmployeeSettingsAxios = () =>
        axios(commonFunctions.getRootUrl() + 'Period/GetEmployeeSettings', {
            params: {
                periodId: this.props.periodId,
                groupId: this.state.filter.groupId,
                isLocal: this.state.filter.isLocal,
                isEvaluated: this.state.filter.isEvaluated,
                statusId: this.state.filter.paStatusId
            }
        })

    getEmployeeSettings = () => {
        this.getEmployeeSettingsAxios()
            .then(response => {
                var settings = response.data;
                settings.map((e, i) => e.rowNum = i + 1);

                this.setState({
                    settings: settings
                });
            })
            .catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter} groups={this.state.groups} evaluateStatuses={this.state.evaluateStatuses} performanceStatuses={this.state.performanceStatuses}
                    onFilterChange={this.updateFilterValue} onSubmit={this.getEmployeeSettings} />
                <Table groups={this.state.groups} data={this.state.settings} />
            </Fragment>
        );
    }
}