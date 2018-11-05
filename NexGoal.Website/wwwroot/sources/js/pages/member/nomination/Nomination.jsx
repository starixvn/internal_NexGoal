import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import InfiniteScroll from 'react-infinite-scroller';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat } from '../../../constants';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import CreateNomination from './CreateNomination';

export default class Nomination extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="record-list">
                <CreateNomination currentUser={this.props.currentUser} employeeOptions={this.props.employeeOptions} isEmployeeSelectable={this.props.isEmployeeSelectable} nomineeId={this.props.nomineeId} />
            </div>
        );
    }
}