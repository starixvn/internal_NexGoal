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
            data: [],
            isHost: document.getElementById('IsHost').checked
        };
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
        var data = this.state.data.slice();
        data = data.filter(e => e.periodId !== periodId);
        this.setState({ data: data });
    }

    handleUpdateStatus = (periodId, isActive) => {
        var data = this.state.data.slice();
        var period = data.find(e => e.periodId === periodId);
        period.isActive = isActive;
        this.setState({ data: data });
    }

    getPeriods = () => {
        axios.get(resolveClientUrl('Period/GetPeriods'), {
            params: {
                isActive: this.state.filter.isActive
            }
        }).then(response => {
            response.data.forEach((e, index) => e.rowNum = index + 1);
            this.setState({ data: response.data });
        }).catch(axiosError);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('EvaluationPeriodManagement')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm targetTypeId={this.state.targetTypeId}
                        isActive={this.state.isActive}
                        onTypeIdChange={this.handleTypeIdChange}
                        onStatusChange={this.handleStatusChange}
                        onSubmit={this.handleSubmit}
                    />
                    <Table data={this.state.data}
                        isHost={this.state.isHost}
                        updateData={this.getPeriods}
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