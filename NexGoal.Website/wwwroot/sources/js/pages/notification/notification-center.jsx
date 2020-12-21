import React, { Component } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';

import { axiosError, getCurrentUserInfo, resolveClientUrl, getUrlByActionTypeId } from '../../common-functions';
import { NotificationActionType, NotificationStatus } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import EmployeeInfo from '../home/EmployeeInfo';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import Marquee from '../../shared/components/Marquee';
import NominationResult from '../home/NominationResult';
import WorkLoad from '../home/workload';
import HrmSummary from '../home/hrm/HRMSummary';
import Announcement from '../../shared/components/Announcement';
import NotificationBoard from './notification-board';
import getResource from '../../resourceHelper';
import CustomScrollbars from '../../shared/components/CustomScrollbars';

export default class NotificationCenter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            periods: [],
            nominations: [],
            performanceAppraisals: [],
            announcements: [],
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
                    announcements: announcementsResponse.data.length > 0 ? announcementsResponse.data : [getResource('DefaultAnnouncement')],
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

    getNotifications = () => {
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
            });
        }).catch(axiosError);
    };

    triggerNotificationBell = notificationId => {
        const notiBell = document.getElementById('trigger-notification-bell');
        if (notiBell) {
            notiBell.setAttribute('notificationId', notificationId);
            notiBell.click();
        }
    };

    addEventListener = () => {
        const notiTrigger = document.getElementById('trigger-notification-center');
        if (notiTrigger) {
            notiTrigger.addEventListener('click', () => {
                const notificationId = parseInt(notiTrigger.getAttribute('notificationId'), 10);
                if (!notificationId) {
                    this.markAsReadSuccessCenter();
                } else {
                    this.handleChangeStatusSuccess(notificationId);
                }
            }, false);
        }
    };

    removeEventListener = () => {
        const notiTrigger = document.getElementById('trigger-notification-center');
        if (notiTrigger) {
            notiTrigger.removeEventListener('click');
        }
    };

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
    };

    handleRedirectToActionCenter = notificationId => {
        const notificationsCenter = this.state.notifications.slice();
        const notiIndexCenter = notificationsCenter.findIndex(noti => noti.notificationId === notificationId);
        if (notiIndexCenter !== -1) {
            const { actionTypeId, recordId } = notificationsCenter[notiIndexCenter];

            const urlCenter = getUrlByActionTypeId(actionTypeId, recordId, NotificationActionType);

            if (urlCenter) {
                window.location = urlCenter;
            }
        }
    };

    markAsReadSuccessCenter = () => {
        const notifications = this.state.notifications.slice();
        for (let i = 0; i < notifications.length; i++) {
            notifications[i].statusId = NotificationStatus.Read;
        }

        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });

        this.triggerNotificationBell();
    };

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsRead'))
                .then(response => {
                    const data = response.data;
                    if (data.isSuccessful) {
                        this.markAsReadSuccessCenter();
                    } else {
                        toast.error(data.message);
                    }
                }).catch(axiosError);
        }
    };

    handleChangeStatusSuccess = notificationId => {
        const notifications = this.state.notifications.slice();
        const index = notifications.findIndex(x => x.notificationId === notificationId);

        if (index >= 0) {
            notifications[index].statusId = NotificationStatus.Read;
            this.setState({
                notifications: notifications,
                unreadNoti: this.state.unreadNoti - 1
            });

            this.triggerNotificationBell(notificationId);
        }
    };

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
                        this.handleRedirectToActionCenter(notificationId);
                    } else {
                        this.handleChangeStatusSuccess(notificationId);
                    }
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        } else if (isRedirect) {
            this.handleRedirectToActionCenter(notificationId);
        }
    };

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false });

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true });

    render() {
        const currentUserInfo = getCurrentUserInfo();

        return (
            <div className="row">
                <ToastLayout />
                <div className="my-page main-layout">
                    <div className="general-info">
                        <CustomScrollbars>
                            <EmployeeInfo employee={currentUserInfo} />
                            <PerformanceAppraisalResult performanceAppraisals={this.state.performanceAppraisals} />
                        </CustomScrollbars>
                    </div>
                    <div className="main-content">
                        <div className="fixed-section">
                            <Marquee messages={this.state.announcements} onClick={this.handleOpenAnnouncementModal} />
                        </div>
                        <div className="noti-record">
                            <NotificationBoard notifications={this.state.notifications}
                                onLoadMore={this.handleLoadMoreNotification}
                                onChangeStatus={this.handleChangeStatus}
                                onMarkAsRead={this.handleMarkAsRead}
                            />
                        </div>
                    </div>
                    <div className="work-info">
                        <CustomScrollbars>
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
                        </CustomScrollbars>
                    </div>
                </div>
                <Announcement showModal={this.state.showAnnouncementModal}
                    onCloseModal={this.handleCloseAnnouncementModal}
                    announcements={this.state.announcements}
                />
            </div >
        );
    }
}