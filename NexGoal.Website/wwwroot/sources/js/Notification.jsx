import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from './common-functions';
import NotificationSetting from './shared/components/NotificationSetting';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notifications: [],
            isShowNoti: false,
            isShowSettingModal: false
        };
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
        this.fakeData();
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    fakeData = () => {
        const data = [
            { description: 'Micheal created a performance record for you.' },
            { description: 'Micheal created a performance record for you.' },
            { description: 'Micheal created a performance record for you.' },
            { description: 'Micheal created a performance record for you.' },
            { description: 'Micheal created a performance record for you.' },
            { description: 'Micheal created a performance record for you.' }
        ];
        this.setState({ notifications: data });
    }

    handleClickOnBell = () => {
        this.setState({
            isShowNoti: !this.state.isShowNoti
        });
    }

    handleMarkAsRead = () => {
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

    getNotifications = () => {
        axios.get(resolveClientUrl('common/GetNotification'))
            .then(response => {
                this.setState({
                    notifications: response.data
                });
            }).catch(axiosError);
    }

    render() {
        const notifications = this.state.notifications;
        const totalNoti = notifications.length;
        return (
            <Fragment>
                <span className="icon-notification"
                    onClick={this.handleClickOnBell}
                >
                    <span className={`noti-number ${totalNoti === 0 ? 'empty-noti' : ''}`}>{totalNoti}</span>
                </span>
                {
                    this.state.isShowNoti &&
                    <div className="noti-wrapper">
                        <div className="noti-header">
                            <span className="title">Notification</span>
                            <a className="mark-read" onClick={this.handleMarkAsRead}>Mark as read</a> |
                            <a className="mark-read" onClick={this.handleClickSetting}>Settings</a>
                        </div>
                        <div className="noti-body">
                            {
                                notifications.map(item => {
                                    return (
                                        <div className="noti-item">
                                            <div className="avatar" />
                                            <div className="description">{item.description}</div>
                                        </div>
                                    );
                                })
                            }
                        </div>
                        <a className="show-all-noti" href="">View all</a>
                    </div>
                }
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