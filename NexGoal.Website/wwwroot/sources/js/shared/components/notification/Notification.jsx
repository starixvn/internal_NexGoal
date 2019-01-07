import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import { NotificationStatus } from '../../../constants';
import NotificationSetting from './NotificationSetting';
import ToastLayout from '../../layouts/ToastLayout';
import CustomScrollbars from '../CustomScrollbars';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (nextState.isShowSettingModal !== this.state.isShowSettingModal
            || JSON.stringify(nextProps.notifications) !== this.props.notifications) {
            return true;
        }
        return false;
    }

    onLoadMoreNotification = (pageIndex) => {
        this.loadMoreNotification();
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
                console.log("Done");
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

    render() {
        const notifications = this.props.notifications;
        return (
            <ToastLayout>
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
                <div className="noti-body">
                    <CustomScrollbars>
                        {
                            notifications &&
                            notifications.map(item => {
                                const notiStatus = item.statusId === NotificationStatus.Unread ? 'unread' : '';
                                return (
                                    <div className={`noti-item ${notiStatus}`}
                                        onClick={(e) => this.handleReadNotification(e, item.notificationId)}
                                    >
                                        <img className="avatar" src={`https://hrm.nexcel.vn/Avatars/${item.creator.code}.jpg`} />
                                        <div className="description">{item.content}</div>
                                    </div>
                                );
                            })
                        }
                    </CustomScrollbars>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<Notification />, document.getElementById('notification'));