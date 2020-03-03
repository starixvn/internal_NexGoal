import React, { Component, Fragment } from 'react';
import { Button, FormGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';
import axios from 'axios';
import { stringify } from 'qs';
import Feedback from './Feedback';
import LikesStatisticModal from './LikesStatisticModal';
import TextareaFullscreenEditor from '../textarea-fullscreen/index';
import { PostRequestHeaders } from '../../../constants';
import { axiosError, getCurrentUserInfo, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';

export default class Interaction extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isEnabled: false,
            isLiked: false,
            showLikeModal: false,
            isOpenForm: false,
            feedbackContent: '',
            reactions: []
        };

        this.textareaEditor = React.createRef();
    };

    componentWillMount = () => {
        const currentUserCode = document.getElementById('employeeInfo-code').value;
        const { reactions, feedbacks } = this.props;
        const isLiked = reactions && reactions.some(reaction => reaction.creator.code === currentUserCode);
        const numOfLikes = reactions ? reactions.length : 0;
        const isCommented = feedbacks && feedbacks.some(feedback => feedback.creator.code === currentUserCode);
        const numOfFeedbacks = feedbacks ? feedbacks.length : 0;

        this.setState(
            {
                isLiked: isLiked,
                reactions: reactions,
                numOfFeedbacks: numOfFeedbacks,
                feedbacks: feedbacks,
                isCommented: isCommented,
                numOfLikes: numOfLikes,
                currentUserCode: currentUserCode
            }
        );
    }

    handleLikeEvent = event => {
        event.preventDefault();
        const recordId = this.props.recordId;
        const curUserCode = this.state.currentUserCode;
        const isLiked = !this.state.isLiked;
        const oldNumOfLikes = this.state.numOfLikes;
        const numOfLikes = isLiked ? oldNumOfLikes + 1 : oldNumOfLikes - 1;
        const url = 'PRReaction/' + (isLiked ? 'Create' : 'Remove');

        this.setState({ numOfLikes: numOfLikes, isLiked: isLiked });

        axios({
            url: resolveClientUrl(url),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PerformanceRecordId: recordId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (response.data.isSuccessful) {
                if (isLiked) {
                    this.addNewReaction(recordId, curUserCode);
                }
                else {
                    this.removeReaction(recordId, curUserCode);
                }
            }
            else {
                this.setState({ numOfLikes: oldNumOfLikes, isLiked: !isLiked });
            }
        }).catch(axiosError);
    }

    handleCloseLikeModal = () => {
        this.setState({ showLikeModal: false });
    }

    handleOpenLikeModalEvent = event => {
        event.preventDefault();

        if (this.state.numOfLikes > 0) {
            this.setState({
                showLikeModal: true
            });
        }
    }

    handleOpenForm = () => {
        this.setState({ isOpenForm: true }, () => {
            this.textareaEditor.current.textareaEditor.current.focus();
        });
    }

    handleCancel = () => {
        this.setState({
            isOpenForm: false,
            feedbackContent: '',
            errors: {}
        });
    }

    handleSubmit = () => {
        const { feedbackContent } = this.state;

        if (feedbackContent === '') {
            this.setState({
                errors: {
                    feedbackContent: 'Comment can\'t be empty'
                }
            });

            return;
        }

        axios({
            url: resolveClientUrl('PRFeedback/Create'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PerformanceRecordId: this.props.recordId,
                Content: feedbackContent
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (response.data.isSuccessful) {
                const numOfFeedbacks = this.state.numOfFeedbacks + 1;
                this.getNewFeedback();

                this.setState({
                    numOfFeedbacks: numOfFeedbacks,
                    isCommented: true
                });
                toast.success(getResource('CreatedSuccessfully'));
                this.handleCancel();
            }
            else {
                toast.error(response.data.message);
            }
        }).catch(axiosError);
    }

    handleOnBlurEvent = () => {
        if (this.state.feedbackContent === '') {
            this.handleCancel();
        }
    }

    handleCommentChange = event => {
        const errors = {};

        if (!event.target.value) {
            this.handleCancel();
        }

        this.setState({
            feedbackContent: event.target.value,
            errors: errors
        });
    }

    renderFeedback = feedback => {
        return (
            <Feedback
                feedback={feedback}
                isReadOnly={this.props.isReadOnly}
                recordId={this.props.recordId}
            />
        );
    }

    renderFeedbackInputBox = () => {
        const {
            isOpenForm,
            feedbackContent,
            isCommented,
            errors } = this.state;
        const { isReadOnly, isDisableComment } = this.props;
        const currentUserInfo = getCurrentUserInfo();

        return (
            (!isReadOnly && !isCommented && !isDisableComment)
            &&
            <div className="comment-box">
                <div className="avatar-user custom-avatar">
                    <img src={currentUserInfo.avatar} alt="avatar" className="avatar" />
                    <div className="input-comment-pr">
                        {
                            !isOpenForm &&
                            <input className="form-control"
                                style={{ height: 36 }}
                                placeholder="Add a comment..."
                                onClick={this.handleOpenForm}
                            />
                        }
                        {
                            isOpenForm &&
                            <form className="form-performance create">
                                <FormGroup>
                                    <TextareaFullscreenEditor ref={this.textareaEditor}
                                        className={`form-control comment-field ${!isEmpty(errors) && errors.feedbackContent ? 'input-validation-error' : ''}`}
                                        disableFullScreenMode="true"
                                        name="comment"
                                        value={feedbackContent}
                                        onChange={this.handleCommentChange}
                                        onBlurEvent={this.handleOnBlurEvent}
                                    />
                                </FormGroup>

                                <FormGroup className="text-right">
                                    <Button bsStyle="primary" name="save" onClick={this.handleSubmit} disabled={this.state.isSubmitting}>Publish</Button>
                                </FormGroup>
                            </form>
                        }
                    </div>
                </div>
            </div>
        );
    }

    renderInteractionBar = () => {
        const {
            isLiked,
            numOfLikes,
            numOfFeedbacks,
            isCommented,
            currentUserCode} = this.state;
        const { isReadOnly, isDisableComment } = this.props;
        const prCreator = this.props.creator;
        const isOwner = (prCreator.code === currentUserCode);

        return (
            <div className={`interaction-statistic ${isReadOnly || isOwner ? 'expand' : ''}`}>
                <div className="likes-interaction">
                    {
                        !isReadOnly && !isOwner &&
                        <a className={`fa ${isLiked ? 'icon-thumb-up active' : 'icon-thumb-up-line'}`}
                            name="like-button"
                            onClick={this.handleLikeEvent}
                        ></a>
                    }
                    <a className="likes-statistic" onClick={this.handleOpenLikeModalEvent}>{numOfLikes} Likes</a>
                </div>
                {
                    !isDisableComment && (
                        (isReadOnly || isCommented) ?
                            <div className="comments-interaction">
                                <a className="comments-statistic">{numOfFeedbacks} Comments</a>
                            </div>
                            :
                            <div className="comments-interaction">
                                <a className="fa icon-comment-text-outline"
                                    name="comment-button"
                                    onClick={this.handleOpenForm}>
                                </a>
                                <a className="comments-statistic" onClick={this.handleOpenForm}>{numOfFeedbacks} Comments</a>
                            </div>
                    )
                }
            </div>
        );
    }

    getNewFeedback = () => {
        const currentUserCode = this.state.currentUserCode;

        axios.get(resolveClientUrl('PRFeedback/GetByPRId'),
            {
                params: {
                    PerformanceRecordId: this.props.recordId,
                    ActorCode: currentUserCode
                }
            }).then(response => {
                const newFeedback = response.data;
                const feedbacks = this.state.feedbacks;
                feedbacks.push(newFeedback);

                this.setState({
                    feedbacks: feedbacks
                });
            }).catch(axiosError);
    }

    addNewReaction = (recordId, creatorCode) => {
        axios.get(
            resolveClientUrl('PRReaction/GetByPRId'),
            {
                params: {
                    PerformanceRecordId: recordId,
                    ActorCode: creatorCode
                }
            }
        ).then(response => {
            const newReaction = response.data;
            const reactions = this.state.reactions;
            reactions.push(newReaction);

            this.setState({ reactions: reactions });
        }).catch(axiosError);
    }

    removeReaction = (recordId, creatorCode) => {
        let reactions = this.state.reactions;
        reactions = reactions.filter(reaction => (reaction.PerformanceRecordId !== recordId
            && reaction.creator.code !== creatorCode));

        this.setState({ reactions: reactions });
    }

    render() {
        const { showLikeModal, reactions, feedbacks } = this.state;

        return (
            <Fragment>
                <LikesStatisticModal
                    showLikeModal={showLikeModal}
                    reactions={reactions}
                    handleCloseLikeModal={this.handleCloseLikeModal}
                />
                <div className="comment-pr">
                    {this.renderInteractionBar()}
                    {this.renderFeedbackInputBox()}
                    {
                        feedbacks &&
                        feedbacks.map(this.renderFeedback)
                    }
                </div>
            </Fragment>
        );
    };
}