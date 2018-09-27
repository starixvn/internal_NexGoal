import React, { Component } from 'react';
import PropTypes from 'prop-types';

export default class PerformanceRecord extends Component {
    static propTypes = {
        avatar: PropTypes.string,
        nickName: PropTypes.string,
        submittedDate: PropTypes.string,
        modifiedDate: PropTypes.string,
        isHost: PropTypes.bool,
        editUrl: PropTypes.string,
        projects: PropTypes.string,
    };

    static defaultProps = {
        avatar: '',
        nickName: '',
        submittedDate: '',
        modifiedDate: '',
        isHost: false,
        editUrl: '',
        projects: ''
    };

    constructor(props) {
        super(props);
        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };
    }

    componentDidMount() {
        this.detectCollapseExpandComment();
    }

    collapseExpandComment = () => {
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
                <div className="date">
                    <div className="final-date">{this.props.submittedDate}</div><div className="edit-date">Edited on {this.props.modifiedDate}</div>
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
                    {
                        this.props.isHost && this.props.editUrl &&
                        <div className="edit">
                            <a href={this.props.editUrl}>
                                <span className="icon-square-edit-outline" />
                            </a>
                        </div>
                    }
                    {
                        this.props.grade &&
                        <div className="grade">Grade: <span className="grade-detail">{this.props.grade}</span></div>
                    }
                    {
                        this.props.projects &&
                        <div className="project">Project: <span className="project-detail">{this.props.projects}</span></div>
                    }
                </div>
            </div >
        );
    }
}