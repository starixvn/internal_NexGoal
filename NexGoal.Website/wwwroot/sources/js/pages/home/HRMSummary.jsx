import React, { Component } from "react";
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';
import moment from 'moment';

export default class HrmSummary extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: {}
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('Discipline/GetHRMSummary'))
            .then(response => {
                this.setState(
                    {
                        data: response.data
                    });
            }).catch(axiosError);
    }

    render() {
        const data = this.state.data;
        return (
            <div className="hrm-summary">
                <h5>HRM</h5>
                <ul className="hrm-info">
                    <li>
                        TOEIC score: {`${data.toeicScore > 0 ? `${data.toeicScore} (${moment(data.toeicExamDate).format("DD MMM, YYYY")})` : 'N/A'}`}
                    </li>
                    <li>Missing Check In/Out: {data.missingCheckInCheckOut}</li>
                    <li>Late In/Early Out: {data.totalLateInEarlyOut}</li>
                    <li>Unpaid leave: {data.unpaidLeave}</li>
                    <li>Unplaned Annual Leave: {data.unplanedAnnualLeave}</li>
                    <li>Incident: {data.incident}</li>
                </ul>
            </div >
        );
    }
} 