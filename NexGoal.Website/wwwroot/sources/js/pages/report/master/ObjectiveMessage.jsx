import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../../resourceHelper';

export default class ObjectiveMessage extends PureComponent {
    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg objective-message-modal"
            >
                <Modal.Header>
                    <Modal.Title>
                        Objectives
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p className="comment">
                        {this.props.message}
                    </p>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}