import React, { Component } from 'react';
import { Button, Form, Modal } from 'react-bootstrap';

class Delete extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit(event) {
        event.preventDefault();
        this.props.handleDeleteFlowTemplate(this.props.flowTemplateSelected);
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Confirmation</Modal.Title>
                </Modal.Header>
                <Form id="delete-eoq-form" horizontal onSubmit={this.handleSubmit.bind(this)}>
                    <Modal.Body>
                        Do you want to delete <strong>{this.props.flowTemplateSelected.name}</strong>?
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.closeModal} className="btn-cancel">Cancel</Button>
                        <Button type="submit" className="btn btn-primary">Submit</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}

export default Delete;