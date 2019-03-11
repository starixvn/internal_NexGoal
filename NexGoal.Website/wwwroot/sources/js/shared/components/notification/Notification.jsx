import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import InfiniteScroll from 'react-infinite-scroller';
import getResource from '../../../resourceHelper';
import { NotificationStatus } from '../../../constants';
import { calendar } from '../../../common-functions';
import NotificationSetting from './NotificationSetting';
import CustomScrollbars from '../CustomScrollbars';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowSettingModal: false,
            notiBodyHeight: 340
        };
    }

    componentDidMount() {
        this.calculateNotiBodyHeight();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (nextState.isShowSettingModal !== this.state.isShowSettingModal
            || nextState.notiBodyHeight !== this.state.notiBodyHeight
            || JSON.stringify(nextProps.notifications) !== this.props.notifications
            || nextProps.hasMore !== this.props.hasMore) {
            return true;
        }

        return false;
    }

    calculateNotiBodyHeight = () => {
        let items = document.getElementsByClassName('noti-item');
        let totalHeight = 0;
        if (items.length > 0) {
            for (var i = 0; i < items.length; i++) {
                totalHeight += items[i].offsetHeight + 4;
            }
        }
        if (totalHeight < 340) {
            this.setState({ notiBodyHeight: totalHeight });
        }
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
            <Fragment>
                <div className="noti-header">
                    <span className="title">{getResource('Notification')}</span>
                    <span className="noti-function">
                        <a className="mark-read" onClick={this.props.onMarkAsRead}>Mark as read</a>
                        <span className="dash" />
                        <a className="setting" onClick={this.handleClickSetting}>Settings</a>
                    </span>
                </div>
                <div className="noti-body" style={{ height: this.state.notiBodyHeight }}>
                    <CustomScrollbars>
                        {
                            notifications &&
                            <InfiniteScroll initialLoad={false}
                                pageStart={1}
                                loadMore={this.props.onLoadMore}
                                hasMore={this.props.hasMore}
                                useWindow={false}
                            >
                                {
                                    notifications.map(item => {
                                        const notiStatus = item.statusId === NotificationStatus.Unread ? 'unread' : '';
                                        return (
                                            <div className={`noti-item ${notiStatus} noti-${item.notificationId}`}
                                                onClick={() => this.props.onChangeStatus(item.notificationId, item.statusId, false)}
                                            >
                                                <img className="avatar" src={item.creatorAvatar} />
                                                <div className="description">
                                                    <a onClick={() => this.props.onChangeStatus(item.notificationId, item.statusId, true)}>
                                                        {item.content}
                                                    </a>
                                                    <div className="created-date">
                                                        {calendar(item.createdDate)}
                                                    </div>
                                                </div>
                                            </div>
                                        );
                                    })
                                }
                            </InfiniteScroll>
                        }
                    </CustomScrollbars>
                </div>
                {
                    this.state.isShowSettingModal &&
                    <NotificationSetting
                        isShowModal={this.state.isShowSettingModal}
                        onCloseModal={this.handleCancelSetting}
                    />
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<Notification />, document.getElementById('notification'));