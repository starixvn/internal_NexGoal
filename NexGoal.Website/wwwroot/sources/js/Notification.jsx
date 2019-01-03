import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import { axiosError, resolveClientUrl } from './common-functions';
import getResource from './resourceHelper';
import { NotificationStatus } from './constants';
import NotificationSetting from './shared/components/NotificationSetting';
import ToastLayout from './shared/layouts/ToastLayout';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notifications: [],
            isShowNoti: false,
            isShowSettingModal: false
        };
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
        this.getNotifications();
    }

    shouldComponentUpdate(nextProps, nextState) {
        return (nextState.isShowNoti !== this.state.isShowNoti
            || nextState.isShowSettingModal !== this.state.isShowSettingModal
            || JSON.stringify(nextState.notifications) !== JSON.stringify(this.state.notifications));
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOnBell = () => {
        this.setState({
            isShowNoti: !this.state.isShowNoti
        });
    }

    handleReadNotification = (notificationId) => {
        axios({
            url: resolveClientUrl(`Notification/ReadNotification`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                notificationId: notificationId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;
            if (data.isSuccessful) {
                this.getNotifications();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleMarkAsRead = () => {
        axios.get(resolveClientUrl('Notification/MarkAsReadNotification'))
            .then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    this.getNotifications();
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
    }

    handleClickSetting = () => {
        this.setState({
            isShowSettingModal: true,
            isShowNoti: false
        });
    }

    handleCancelSetting = () => {
        this.setState({
            isShowSettingModal: false
        });
    }

    getNotifications = () => {
        axios.get(resolveClientUrl('Notification/GetNotification'))
            .then(response => {
                this.setState({
                    notifications: response.data
                });
            }).catch(axiosError);
    }

    render() {
        const notifications = this.state.notifications;
        const unreadNoti = notifications.filter(noti => noti.statusId === NotificationStatus.Unread).length;
        return (
            <ToastLayout>
                <span className="icon-notification"
                    onClick={this.handleClickOnBell}
                >
                    {
                        unreadNoti > 0 &&
                        <span className={`noti-number ${unreadNoti === 0 ? 'empty-noti' : ''}`}>{unreadNoti}</span>
                    }
                </span>
                {
                    this.state.isShowNoti &&
                    <div className="noti-wrapper">
                        <div className="noti-header">
                            <span className="title">{getResource('Notification')}</span>
                            <span className="noti-function">
                                <a className="mark-read" onClick={this.handleMarkAsRead}>Mark as read</a>
                                <span className="dash" />
                                <a className="setting" onClick={this.handleClickSetting}>Settings</a>
                            </span>

                        </div>
                        <div className="noti-body">
                            {
                                notifications.map(item => {
                                    return (
                                        <div className={`noti-item ${item.statusId === NotificationStatus.Unread ? 'unread' : ''}`}
                                            onClick={() => this.handleReadNotification(item.notificationId)}
                                        >
                                            <img className="avatar" src={`https://hrm.nexcel.vn/Avatars/${item.creator.code}.jpg`} />
                                            <div className="description">{item.content}</div>
                                        </div>
                                    );
                                })
                            }
                        </div>
                        <a className="show-all-noti" href="/Notification">View all</a>
                    </div>
                }
                {
                    this.state.isShowSettingModal &&
                    <NotificationSetting
                        isShowModal={this.state.isShowSettingModal}
                        onCloseModal={this.handleCancelSetting}
                    />
                }
            </ToastLayout>
        );
    }
}

ReactDOM.render(<Notification />, document.getElementById('notification'));