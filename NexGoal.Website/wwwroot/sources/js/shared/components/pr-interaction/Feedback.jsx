import React, { Component, Fragment } from 'react';
import { Button, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { isEmpty } from 'lodash';
import { PostRequestHeaders } from '../../../constants';
import { getRoleClassName, calendar, resolveClientUrl, axiosError } from '../../../common-functions';
import LikesStatisticModal from './LikesStatisticModal';
import getResource from '../../../resourceHelper';
import { toast } from 'react-toastify';

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
        const feedback = this.props.feedback;
        const reactions = feedback.reactions;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isLiked = reactions && reactions.some(reaction => reaction.creator.code === curUserCode);
        const numOfLikes = reactions ? reactions.length : 0;

        this.setState({
            isLiked: isLiked,
            numOfLikes: numOfLikes,
            content: feedback.content,
            modifiedDate: feedback.modifiedDate,
            reactions: reactions
        });
    }

    componentDidMount = () => {
        if (this.props.feedback) {
            this.detectCollapseExpandComment();
        }
    }

    handleLikeEvent = event => {
        event.preventDefault();
        const isLiked = !this.state.isLiked;
        const oldNumOfLikes = this.state.numOfLikes;
        const numOfLikes = isLiked ? oldNumOfLikes + 1 : oldNumOfLikes - 1;
        const url = 'PRFeedbackReaction/' + (isLiked ? 'Create' : 'Remove');
        const feedbackId = this.props.feedback.prFeedbackId;
        const curUserCode = document.getElementById('employeeInfo-code').value;

        this.setState({ numOfLikes: numOfLikes, isLiked: isLiked });

        axios({
            url: resolveClientUrl(url),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PRFeedbackId: feedbackId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (response.data.isSuccessful) {
                if (isLiked) {
                    this.addNewReaction(feedbackId, curUserCode);
                }
                else {
                    this.removeReaction(feedbackId, curUserCode);
                }
            }
            else {
                this.setState({ numOfLikes: oldNumOfLikes, isLiked: !isLiked });
            }
        }).catch(axiosError);
    }

    handleOpenLikeModalEvent = event => {
        event.preventDefault();

        if (this.state.numOfLikes !== 0) {
            this.setState({
                showLikeModal: true
            });
        }
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

        axios({
            url: resolveClientUrl('PRFeedback/Edit'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PRFeedbackId: this.props.feedback.prFeedbackId,
                Content: this.state.newContent
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (response.data.isSuccessful) {
                this.setState({
                    content: this.state.newContent,
                    isEditing: false,
                    modifiedDate: new Date()
                });
                toast.success(getResource('UpdatedSuccessfully'));
                this.detectCollapseExpandComment()
            }
            else {
                toast.error(response.data.message);
            }
        }).catch(axiosError);
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
        else {
            this.setState({
                showCollapseExpandButton: false,
                isExpanded: false
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

    addNewReaction = (feedbackId, creatorCode) => {
        axios.get(
            resolveClientUrl('PRFeedbackReaction/GetByFeedbackId'),
            {
                params: {
                    PRFeedbackId: feedbackId,
                    ActorCode: creatorCode
                }
            }
        ).then(response => {
            const newReaction = response.data;
            let reactions = this.state.reactions;
            reactions.push(newReaction);

            this.setState({ reactions: reactions });
        }).catch(axiosError);
    }

    removeReaction = (feedbackId, creatorCode) => {
        let reactions = this.props.feedback.reactions;

        reactions = reactions.filter(reaction => (reaction.feedbackId !== feedbackId
            && reaction.creator.code !== creatorCode));

        this.setState({ reactions: reactions });
    }

    render() {
        const { isLiked,
            showLikeModal,
            numOfLikes,
            modifiedDate,
            reactions } = this.state;
        const { feedback, isReadOnly } = this.props;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isOwner = (curUserCode === feedback.creator.code);
        const now = moment(new Date());
        const duration = moment.duration(now.diff(feedback.createdDate));
        const isExpired = duration._data.days >= 7;
        const showEditBtn = isOwner && !isReadOnly && !isExpired;

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
                                showEditBtn &&
                                <a class="fa icon-edit custom-icon-edit" onClick={this.handleEditFeedback}></a>
                            }
                            {
                                modifiedDate &&
                                <div className="edited-date">
                                    <span className="icon-edited-date" />
                                    <span className={`edited-tooltip ${!showEditBtn ? 'float-right' : ''}`}>
                                        <i className="arrow" />
                                        Edited on {calendar(modifiedDate)}
                                    </span>
                                </div>
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