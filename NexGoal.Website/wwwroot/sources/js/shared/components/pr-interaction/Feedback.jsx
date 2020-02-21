import React, { Component, Fragment } from 'react';
import { Button, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import { isEmpty } from 'lodash';
import { PostRequestHeaders } from '../../../constants';
import { getRoleClassName, calendar, resolveClientUrl, axiosError } from '../../../common-functions';
import LikesStatisticModal from './LikesStatisticModal';

export default class Feedback extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLiked: false,
            showLikeModal: false,
            showCollapseExpandButton: false,
            isExpanded: false,
            numOfLikes: 0,
            isEditing: false,
            content: '',
            enableSaveChange: false
        };
    };

    componentWillMount = () => {
        const reactions = this.props.feedback.reactions;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isLiked = reactions && reactions.some(reaction => reaction.creator.code === curUserCode);
        const numOfLikes = reactions ? reactions.length : 0;

        this.setState({
            isLiked: isLiked,
            numOfLikes: numOfLikes,
            content: this.props.feedback.content
        });
    }

    componentDidMount = () => {
        if (this.props.feedback) {
            this.detectCollapseExpandComment();
        }
    }

    handleLikeEvent = event => {
        event.preventDefault();
        const isLike = !this.state.isLiked;
        const oldNumOfLikes = this.state.numOfLikes; 
        const numOfLikes = isLike ? oldNumOfLikes + 1 : oldNumOfLikes - 1;
        const url = 'PRFeedbackReaction/' + (isLike ? 'Create' : 'Remove');

        this.setState({ numOfLikes: numOfLikes, isLiked: isLike });

        axios({
            url: resolveClientUrl(url),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PRFeedbackId: this.props.feedback.prFeedbackId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (!response.data.isSuccessful) {
                this.setState({ numOfLikes: oldNumOfLikes, isLiked: !isLike });
            }
        }).catch(axiosError);
    }

    handleOpenLikeModalEvent = event => {
        event.preventDefault();

        if (this.state.numOfLikes === 0) {
            return;
        }

        axios.get(
            resolveClientUrl('PRFeedbackReaction/GetByFeedbackId'),
            { params: { PRFeedbackId: this.props.feedback.prFeedbackId, ActorId: 0 } }
        ).then(response => {
            const reactions = response.data;
            const numOfLikes = reactions ? reactions.length : 0;

            this.setState({ showLikeModal: true, reactions: reactions, numOfLikes: numOfLikes });
        }).catch(axiosError);
    }

    handleCloseLikeModal = () => {
        this.setState({
            showLikeModal: false
        });
    }

    handleEditFeedback = () => {
        this.setState({
            isEditing: true,
            enableSaveChange: false,
            newContent: this.state.content
        });
    }

    handleCancelChange = () => {
        this.setState({
            isEditing: false,
            enableSaveChange: false,
            newContent: this.state.content
        });
    }

    handleChangeFeedbackContent = event => {
        const newContent = event.target.value;
        const oldContent = this.state.content;
        const enableSaveChange = newContent !== oldContent;

        this.setState({
            newContent: newContent,
            enableSaveChange: enableSaveChange
        });
    }

    handleSaveChange = event => {
        event.preventDefault();

        if (this.state.newContent === '') {
            this.setState({
                errors: {
                    feedbackContent: 'Content cant be empty'
                }
            });
        }
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        if (!this.element) {
            return;
        }

        const height = this.element.offsetHeight;
        if (height > 40) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    }

    renderFeedbackContent = () => {
        const {
            showCollapseExpandButton,
            isEditing,
            newContent,
            enableSaveChange,
            content,
            errors } = this.state;

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';
        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

        if (!isEditing) {
            return (
                <div>
                    <p className={commentWrapperClass} ref={element => this.element = element}>{content}</p>
                    {
                        showCollapseExpandButton ?
                            <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
            );
        }

        return (
            <div className="editFeedback">
                <textarea className={`form-control comment-field ${!isEmpty(errors) && errors.feedbackContent ? 'input-validation-error' : ''}`}
                    onChange={this.handleChangeFeedbackContent}
                    value={newContent}
                    autoFocus />
                <FormGroup className="text-right">
                    <Button className="btn-cancel btn btn-default" name="cancel" onClick={this.handleCancelChange}>Cancel</Button>
                    <Button bsStyle="primary" name="save" onClick={this.handleSaveChange} disabled={!enableSaveChange}>Save Change</Button>
                </FormGroup>
            </div>
        );
    }

    render() {
        const { isLiked,
            showLikeModal,
            numOfLikes} = this.state;
        const { feedback, isReadOnly } = this.props;
        const reactions = feedback.reactions;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isOwner = (curUserCode === feedback.creator.code);

        return (
            <Fragment>
                <LikesStatisticModal
                    showLikeModal={showLikeModal}
                    reactions={reactions}
                    handleCloseLikeModal={this.handleCloseLikeModal}
                />
                <div className="feedback">
                    <div className="avatar-user small">
                        <img src={feedback.creator.avatar} alt="avatar" className="avatar" />
                    </div>
                    <div className="feedback-content">
                        <div className="person-info">
                            <span className={getRoleClassName(feedback.roleId)}> {feedback.creator.nickName} </span>
                            {
                                isOwner && !isReadOnly &&
                                <a class="fa icon-edit custom-icon-edit" onClick={this.handleEditFeedback}></a>
                            }
                            <span class="published-time">{calendar(feedback.createdDate)}</span>
                        </div>
                        {
                            this.renderFeedbackContent()
                        }
                    </div>
                    <div className="feedback-reaction">
                        <div className="interaction-statistic">
                            <div className="likes-interaction">
                                {
                                    !this.props.isReadOnly && !isOwner &&
                                    <a className={`fa ${isLiked ? 'icon-thumb-up active' : 'icon-thumb-up-line'}`}
                                        name="like-button"
                                        onClick={this.handleLikeEvent}
                                    />
                                }
                                <a className="likes-statistic" onClick={this.handleOpenLikeModalEvent}>{numOfLikes} Likes</a>
                            </div>
                        </div>
                    </div>
                </div>
            </Fragment>
        );
    }
}