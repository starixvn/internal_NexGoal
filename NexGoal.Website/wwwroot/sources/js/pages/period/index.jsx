import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import SearchForm from './SearchForm';
import Table from './Table';
import { StatusOptions } from '../../constants';

export default class EvaluationPeriod extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                isActive: StatusOptions[0]
            },
            periods: []
        };

        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        this.getPeriods();
    }

    updateFilterValue = (name, value) => {
        const filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    };

    handleStatusChange = value => this.updateFilterValue('isActive', value);

    handleSubmit = event => {
        event.preventDefault();
        this.getPeriods();
    };

    handleDeletePeriod = periodId => {
        let periods = this.state.periods.slice();
        periods = periods.filter(e => e.periodId !== periodId);
        this.setState({ periods: periods });
    };

    handleUpdateStatus = (periodId, isActive) => {
        const periods = this.state.periods.slice();
        const period = periods.find(e => e.periodId === periodId);
        period.isActive = isActive;
        this.setState({ periods: periods });
    };

    getPeriods = () => {
        axios.get(resolveClientUrl('Period/GetPeriods'), {
            params: {
                isActive: this.state.filter.isActive.value
            }
        }).then(response => {
            this.setState({ periods: response.data });
        }).catch(axiosError);
    };

    render() {
        return (
            <div className="row content-wrapper">
                <Breadcrumb>
                    <li className="current">{getResource('EvaluationPeriod')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm filter={this.state.filter}
                        onStatusChange={this.handleStatusChange}
                        onSubmit={this.handleSubmit}
                        statusOptions={StatusOptions}
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

ReactDOM.render(<EvaluationPeriod />, document.getElementById('period-wrapper'));