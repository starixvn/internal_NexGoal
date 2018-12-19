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
import NominationProcess from './NominationProcess';
import Table from './Table';

export default class EOQAssessment extends Component {
    constructor(props) {
        super(props);

        let defaultStatus = { value: '', label: '-- All --' };

        this.state = {
            filter: {
                status: defaultStatus,
            },
            currentDate: moment(),
            selectedNominationperiod: null,
            tableData: [],
            nominationPeriod: null,
            showModal: false,
            statusOptions: [defaultStatus, ...NominationStatuses.map(ns => ({ value: ns.value, label: ns.text }))],
            activeTabKey: 1
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('NominationAssessment/GetCurrentNominationPeriodForComittee'), {
            params: {
                isQuarter: this.props.isQuarter
            }
        }).then(response => {
            this.setState(
                {
                    nominationPeriod: response.data,
                }, () => {
                    if (response.data) {
                        this.getNominations();
                    }
                });
        }).catch(axiosError);
    }

    getNominations = () => {
        axios.get(resolveClientUrl('NominationAssessment/GetNominationsByPeriodIds'), {
            params: {
                nominationPeriodIds: this.state.nominationPeriod.nominationPeriodId,
                statusId: this.state.filter.status.value
            }
        }).then(response => {
            this.setState(prevState => {
                let period = prevState.nominationPeriod;
                return {
                    tableData: response.data,
                    canUpdateNominationStatus: prevState.currentDate.isAfter(moment(period.endDateForNominator), 'day') && prevState.currentDate.isSameOrBefore(moment(period.endDateForCommittee), 'day')
                };
            });
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
        if (this.state.nominationPeriod) {
            return (
                <Fragment>
                    <SearchForm filter={this.state.filter}
                        statusOptions={this.state.statusOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.handleFilterSubmit}
                    />

                    <NominationProcess period={this.state.nominationPeriod} />

                    <Table tableData={this.state.tableData}
                        onUpdateStatus={this.handleUpdateNominationStatus}
                        onShowNominatorComment={this.handleShowNominatorComment}
                        canUpdateNominationStatus={this.state.canUpdateNominationStatus}
                    />
                </Fragment>
            );
        }

        return 'There is no on going nomination';
    }
}