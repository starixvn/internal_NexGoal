import React, { Component } from 'react';
import { getRoleClassName, calendar } from '../../../common-functions';


export default class Feedback extends Component {
    constructor(props) {
        super(props);

        this.state = {
            feedbacks : []
        };
    };

    componentWillMount = () => {
        this.setState({
            isLiked: false,
            showCollapseExpandButton: false,
            isExpanded: false
        });
    }

    componentDidMount = () => {
        if (this.props.feedback)
            this.detectCollapseExpandComment();
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        const height = this.element.offsetHeight;
        if (height > 40) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    }

    render() {
        const { isLiked, showCollapseExpandButton } = this.state;
        const { feedback, isReadOnly } = this.props;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isOwner = (curUserCode == feedback.creator.code); 

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';
        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

        return (
            <div className="feedback">
                <div className="avatar-user small">
                    <img src={feedback.creator.avatar} alt="avatar" className="avatar" />
                </div>
                <div className="feedback-content">
                    <div className="person-info">
                        <span className={getRoleClassName(feedback.roleId)}> {feedback.creator.nickName} </span>
                        {
                            isOwner && !isReadOnly &&
                            <a class="fa icon-edit custom-icon-edit"></a>
                        }
                        <span class="published-time">{calendar(feedback.createdDate)}</span>
                    </div>
                    <p className={commentWrapperClass} ref={element => this.element = element}>{feedback.content}</p>
                    {
                        showCollapseExpandButton ?
                            <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
                <div className="feedback-reaction">
                    <div className="interaction-statistic">
                        <div className="likes-interaction">
                            {
                                !this.props.isReadOnly &&
                                <a className={`fa ${isLiked ? 'icon-thumb-up active' : 'icon-thumb-up-line'}`}
                                    name="like-button"
                                    onClick={this.handleLikeEvent}
                                ></a>
                            }
                            <a className="likes-statistic" onClick={this.handleOpenLikeModalEvent}>0 Likes</a>
                        </div>
                    </div>
                </div>
            </div>
        );
    };
}