import React, { Component, Fragment } from 'react';
import moment from 'moment';
import axios from 'axios';

import commonFunctions from '../../../common-functions';
import Filter from './Filter';

export default class PerformanceCheckIn extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                fromDate: moment().subtract(5, 'months').startOf('month'),
                toDate: moment(),
                creators: [],
                projects: []
            },
            performanceRecords: [],
            creatorOptions: [],
            projectOptions: []
        };

        this.handleFilterChange = this.handleFilterChange.bind(this);
        this.handleFilterSubmit = this.handleFilterSubmit.bind(this);
    }

    componentDidMount() {
        this.getCreators();
        this.getProjects();
        this.getPerformanceRecords();
    }

    getCreators() {
        axios(commonFunctions.getRootUrl() + 'Self/PerformanceCheckIn/GetCreators')
            .then(response => {
                const creatorOptions = response.data.map((creator) => {
                    return { value: creator.employeeId, label: `${creator.fullName} (${creator.nickName})` };
                });

                this.setState({
                    creatorOptions: creatorOptions,
                    filter: Object.assign({}, this.state.filter, { creators: creatorOptions })
                });
            });
    }

    getProjects() {
        axios(commonFunctions.getRootUrl() + 'Common/GetProjects')
            .then(response => {
                const projectOptions = response.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({
                    projectOptions: projectOptions
                });
            });
    }

    getPerformanceRecords() {
        let filter = this.state.filter;

        axios(commonFunctions.getRootUrl() + `Self/PerformanceCheckIn/Index?fromDate=${filter.fromDate.format('YYYY-MM-DD')}`
            + `&toDate=${filter.toDate.format('YYYY-MM-DD')}`
            + `&creatorIds=${filter.creators.map((creator) => { return creator.value; }).join(',')}`
            + `&projectIds=${filter.projects.map((project) => { return project.value; }).join(',')}`)
            .then(response => {
                this.setState({
                    performanceRecords: response.data
                });
            });
    }

    handleFilterChange(name, value) {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit() {
        this.getPerformanceRecords();
    }

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    creatorOptions={this.state.creatorOptions}
                    projectOptions={this.state.projectOptions}
                    onFilterChange={this.handleFilterChange}
                    onFilterSubmit={this.handleFilterSubmit}
                />
            </Fragment>
        );
    }
}