import React, { Component, Fragment } from 'react';
import axios from 'axios';

import commonFunctions from '../../../common-functions';
import Filter from './Filter';

export default class PerformanceCheckIn extends Component {
    constructor(props) {
        super(props);

        this.state = {
        };
    }

    componentDidMount() {
    }

    render() {
        return (
            <Fragment>
                <Filter />
                <a href={`${commonFunctions.getRootUrl()}Self/PerformanceCheckIn/Create`} className="btn btn-primary">
                    <i className="fa fa-plus" />Performance Check-in
                </a>
            </Fragment>
        );
    }
}