import React, { Component } from 'react';

export default class PerformanceRecord extends Component {
    constructor(props) {
        super(props);
        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };

        this.collapseExpandComment = this.collapseExpandComment.bind(this);
    }

    componentDidMount() {
        this.detectCollapseExpandComment();
    }

    collapseExpandComment() {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        let height = this.element.offsetHeight;
        if (height >= 55) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
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
                    <p className={this.state.isExpanded ? "collapsed" : "expanded"} ref={(element) => this.element = element}>{this.props.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={this.state.isExpanded ? "icon-chevron-down" : "icon-chevron-up"} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>

                <div className="footer">
                    <div className="edit"><span className="icon-square-edit-outline" /></div>
                    <div className="grade">Grade <span className="grade-exceptional">{this.props.grade}</span></div>
                    <div className="project">Project <span className="project-detail">{this.props.projects}</span></div>
                </div>
            </div >
        );
    }
}