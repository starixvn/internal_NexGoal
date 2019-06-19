import React, { Component } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Table from './Table';

export default class Probation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationers: null,
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Probation/GetEvaluations')),
        ];

        Promise.all(promises)
            .then(([evaluationResponse]) => {
                this.setState({
                    probationers: evaluationResponse.data
                });
            }).catch(axiosError);
    }

    render() {
        return (
            <div className="probation-content">
                <Table tableData={this.state.probationers} />
            </div>

        );
    }
}