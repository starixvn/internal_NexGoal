import React, { Component } from 'react';
import axios from 'axios';
import ReactTooltip from 'react-tooltip';
import InfiniteScroll from 'react-infinite-scroller';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import { axiosError, calculateTimelineNode, getCurrentUserInfo, resolveClientUrl } from '../../common-functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import EmployeeInfo from '../home/EmployeeInfo';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import Marquee from '../../shared/components/Marquee';
import CustomScrollbars from '../../shared/components/CustomScrollbars';
import NominationResult from '../home/NominationResult';
import WorkLoad from '../home/workload';
import HrmSummary from '../home/hrm/HRMSummary';
import Announcement from '../../shared/components/Announcement';
import Notification from '../../shared/components/notification/Notification';

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
            pageIndex: 0
        };

        this.pageSize = 8;
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
                    hasMore: notiData.notifications.length < notiData.total
                });
            }).catch(axiosError);
    }

    handleCloseAnnouncementModal = () => this.setState({ showAnnouncementModal: false })

    handleOpenAnnouncementModal = () => this.setState({ showAnnouncementModal: true })

    render() {
        let currentUserInfo = this.state.currentUserInfo;

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
                        <div className="record-list">
                            <Notification notifications={this.state.notifications} />
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