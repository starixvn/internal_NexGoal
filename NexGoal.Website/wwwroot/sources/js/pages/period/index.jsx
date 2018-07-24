import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import commonFunctions from '../../common-functions';
import Breadcrumb from '../../shared/components/Breadcrumb';
import ToastLayout from '../../shared/layouts/ToastLayout';
import SearchForm from './SearchForm'

export default class EvaluationPeriod extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                typeId: '',
                isActive: ''
            },
            data: []
        };
    }

    canEdit = document.getElementById('CanEdit').value === 'True'

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    handleTypeIdChange = value => this.updateFilterValue('typeId', value)

    handleStatusChange = value => this.updateFilterValue('isActive', value)

    getPeriods = () => {
        axios.get('/Period/GetPeriods', {
            params: {
                typeId: this.state.filter.typeId,
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
                    <SearchForm typeId={this.state.typeId} isActive={this.state.isActive} onTypeIdChange={this.handleTypeIdChange} onStatusChange={this.handleStatusChange} />
                </div>
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<EvaluationPeriod />, document.getElementById("period-wrapper"));