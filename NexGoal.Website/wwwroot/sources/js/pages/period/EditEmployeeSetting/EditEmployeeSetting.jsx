import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import commonFunctions from '../../../common-functions';
import { toast } from 'react-toastify';
import { camelCase, range } from 'lodash';
import SearchForm from './SearchForm'
import Table from './Table';

export default class EditEmployeeSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                groupId: '',
                isLocal: '',
                isGenerated: '',
                evaluatedStatusId: '',
                paStatusId: '',
                excludedEmployee: false,
            },
            evaluateStatuses: [],
            performanceStatuses: []
        }
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    componentDidMount = () => {
        this.getEvaluateStatuses();
        this.getPerformanceStatuses();
    }

    getPerformanceStatuses = () => {
        axios.get(commonFunctions.getRootUrl() + 'Common/GetPerformanceStatuses')
            .then(response => {
                this.setState({ performanceStatuses: response.data });
            }).catch(commonFunctions.axiosError);
    }

    getEvaluateStatuses = () => {
        axios.get(commonFunctions.getRootUrl() + 'Common/GetEvaluateStatuses')
            .then(response => {
                this.setState({ evaluateStatuses: response.data });
            }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter} evaluateStatuses={this.state.evaluateStatuses} performanceStatuses={this.state.performanceStatuses} onFilterChange={this.updateFilterValue} />
                <Table />
            </Fragment>

        );
    }
}