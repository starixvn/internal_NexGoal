import React, { Component } from "react";
import moment from 'moment';

export default class TimelineLabel extends Component {
    constructor(props) {
        super(props);
        this.state = {
            PCI: this.props.PCI,
            isShowComment: false
        };
        this.showComment = this.showComment.bind(this);
        this.hideComment = this.hideComment.bind(this);
    }

    showComment = function () {
        this.setState({ isShowComment: true });
    }

    hideComment = function () {
        this.setState({ isShowComment: false });
    }

    render() {
        const pci = this.props.PCI;

        return (
            <div>
                <p style={{ margin: 0 }}
                    onMouseEnter={this.showComment}
                    onMouseLeave={this.hideComment}
                >
                    <b>{pci.creator.nickName}</b>: <span className="grade-exceptional">{pci.grade.name}</span> ({moment(pci.createdDate).format("DD MMM")})
                </p>
                {this.state.isShowComment &&
                    <div className="comment"
                        style={{
                            position: "absolute",
                            width: "300px",
                            top: "63px",
                            left: "-50px",
                            border: "1px solid",
                            borderRadius: "5px",
                            backgroundColor: "#eaf8f9",
                            textAlign: "left",
                            padding: "5px"
                        }}
                    >
                        <p><b>{pci.creator.nickName}</b> checked in at {moment(pci.createdDate).format("DD MMM")}</p>
                        <p>Grade: <span className="grade-exceptional">{pci.grade.name}</span> </p>
                        <p>{pci.comment}</p>
                    </div>
                }
            </div>

        );
    }
}