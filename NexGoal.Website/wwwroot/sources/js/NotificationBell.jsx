import React, { PureComponent } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';
import Notification from './shared/components/notification/Notification';
import { NotificationActionType, NotificationStatus } from './constants';
import { toast } from 'react-toastify';

export default class NotificationBell extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isShowNoti: false,
            notifications: [],
            unreadNoti: parseInt(document.getElementById('employeeInfo-unreadNoti').innerText, 10),
            totalItems: 0,
            pageIndex: 0,
            hasMore: true
        };

        this.pageSize = 6;
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
        this.addEventListener();
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
        this.removeEventListener();
    }

    getNotifications = callback => {
        axios.get(resolveClientUrl('Notification/GetNotification'), {
            params: {
                pageIndex: this.state.pageIndex,
                pageSize: this.pageSize
            }
        }).then(response => {
            const data = response.data;
            const notifications = this.state.notifications;
            notifications.push(...data.notifications);

            this.setState({
                notifications: notifications,
                unreadNoti: data.unreadNoti,
                hasMore: notifications.length < data.total
            }, () => {
                if (callback) {
                    callback();
                }
            });
        }).catch(axiosError);
    }

    triggerNotificationCenter = notificationId => {
        const notiCenterTrigger = document.getElementById('trigger-notification-center');
        if (notiCenterTrigger) {
            notiCenterTrigger.setAttribute('notificationId', notificationId);
            notiCenterTrigger.click();
        }
    }

    addEventListener = () => {
        const notiTrigger = document.getElementById('trigger-notification-bell');
        if (notiTrigger) {
            notiTrigger.addEventListener('click', () => {
                const notificationId = parseInt(notiTrigger.getAttribute('notificationId'), 10);
                if (!notificationId) {
                    this.markAsReadSuccess();
                } else {
                    this.handleChangeStatusSuccess(notificationId, false);
                }
            }, false);
        }
    }

    removeEventListener = () => {
        const notiTrigger = document.getElementById('trigger-notification-bell');
        if (notiTrigger) {
            notiTrigger.removeEventListener('click');
        }
    }

    handleRedirectToAction = notificationId => {
        const notifications = this.state.notifications.slice();
        const notiIndex = notifications.findIndex(noti => noti.notificationId === notificationId);
        if (notiIndex !== -1) {
            const { actionTypeId, recordId } = notifications[notiIndex];
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
                case NotificationActionType.ProbationSettingPage:
                    url = resolveClientUrl('Probation/Setting');
                    break;
                case NotificationActionType.MemberPageWithActiveProbationTab:
                    url = resolveClientUrl('member?activeTab=4');
                    break;
            }

            if (url) {
                window.location = url;
            }
        }
    }

    markAsReadSuccess = () => {
        const notifications = this.state.notifications.slice();
        if (notifications.length > 0) {
            for (let i = 0; i < notifications.length; i++) {
                notifications[i].statusId = NotificationStatus.Read;
            }
        }

        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });

        this.triggerNotificationCenter();
    }

    handleClickOutside = event => {
        const notiSettings = document.getElementById('notification-settings');
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)
            && (notiSettings === null || !notiSettings.contains(event.target))) {
            this.onHideNoti();
        }
    }

    handleClickOnBell = () => {
        if (this.state.isShowNoti) {
            this.onHideNoti();
        }
        else {
            this.getNotifications(() => {
                this.setState({ isShowNoti: !this.state.isShowNoti });
            });
        }
    }

    onHideNoti = () => {
        this.setState({
            isShowNoti: false,
            notifications: [],
            pageIndex: 0,
            hasMore: true
        });
    }

    handleLoadMoreNotification = () => {
        if (this.state.hasMore) {
            this.setState({
                pageIndex: this.state.pageIndex + 1
            }, () => this.getNotifications());
        }
    }

    handleChangeStatusSuccess = (notificationId, isTriggerNotiCenter) => {
        const notifications = this.state.notifications.slice();
        const index = notifications.findIndex(x => x.notificationId === notificationId);

        if (index >= 0) {
            notifications[index].statusId = NotificationStatus.Read;
            this.setState({
                notifications: notifications
            });
        }

        if (isTriggerNotiCenter) {
            this.triggerNotificationCenter(notificationId);
        }

        this.setState({
            unreadNoti: this.state.unreadNoti - 1
        });
    }

    handleChangeStatus = (notificationId, statusId, isRedirect) => {
        if (statusId === NotificationStatus.Unread) {
            axios.get(resolveClientUrl('Notification/ReadNotification'), {
                params: {
                    notificationId: notificationId
                }
            }).then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    if (isRedirect) {
                        this.handleRedirectToAction(notificationId);
                    } else {
                        this.handleChangeStatusSuccess(notificationId, true);
                    }
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        } else if (isRedirect) {
            this.handleRedirectToAction(notificationId);
        }
    }

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsRead'))
                .then(response => {
                    const data = response.data;
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
            <div className="header-notification" ref={node => this.wrapperRef = node}>
                <span className="icon-bell"
                    onClick={this.handleClickOnBell}
                />
                {
                    unreadNoti > 0 &&
                    <span className={`noti-number ${unreadNoti === 0 ? 'empty-noti' : ''}`}>{unreadNoti}</span>
                }
                {
                    this.state.isShowNoti &&
                    <div className="noti-wrapper">
                        <Notification notifications={this.state.notifications}
                            onLoadMore={this.handleLoadMoreNotification}
                            onChangeStatus={this.handleChangeStatus}
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