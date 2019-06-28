import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import getResource from '../../../resourceHelper';

export default function Announcement(props) {
    return (
        <Modal
            show={props.showModal} onHide={props.onCloseModal}
            dialogClassName="custom-modal announcement-modal"
        >
            <Modal.Header closeButton>
                <Modal.Title>{getResource('Announcement')}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <ul>
                    <li>
                        {props.content}
                    </li>
                </ul>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={props.onCloseModal} className="btn btn-primary">{getResource('Close')}</Button>
            </Modal.Footer>
        </Modal>
    );
}