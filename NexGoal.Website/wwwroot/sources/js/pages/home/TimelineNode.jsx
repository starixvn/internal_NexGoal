import React, { Component, Fragment } from "react";
import moment from 'moment';
import { DateFormat } from '../../constants';

export default class TimelineNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isShowComment: false
        };
    }

    getRoleColor = (data) => {
        var className = "";
        if (data.creatorId === data.employeeId) {
            className = "role-member";
        } else if (data.creator.roles.indexOf("PM") !== -1) {
            className = "role-pm";
        } else {
            className = "role-evaluator";
        }

        return className;
    }

    handleClick = () => this.props.onClick(this.props.data.performanceRecordId)

    render() {
        const data = this.props.data;
        const position = this.props.position;
        const className = this.getRoleColor(data);
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