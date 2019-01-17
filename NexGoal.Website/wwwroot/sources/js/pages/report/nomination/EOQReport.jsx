import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { NominationStatuses } from '../../../constants';
import SearchForm from './SearchForm';
import Table from './Table';

export default class EOQReport extends Component {
    constructor(props) {
        super(props);

        let defaultStatus = { value: '', label: '-- All --' };

        this.state = {
            filter: {
                periods: [],
                groups: [],
                employees: [],
                status: defaultStatus
            },
            tableData: [],
            showModal: false,
            activeTabKey: 1
        };

        this.statusOptions = [defaultStatus, ...NominationStatuses.map(ns => ({ value: ns.value, label: ns.text }))];
        this.periodOptions = [];
        this.groupOptions = [];
        this.employeeOptions = [];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees')),
            axios.get(resolveClientUrl('NominationReport/GetNominationPeriods'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse, nominationPeriodResponse]) => {
                this.groupOptions = groupResponse.data.map((group) => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });
                this.employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.nickName} (${item.fullName})`
                    };
                });
                this.employees = employeeResponse.data;
                this.periodOptions = nominationPeriodResponse.data.map(x => ({ value: x.nominationPeriodId, label: x.title }));

                let filter = {
                    ... this.state.filter,
                    ...{
                        groups: this.groupOptions,
                        periods: this.periodOptions.length > 0 ? [this.periodOptions[0]] : []
                    }
                };

                this.getNominations(filter);

                this.setState({ filter });
            }).catch(axiosError);
    }

    getNominations = (filter) => {
        axios.get(resolveClientUrl('NominationReport/GetNominations'), {
            params: {
                nominationPeriodIds: filter.periods.map(e => e.value).toString(),
                groupIds: filter.groups.map(x => x.value).toString(),
                employeeIds: filter.employees.map(x => x.value).toString(),
                statusId: filter.status.value
            }
        }).then(response => {
            this.setState({
                tableData: filter.periods.map(p => ({
                    nominationPeriodId: p.value,
                    title: p.label,
                    nominations: response.data.filter(n => n.nominationPeriodId === p.value)
                }))
            });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            this.employeeOptions = this.employees.filter(e => value.some(v => v.value === e.groupId))
                .map(e => ({
                    value: e.employeeId,
                    label: `${e.nickName} (${e.fullName})`
                }));
        }
        this.setState({
            filter: { ...this.state.filter, ...{ [name]: value } }
        });
    }

    handleFilterSubmit = () => {
        this.getNominations(this.state.filter);
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(
            nomination,
            performanceRecordId,
            this.periodOptions.find(x => x.value === nomination.nominationPeriodId).label
        );
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    statusOptions={this.statusOptions}
                    groupOptions={this.groupOptions}
                    periodOptions={this.periodOptions}
                    employeeOptions={this.employeeOptions}
                    onFilterChange={this.handleFilterChange}
                    onFilterSubmit={this.handleFilterSubmit}
                />

                <Table tableData={this.state.tableData}
                    onUpdateStatus={this.handleUpdateNominationStatus}
                    onShowNominatorComment={this.handleShowNominatorComment}
                />
            </Fragment>
        );
    }
}