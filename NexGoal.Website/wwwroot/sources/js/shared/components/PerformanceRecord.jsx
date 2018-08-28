import React, { Component } from 'react';

export default class PerformanceRecord extends Component {
    constructor(props) {
        super(props);
        this.state = {
            showCollapseExpandButton: false,
            arrow: 'down'
        };

        this.collapseExpandComment = this.collapseExpandComment.bind(this);
    }

    collapseExpandComment() {
        if (this.state.height == 'auto') {
            this.setState({
                height: '55px',
                arrow: 'down'
            })
        } else {
            this.setState({
                height: 'auto',
                arrow: 'up'
            })
        }
    }

    detectCollapseExpandComment() {
        let height = this.element.offsetHeight;
        if (height >= 55) {
            this.setState({
                showCollapseExpandButton: true,
                height: '55px'
            });
        }
    }

    componentDidMount() {
        this.detectCollapseExpandComment();
    }

    render() {
        return (
            <div className="record">
                <div className="avatar-user">
                    <img src={this.props.avatar} className="avatar" />
                    <div className="name">{this.props.nickName}</div>
                    <span className="role">{this.props.jobTitle}</span>
                </div>
                <div className="nominated">{this.props.action} <span className="employee">{this.props.nickName}</span> for {this.props.period}</div>
                <div className="date">
                    <div className="final-date">{this.props.finalDate}</div><div className="edit-date">Edited on {this.props.editDate}</div>
                </div>

                <div className="comment" >
                    <p className="content" ref={(element) => this.element = element} style={{ height: this.state.height }}>{this.props.comment}</p>
                    {this.state.showCollapseExpandButton ?
                        <span className={"icon-chevron-" + this.state.arrow} onClick={this.collapseExpandComment}></span>
                        : null}
                </div>

                <div className="footer">
                    <div className="edit"><span className="icon-square-edit-outline"></span></div>
                    <div className="grade">Grade <span className="grade-exceptional">{this.props.grade}</span></div>
                    <div className="project">Project <span className="project-detail">{this.props.projects}</span></div>
                </div>
            </div>
        )
    }
}