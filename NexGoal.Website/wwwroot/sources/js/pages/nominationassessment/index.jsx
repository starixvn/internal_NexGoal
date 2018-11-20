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
                nominationPeriod: null,
            },
            currentDate: moment(),
            selectedNominationperiod: null,
            nominations: [],
            nominationPeriods: null,
            showModal: false,
            periodOptions: [],
            canUpdateNominationStatus: false
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
                nominationPeriodId: this.state.filter.nominationPeriod.value
            }
        }).then(response => {
            this.setState(prevState => {
                let period = prevState.nominationPeriods.find(e => e.nominationPeriodId == prevState.filter.nominationPeriod.value);
                return {
                    selectedNominationperiod: period,
                    nominations: response.data,
                    canUpdateNominationStatus: prevState.currentDate > moment(period.endDateForNominator) && prevState.currentDate <= moment(period.endDateForCommittee)
                };
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
                var nominations = this.state.nominations.slice();
                var nomination = nominations.find(x => x.nominationId === nominationId);
                nomination.statusId = statusId;

                this.setState({ nominations });
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
                    {this.state.selectedNominationperiod &&
                        <NominationProcess period={this.state.selectedNominationperiod} />
                    }
                    <Table canUpdate={this.state.canUpdateNominationStatus}
                        nominations={this.state.nominations}
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