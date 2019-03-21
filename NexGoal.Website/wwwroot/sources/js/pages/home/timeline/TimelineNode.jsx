import React, { Component, Fragment } from 'react';
import moment from 'moment';
import getResource from '../../../resourceHelper';
import { getRoleClassName, getNominationClassName } from '../../../common-functions';
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
        let status = getNominationClassName(data.nominationStatusId);
        let statusText = NominationStatuses.find(x => x.value === data.nominationStatusId).text;

        return (
            <Fragment>
                <p className={`timeline-grade ${status}`} style={{ right: position - 22 }} onClick={this.handleClick} >
                    {statusText}
                </p>
                <div className={`timeline-info ${status}`} style={{ right: position - 22 }} onClick={this.handleClick}>
                    <p className={`name ${status}`}>{data.nominationPeriod.shortTitle}</p>
                </div>
            </Fragment>
        );
    }

    render() {
        const { data, position } = this.props;

        if (data.typeId === PerformanceRecordType.SystemNomination) {
            return this.renderSystemNomination(data, position);
        }

        return (
            <Fragment>
                <p className={`timeline-grade ${getRoleClassName(data.roleId)}`} style={{ right: position }} onClick={this.handleClick} >
                    {data.grade ? data.grade.name : getResource('N/A')}
                </p>
                <div className={`timeline-info ${getRoleClassName(data.roleId)}`} style={{ right: position - 22 }} onClick={this.handleClick}>
                    <p className={`name ${getRoleClassName(data.roleId)}`}>{data.creator.nickName}</p>
                    <p className="create-date">{moment(data.createdDate).format(DateFormat)}</p>
                </div>
            </Fragment >
        );
    }
}