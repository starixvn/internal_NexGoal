import React, { Component } from 'react';
import { Button, Form, Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';

class DeleteEOY extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit() {
        axios({
            url: commonFunctions.getRootUrl() + 'EOY/Delete',
            method: 'DELETE',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                id: this.props.eoySelected.eoqyId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Deleted successfully!');

                this.props.closeModal();
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
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
                        Do you want to delete <strong>{this.props.eoySelected.title}</strong>?
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

export default DeleteEOY;