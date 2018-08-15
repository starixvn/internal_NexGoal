import React, { Component, Fragment } from 'react';
import axios from 'axios';

import commonFunctions from '../../../common-functions';
import Filter from './Filter';
import Table from './Table';

export default class EOQ extends Component {
    constructor(props) {
        super(props);

        this.state = {
            year: '',
            statusId: 1,
            years: [],
            eoqs: [],
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getYears();
        this.getAllEOQ();
    }

    getYears() {
        axios.get(commonFunctions.getRootUrl() + 'EOQ/GetYears')
            .then(response => {
                let data = response.data;

                this.setState({
                    years: data,
                    year: data[0]
                });
            }).catch(commonFunctions.axiosError);
    }

    getAllEOQ() {
        axios.get(commonFunctions.getRootUrl() + `EOQ/GetAll?year=${this.state.year}&statusId=${this.state.statusId}`)
            .then(response => {
                let data = response.data;

                this.setState({ eoqs: data });
            }).catch(commonFunctions.axiosError);
    }

    handleYearChange(year) {
        this.setState({ year });
    }

    handleStatusChange(statusId) {
        this.setState({ statusId });
    }

    handleSubmit(event) {
        this.getAllEOQ();
    }

    render() {
        return (
            <Fragment>
                <Filter years={this.state.years}
                    year={this.state.year}
                    statusId={this.state.statusId}
                    handleYearChange={this.handleYearChange.bind(this)}
                    handleStatusChange={this.handleStatusChange.bind(this)}
                    handleFilterSubmit={this.handleSubmit.bind(this)} />

                <Table
                    eoqs={this.state.eoqs}
                    isHost={this.state.isHost}
                    handleFilterSubmit={this.handleSubmit.bind(this)} />
            </Fragment>
        );
    }
}