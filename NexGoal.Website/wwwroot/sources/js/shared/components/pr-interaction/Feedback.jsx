import React, { Component, Fragment } from 'react';
import { Modal, Button } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import { PostRequestHeaders } from '../../../constants';
import { getRoleClassName, calendar, resolveClientUrl } from '../../../common-functions';


export default class Feedback extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLiked: false,
            showLikeModal: false,
            showCollapseExpandButton: false,
            isExpanded: false,
            numOfLikes: 0
        };
    };

    componentWillMount = () => {
        const reactions = this.props.feedback.reactions;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isLiked = reactions && reactions.some(reaction => reaction.creator.code == curUserCode);
        const numOfLikes = reactions ? reactions.length : 0;

        this.setState({
            isLiked: isLiked,
            numOfLikes: numOfLikes
        })
    }

    componentDidMount = () => {
        if (this.props.feedback) {
            this.detectCollapseExpandComment();
        }
    }

    handleLikeEvent = event => {
        event.preventDefault();
        const isLike = !this.state.isLiked;
        const numOfLikes = isLike ? this.state.numOfLikes + 1 : this.state.numOfLikes - 1;
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
                var numOfLikes = isLike ? this.state.numOfLikes - 1 : this.state.numOfLikes + 1;
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
            resolveClientUrl('PRFeedbackReaction/GetByFeedbackId'),
            { params: { PRFeedbackId: this.props.feedback.prFeedbackId, ActorId: 0 } }
        ).then(response => {
            const reactions = response.data;
            const numOfLikes = reactions ? reactions.length : 0;

            this.setState({ showLikeModal: true, reactions: reactions, numOfLikes: numOfLikes });
        }).catch(axiosError);
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

    render() {
        const { isLiked, showCollapseExpandButton, showLikeModal, numOfLikes } = this.state;
        const { feedback, isReadOnly } = this.props;
        const reactions = feedback.reactions;
        const curUserCode = document.getElementById('employeeInfo-code').value;
        const isOwner = (curUserCode == feedback.creator.code); 

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';
        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

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
                                    !this.props.isReadOnly && !isOwner &&
                                    <a className={`fa ${isLiked ? 'icon-thumb-up active' : 'icon-thumb-up-line'}`}
                                        name="like-button"
                                        onClick={this.handleLikeEvent}
                                    ></a>
                                }
                                <a className="likes-statistic" onClick={this.handleOpenLikeModalEvent}>{numOfLikes} Likes</a>
                            </div>
                        </div>
                    </div>
                </div>
            </Fragment>
        );
    };
}