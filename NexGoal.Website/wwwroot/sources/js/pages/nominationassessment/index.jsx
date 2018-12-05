import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs'

import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import NominationProcess from './NominationProcess';
import Table from './Table';
import NominatorComment from './NominatorComment';

export default class NominationAssessment extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                nominationPeriods: [],
            },
            currentDate: moment(),
            selectedNominationperiod: null,
            tableData: [],
            nominationPeriods: null,
            showModal: false,
            periodOptions: [],
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('NominationAssessment/GetNominationPeriodsForNominator'))
            .then(response => {
                this.setState(
                    {
                        nominationPeriods: response.data,
                        periodOptions: response.data.map(e => ({
                            label: e.title,
                            value: e.nominationPeriodId
                        })),
                    });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: { ...this.state.filter, ...{ [name]: value } }
        });
    }

    handleFilterSubmit = () => {
        axios.get(resolveClientUrl('NominationAssessment/GetNominationsByPeriodId'), {
            params: {
                nominationPeriodIds: this.state.filter.nominationPeriods.map(n => n.value).toString()
            }
        }).then(response => {
            this.setState(prevState => {
                let periods = prevState.nominationPeriods.filter(p => prevState.filter.nominationPeriods.some(e => e.value === p.nominationPeriodId));

                let tableData = periods.map(p => {
                    return {
                        period: p,
                        nominations: response.data.filter(n => n.nominationPeriodId === p.nominationPeriodId),
                        canUpdateNominationStatus: prevState.currentDate > moment(p.endDateForNominator) && prevState.currentDate <= moment(p.endDateForCommittee)
                    }
                });

                return {
                    tableData: tableData,
                }
            });
        }).catch(axiosError);
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
                let nominations = [].concat(...tableData.map(x => x.nominations));
                let nomination = nominations.find(x => x.nominationId === nominationId);
                nomination.statusId = statusId;

                this.setState({ tableData });
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleShowNominatorComment = nominator => {
        this.setState({ showModal: true, nominator: nominator });
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('EmployeeNomination')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        periodOptions={this.state.periodOptions}
                        onFilterChange={this.handleFilterChange}
                        onFilterSubmit={this.handleFilterSubmit}
                    />

                    <Table tableData={this.state.tableData}
                        onUpdateStatus={this.handleUpdateNominationStatus}
                        onShowNominatorComment={this.handleShowNominatorComment}
                    />
                </div>
                <NominatorComment nominator={this.state.nominator} showModal={this.state.showModal} onCloseModal={this.closeModal} />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<NominationAssessment />, document.getElementById("nomination-assessment-wrapper"));