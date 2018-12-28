import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import SearchForm from './SearchForm';
import Table from './Table';

export default class EvaluationPeriod extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                isActive: ''
            },
            periods: []
        };

        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        this.getPeriods();
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    handleStatusChange = value => this.updateFilterValue('isActive', value)

    handleSubmit = event => {
        event.preventDefault();
        this.getPeriods();
    }

    handleDeletePeriod = (periodId) => {
        let periods = this.state.periods.slice();
        periods = periods.filter(e => e.periodId !== periodId);
        this.setState({ periods: periods });
    }

    handleUpdateStatus = (periodId, isActive) => {
        let periods = this.state.periods.slice();
        let period = periods.find(e => e.periodId === periodId);
        period.isActive = isActive;
        this.setState({ periods: periods });
    }

    getPeriods = () => {
        axios.get(resolveClientUrl('Period/GetPeriods'), {
            params: {
                isActive: this.state.filter.isActive
            }
        }).then(response => {
            this.setState({ periods: response.data });
        }).catch(axiosError);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('EvaluationPeriodManagement')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm isActive={this.state.isActive}
                        onStatusChange={this.handleStatusChange}
                        onSubmit={this.handleSubmit}
                    />
                    <Table periods={this.state.periods}
                        isHost={this.isHost}
                        refreshData={this.getPeriods}
                        onUpdateStatus={this.handleUpdateStatus}
                        onDeletePeriod={this.handleDeletePeriod}
                    />
                </div>
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<EvaluationPeriod />, document.getElementById("period-wrapper"));