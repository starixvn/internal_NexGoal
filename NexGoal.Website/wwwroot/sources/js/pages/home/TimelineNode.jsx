import React, { Component, Fragment } from "react";
import moment from 'moment';

export default class TimelineNode extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isShowComment: false
        };
    }

    render() {
        const data = this.props.data;
        const position = this.props.position;
        return (
            <Fragment>
                <p className="timeline-grade" style={{ left: position }}>{data.grade.name}</p>
                <div className="timeline-info" style={{ left: position - 25 }}>
                    <p className="name" style={{ backgroundColor: "#61c361" }}>{data.creator.nickName}</p>
                    <p className="create-date">{moment(data.createdDate).format("DD MMM")}</p>
                </div>
            </Fragment >
        );
    }
}