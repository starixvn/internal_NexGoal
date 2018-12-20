import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs'

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
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
                status: defaultStatus,
            },
            currentDate: moment(),
            selectedNominationperiod: null,
            tableData: [],
            showModal: false,
            statusOptions: [defaultStatus, ...NominationStatuses.map(ns => ({ value: ns.value, label: ns.text }))],
            periodOptions: [],
            groupOptions: [],
            employeeOptions: [],
            nominationPeriods: [],
            activeTabKey: 1
        };
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
                let groupOptions = groupResponse.data.map((group) => {
                    return {
                        value: group.groupId,
                        label: group.name
                    };
                });

                let employeeOptions = employeeResponse.data.map(item => {
                    return {
                        value: item.employeeId,
                        label: `${item.fullName} (${item.nickName})`
                    };
                });

                let nominationPeriodOptions = nominationPeriodResponse.data.map(x => ({ value: x.nominationPeriodId, label: x.title }));

                this.setState({
                    groupOptions: groupOptions,
                    employeeOptions: employeeOptions,
                    periodOptions: nominationPeriodOptions,
                    nominationPeriods: nominationPeriodResponse.data,

                    filter: {
                        ... this.state.filter,
                        ...{
                            groups: groupOptions,
                            period: nominationPeriodOptions.length > 0 ? nominationPeriodOptions[0] : null
                        }
                    }
                }, () => {
                    if (this.state.filter.period) {
                        this.getNominations();
                    }
                });
            }).catch(axiosError);
    }

    getNominations = () => {
        axios.get(resolveClientUrl('NominationReport/GetNominations'), {
            params: {
                nominationPeriodId: this.state.filter.period ? this.state.filter.period.value : null,
                groupIds: this.state.filter.groups.map(x => x.value).toString(),
                employeeId: this.state.filter.employee ? this.state.filter.employee.value : null,
                statusId: this.state.filter.status.value
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
        this.getNominations();
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId, this.state.nominationPeriods.find(x => x.nominationPeriodId === nomination.nominationPeriodId));
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter}
                    statusOptions={this.state.statusOptions}
                    groupOptions={this.state.groupOptions}
                    periodOptions={this.state.periodOptions}
                    employeeOptions={this.state.employeeOptions}
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