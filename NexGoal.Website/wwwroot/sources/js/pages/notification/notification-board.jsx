import React, { Component } from 'react';
import InfiniteScroll from 'react-infinite-scroller';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import { NotificationStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import NotificationSetting from '../../shared/components/notification/NotificationSetting';

export default class NotificationBoard extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false
        };
    }

    handleReadNotification = (statusId, notificationId) => {
        this.props.onReadNotification(notificationId, statusId);
    }

    onLoadMoreNotification = () => {
        this.props.loadMoreNotification;
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
        const notifications = this.props.notifications;

        return (
            <ToastLayout>
                <div className="noti-header">
                    <span className="title">{getResource('Notification')}</span>
                    <span className="noti-function">
                        <a className="mark-read" onClick={this.props.onMarkAsRead}>Mark as read</a>
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
                            <InfiniteScroll initialLoad={false}
                                pageStart={1}
                                loadMore={this.onLoadMoreNotification}
                                hasMore={this.props.hasMore}
                                useWindow={false}
                            >
                                {
                                    notifications.map(item => {
                                        const notiStatus = item.statusId === NotificationStatus.Unread ? 'unread' : '';
                                        return (
                                            <div className={`noti-center-item ${notiStatus} noti-${item.notificationId}`}
                                                onClick={() => this.handleReadNotification(item.statusId, item.notificationId)}
                                            >
                                                <img className="avatar" src={item.creator.avatar} />
                                                <div className="description">{item.content}</div>
                                            </div>
                                        );
                                    })
                                }
                            </InfiniteScroll>
                        }
                    </CustomScrollbars>
                </div>
            </ToastLayout>
        );
    }
}