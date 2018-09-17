import React, { Component } from 'react';
import { Modal, Button, Form, FormGroup, ControlLabel, Checkbox, Col } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

export default class EditUserRole extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userRoles: []
        };
    }

    componentDidMount() {
        this.getUserRoles();
    }

    getUserRoles = () => {
        axios.get(resolveClientUrl(`User/GetRolesForUser?employeeId=${this.props.user.employeeId}`))
            .then(response => {
                this.setState({ userRoles: response.data });
            }).catch(axiosError);
    }

    handleEditUserRoles = (e) => {
        e.preventDefault();

        var selectedIds = [];
        Array.from(e.target.querySelectorAll('input')).map(e => {
            if (e.checked) {
                selectedIds.push(e.value);
            }
        });

        let userRole = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            employeeId: this.props.user.employeeId,
            selectedRoleIds: selectedIds.toString()
        });

        axios({
            url: resolveClientUrl('User/EditUserRoles'),
            method: 'POST',
            data: userRole,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            this.props.callback(response.data);
        }).catch(error => axiosError(error));
    }

    render() {
        return (
            this.state.userRoles.length > 0 &&
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AssignRolesToUser')}</Modal.Title>
                </Modal.Header>
                <Form id="edit-user-role-form" horizontal onSubmit={this.handleEditUserRoles}>
                    <Modal.Body>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>{getResource('NickName')}</Col>
                            <div className="col-sm-10">
                                <ControlLabel>{this.props.user.nickName}</ControlLabel>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>{getResource('Roles')}</Col>
                            <Col sm={10}>
                                {this.state.userRoles.map(role =>
                                    <Checkbox key={role.roleId} defaultChecked={role.hasRole} value={role.roleId}>{role.role}</Checkbox>
                                )}
                            </Col>
                        </FormGroup>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn-cancel" onClick={this.props.closeModal}>{getResource('Cancel')}</Button>
                        <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}