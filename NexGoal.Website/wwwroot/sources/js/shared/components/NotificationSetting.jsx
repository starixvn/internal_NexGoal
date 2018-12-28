import React, { Component } from 'react';
import { Button, Modal } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import { resolveClientUrl, axiosError } from '../../common-functions';
import getResource from '../../resourceHelper';

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

    handleChange = (event, typeId, name) => {
        let settings = [...this.state.notificationSettings];
        settings.find(setting => setting.type === typeId)[name] = event.target.checked;
        this.setState({
            notificationSettings: settings
        });
    }

    saveSetting = () => {
        axios({
            url: resolveClientUrl('Common/UpdateNotiSetting'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                settings: this.state.notificationSettings
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let result = response.data;
            if (result.isSuccessful) {
                this.props.onCloseModal();
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(result.message);
            }
        }).catch(axiosError);
    }

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-xs form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('NotificationSetting')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>{getResource('Type')}</th>
                                <th>{getResource('Notification')}</th>
                                <th>{getResource('Email')}</th>
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
                                                    onClick={(e) => this.handleChange(e, setting.type, "receiveNotification")}
                                                />
                                                <label htmlFor={`chb-${setting.type}-noti`} />
                                            </td>
                                            <td className="text-center">
                                                <input type="checkbox" className="checkbox-custom"
                                                    id={`chb-${setting.type}-email`}
                                                    checked={setting.receiveEmail}
                                                    onClick={(e) => this.handleChange(e, setting.type, "receiveEmail")}
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
                    <Button className="btn btn-primary" onClick={this.saveSetting}>{getResource('Submit')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}