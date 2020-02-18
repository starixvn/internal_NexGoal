import React, { Component, Fragment } from 'react';
import { Modal, Button, FormGroup } from 'react-bootstrap';
import { isEmpty } from 'lodash';
import axios from 'axios';
import { stringify } from 'qs';
import CommentList from './CommentList';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../textarea-fullscreen/index';
import { PostRequestHeaders } from '../../../constants';
import { axiosError, getRoleClassName, getCurrentUserInfo, resolveClientUrl } from '../../../common-functions';

export default class Comment extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isEnabled: false,
            isLiked: false,
            showLikeModal: false,
            isOpenForm: false,
            prComment: {
                content: ''
            }
        };

        this.textareaEditor = React.createRef();
    };

    componentWillMount = () => {
        const currentUserCode = document.getElementById('employeeInfo-code').value;
        const reactions = this.props.reactions;

        const isLiked = reactions.some(reaction => reaction.creator.code == currentUserCode);
        const numOfLikes = reactions ? reactions.length : 0;

        this.setState({ isLiked: isLiked, numOfLikes: numOfLikes, reactions: reactions });
    }

    handleLikeEvent = event => {
        event.preventDefault();
        var isLike = !this.state.isLiked;
        var numOfLikes = isLike ? this.state.numOfLikes + 1 : this.state.numOfLikes - 1;

        this.setState({ numOfLikes: numOfLikes, isLiked: isLike });

        var url = 'PRReaction/' + (isLike ? 'Create' : 'Remove');

        axios({
            url: resolveClientUrl(url),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                PerformanceRecordId: this.props.recordId
            }),
            headers: PostRequestHeaders
        }).then(response => {
            if (!response.data.isSuccessful)
            {
                var numOfLikes = isLike ? this.state.numOfLikes - 1 : this.state.numOfLikes + 1;
                this.setState({ numOfLikes: numOfLikes, isLiked: !isLike });
            }
        }).catch(axiosError => {
            var numOfLikes = this.state.isLiked ? this.state.numOfLikes - 1
                : this.state.numOfLikes + 1;
            this.setState({ numOfLikes: numOfLikes, isLiked: !this.state.isLiked });
        });
        
    }

    handleCloseLikeModal = () => {
        this.setState({ showLikeModal: false });
    }

    handleOpenLikeModalEvent = event => {
        event.preventDefault();
        axios.get(
            resolveClientUrl("PRReaction/GetByPRId"),
            { params: { PerformanceRecordId: this.props.recordId, ActorId: 0 } }
        ).then(response => {
            const reactions = response.data;
            const numOfLikes = reactions ? reactions.length : 0;

            this.setState({ showLikeModal: true, reactions: reactions, numOfLikes: numOfLikes });
        }).catch(axiosError => {
        });
    }

    handleOpenForm = () => {
        this.setState({ isOpenForm: true }, () => {
            this.textareaEditor.current.textareaEditor.current.focus();
        });
    }

    handleCancel = () => {
        this.setState({
            isOpenForm: false,
            prComment: {
                content: ''
            },
            errors: {}
        });
    }

    handleCommentChange = event => {
        const errors = {};

        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            prComment: event.target.value,
            errors: errors
        });
    }

    renderUserLiked = (reaction) => {
        return (
            <div className="avatar-user">
                <img src={reaction.creator.avatar} alt="avatar" className="avatar" />
                <div className="person-info">
                    <span className={getRoleClassName(reaction.roleId)}> {reaction.creator.nickName} </span>
                </div>
            </div>
            );
    }

    render() {
        const { isLiked, showLikeModal, isOpenForm, errors, prComment, numOfLikes, reactions } = this.state;
        const currentUserInfo = getCurrentUserInfo();
        const { isReadOnly, feedbacks} = this.props;

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
                                <a className={`fa fa-thumbs-up ${isLiked ? 'active' : ''}`}
                                    name="like-button"
                                    onClick={this.handleLikeEvent}
                                ></a>
                            }
                            <a className="likes-statistic" onClick={this.handleOpenLikeModalEvent}>{numOfLikes} Likes</a>
                        </div>
                        {
                            isReadOnly ?
                            <div className="comments-interaction">
                                <a className="comments-statistic">3 Comments</a>
                            </div>
                                :
                            <div className="comments-interaction">
                                <a className="fa icon-comment-text-outline custom-comment-icon"
                                    name="comment-button"
                                    onClick={this.handleOpenForm}>
                                </a>
                                <a className="comments-statistic" onClick={this.handleOpenForm}>3 Comments</a>
                            </div>
                        }
                        
                    </div>
                    {
                        !isReadOnly &&
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
                                                    className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                                    disableFullScreenMode="true"
                                                    name="comment"
                                                    value={prComment.content}
                                                    onChange={this.handleCommentChange}
                                                />
                                            </FormGroup>

                                            <FormGroup className="text-right">
                                                <Button className="btn-cancel" onClick={this.handleCancel}>Cancle</Button>
                                                <Button bsStyle="primary" name="save" onClick={this.handleSubmit} disabled={this.state.isSubmitting}>Send</Button>
                                            </FormGroup>
                                        </form>
                                    }
                                </div>
                            </div>
                        </div>      
                    }
                    <CommentList></CommentList>
                </div>
            </Fragment>
        );
    };
}