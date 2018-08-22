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
                evaluatedStatusId: '',
                paStatusId: '',
            },
            evaluateStatuses: [],
            performanceStatuses: [],
            groups: []
        }
    }

    updateFilterValue = (name, value) => {
        var filter = { [name]: value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    componentDidMount = () => {
        this.getGroups();
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

    getGroups = () => {
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });

                this.setState({
                    groups: groupOptions,
                });
            });
    }

    render() {
        return (
            <Fragment>
                <SearchForm filter={this.state.filter} groups={this.state.groups} evaluateStatuses={this.state.evaluateStatuses} performanceStatuses={this.state.performanceStatuses} onFilterChange={this.updateFilterValue} />
                <Table groups={this.state.groups} />
            </Fragment>
        );
    }
}