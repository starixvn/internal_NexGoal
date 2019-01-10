import React, { Component } from 'react';
import axios from 'axios';
import { axiosError, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
import { NotificationStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import EmployeeInfo from '../home/EmployeeInfo';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import Marquee from '../../shared/components/Marquee';
import NominationResult from '../home/NominationResult';
import WorkLoad from '../home/workload';
import HrmSummary from '../home/hrm/HRMSummary';
import Announcement from '../../shared/components/Announcement';
import NotificationBoard from './notification-board';

export default class NotificationCenter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            periods: [],
            nominations: [],
            performanceAppraisals: [],
            announcements: [],
            currentUserInfo: getCurrentUserInfo(),
            showAnnouncementModal: false,
            notifications: [],
            hasMore: true,
            pageIndex: 0,
            unreadNoti: 0
        };

        this.pageSize = 20;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('home/GetNominationResult')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),
            axios(resolveClientUrl('common/GetAnnouncements')),
            axios(resolveClientUrl('Notification/GetNotification'), {
                params: {
                    pageIndex: 0,
                    pageSize: this.pageSize
                }
            })
        ];

        Promise.all(promises)
            .then(([periodResponse, nominationResponse, performanceAppraisalsResponse, announcementsResponse, notificationResponse]) => {
                const notiData = notificationResponse.data;
                this.setState({
                    periods: periodResponse.data,
                    announcements: announcementsResponse.data,
                    nominations: nominationResponse.data,
                    performanceAppraisals: performanceAppraisalsResponse.data,
                    notifications: notiData.notifications,
                    unreadNoti: notiData.unreadNoti,
                    hasMore: notiData.notifications.length < notiData.total
                });
            }).catch(axiosError);

        this.addEventListener();
    }

    componentWillUnmount() {
        this.removeEventListener();
    }

    addEventListener = () => {
        let notiTrigger = document.getElementById('trigger-notification-center');
        if (notiTrigger) {
            notiTrigger.addEventListener('click', () => {
                let notificationId = parseInt(notiTrigger.getAttribute("notificationId"));
                if (!notificationId) {
                    this.markAsReadSuccess();
                } else {
                    this.readNotificationSucess(notificationId);
                }
            }, false);
        }
    }

    removeEventListener = () => {
        let notiTrigger = document.getElementById('trigger-notification-center');
        if (notiTrigger) {
            notiTrigger.removeEventListener('click');
        }
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

    triggerNotificationBell = (notificationId) => {
        let notiBell = document.getElementById("trigger-notification-bell");
        if (notiBell) {
            notiBell.setAttribute("notificationId", notificationId);
            notiBell.click();
        }
    }

    readNotificationSucess = (notiId) => {
        let notifications = [...this.state.notifications];
        const notiIndex = this.state.notifications.findIndex(noti => noti.notificationId === notiId);
        notifications[notiIndex].statusId = NotificationStatus.Read;
        this.setState({
            notifications: notifications
        });
    }

    markAsReadSuccess = () => {
        let notifications = [...this.state.notifications];
        for (var i = 0; i < notifications.length; i++) {
            notifications[i].statusId = NotificationStatus.Read;
        }
        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });
    }

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsReadNotification'))
                .then(response => {
                    let data = response.data;
                    if (data.isSuccessful) {
                        this.markAsReadSuccess();
                        this.triggerNotificationBell();
                    } else {
                        toast.error(data.message);
                    }
                }).catch(axiosError);
        }
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
                this.readNotificationSucess(notiId);
                this.triggerNotificationBell(notiId);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
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

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false })

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true })

    render() {
        const currentUserInfo = this.state.currentUserInfo;
        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <EmployeeInfo employee={currentUserInfo} />
                        <PerformanceAppraisalResult performanceAppraisals={this.state.performanceAppraisals} />
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <Marquee messages={this.state.announcements} onClick={this.handleOpenAnnouncementModal} />
                        </div>
                        <div className="noti-record">
                            <NotificationBoard notifications={this.state.notifications}
                                onLoadMoreNotification={this.handleLoadMoreNotification}
                                readNotification={this.handleReadNotification}
                                markAsRead={this.handleMarkAsRead}
                            />
                        </div>
                    </div>
                    <div className="work-info">
                        <div className="work-info-content">
                            {
                                this.state.nominations.length > 0 &&
                                <NominationResult nominations={this.state.nominations} />
                            }
                            {
                                this.state.periods.length > 0 &&
                                <WorkLoad periods={this.state.periods} />
                            }
                            <HrmSummary />
                        </div>
                    </div>
                </div>
                <Announcement showModal={this.state.showAnnouncementModal} onCloseModal={this.handleCloseAnnouncementModal} announcements={this.state.announcements} />
            </div >
        );
    }
}