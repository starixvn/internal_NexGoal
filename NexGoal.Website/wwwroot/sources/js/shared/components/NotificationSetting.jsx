import React, { Component, Fragment } from 'react';
import { Button, ControlLabel, Modal } from 'react-bootstrap';
import { resolveClientUrl, axiosError } from '../../common-functions';
import getResource from '../../resourceHelper';
import ReactDOM from 'react-dom';
import axios from 'axios';

export default class NotificationSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notificationSettings: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('common/GetNotificationSetting'))
            .then(response => {
                this.setState({
                    notificationSettings: response.data
                });
            }).catch(axiosError);
    }

    render() {
        console.log(this.state.notificationSettings);
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-xs form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>Notification Settings</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>Type</th>
                                <th>Notification</th>
                                <th>Email</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.notificationSettings.map((setting) => {
                                    return (
                                        <tr>
                                            <td>{setting.description}</td>
                                            <td className="text-center">
                                                <input type="checkbox" className="checkbox-custom"
                                                    id={`chb-${setting.type}-noti`}
                                                    checked={setting.receiveNotification}
                                                />
                                                <label htmlFor={`chb-${setting.type}-noti`} />
                                            </td>
                                            <td className="text-center">
                                                <input type="checkbox" className="checkbox-custom"
                                                    id={`chb-${setting.type}-email`}
                                                    checked={setting.receiveEmail}
                                                />
                                                <label htmlFor={`chb-${setting.type}-email`} />
                                            </td>
                                        </tr>
                                    );
                                })
                            }
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                    <Button type="submit" className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}