import React, { Component } from 'react';
import { Modal, Button } from 'react-bootstrap';
import { getRoleClassName } from '../../../common-functions';

export default class LikesStatisticModal extends Component {
    constructor(props) {
        super(props);
    };

    renderUserLiked = (reaction, index) => {
        return (
            <div className="avatar-user" key={`user-like-${index}`}>
                <img src={reaction.creator.avatar} alt="avatar" className="avatar" />
                <div className="person-info">
                    <span className={getRoleClassName(reaction.roleId)}> {reaction.creator.nickName} </span>
                </div>
            </div>
        );
    }

    render() {
        const { showLikeModal, reactions } = this.props;
        const numOfLikes = reactions ? reactions.length : 0;

        return (
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
                    <Button className="btn-cancel" onClick={this.props.handleCloseLikeModal}> Close </Button>
                </Modal.Footer>
            </Modal>
            );
    }
}