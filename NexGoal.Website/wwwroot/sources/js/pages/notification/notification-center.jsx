import React, { Component } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { axiosError, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
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
            let notifications = this.state.notifications;
            notifications.push(...data.notifications);
            this.setState({
                notifications: notifications,
                unreadNoti: data.unreadNoti,
                hasMore: notifications.length < data.total
            });
        }).catch(axiosError);
    }

    triggerNotificationBell = (notificationId) => {
        let notiBell = document.getElementById("trigger-notification-bell");
        if (notiBell) {
            notiBell.setAttribute("notificationId", notificationId);
            notiBell.click();
        }
    }

    addEventListener = () => {
        let notiTrigger = document.getElementById('trigger-notification-center');
        if (notiTrigger) {
            notiTrigger.addEventListener('click', () => {
                let notificationId = parseInt(notiTrigger.getAttribute("notificationId"), 10);
                if (!notificationId) {
                    this.markAsReadSuccess();
                } else {
                    this.handleChangeStatusSuccess(notificationId);
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

    handleRedirectToAction = (notificationId) => {
        let notifications = this.state.notifications.slice();
        let notiIndex = notifications.findIndex(noti => noti.notificationId === notificationId);
        if (notiIndex !== -1) {
            let actionTypeId = notifications[notiIndex].actionTypeId;
            let recordId = notifications[notiIndex].recordId;
            let url = '';
            switch (actionTypeId) {
                case NotificationActionType.MyPageWithCreatePerformanceCheckin:
                    url = resolveClientUrl('?action=newrecord');
                    break;

                case NotificationActionType.MyPageWithSpecificPerformanceCheckin:
                    url = resolveClientUrl(`?recordId=${recordId}`);
                    break;

                case NotificationActionType.MemberPageWithCreatePerformanceCheckin:
                    url = resolveClientUrl('member?action=newrecord');
                    break;

                case NotificationActionType.MemberPageWithSpecificPerformanceCheckin:
                    url = resolveClientUrl(`member?recordId=${recordId}`);
                    break;

                case NotificationActionType.MemberPageWithActiveNominationTab:
                    url = resolveClientUrl('member?activeTab=2');
                    break;

                case NotificationActionType.MemberPageWithActivePerformanceAppraisalTab:
                    url = resolveClientUrl('member?activeTab=3');
                    break;

                case NotificationActionType.PAFocus:
                    url = resolveClientUrl('pafocus');
                    break;
            }

            if (url) {
                window.location = url;
            }
        }
    }

    markAsReadSuccess = () => {
        let notifications = this.state.notifications.slice();
        for (var i = 0; i < notifications.length; i++) {
            notifications[i].statusId = NotificationStatus.Read;
        }

        this.setState({
            notifications: notifications,
            unreadNoti: 0
        });

        this.triggerNotificationBell();
    }

    handleMarkAsRead = () => {
        if (this.state.unreadNoti > 0) {
            axios.get(resolveClientUrl('Notification/MarkAsRead'))
                .then(response => {
                    let data = response.data;
                    if (data.isSuccessful) {
                        this.markAsReadSuccess();
                    } else {
                        toast.error(data.message);
                    }
                }).catch(axiosError);
        }
    }

    handleChangeStatusSuccess = (notificationId) => {
        let notifications = this.state.notifications.slice();
        let index = notifications.findIndex(x => x.notificationId === notificationId);

        if (index >= 0) {
            notifications[index].statusId = NotificationStatus.Read;
            this.setState({
                notifications: notifications,
                unreadNoti: this.state.unreadNoti - 1
            });

            this.triggerNotificationBell(notificationId);
        }
    }

    handleChangeStatus = (notificationId, statusId, isRedirect) => {
        if (statusId === NotificationStatus.Unread) {
            axios.get(resolveClientUrl('Notification/ReadNotification'), {
                params: {
                    notificationId: notificationId
                }
            }).then(response => {
                let data = response.data;
                if (data.isSuccessful) {
                    if (isRedirect) {
                        this.handleRedirectToAction(notificationId);
                    } else {
                        this.handleChangeStatusSuccess(notificationId);
                    }
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        } else if (isRedirect) {
            this.handleRedirectToAction(notificationId);
        }
    }

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false })

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true })

    render() {
        let currentUserInfo = getCurrentUserInfo();

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