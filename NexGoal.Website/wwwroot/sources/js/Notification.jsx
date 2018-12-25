import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from './common-functions';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notifications: [],
            isShowNoti: false
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

    getNotifications = () => {
        axios.get(resolveClientUrl('common/GetNotification'))
            .then(response => {
                this.setState({
                    notifications: response.data && response.data.length > 0
                });
            }).catch(axiosError);
    }

    render() {
        const notifications = this.state.notifications;
        const totalNoti = notifications.length;
        return (
            <Fragment>
                <span className={`icon-notification ${totalNoti > 0 ? "red" : ''}`}
                    onClick={this.handleClickOnBell}
                >
                    {totalNoti}
                </span>
                {
                    this.state.isShowNoti &&
                    <div className="noti-wrapper">
                        <div className="noti-header">
                            <span className="title">Notification</span>
                            <a className="mark-read">Mark as read</a>
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
            </Fragment>
        );
    }
}

ReactDOM.render(<Notification />, document.getElementById('notification'));