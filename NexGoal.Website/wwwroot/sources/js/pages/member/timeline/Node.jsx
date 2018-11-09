import React, { Component, Fragment } from "react";

import { getRoleClassName } from '../../../common-functions';

export default class TimelineNode extends Component {
    shouldComponentUpdate(nextProps) {
        if (this.props.position !== nextProps.position
            || JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

    handleClick = () => {
        this.props.onClick(this.props.data.performanceRecordId);
    }

    render() {
        const data = this.props.data;
        const position = this.props.position;

        return (
            <Fragment>
                <p className={`timeline-grade ${getRoleClassName(data.roleId)}`} style={{ right: position - 34 }} onClick={this.handleClick} >{data.grade ? data.grade.name : ""}</p>
                <div className="timeline-info" style={{ right: position - 44 }} onClick={this.handleClick}>
                    <p className={`name ${getRoleClassName(data.roleId)}`}>{data.creator.nickName}</p>
                </div>
            </Fragment >
        );
    }
}