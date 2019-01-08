import React, { Component } from 'react';
import getResource from '../../resourceHelper';
import NotificationSetting from '../../shared/components/notification/NotificationSetting';

export default class NotificationFunction extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false
        };
    }

    handleMarkAsRead = () => {
        this.props.onMarkAsRead();
    }

    handleClickSetting = () => {
        this.setState({
            isShowSettingModal: true
        });
    }

    handleCancelSetting = () => {
        this.setState({
            isShowSettingModal: false
        });
    }

    render() {
        return (
            <div className="noti-header">
                <span className="title">{getResource('Notification')}</span>
                <span className="noti-function">
                    <a className="mark-read" onClick={this.handleMarkAsRead}>Mark as read</a>
                    <span className="dash" />
                    <a className="setting" onClick={this.handleClickSetting}>Settings</a>
                </span>
                {
                    this.state.isShowSettingModal &&
                    <NotificationSetting
                        isShowModal={this.state.isShowSettingModal}
                        onCloseModal={this.handleCancelSetting}
                    />
                }
            </div>
        );
    }
}