import React from "react";
import moment from 'moment';

export default function HrmDetail(props) {
    const toeicClass = props.hrmDetail.isPassedToeic ? '' : 'color-red';
    let toeicScore;
    if (props.hrmDetail.toeicScore > 0) {
        toeicScore = (<li>Toeic Score: <span className={toeicClass}>{props.hrmDetail.toeicScore} </span>
            ({moment(props.hrmDetail.toeicExamDate).format("DD MMM, YYYY")})</li>);
    } else {
        toeicScore = <li>TOEIC Score: <span className={toeicClass}>N/A</span></li>;
    }

    return (
        <div className="hrm-summary box">
            <h5>HRM</h5>
            <ul className="hrm-info">
                {toeicScore}
                <li>Missing Check In/Out: {props.hrmDetail.missingCheckInCheckOut}</li>
                <li>Average Working Days: {props.hrmDetail.averageWorkingDays}</li>
                <li>Average Spent Time: {props.hrmDetail.averageSpentTime}</li>
                <li>Late In/Early Out: {props.hrmDetail.totalLateInEarlyOut}</li>
                <li>Unpaid Leave: {props.hrmDetail.unpaidLeave}</li>
                <li>Unplaned Annual Leave: {props.hrmDetail.unplanedAnnualLeave}</li>
                <li>Discipline: {props.hrmDetail.incident}</li>
            </ul>
        </div >
    );
}