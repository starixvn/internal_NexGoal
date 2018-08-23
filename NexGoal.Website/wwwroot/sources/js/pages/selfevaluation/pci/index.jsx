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
            creatorOptions: [],
            projectOptions: []
        };
    }

    componentDidMount() {
        this.getCreators();
        this.getProjects();
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

    render() {
        return (
            <Fragment>
                <Filter filter={this.state.filter}
                    creatorOptions={this.state.creatorOptions}
                    projectOptions={this.state.projectOptions} />
            </Fragment>
        );
    }
}