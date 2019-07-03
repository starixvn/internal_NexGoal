import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import CustomScrollbars from '../../../shared/components/CustomScrollbars';

export default class CommentModal extends PureComponent {
    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg comment-modal" backdrop="static"
            >
                <Modal.Header>
                    <Modal.Title>
                        Evaluator's Comment
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <CustomScrollbars>
                        <p className="comment">
                            {this.props.content}
                        </p>
                    </CustomScrollbars>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}