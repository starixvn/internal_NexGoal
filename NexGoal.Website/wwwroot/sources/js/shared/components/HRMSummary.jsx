import React from 'react';
import moment from 'moment';
import { DateFormat } from '../../constants';
import getResource from '../../resourceHelper';

export default function HrmDetail(props) {
    const toeicClass = props.hrmDetail.isPassedToeic ? '' : 'red';
    const expireDate = moment(props.hrmDetail.toeicExamDate).add(3, 'years').format(DateFormat);
    const renderToeicScore = () => {
        if (props.hrmDetail.toeicScore > 0) {
            return (
                <li>{getResource('Toeic')}<span className="bold">: <span className={toeicClass}>{props.hrmDetail.toeicScore} </span>
                    (exp {expireDate})</span>
                </li>
            );
        } else {
            return (
                <li>{getResource('Toeic')}<span className="bold">: <span className={toeicClass}>{getResource('N/A')}</span></span>
                </li>
            );
        }
    };

    return (
        <div className="hrm-summary box">
            <h5>{getResource('Hrm')}</h5>
            <ul className="hrm-info">
                {renderToeicScore()}
                <li>{getResource('MissingCheckInOut')}<span className="bold">: {props.hrmDetail.missingCheckInCheckOut}</span></li>
                <li>{getResource('AverageWorkDays')}<span className="bold">: {props.hrmDetail.averageWorkingDays}</span></li>
                <li>{getResource('AverageSpentTime')}<span className="bold">: {props.hrmDetail.averageSpentTime}</span></li>
                <li>{getResource('LateInEarlyOut')}<span className="bold">: {props.hrmDetail.totalLateInEarlyOut}</span></li>
                <li>{getResource('UnpaidLeave')}<span className="bold">: {props.hrmDetail.unpaidLeave}</span></li>
                <li>{getResource('UnplanedLeave')}<span className="bold">: {props.hrmDetail.unplanedAnnualLeave}</span></li>
                <li>{getResource('Discipline')}<span className="bold">: {props.hrmDetail.incident}</span></li>
            </ul>
        </div >
    );
}