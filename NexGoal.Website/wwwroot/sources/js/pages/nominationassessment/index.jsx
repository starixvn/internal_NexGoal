import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';

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
            selectedNominationperiod: null,
            nominees: [],
            nominationPeriods: null,
            showModal: false,
            periodOptions: []
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
        this.setState(prevState => ({
            selectedNominationperiod: prevState.nominationPeriods.find(e => e.nominationPeriodId == prevState.filter.nominationPeriod.value)
        }));
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
                    </div>

                    <Table />
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<NominationAssessment />, document.getElementById("nomination-assessment-wrapper"));