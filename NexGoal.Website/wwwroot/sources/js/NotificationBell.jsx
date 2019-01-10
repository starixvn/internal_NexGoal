import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from './common-functions';
import Notification from './shared/components/notification/Notification';
import { NotificationStatus } from './constants';

export default class NotificationBell extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowNoti: false,
            notifications: [],
            unreadNoti: 0,
            totalItems: 0,
            pageIndex: 0,
            hasMore: true
        };

        this.pageSize = 6;
        this.setWrapperRef = this.setWrapperRef.bind(this);
        this.handleClickOutside = this.handleClickOutside.bind(this);
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
        this.addEventListener();
        this.getNotifications();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (nextState.isShowNoti !== this.state.isShowNoti
            || nextState.unreadNoti !== this.state.unreadNoti
            || nextState.totalItems !== this.state.totalItems
            || nextState.pageIndex !== this.state.pageIndex
            || nextState.hasMore !== this.state.hasMore
            || JSON.stringify(nextState.notifications) !== JSON.stringify(this.state.notifications)) {
            return true;
        }
        return false;
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
        this.removeEventListener();
    }

    addEventListener = () => {
        let notiTrigger = document.getElementById('trigger-notification-bell');
        if (notiTrigger) {
            notiTrigger.addEventListener('click', () => {
                let notificationId = parseInt(notiTrigger.getAttribute("notificationId"));
                if (!notificationId) {
                    this.markAsReadSuccess();
                } else {
                    this.readNotificationSucess(notificationId);
                }
            }, false);
        }
    }

    removeEventListener = () => {
        let notiTrigger = document.getElementById('trigger-notification-bell');
        if (notiTrigger) {
            notiTrigger.removeEventListener('click');
        }
    }

    readNotificationSucess = (notificationId) => {
        const notiId = notificationId;
        let notifications = [...this.state.notifications];
        const notiIndex = notifications.findIndex(noti => noti.notificationId === notiId);
        if (notiIndex !== -1) {
            notifications[notiIndex].statusId = NotificationStatus.Read;
        }
        this.setState({
            notifications: notifications,
            unreadNoti: this.state.unreadNoti - 1
        });

        this.triggerNotificationCenter(notiId);
    }

    markAsReadSuccess = () => {
        let notifications = [...this.state.notifications];
        for (var i = 0; i < notifications.length; i++) {
            notifications[i].statusId = NotificationStatus.Read;
        }
        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });

        this.triggerNotificationCenter();
    }

    triggerNotificationCenter = (notificationId) => {
        let notiCenterTrigger = document.getElementById('trigger-notification-center');
        if (notiCenterTrigger) {
            notiCenterTrigger.setAttribute("notificationId", notificationId);
            notiCenterTrigger.click();
        }
    }

    handleClickOutside(event) {
        const notiSettings = document.getElementById('notification-settings');
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)
            && (notiSettings === null || !notiSettings.contains(event.target))) {
            this.onHideNoti();
        }
    }

    setWrapperRef(node) {
        this.wrapperRef = node;
    }

    handleClickOnBell = () => {
        this.setState({
            isShowNoti: !this.state.isShowNoti
        });
    }

    onHideNoti = () => {
        this.setState({
            isShowNoti: false
        });
    }

    handleLoadMoreNotification = () => {
        if (this.state.hasMore) {
            this.setState({
                pageIndex: this.state.pageIndex + 1
            }, () => {
                if (this.state.hasMore) {
                    this.getNotifications();
                }
            });
        }
    }

    getNotifications = () => {
        axios.get(resolveClientUrl('Notification/GetNotification'), {
            params: {
                pageIndex: this.state.pageIndex,
                pageSize: this.pageSize
            }
        }).then(response => {
            const data = response.data;
            let notifications = this.state.notifications;
            notifications.push(...data.notifications);
            this.setState({
                notifications: notifications,
                unreadNoti: data.unreadNoti,
                hasMore: notifications.length < data.total
            });
        }).catch(axiosError);
    }

    handleReadNotification = (notificationId) => {
        const notiId = notificationId;
        axios.get(resolveClientUrl('Notification/ReadNotification'), {
            params: {
                notificationId: notiId
            }
        }).then(response => {
            let data = response.data;
            if (data.isSuccessful) {
                this.readNotificationSucess(notiId);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsReadNotification'))
                .then(response => {
                    let data = response.data;
                    if (data.isSuccessful) {
                        this.markAsReadSuccess();
                    } else {
                        toast.error(data.message);
                    }
                }).catch(axiosError);
        }
    }

    render() {
        const unreadNoti = this.state.unreadNoti;
        return (
            <div ref={this.setWrapperRef}>
                <span className="icon-bell"
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
                        <Notification notifications={this.state.notifications}
                            loadMoreNotification={this.handleLoadMoreNotification}
                            onReadNotification={this.handleReadNotification}
                            onMarkAsRead={this.handleMarkAsRead}
                            onClickSetting={this.onHideNoti}
                            hasMore={this.state.hasMore}
                        />
                        <a className="show-all-noti" href="/Notification">View all</a>
                    </div>
                }
            </div>
        );
    }
}

ReactDOM.render(<NotificationBell />, document.getElementById('notification'));