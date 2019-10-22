import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from 'react-bootstrap';

class ConfirmationModal extends Component {
    static propTypes = {
        action: PropTypes.string,
        name: PropTypes.string,
        showModal: PropTypes.bool,
        onCloseModal: PropTypes.func,
        onSubmit: PropTypes.func
    };

    static defaultProps = {
        action: '',
        name: '',
        showModal: false,
        onCloseModal: function () {
            //empty
        },
        onSubmit: function () {
            //empty
        }
    };

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Confirmation</Modal.Title>
                </Modal.Header>
                <Form id="confirmation-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        Are you sure you want to {this.props.action} <strong>{this.props.name}</strong>?
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.onCloseModal} className="btn-cancel">No</Button>
                        <Button type="submit" className="btn btn-primary">Yes</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}

export default ConfirmationModal;