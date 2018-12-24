import React, { Component } from 'react';
import { Button, Modal } from 'react-bootstrap';

export default class Announcement extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Announcement</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <ul>
                        {this.props.announcements.map((e, i) => <li key={i}>{e}</li>)}
                    </ul>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onCloseModal} className="btn-cancel">Close</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}