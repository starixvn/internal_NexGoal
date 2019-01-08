import React, { Component } from 'react';
import InfiniteScroll from 'react-infinite-scroller';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import { NotificationStatus } from '../../constants';

export default class NotificationList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false
        };
    }

    onLoadMoreNotification = () => {
        this.props.loadMoreNotification;
    }
    render() {
        const notifications = this.props.notifications;
        return (
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
                                        <div className={`noti-item ${notiStatus} noti-${item.notificationId}`}
                                            onClick={() => this.handleReadNotification(item.statusId, item.notificationId)}
                                        >
                                            <img className="avatar" src={`https://hrm.nexcel.vn/Avatars/${item.creator.code}.jpg`} />
                                            <div className="description">{item.content}</div>
                                        </div>
                                    );
                                })
                            }
                        </InfiniteScroll>
                    }
                </CustomScrollbars>
            </div>
        );
    }
}