import React, { Component } from 'react';
import { Modal, Button, FormGroup, ControlLabel, Col, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { Formik } from 'formik';
import { isEmpty } from 'lodash';
import Select from 'react-select';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

export default class AddMember extends Component {
    constructor(props) {
        super(props);
    }

    initialObj = {
        members: [],
    }

    validateForm = values => {
        let errors = {};

        if (!values.members || values.members.length === 0) {
            errors.members = 'Please select member';
        }

        return errors;
    }

    onSubmit = (values, { setErrors }) => {
        axios({
            url: resolveClientUrl(`PMSetting/AddMembers`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                projectManagerId: this.props.pm.employeeId,
                memberIds: values.members.map(e => e.value).toString()
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('AddedSuccessfully'));
                this.props.onAddMemberSuccess();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <Form onSubmit={handleSubmit} className="add-member-form">
                <Modal.Body>
                    <div className="lb-member">{getResource('Members')}</div>
                    <Select name="members"
                        className={`react-select-container ${touched.members && errors.members && 'input-validation-error'}`}
                        classNamePrefix="react-select" isClearable
                        options={this.props.memberOptions} placeholder="Nickname or Full name"
                        value={values.members} isMulti closeMenuOnSelect={false}
                        onChange={value => setFieldValue('members', value)}
                    />
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Assign new member to {this.props.pm ? this.props.pm.nickName : ''}</Modal.Title>
                </Modal.Header>
                <Formik initialValues={this.initialObj} validate={this.validateForm} onSubmit={this.onSubmit} render={this.renderForm} />
            </Modal>
        );
    }
}