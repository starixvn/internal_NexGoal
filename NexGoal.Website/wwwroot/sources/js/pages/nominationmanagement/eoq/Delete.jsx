import React, { Component } from 'react';
import { Button, Form, Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';

import commonFunctions from '../../../common-functions';

class DeleteEOQ extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit(event) {
        event.preventDefault();

        let formData = new FormData();
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);
        formData.append('id', this.props.eoqSelected.eoqyId);

        axios({
            url: commonFunctions.getRootUrl() + 'EOQ/Delete',
            method: 'DELETE',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Deleted successfully!');

                this.props.closeModal();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Modal show={this.props.showModal} keyboard={false}
                onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Confirmation</Modal.Title>
                </Modal.Header>
                <Form id="delete-eoq-form" horizontal onSubmit={this.handleSubmit.bind(this)}>
                    <Modal.Body>
                        Do you want to delete <strong>{this.props.eoqSelected.title}</strong>?
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

export default DeleteEOQ;