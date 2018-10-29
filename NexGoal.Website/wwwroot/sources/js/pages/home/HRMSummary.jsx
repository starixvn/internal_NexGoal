import React from "react";
import moment from 'moment';

export default function HrmDetail(props) {
    return (
        <div className="hrm-summary box">
            <h5>HRM</h5>
            <ul className="hrm-info">
                <li>
                    TOEIC score: {`${props.hrmDetail.toeicScore > 0 ? `${props.hrmDetail.toeicScore} (${moment(props.hrmDetail.toeicExamDate).format("DD MMM, YYYY")})` : 'N/A'}`}
                </li>
                <li>Missing Check In/Out: {props.hrmDetail.missingCheckInCheckOut}</li>
                <li>Late In/Early Out: {props.hrmDetail.totalLateInEarlyOut}</li>
                <li>Unpaid leave: {props.hrmDetail.unpaidLeave}</li>
                <li>Unplaned Annual Leave: {props.hrmDetail.unplanedAnnualLeave}</li>
                <li>Incident: {props.hrmDetail.incident}</li>
            </ul>
        </div >
    );
}