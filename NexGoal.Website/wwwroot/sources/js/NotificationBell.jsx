import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';
import Notification from './shared/components/notification/Notification';
import { NotificationActionType, NotificationStatus } from './constants';

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

    handleRedirectToAction = (notificationId) => {
        let notifications = this.state.notifications.slice();
        let notiIndex = notifications.findIndex(noti => noti.notificationId === notificationId);
        if (notiIndex !== -1) {
            let actionTypeId = notifications[notiIndex].actionTypeId;
            let recordId = notifications[notiIndex].recordId;
            let url = '';
            switch (actionTypeId) {
                case NotificationActionType.MyPageWithCreatePerformanceCheckin:
                    url = resolveClientUrl('?action=newrecord');
                    break;

                case NotificationActionType.MyPageWithSpecificPerformanceCheckin:
                    url = resolveClientUrl(`?recordId=${recordId}`);
                    break;

                case NotificationActionType.MemberPageWithCreatePerformanceCheckin:
                    url = resolveClientUrl('member?action=newrecord');
                    break;

                case NotificationActionType.MemberPageWithSpecificPerformanceCheckin:
                    url = resolveClientUrl(`member?recordId=${recordId}`);
                    break;

                case NotificationActionType.MemberPageWithActiveNominationTab:
                    url = resolveClientUrl('member?activeTab=2');
                    break;

                case NotificationActionType.MemberPageWithActivePerformanceAppraisalTab:
                    url = resolveClientUrl('member?activeTab=3');
                    break;

                case NotificationActionType.PAFocus:
                    url = resolveClientUrl('pafocus');
                    break;
            }

            if (url) {
                window.location = url;
            }
        }
    }

    markAsReadSuccess = () => {
        let notifications = this.state.notifications.slice();
        for (let i = 0; i < notifications.length; i++) {
            notifications[i].statusId = NotificationStatus.Read;
        }

        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });

        let notiCenterTrigger = document.getElementById('trigger-notification-center');
        if (notiCenterTrigger) {
            notiCenterTrigger.click();
        }
    }

    handleClickOutside = (event) => {
        const notiSettings = document.getElementById('notification-settings');
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)
            && (notiSettings === null || !notiSettings.contains(event.target))) {
            this.onHideNoti();
        }
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

    handleReadNotification = (notificationId, statusId) => {
        if (statusId === NotificationStatus.Unread) {
            axios.get(resolveClientUrl('Notification/ReadNotification'), {
                params: {
                    notificationId: notificationId
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    this.handleRedirectToAction(notificationId);
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        } else {
            this.handleRedirectToAction(notificationId);
        }
    }

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsRead'))
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
            <div ref={node => this.wrapperRef = node}>
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
                        <a className="show-all-noti" href={resolveClientUrl('Notification')}>View all</a>
                    </div>
                }
            </div>
        );
    }
}

ReactDOM.render(<NotificationBell />, document.getElementById('notification'));