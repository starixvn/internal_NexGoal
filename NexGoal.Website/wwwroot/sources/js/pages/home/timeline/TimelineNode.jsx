import React, { Component, Fragment } from "react";
import moment from 'moment';

import { getRoleClassName } from '../../../common-functions';
import { DateFormat, PerformanceRecordType, NominationStatuses } from '../../../constants';

export default class TimelineNode extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.position !== nextProps.position
            || JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

    handleClick = () => this.props.onClick(this.props.data.performanceRecordId)

    renderSystemNomination = (data, position) => {
        let status = NominationStatuses.find(x => x.value === data.nominationStatusId);

        return (
            <Fragment>
                <p className={`timeline-grade ${status.text}`} style={{ right: position }} onClick={this.handleClick} >
                    {status.text}
                </p>
                <div className="timeline-info" style={{ right: position - 22 }} onClick={this.handleClick}>
                    <p className={`name ${status.text}`}>{data.nominationPeriod.shortTitle}</p>
                </div>
            </Fragment >
        );
    }

    render() {
        const data = this.props.data;
        const position = this.props.position;

        if (data.typeId === PerformanceRecordType.SystemNomination) {
            return this.renderSystemNomination(data, position);
        }

        return (
            <Fragment>
                <p className={`timeline-grade ${getRoleClassName(data.roleId)}`} style={{ right: position }} onClick={this.handleClick} >
                    {data.grade ? data.grade.name : 'N/A'}
                </p>
                <div className="timeline-info" style={{ right: position - 22 }} onClick={this.handleClick}>
                    <p className={`name ${getRoleClassName(data.roleId)}`}>{data.creator.nickName}</p>
                    <p className="create-date">{moment(data.createdDate).format(DateFormat)}</p>
                </div>
            </Fragment >
        );
    }
}