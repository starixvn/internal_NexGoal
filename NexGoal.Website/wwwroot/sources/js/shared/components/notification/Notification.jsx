import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import InfiniteScroll from 'react-infinite-scroller';
import getResource from '../../../resourceHelper';
import { NotificationStatus } from '../../../constants';
import NotificationSetting from './NotificationSetting';
import ToastLayout from '../../layouts/ToastLayout';
import CustomScrollbars from '../CustomScrollbars';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false,
            notiBodyHeight: 400
        };
    }

    componentDidMount() {
        this.calculateNotiBodyHeight();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (nextState.isShowSettingModal !== this.state.isShowSettingModal
            || nextState.notiBodyHeight !== this.state.notiBodyHeight
            || JSON.stringify(nextProps.notifications) !== this.props.notifications) {
            return true;
        }
        return false;
    }

    calculateNotiBodyHeight = () => {
        let items = document.getElementsByClassName('noti-item');
        let totalHeight = 0;
        if (items.length > 0) {
            for (var i = 0; i < items.length; i++) {
                totalHeight += items[i].offsetHeight;
            }
        }
        if (totalHeight < 400) {
            this.setState({ notiBodyHeight: totalHeight });
        }
    }

    onLoadMoreNotification = () => {
        this.props.loadMoreNotification();
    }

    handleReadNotification = (statusId, notificationId) => {
        if (statusId === NotificationStatus.Unread) {
            this.props.onReadNotification(notificationId);
        }
    }

    handleMarkAsRead = () => {
        this.props.onMarkAsRead();
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
                <div className="noti-body" style={{ height: this.state.notiBodyHeight }}>
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
                                        console.log(item);
                                        const notiStatus = item.statusId === NotificationStatus.Unread ? 'unread' : '';
                                        const avatar = item.creatorId > 0 ? item.creator.avatar : '/dist/avatars/icon-nexgoal.svg';
                                        return (
                                            <div className={`noti-item ${notiStatus} noti-${item.notificationId}`}
                                                onClick={() => this.handleReadNotification(item.statusId, item.notificationId)}
                                            >
                                                <img className="avatar" src={avatar} />
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

ReactDOM.render(<Notification />, document.getElementById('notification'));