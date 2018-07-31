import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import commonFunctions from '../../common-functions';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import SearchForm from './SearchForm'
import Table from './Table'

export default class EvaluationPeriod extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                targetTypeId: '',
                isActive: ''
            },
            data: [],
            targetTypes: []
        };
    }

    componentDidMount() {
        this.getPeriods();
        this.getTargetTypes();
    }

    hasHostPermission = document.getElementById('has-host-permission').value === 'True'

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    handleTypeIdChange = value => this.updateFilterValue('targetTypeId', value)

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
        this.setState({ data: data })
    }

    getTargetTypes = () => {
        axios.get(commonFunctions.getRootUrl() + 'Common/GetTargetTypes')
            .then(response => {
                this.setState({ targetTypes: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getPeriods = () => {
        axios.get(commonFunctions.getRootUrl() + 'Period/GetPeriods', {
            params: {
                targetTypeId: this.state.filter.targetTypeId,
                isActive: this.state.filter.isActive
            }
        }).then(response => {
            response.data.forEach((e, index) => e.rowNum = index + 1);
            this.setState({ data: response.data });
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Evaluation Period Management</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <SearchForm targetTypeId={this.state.targetTypeId} isActive={this.state.isActive} targetTypes={this.state.targetTypes}
                        onTypeIdChange={this.handleTypeIdChange} onStatusChange={this.handleStatusChange} onSubmit={this.handleSubmit} />
                    <Table data={this.state.data} hasHostPermission={this.hasHostPermission} targetTypes={this.state.targetTypes} updateData={this.getPeriods}
                        onUpdateStatus={this.handleUpdateStatus} onDeletePeriod={this.handleDeletePeriod} />
                </div>
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<EvaluationPeriod />, document.getElementById("period-wrapper"));