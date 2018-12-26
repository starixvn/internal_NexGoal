import React, { Component, Fragment } from 'react';
import { Button, ControlLabel, Modal } from 'react-bootstrap';
import getResource from '../../resourceHelper';
import ReactDOM from 'react-dom';
import axios from 'axios';

export default class NotificationSetting extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>Notification Settings</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Body>
            </Modal>
        );
    }
}