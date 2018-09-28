import React, { Component } from 'react';
import { Modal, Button, Form, FormGroup, ControlLabel, Checkbox, Col } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

export default class EditEmployeeRole extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeRoles: []
        };
    }

    componentDidMount() {
        this.getRolesByEmployeeId();
    }

    getRolesByEmployeeId = () => {
        axios.get(resolveClientUrl(`employee/GetRolesByEmployeeId?employeeId=${this.props.employee.employeeId}`))
            .then(response => {
                this.setState({ employeeRoles: response.data });
            }).catch(axiosError);
    }

    handleEditEmployeeRoles = (e) => {
        e.preventDefault();

        var selectedIds = [];
        Array.from(e.target.querySelectorAll('input')).map(e => {
            if (e.checked) {
                selectedIds.push(e.value);
            }
        });

        let employeeRole = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            employeeId: this.props.employee.employeeId,
            selectedRoleIds: selectedIds.toString()
        });

        axios({
            url: resolveClientUrl('employee/EditEmployeeRoles'),
            method: 'POST',
            data: employeeRole,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            this.props.callback(response.data);
        }).catch(error => axiosError(error));
    }

    render() {
        return (
            this.state.employeeRoles.length > 0 &&
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.closeModal}
                dialogClassName="custom-modal"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('AssignRolesToEmployee')}</Modal.Title>
                </Modal.Header>
                <Form id="edit-employee-role-form" horizontal onSubmit={this.handleEditEmployeeRoles}>
                    <Modal.Body>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>{getResource('NickName')}</Col>
                            <div className="col-sm-10">
                                <ControlLabel>{this.props.employee.nickName}</ControlLabel>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <Col componentClass={ControlLabel} sm={2}>{getResource('Roles')}</Col>
                            <Col sm={10}>
                                {this.state.employeeRoles.map(role =>
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