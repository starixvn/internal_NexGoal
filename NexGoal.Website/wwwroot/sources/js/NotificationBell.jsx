import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from './common-functions';
import getResource from './resourceHelper';
import Notification from './shared/components/notification/Notification';

export default class NotificationBell extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowNoti: false,
            notifications: [],
            unreadNoti: 0,
            totalItems: 0,
            pageIndex: 0,
            hasMore: true,
            isShowSettingModal: false
        };

        this.pageSize = 10;
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
            || nextState.isShowSettingModal !== this.state.isShowSettingModal
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

    loadMoreNotification = () => {
        this.setState({
            pageIndex: this.state.pageIndex + 1
        }, () => {
            if (this.state.hasMore) {
                this.getNotifications();
            }
        });
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
            notifications.unshift(...data.notifications);
            this.setState({
                notifications: notifications,
                unreadNoti: data.unreadNoti,
                totalItems: data.total,
                hasMore: notifications.length < data.total
            }, () => console.log(this.state));
        }).catch(axiosError);
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
                        <Notification notifications={this.state.notifications} />
                        <a className="show-all-noti" href="/Notification">View all</a>
                    </div>
                }

            </Fragment>
        );
    }
}

ReactDOM.render(<NotificationBell />, document.getElementById('notification'));