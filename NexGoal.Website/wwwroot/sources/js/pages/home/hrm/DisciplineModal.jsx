import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import getResource from '../../../resourceHelper';

export default function Discipline(props) {
    const disciplines = props.disciplines;
    return (
        <Modal keyboard backdrop="static"
            show={props.isShowModal} onHide={props.onCloseModal}
            dialogClassName="custom-modal modal-md form-performance edit "
        >
            <Modal.Header>
                <Modal.Title>{getResource('Discipline')}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {JSON.stringify(disciplines)}
            </Modal.Body>
            <Modal.Footer>
                <Button className="btn-cancel" onClick={props.onCloseModal}>{getResource('Close')}</Button>
            </Modal.Footer>
        </Modal>
    );
}