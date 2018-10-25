import React, { Component, Fragment } from "react";
import moment from 'moment';

import { DateFormat, PerformanceProcess } from '../../../constants';

export default class TimelineNode extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.position !== nextProps.position
            || JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

    getRoleClassName = (roleId) => {
        let className = '';
        switch (roleId) {
            case PerformanceProcess.Self:
                className = 'role-member';
                break;
            case PerformanceProcess.ProjectManager:
                className = 'role-pm';
                break;
            case PerformanceProcess.Evaluator:
                className = 'role-evaluator';
                break;
            case PerformanceProcess.Reviewer:
                className = 'role-reviewer';
                break;
        }

        return className;
    }

    handleClick = () => this.props.onClick(this.props.data.performanceRecordId)

    render() {
        const data = this.props.data;
        const position = this.props.position;
        const className = this.getRoleClassName(data.roleId);

        return (
            <Fragment>
                <p className="timeline-grade" style={{ right: position }} onClick={this.handleClick} >{data.grade ? data.grade.name : ""}</p>
                <div className="timeline-info" style={{ right: position - 22 }} onClick={this.handleClick}>
                    <p className={"name " + className}>{data.creator.nickName}</p>
                    <p className="create-date">{moment(data.createdDate).format(DateFormat)}</p>
                </div>
            </Fragment >
        );
    }
}