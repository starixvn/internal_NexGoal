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

    getTargetTypes = () => {
        axios.get('/Period/GetTargetTypes')
            .then(response => {
                this.setState({ targetTypes: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getPeriods = () => {
        axios.get('/Period/GetPeriods', {
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
                    <SearchForm targetTypeId={this.state.targetTypeId} isActive={this.state.isActive} targetTypes={this.state.targetTypes} onTypeIdChange={this.handleTypeIdChange} onStatusChange={this.handleStatusChange} />
                    <Table data={this.state.data} hasHostPermission={this.hasHostPermission} />
                </div>
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<EvaluationPeriod />, document.getElementById("period-wrapper"));