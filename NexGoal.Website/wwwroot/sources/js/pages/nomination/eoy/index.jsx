import React, { Component, Fragment } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Filter from './Filter';
import Table from './Table';

export default class EOY extends Component {
    constructor(props) {
        super(props);

        this.state = {
            statusId: 1,
            eoys: [],
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getAllEOY();
    }

    getAllEOY() {
        axios.get(resolveClientUrl(`EOY/GetAll?statusId=${this.state.statusId}`))
            .then(response => {
                let data = response.data;

                this.setState({ eoys: data });
            }).catch(axiosError);
    }

    handleStatusChange = (statusId) => {
        this.setState({ statusId });
    }

    handleSubmit = () => {
        this.getAllEOY();
    }

    render() {
        return (
            <Fragment>
                <Filter statusId={this.state.statusId}
                    handleStatusChange={this.handleStatusChange}
                    handleFilterSubmit={this.handleSubmit}
                />

                <Table
                    eoys={this.state.eoys}
                    isHost={this.state.isHost}
                    handleFilterSubmit={this.handleSubmit}
                />
            </Fragment>
        );
    }
}