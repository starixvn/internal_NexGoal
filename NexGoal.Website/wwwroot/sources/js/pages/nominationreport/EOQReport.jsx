import React, { Component, Fragment } from 'react';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { NominationStatuses } from '../../constants';
import SearchForm from './SearchForm';
import Table from './Table';

export default class EOQAssessment extends Component {
    constructor(props) {
        super(props);

        let defaultStatus = { value: '', label: '-- All --' };

        this.state = {
            filter: {
                period: null,
                groups: [],
                employee: null,
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
            axios.get(resolveClientUrl('NominationReport/GetNominationPeriods'), {
                params: {
                    isQuarter: this.props.isQuarter
                }
            })
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
                this.periodOptions = nominationPeriodResponse.data.map(x => ({ value: x.nominationPeriodId, label: x.title }));

                let filter = {
                    ... this.state.filter,
                    ...{
                        groups: this.groupOptions,
                        period: this.periodOptions.length > 0 ? this.periodOptions[0] : null
                    }
                };

                this.getNominations(filter);

                this.setState({ filter });
            }).catch(axiosError);
    }

    getNominations = (filter) => {
        axios.get(resolveClientUrl('NominationReport/GetNominations'), {
            params: {
                nominationPeriodId: filter.period ? filter.period.value : null,
                groupIds: filter.groups.map(x => x.value).toString(),
                employeeId: filter.employee ? filter.employee.value : null,
                statusId: filter.status.value
            }
        }).then(response => {
            this.setState({ tableData: response.data });
        }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
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
            this.periodOptions.find(x => x.value === nomination.nominationPeriodId)
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