import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Filter from './Filter';
import Table from './Table';

export default class EOQ extends Component {
    constructor(props) {
        super(props);

        this.state = {
            year: '',
            statusId: 1,
            years: [],
            eoqs: []
        };

        this.isHost = document.getElementById('IsHost').checked;
    }

    componentDidMount() {
        this.getYears();
        this.getAllEOQ();
    }

    getYears = () => {
        axios.get(resolveClientUrl('EOQ/GetYears'))
            .then(response => {
                const data = response.data;

                this.setState({
                    years: data,
                    year: data[0]
                });
            }).catch(axiosError);
    };

    getAllEOQ = () => {
        axios.get(resolveClientUrl(`EOQ/GetAll?year=${this.state.year}&statusId=${this.state.statusId}`))
            .then(response => {
                const data = response.data;

                this.setState({ eoqs: data });
            }).catch(axiosError);
    };

    handleYearChange = year => {
        this.setState({ year });
    };

    handleStatusChange = statusId => {
        this.setState({ statusId });
    };

    handleSubmit = () => {
        this.getAllEOQ();
    };

    render() {
        return (
            <Fragment>
                <Filter years={this.state.years}
                    year={this.state.year}
                    statusId={this.state.statusId}
                    handleYearChange={this.handleYearChange}
                    handleStatusChange={this.handleStatusChange}
                    handleFilterSubmit={this.handleSubmit}
                />

                <Table
                    eoqs={this.state.eoqs}
                    isHost={this.isHost}
                    handleFilterSubmit={this.handleSubmit}
                />
            </Fragment>
        );
    }
}