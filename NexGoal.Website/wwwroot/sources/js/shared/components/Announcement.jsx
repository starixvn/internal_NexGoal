import React, { PureComponent } from 'react';
import { Button, Modal } from 'react-bootstrap';
import getResource from '../../resourceHelper';

export default class Announcement extends PureComponent {
    render() {
        const content = this.props.announcements.map((e, i) => <li key={i}><p>{e}</p></li>);

        return (
            <Modal
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal announcement-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Announcement</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ul>{content}</ul>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onCloseModal} className="btn btn-primary">{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}