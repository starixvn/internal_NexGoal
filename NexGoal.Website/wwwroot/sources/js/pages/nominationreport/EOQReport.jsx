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
                        // this.getNominations();
                    }
                });
            }).catch(axiosError);
    }

    getNominations = () => {
        axios.get(resolveClientUrl('NominationReport/GetNominations'), {
            params: {
                nominationPeriodIds: this.state.nominationPeriod.nominationPeriodId,
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

    handleUpdateNominationStatus = (nominationId, statusId) => {
        axios({
            url: resolveClientUrl(`NominationAssessment/EditStatus`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                nominationId: nominationId,
                statusId: statusId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                let tableData = this.state.tableData;
                let nomination = tableData.find(x => x.nominationId === nominationId);
                nomination.statusId = statusId;

                this.setState({ tableData });
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId, this.state.nominationPeriod);
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