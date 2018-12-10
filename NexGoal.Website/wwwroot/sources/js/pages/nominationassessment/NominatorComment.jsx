import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';

import getResource from '../../resourceHelper';

export default class NominatorComment extends PureComponent {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Modal backdrop="static" show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-lg"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('Comment')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>{this.props.nominator && this.props.nominator.comment} </p>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}