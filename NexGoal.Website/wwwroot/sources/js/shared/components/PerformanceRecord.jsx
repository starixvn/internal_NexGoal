import React, { Component } from 'react';
import PropTypes from 'prop-types';
import avatar1 from '../../../styles/images/avatar.png';

export default class PerformanceRecord extends Component {
    static propTypes = {
        objectId: PropTypes.number,
        avatar: PropTypes.string,
        nickName: PropTypes.string,
        submittedDate: PropTypes.string,
        modifiedDate: PropTypes.string,
        isHost: PropTypes.bool,
        projects: PropTypes.string,
        onEdit: PropTypes.func
    };

    static defaultProps = {
        objectId: 0,
        avatar: '',
        nickName: '',
        submittedDate: '',
        modifiedDate: '',
        isHost: false,
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

    handleEdit = () => {
        this.props.onEdit(this.props.objectId);
    }

    render() {
        const avatar = `dist/styles/${avatar1}`;
        return (
            <div className="record">
                <div className="avatar-user">
                    {/* <img src={this.props.avatar} className="avatar" /> */}
                    <img src={avatar} className="avatar" />
                    <div className="name">{this.props.nickName}</div>
                    <div className="submit-date">{this.props.submittedDate}</div>
                </div>
                <div className="date">
                    {
                        this.props.modifiedDate &&
                        <div className="edit-date">Edited on {this.props.modifiedDate}<span class="icon-playlist"></span></div>
                    }
                </div>
                <div className="comment" >
                    <p className={this.state.isExpanded ? "collapsed" : "expanded"} ref={(element) => this.element = element}>{this.props.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={this.state.isExpanded ? "see-more" : "see-less"} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>                    
                <div className="grade">Grade: <div className="grade-detail">{this.props.grade}</div></div>            
                <div className="project">Project: <div className="project-detail">{this.props.projects}</div></div>
            </div >
        );
    }
}