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

    componentDidMount = function () {
        this.getHrmSummary();
    }

    getHrmSummary = function () {
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
            <div style={{ marginTop: 20, border: "2px solid #ccc", borderRadius: "5px", width: "max-content", padding: 20, position: "relative" }}>
                <h3 style={{
                    margin: 0,
                    position: "absolute",
                    top: "-20px",
                    background: "#fff",
                    padding: "5px"
                }}>HRM</h3>
                <ul style={{ fontSize: "14px", fontWeight: 550 }}>
                    <li style={{ paddingTop: "15px" }} >
                        TOEIC score:
                    {
                            data.toeicScore > 0 ? data.toeicScore(moment(data.toeicExamDate).format("DD MMM, YYYY")) : " No Score"
                        }
                    </li>
                    <li>Missing Check In/Out: {data.missingCheckInCheckOut}</li>
                    <li>Late In/Early Out: {data.totalLateInEarlyOut}</li>
                    <li>Unpaid leave: {data.unpaidLeave}</li>
                    <li>Unplaned Annual Leave: {data.unplanedAnnualLeave}</li>
                    <li>Incident: <a href="/Discipline/index">{data.incident}</a></li>
                </ul>
            </div >
        );
    }
} 