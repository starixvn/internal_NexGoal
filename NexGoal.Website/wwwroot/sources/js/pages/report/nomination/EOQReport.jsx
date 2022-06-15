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

        const defaultStatus = { value: '', label: '-- All --' };

        this.state = {
            filter: {
                periods: [],
                groups: [],
                employees: [],
                status: defaultStatus
            },
            tableData: null
        };

        this.statusOptions = [defaultStatus, ...NominationStatuses.map(ns => ({ value: ns.value, label: ns.text }))];
        this.periodOptions = [];
        this.groupOptions = [];
        this.employeeOptions = [];
        this.today = moment();
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetGroups')),
            axios(resolveClientUrl('common/GetEmployees')),
            axios.get(resolveClientUrl('NominationReport/GetNominationPeriods'))
        ];

        Promise.all(promises)
            .then(([groupResponse, employeeResponse, nominationPeriodResponse]) => {
                this.groupOptions = groupResponse.data.map(group => {
                    return {
                        id: group.groupId,
                        text: group.name,
                        checked: true
                    };
                });
                this.employeeOptions = employeeResponse.data.map(item => {
                    return {
                        id: item.employeeId,
                        text: `${item.nickName} (${item.fullName})`,
                        checked: true
                    };
                });
                this.employees = employeeResponse.data;
                this.periods = nominationPeriodResponse.data;
                this.periodOptions = nominationPeriodResponse.data.map((item, index) => {
                    return {
                        id: item.nominationPeriodId,
                        text: item.title,
                        startDateForNominator: item.startDateForNominator,
                        endDateForCommittee: item.endDateForCommittee,
                        checked: index === 0
                    };
                });

                const filter = {
                    ... this.state.filter,
                    groups: this.groupOptions,
                    periods: this.periodOptions.length > 0 ? [this.periodOptions[0]] : [],
                    employees: this.employeeOptions
                };

                this.getNominations(filter);

                this.setState({ filter });
            }).catch(axiosError);
    }

    getNominations = filter => {
        axios.get(resolveClientUrl('NominationReport/GetNominations'), {
            params: {
                nominationPeriodIds: filter.periods.map(e => e.id).toString(),
                groupIds: filter.groups.map(x => x.id).toString(),
                employeeIds: filter.employees.map(x => x.id).toString(),
                statusId: filter.status.value
            }
        }).then(response => {
            this.setState({
                tableData: filter.periods.map(p => ({
                    nominationPeriodId: p.id,
                    title: p.text,
                    isInProgress: this.today.isSameOrAfter(p.startDateForNominator) && this.today.isSameOrBefore(p.endDateForCommittee),
                    nominations: response.data.filter(n => n.period.nominationPeriodId === p.id)
                }))
            });
        }).catch(axiosError);
    };

    handleFilterChange = (name, value) => {
        if (name === 'groups') {
            this.employeeOptions = this.employees.filter(e => value.some(v => v.id === e.groupId))
                .map(e => ({
                    id: e.employeeId,
                    text: `${e.nickName} (${e.fullName})`,
                    checked: true
                }));

            this.setState({
                filter: {
                    ...this.state.filter,
                    [name]: value,
                    employees: this.employeeOptions
                }
            });
        } else {
            this.setState({
                filter: { ...this.state.filter, [name]: value }
            });
        }
    };

    handleFilterSubmit = () => {
        this.getNominations(this.state.filter);
    };

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(
            nomination,
            performanceRecordId,
            this.periodOptions.find(x => x.id === nomination.period.nominationPeriodId)
        );
    };

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