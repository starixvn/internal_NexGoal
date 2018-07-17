import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Modal, Button, Form, FormGroup, ControlLabel, Checkbox, Col, FormControl } from 'react-bootstrap';
import commonFunctions from '../../common-functions';

export default class EditUserRole extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userRoles: []
        };
    }

    componentDidMount = function () {
        this.getUserRoles();
    }

    getUserRoles = () => {
        fetch(`/User/GetRolesForUser?employeeId=${this.props.user.employeeId}`,
            {
                credentials: "same-origin"
            })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                }

                return Promise.reject(response);
            })
            .then(data => {
                this.setState({ userRoles: data });
            })
            .catch(error => commonFunctions.axiosError(error));
    }

    handleEditUserRoles = (e) => {
        e.preventDefault();

        var selectedIds = [];
        Array.from(e.target.querySelectorAll('input')).map(e => {
            if (e.checked)
                selectedIds.push(e.value);
        });

        var employeeId = this.props.user.employeeId;

        fetch('/User/EditUserRoles',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                },
                credentials: 'same-origin',
                body: `employeeId=${employeeId}&selectedRoleIds=${selectedIds.toString()}&__RequestVerificationToken=${document.getElementsByName('__RequestVerificationToken')[0].value}`
            })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                }

                throw new Error(response);
            })
            .then(data => {
                this.props.callback(data);
            })
            .catch(error => commonFunctions.axiosError(error));
    }

    render() {
        return (
            this.state.userRoles.length > 0 &&
            <Modal show={this.props.showModal} onHide={this.props.closeModal} dialogClassName="custom-modal">
                <Modal.Header closeButton>
                    <Modal.Title>Assign Roles To User </Modal.Title>
                </Modal.Header>
                <Form id="edit-user-role-form" horizontal onSubmit={this.handleEditUserRoles}>
                    <Modal.Body>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>Nick Name</Col>
                            <div className="col-sm-10">
                                <ControlLabel>{this.props.user.nickName}</ControlLabel>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>
                                Roles
                                    </Col>
                            <Col sm={10}>
                                {this.state.userRoles.map(role =>
                                    <Checkbox key={role.roleId} defaultChecked={role.hasRole} value={role.roleId}>{role.role}</Checkbox>
                                )}
                            </Col>
                        </FormGroup>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.closeModal}>Cancel</Button>
                        <Button type="submit" className="btn btn-primary">Submit</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}