import React, { Component, Fragment } from 'react';
import { Modal, Button, FormGroup } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';

import axios from 'axios';
import { stringify } from 'qs';
import Feedback from './Feedback';
import TextareaFullscreenEditor from '../textarea-fullscreen/index';
import { PostRequestHeaders } from '../../../constants';
import { axiosError, getRoleClassName, getCurrentUserInfo, resolveClientUrl } from '../../../common-functions';

export default class Interaction extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isEnabled: false,
            isLiked: false,
            showLikeModal: false,
            isOpenForm: false,
            feedbackContent: ''
        };

        this.textareaEditor = React.createRef();
    };

    componentWillMount = () => {
        const currentUserCode = document.getElementById('employeeInfo-code').value;
        const { reactions, feedbacks } = this.props;
        const isLiked = reactions && reactions.some(reaction => reaction.creator.code === currentUserCode);
        const isCommented = feedbacks && feedbacks.some(feedback => feedback.creator.code === currentUserCode);
        const numOfLikes = reactions ? reactions.length : 0;
        const numOfFeedbacks = feedbacks ? feedbacks.length : 0;

        this.setState(
            {
                isLiked: isLiked,
                numOfLikes: numOfLikes,
                reactions: reactions,
                numOfFeedbacks: numOfFeedbacks,
                feedbacks: feedbacks,
                isCommented: isCommented
            }
        );
    }

    handleLikeEvent = event => {
        event.preventDefault();
        const isLike = !this.state.isLiked;
        const numOfLikes = isLike ? this.state.numOfLikes + 1 : this.state.numOfLikes - 1;
        const url = 'PRReaction/' + (isLike ? 'Create' : 'Remove');

        this.setState({ numOfLikes: numOfLikes, isLiked: isLike });

        axios({
            url: resolveClientUrl(url),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PerformanceRecordId: this.props.recordId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (!response.data.isSuccessful) {
                const numOfLikes = isLike ? this.state.numOfLikes - 1 : this.state.numOfLikes + 1;
                this.setState({ numOfLikes: numOfLikes, isLiked: !isLike });
            }
        }).catch(axiosError);
    }

    handleCloseLikeModal = () => {
        this.setState({ showLikeModal: false });
    }

    handleOpenLikeModalEvent = event => {
        event.preventDefault();

        if (this.state.numOfLikes == 0) {
            return;
        }

        axios.get(
            resolveClientUrl('PRReaction/GetByPRId'),
            { params: { PerformanceRecordId: this.props.recordId, ActorId: 0 } }
        ).then(response => {
            const reactions = response.data;
            const numOfLikes = reactions ? reactions.length : 0;

            this.setState({ showLikeModal: true, reactions: reactions, numOfLikes: numOfLikes });
        }).catch(axiosError);
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
                let numOfFeedbacks = this.state.numOfFeedbacks + 1;
                this.getNewFeedback();

                this.setState({
                    numOfFeedbacks: numOfFeedbacks,
                    isCommented: true
                });

                this.handleCancel();
            }
            else {
                toast.error(response.data.message);
            }
        }).catch(axiosError);
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

    renderUserLiked = reaction => {
        return (
            <div className="avatar-user">
                <img src={reaction.creator.avatar} alt="avatar" className="avatar" />
                <div className="person-info">
                    <span className={getRoleClassName(reaction.roleId)}> {reaction.creator.nickName} </span>
                </div>
            </div>
            );
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

    getNewFeedback = () => {
        axios.get(resolveClientUrl('PRFeedback/GetByPRId'),
            {
                params: {
                    PerformanceRecordId: this.props.recordId
                }
            }).then(response => {
                this.setState({ feedbacks: response.data });
            }).catch(axiosError);
    }

    render() {
        const {
            isLiked,
            isOpenForm,
            showLikeModal,
            feedbackContent,
            numOfLikes,
            reactions,
            numOfFeedbacks,
            feedbacks,
            isCommented,
            errors } = this.state;
        const { isReadOnly, isDisableComment } = this.props;
        const currentUserInfo = getCurrentUserInfo();

        return (
            <Fragment>
                <Modal
                    size="sm"
                    aria-labelledby="example-modal-sizes-title-sm"
                    show={showLikeModal}
                    onHide={this.handleCloseLikeModal}
                    className="like-statistic-modal"
                    dialogClassName="like-dialog">
                    <Modal.Header className="custom-modal-header">
                        <h4 className="likes-counter">{numOfLikes} LIKES</h4>
                    </Modal.Header>
                    <Modal.Body className="custom-modal-content">
                        {
                            reactions &&
                            reactions.map(this.renderUserLiked)
                        }
                    </Modal.Body>
                    <Modal.Footer className="custom-modal-footer">
                        <Button className="btn-cancel" onClick={this.handleCloseLikeModal}> Close </Button>
                    </Modal.Footer>
                </Modal>
                <div className="comment-pr">
                    <div className="interaction-statistic">
                        <div className="likes-interaction">
                            {
                                !isReadOnly &&
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
                                        <a className="fa icon-comment-text-outline custom-comment-icon"
                                            name="comment-button"
                                            onClick={this.handleOpenForm}>
                                        </a>
                                        <a className="comments-statistic" onClick={this.handleOpenForm}>{numOfFeedbacks} Comments</a>
                                    </div>
                                )
                        }
                    </div>
                    {
                        (!isReadOnly && !isCommented && !isDisableComment)
                        &&
                        <div className="comment-box">
                            <div className="avatar-user custom-avatar small">
                                <img src={currentUserInfo.avatar} alt="avatar" className="avatar" />
                                <div className="input-comment-pr">
                                    {
                                        !isOpenForm &&
                                        <input className="form-control"
                                            style={{ height: 30 }}
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
                    }
                    {
                        feedbacks &&
                        feedbacks.map(this.renderFeedback)
                    }
                </div>
            </Fragment>
        );
    };
}