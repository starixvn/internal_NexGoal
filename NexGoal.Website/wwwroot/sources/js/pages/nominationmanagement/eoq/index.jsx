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
            isHost: false
        };
    }

    componentWillMount() {
        this.getYears();
        this.getAllEOQ();
    }

    getYears() {
        axios.get(commonFunctions.getRootUrl() + 'EOQ/GetYears')
            .then(response => {
                let data = response.data;

                this.setState({
                    years: data,
                    eoqs: [],
                    year: data[0],
                    statusId: this.state.statusId,
                    isHost: document.getElementById('IsHost').checked
                });
            }).catch(commonFunctions.axiosError);
    }

    getAllEOQ() {
        axios.get(commonFunctions.getRootUrl() + `EOQ/GetAllEOQ?year=${this.state.year}&statusId=${this.state.statusId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    years: this.state.years,
                    eoqs: data,
                    year: this.state.year,
                    statusId: this.state.statusId,
                    isHost: this.state.isHost
                });
            }).catch(commonFunctions.axiosError);
    }

    handleYearChange(year) {
        this.setState({
            years: this.state.years.slice(),
            eoqs: this.state.eoqs.slice(),
            statusId: this.state.statusId,
            isHost: this.state.isHost,
            year
        });
    }

    handleStatusChange(statusId) {
        this.setState({
            years: this.state.years.slice(),
            eoqs: this.state.eoqs.slice(),
            year: this.state.year,
            isHost: this.state.isHost,
            statusId
        });
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
                    handleSubmit={this.handleSubmit.bind(this)} />

                <Table
                    eoqs={this.state.eoqs}
                    isHost={this.state.isHost} />
            </Fragment>
        );
    }
}