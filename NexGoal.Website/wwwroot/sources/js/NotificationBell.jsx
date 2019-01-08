import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
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
    }

    componentDidMount() {
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

    handleClickOnBell = () => {
        this.setState({
            isShowNoti: !this.state.isShowNoti
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
                let notifications = [...this.state.notifications];
                const notiIndex = this.state.notifications.findIndex(noti => noti.notificationId === notiId);
                notifications[notiIndex].statusId = NotificationStatus.Read;
                this.setState({
                    notifications: notifications,
                    unreadNoti: this.state.unreadNoti - 1
                });
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
                        let notifications = [...this.state.notifications];
                        for (var i = 0; i < notifications.length; i++) {
                            notifications[i].statusId = NotificationStatus.Read;
                        }
                        this.setState({
                            notifications: notifications,
                            unreadNoti: 0
                        });
                    } else {
                        toast.error(data.message);
                    }
                }).catch(axiosError);
        }
    }

    render() {
        const unreadNoti = this.state.unreadNoti;
        return (
            <Fragment>
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
                            hasMore={this.state.hasMore}
                        />
                        <a className="show-all-noti" href="/Notification">View all</a>
                    </div>
                }

            </Fragment>
        );
    }
}

ReactDOM.render(<NotificationBell />, document.getElementById('notification'));