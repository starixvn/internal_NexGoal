import React from "react";
import moment from 'moment';

export default function HrmDetail(props) {
    const toeicClass = props.hrmDetail.isPassedToeic ? '' : 'red';
    let toeicScore;
    if (props.hrmDetail.toeicScore > 0) {
        toeicScore = (<li>Toeic Score: <span className="bold"><span className={toeicClass}>{props.hrmDetail.toeicScore} </span>
            ({moment(props.hrmDetail.toeicExamDate).format("DD MMM, YYYY")})</span></li>);
    } else {
        toeicScore = <li>TOEIC Score: <span className={toeicClass}>N/A</span></li>;
    }

    return (
        <div className="hrm-summary box">
            <h5>HRM</h5>
            <ul className="hrm-info">
                {toeicScore}
                <li>Missing Check In/Out<span className="bold">: {props.hrmDetail.missingCheckInCheckOut}</span></li>
                <li>Average Working Days<span className="bold">: {props.hrmDetail.averageWorkingDays}</span></li>
                <li>Average Spent Time<span className="bold">: {props.hrmDetail.averageSpentTime}</span></li>
                <li>Late In/Early Out<span className="bold">: {props.hrmDetail.totalLateInEarlyOut}</span></li>
                <li>Unpaid Leave<span className="bold">: {props.hrmDetail.unpaidLeave}</span></li>
                <li>Unplaned Annual Leave<span className="bold">: {props.hrmDetail.unplanedAnnualLeave}</span></li>
                <li>Discipline<span className="bold">: {props.hrmDetail.incident}</span></li>
            </ul>
        </div >
    );
}