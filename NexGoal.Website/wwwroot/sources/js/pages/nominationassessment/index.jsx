import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';

import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import NominationProcess from './NominationProcess';
import Table from './Table';

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

    closeModal = () => {
        this.setState({ showModal: false });
    }

    showModal = () => {
        this.setState({ showModal: true });
    }

    render() {
        return (
            <ToastLayout>
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
                        <Table canUpdate={this.state.canUpdateNominationStatus} nominations={this.state.nominations} />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<NominationAssessment />, document.getElementById("nomination-assessment-wrapper"));