import React, { Component, Fragment } from "react";
import moment from 'moment';

export default class TimelineNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isShowComment: false
        };
    }

    getRoleColor = (data) => {
        var color = "";
        if (data.creatorId === data.employeeId) {
            color = "#61c361";
        } else if (data.creator.roles.indexOf("PM") !== -1) {
            color = "#0b33ee";
        } else {
            color = "#b707d8";
        }

        return color;
    }
    render() {
        const data = this.props.data;
        const position = this.props.position;
        const color = this.getRoleColor(data);
        return (
            <Fragment>
                <p className="timeline-grade" style={{ left: position }}>{data.grade.name}</p>
                <div className="timeline-info" style={{ left: position - 25 }}>
                    <p className="name" style={{ backgroundColor: color }}>{data.creator.nickName}</p>
                    <p className="create-date">{moment(data.createdDate).format("DD MMM")}</p>
                </div>
            </Fragment >
        );
    }
}