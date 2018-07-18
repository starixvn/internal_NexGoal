import React, { Component, Fragment } from 'react';
import axios from 'axios';

import commonFunctions from '../../../common-functions';
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

    componentWillMount() {
        this.getAllEOY();
    }

    getAllEOY() {
        axios.get(commonFunctions.getRootUrl() + `EOY/GetAll?statusId=${this.state.statusId}`)
            .then(response => {
                let data = response.data;

                this.setState({ eoys: data });
            }).catch(commonFunctions.axiosError);
    }

    handleStatusChange(statusId) {
        this.setState({ statusId });
    }

    handleSubmit(event) {
        this.getAllEOY();
    }

    render() {
        return (
            <Fragment>
                <Filter statusId={this.state.statusId}
                    handleStatusChange={this.handleStatusChange.bind(this)}
                    handleSubmit={this.handleSubmit.bind(this)} />

                <Table
                    eoys={this.state.eoys}
                    isHost={this.state.isHost} />
            </Fragment>
        );
    }
}