import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { toast } from 'react-toastify';

export default class DeletePeriod extends Component {
    constructor(props) {
        super(props);
    }

    handleSubmit = (event) => {
        event.preventDefault();

        axios({
            url: resolveClientUrl(`period/delete`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                periodId: this.props.period.periodId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Delete Successfully");
                this.props.onDeleteSuccess(this.props.period.periodId);
                this.props.closeModal();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        return (
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Delete Evaluation Period</Modal.Title>
                </Modal.Header>
                <form onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        Do you want to delete {this.props.period && this.props.period.name} ?
                    </Modal.Body>
                    <Modal.Footer>
                        <button className="btn btn-cancel" onClick={this.props.closeModal}>No</button>
                        <button type="submit" className="btn btn-primary">Yes</button>
                    </Modal.Footer>
                </form>
            </Modal>
        );
    }
}