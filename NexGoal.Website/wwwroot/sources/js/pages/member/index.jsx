import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import InfiniteScroll from 'react-infinite-scroller';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, NominationStatuses, PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import NominationResult from '../../shared/components/NominationResult';
import MemberWorkload from './MemberWorkload';
import Timeline from './timeline/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberPerformances: [],
            memberPerformanceAppraisals: [],
            memberNominations: [],
            periods: [],
            displayedMemberPerformances: []
        };

        this.itemPerPage = 4;
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetAllPeriods')),
            axios(resolveClientUrl('member/GetMemberNominationResult')),
            axios(resolveClientUrl('member/Performance'))
        ];

        Promise.all(promises)
            .then(([periodResponse, memberNominationResponse, memberPerformanceResponse]) => {
                let displayedMemberPerformances = memberPerformanceResponse.data.slice(0, this.itemPerPage);

                this.setState({
                    periods: periodResponse.data,
                    memberNominations: memberNominationResponse.data,
                    memberPerformances: memberPerformanceResponse.data,
                    displayedMemberPerformances: displayedMemberPerformances,
                    hasMore: displayedMemberPerformances.length < memberPerformanceResponse.data.length
                });
            }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        let isHost = performanceRecord.statusId === PerformanceRecordStatus.Draft
            || performanceRecord.statusId === PerformanceRecordStatus.Submitted;
        let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
        let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                objectId={performanceRecord.performanceRecordId}
                employee={performanceRecord.employee}
                creator={performanceRecord.creator}
                roleId={performanceRecord.roleId}
                statusId={performanceRecord.statusId}
                submittedDate={submittedDate} modifiedDate={modifiedDate}
                isHost={isHost}
                grade={performanceRecord.grade}
                comment={performanceRecord.comment}
                projects={performanceRecord.projects.map(project => project.name)}
                onEdit={this.handleEditPerformanceRecord}
                isHighlighted={this.state.selectedPCIId === performanceRecord.performanceRecordId}
            />
        );
    }

    loadMorePerformances = (page) => {
        let records = this.state.memberPerformances.slice(0, page * this.itemPerPage);
        this.setState({ displayedMemberPerformances: records, hasMore: records.length < this.state.memberPerformances.length });
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <div className="main-layout member-page">
                        <div className="general-info">
                            <Timeline />
                        </div>
                        <div className="main-content">
                            <div className="fixed-section">
                                <div className="warning marquee">
                                    <div>News: PA process will be available from May 02, 2018 00:00 and any change after May 08, 2018 23:59 are not allowed</div>
                                </div>
                            </div>
                            <div className="record-list">
                                {
                                    <InfiniteScroll
                                        pageStart={1} initialLoad={true}
                                        loadMore={this.loadMorePerformances}
                                        hasMore={this.state.hasMore}
                                        threshold={0}
                                        useWindow={true}
                                    >
                                        {
                                            this.state.displayedMemberPerformances.map(this.renderPerformanceRecord)
                                        }
                                    </InfiniteScroll>
                                }
                            </div>
                        </div>
                        <div className="work-info">
                            <NominationResult isSupervisor={true} nominations={this.state.memberNominations} />
                            {this.state.periods.length > 0 && <MemberWorkload periods={this.state.periods} />}
                        </div>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('member-wrapper'));